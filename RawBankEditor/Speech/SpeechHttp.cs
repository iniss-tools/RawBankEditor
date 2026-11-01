using System.Net;
using System.Net.Http.Headers;
using Serilog;

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
            // len cesta - dotaz moze obsahovat text pouzivatela, hlavicky kluce
            string? endpoint = null;
            try
            {
                using var request = create();
                endpoint = Endpoint(request);
                response = await http.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancel).ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                Log.Warning(e, "Sluzba reci: poziadavka {Endpoint} zlyhala", endpoint);
                throw new SpeechException(e.Message, e);
            }
            catch (TaskCanceledException e) when (!cancel.IsCancellationRequested)
            {
                // vyprsal cas poziadavky, nie zrusenie pouzivatelom
                Log.Warning("Sluzba reci: poziadavka {Endpoint} nedostala odpoved do {Timeout}", endpoint, http.Timeout);
                throw new SpeechException(e.Message, e);
            }

            if (response.IsSuccessStatusCode)
                return response;

            var status = (int)response.StatusCode;
            if (attempt < MaxRetries && (response.StatusCode == HttpStatusCode.TooManyRequests || status >= 500))
            {
                var wait = RetryAfter(response.Headers.RetryAfter) ?? TimeSpan.FromSeconds(1 << attempt);
                Log.Warning("Sluzba reci: {Endpoint} vratila {Status}, pokus {Attempt}/{MaxRetries} sa zopakuje o {Wait}", endpoint, status, attempt + 1,
                    MaxRetries, wait);
                response.Dispose();
                await delay(wait, cancel).ConfigureAwait(false);
                continue;
            }

            using (response)
            {
                var text = await response.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);
                var message = errorMessage(text) ?? $"{status} {response.ReasonPhrase}";
                // sprava sluzby moze byt cele telo odpovede (nie JSON) - skratit
                Log.Warning("Sluzba reci: {Endpoint} odmietla poziadavku ({Status}): {Message}", endpoint, status, message.Length > 300 ? message[..300] : message);
                throw new SpeechException(message) { StatusCode = status };
            }
        }
    }

    /// <summary>Metoda a adresa poziadavky bez dotazu (moze obsahovat text pouzivatela) pre zaznam.</summary>
    private static string Endpoint(HttpRequestMessage request)
    {
        var uri = request.RequestUri;
        var path = uri is null ? "" : uri.IsAbsoluteUri ? uri.GetLeftPart(UriPartial.Path) : uri.OriginalString.Split('?')[0];
        return $"{request.Method} {path}";
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
