using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace econest.Service
{
    public class HuggingFaceService
    {
          private readonly HttpClient _httpClient;
            private readonly string _apiKey;

            public HuggingFaceService(IConfiguration config)
            {
                _httpClient = new HttpClient();
                _apiKey = config["HuggingFace:ApiKey"]; // Get from appsettings.json
            }

        public async Task<string> GetChatbotResponse(string userMessage)
        {
            var requestBody = new
            {
                inputs = $":User  {userMessage}\nAssistant:"
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync("https://api-inference.huggingface.co/models/tiiuae/falcon-7b-instruct", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Log the status code and response content
            Console.WriteLine($"Status Code: {response.StatusCode}");

            Console.WriteLine($"Response Content: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                return $"Error: {response.StatusCode}, Details: {responseContent}";
            }


            try
            {
                using var document = JsonDocument.Parse(responseContent);
                if (document.RootElement.ValueKind == JsonValueKind.Array &&
                    document.RootElement.GetArrayLength() > 0 &&
                    document.RootElement[0].TryGetProperty("generated_text", out var reply))
                {
                    return reply.GetString().Replace("User :", "").Replace("Assistant:", "").Trim();
                }

                if (document.RootElement.TryGetProperty("error", out var error))
                {
                    return $"Hugging Face API Error: {error.GetString()}";
                }

                return "Unexpected response format.";
            }
            catch
            {
                return "Sorry, something went wrong while getting the response.";
            }
        }

    }

}
