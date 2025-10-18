using System.Net.Http.Headers;
using System.Web;
using System.Text;
using System.Text.Json;

namespace ConversantAI.Services;

public class OpenAIClient
{
    private readonly HttpClient _http = new();
    private readonly string _apiKey;

    public OpenAIClient(string apiKey) => _apiKey = apiKey;

    public async Task<string> CompleteAsync(string system, string prompt, string jsonState)
    {
        var body = new
        {
            model = "gpt-5",
            messages = new object[]
            {
                new { role = "system", content = system },
                // pass the compressed state explicitly so the model can use it
                new { role = "system", content = $"[State]: {jsonState}" },
                new { role = "user", content = prompt }
            },
            temperature = 0.7
        };

        var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        try
        {
          var res = await _http.SendAsync(req);
          res.EnsureSuccessStatusCode();
          var json = await res.Content.ReadAsStringAsync();
        }
        catch (HttpException e)
        {
          Console.WriteLine("Received error response: " + e.Message);
        }

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement
                  .GetProperty("choices")[0]
                  .GetProperty("message")
                  .GetProperty("content")
                  .GetString() ?? "";
    }
}

