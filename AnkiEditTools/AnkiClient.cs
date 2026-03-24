using System.Text;
using System.Text.Json;

namespace AnkiEditTools;

using System.Net.Http.Json;

public class AnkiClient
{
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("http://localhost:8765") };

    public async Task<T?> SendAsync<T>(string action, object @params = null)
    {
        // 1. Manually create the JSON string with camelCase
        var payload = new { action, version = 6, @params };
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        string jsonString = JsonSerializer.Serialize(payload, jsonOptions);

        // 2. Wrap it in a StringContent with the correct MediaType
        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

        // 3. Send the POST request
        var response = await _httpClient.PostAsync("/", content);
    
        // 4. Read the raw response to debug
        string responseBody = await response.Content.ReadAsStringAsync();
    
        // If you see {"apiVersion": "AnkiConnect v.6"} here, 
        // it means Anki didn't "see" your JSON body at all.
        Console.WriteLine($"Raw Response: {responseBody}");

        var result = JsonSerializer.Deserialize<AnkiResponse<T>>(responseBody, jsonOptions);
    
        if (result == null) throw new Exception("Failed to deserialize Anki response.");
        if (result.Error != null) throw new Exception($"Anki Error: {result.Error}");

        return result.Result;
    }

    private class AnkiResponse<T>
    {
        public T Result { get; set; }
        public string Error { get; set; }
    }
}