using System.Net;
using System.Net.Http.Headers;

namespace RawBankEditor.Speech;

/// <summary>
/// Poziadavky na sluzby syntezy cez HTTP - opakovanie pri pretazeni a chyby ako <see cref="SpeechException" />.
/// </summary>
internal static class SpeechHttp
{
    /// <summary>Najvyssi pocet opakovani poziadavky.</summary>
    public const int MaxRetries = 3;

    /// <summary>
    /// Posle poziadavku; pri 429 a 5xx ju az <see cref="MaxRetries" />-krat zopakuje (podla Retry-After, inak
    /// 1, 2, 4 s). Neuspesnu odpoved zmeni na <see cref="SpeechException" /> so spravou sluzby.
    /// </summary>
    /// <param name="http">klient</param>
    /// <param name="create">nova poziadavka pre kazdy pokus</param>
    /// <param name="delay">cakanie pred opakovanim</param>
    /// <param name="errorMessage">sprava z tela chybovej odpovede; <see langword="null" /> = stav HTTP</param>
    /// <param name="cancel">zrusenie</param>
    public static async Task<HttpResponseMessage> SendAsync(HttpClient http, Func<HttpRequestMessage> create, Func<TimeSpan, CancellationToken, Task> delay,
        Func<string, string?> errorMessage, CancellationToken cancel)
    {
        for (var attempt = 0; ; attempt++)
        {
            HttpResponseMessage response;
            try
            {
                using var request = create();
                response = await http.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancel).ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                throw new SpeechException(e.Message, e);
            }
            catch (TaskCanceledException e) when (!cancel.IsCancellationRequested)
            {
                // vyprsal cas poziadavky, nie zrusenie pouzivatelom
                throw new SpeechException(e.Message, e);
            }

            if (response.IsSuccessStatusCode)
                return response;

            var status = (int)response.StatusCode;
            if (attempt < MaxRetries && (response.StatusCode == HttpStatusCode.TooManyRequests || status >= 500))
            {
                var wait = RetryAfter(response.Headers.RetryAfter) ?? TimeSpan.FromSeconds(1 << attempt);
                response.Dispose();
                await delay(wait, cancel).ConfigureAwait(false);
                continue;
            }

            using (response)
            {
                var text = await response.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);
                throw new SpeechException(errorMessage(text) ?? $"{status} {response.ReasonPhrase}") { StatusCode = status };
            }
        }
    }

    private static TimeSpan? RetryAfter(RetryConditionHeaderValue? header)
    {
        if (header?.Delta is { } delta)
            return delta;
        if (header?.Date is { } date)
            return date - DateTimeOffset.UtcNow;
        return null;
    }
}
