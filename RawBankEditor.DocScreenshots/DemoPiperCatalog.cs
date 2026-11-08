using System.Net;

namespace RawBankEditor.DocScreenshots;

/// <summary>
/// Zoznam hlasov Piper (výber z voices.json) bez pripojenia.
/// </summary>
internal sealed class DemoPiperCatalog : HttpMessageHandler
{
    private const string Json = """
        {
          "sk_SK-lili-medium": {"quality": "medium", "num_speakers": 1, "language": {"family": "sk", "name_native": "Slovenčina"},
            "files": {"sk/sk_SK/lili/medium/sk_SK-lili-medium.onnx": {"size_bytes": 63201294, "md5_digest": ""},
                      "sk/sk_SK/lili/medium/sk_SK-lili-medium.onnx.json": {"size_bytes": 4900, "md5_digest": ""}}},
          "cs_CZ-jirka-low": {"quality": "low", "num_speakers": 1, "language": {"family": "cs", "name_native": "Čeština"},
            "files": {"cs/cs_CZ/jirka/low/cs_CZ-jirka-low.onnx": {"size_bytes": 28130791, "md5_digest": ""},
                      "cs/cs_CZ/jirka/low/cs_CZ-jirka-low.onnx.json": {"size_bytes": 5025, "md5_digest": ""}}},
          "cs_CZ-jirka-medium": {"quality": "medium", "num_speakers": 1, "language": {"family": "cs", "name_native": "Čeština"},
            "files": {"cs/cs_CZ/jirka/medium/cs_CZ-jirka-medium.onnx": {"size_bytes": 63201294, "md5_digest": ""},
                      "cs/cs_CZ/jirka/medium/cs_CZ-jirka-medium.onnx.json": {"size_bytes": 5025, "md5_digest": ""}}},
          "en_GB-alba-medium": {"quality": "medium", "num_speakers": 1, "language": {"family": "en", "name_native": "English"},
            "files": {"en/en_GB/alba/medium/en_GB-alba-medium.onnx": {"size_bytes": 63201294, "md5_digest": ""},
                      "en/en_GB/alba/medium/en_GB-alba-medium.onnx.json": {"size_bytes": 4880, "md5_digest": ""}}}
        }
        """;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(Json) });
}
