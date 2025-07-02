using RestSharp;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using static Structures;

namespace Smart_Document_Management_System.Helpers
{
    public class GemmaProcessor
    {
        private readonly string _gemmaApiKey;
        private readonly string _gemmaApiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemma-3-12b-it:generateContent?key="; // Replace with the correct endpoint

        public GemmaProcessor(string gemmaApiKey)
        {
            _gemmaApiKey = gemmaApiKey;
        }

        public async Task<(string Summary, string Category, string Reasoning)> ProcessFileAsync(string FilePath,string Text,string guid)
        {
            try
            {
                CommonMethods.DebugLog($"PROCESSPDFASYNC_REQUEST|{guid}|{FilePath}");

                //CommonMethods.ExtractTextFrom(pdfFilePath, out string pdfText);

            // Construct the API request payload
            var requestData = new
            {
                contents = new[]
                {
                new
                {
                    parts = new[]
                    {
                        new { text = "Summarize the following text and then categorize it into one of these categories: Technology, Finance, Health, Education, News. In Json format {Summary:<summary>,Category:<category>},Reasoning:<reasoning>" },
                        new { text = Text }
                    }
                }
            }
            };

            string jsonPayload = JsonSerializer.Serialize(requestData);

            // Call the Gemma API
            string responseContent = await CallGemmaApiAsync(jsonPayload);

            // Parse the API response
            var response = JsonSerializer.Deserialize<GeminiResponse>(responseContent);
                var text = response?.candidates?[0]?.content?.parts.FirstOrDefault()?.text;
                int start = text.IndexOf('{');
                int end = text.LastIndexOf('}');
                if (start == -1 || end == -1 || end <= start)
                    throw new Exception("No valid JSON object found in response.");

                string jsonOnly = text.Substring(start, end - start + 1);

                TextResponse textResponse = JsonSerializer.Deserialize<TextResponse>(jsonOnly);
                // Extract the summary and category (adjust based on the actual response format)
                //TextResponse textResponse = JsonSerializer.Deserialize<TextResponse>(text.Substring(7,text.Length-3));
                string summary = textResponse.Summary ?? "Summary not available";
            string category = textResponse.Category ?? "Category not available"; // Assuming category is in the second part
            string reasoning = textResponse.Reasoning ?? "Reasoning not available"; // Assuming category is in the second part

            return (summary, category, reasoning);
            }
            catch (Exception ex)
            {
                CommonMethods.ExceptionLog(ex, "ProcessPdfAsync");
                return ("Error generating summary", "Error categorizing","Error Reasoning");
            }
        }

        private async Task<string> CallGemmaApiAsync(string jsonPayload)
        {
            var client = new RestSharp.RestClient();
            var request = new RestSharp.RestRequest(_gemmaApiEndpoint + _gemmaApiKey, RestSharp.Method.Post);
            request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("Authorization", $"Bearer {_gemmaApiKey}");
            request.AddStringBody(jsonPayload, RestSharp.DataFormat.Json);

            try
            {
                var response = await client.ExecuteAsync(request);
                if (!response.IsSuccessful)
                {
                    Console.WriteLine($"API request failed: {response.StatusCode} - {response.ErrorMessage}");
                    return string.Empty;
                }
                return response.Content ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API request failed: {ex.Message}");
                return string.Empty;
            }
        }
        public async Task<List<float>> GetEmbeddingAsync(string text)
        {
            var apiKey = CommonMethods.GetFromConfig("OTHER", "API-KEY");
            var client = new RestClient($"https://generativelanguage.googleapis.com/v1beta/models/embedding-001:embedContent?key={apiKey}");
            var request = new RestRequest("", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            var body = new
            {
                content = new
                {
                    parts = new[]
                    {
                new { text = text }
            }
                }
            };
            request.AddStringBody(System.Text.Json.JsonSerializer.Serialize(body), DataFormat.Json);

            var response = await client.ExecutePostAsync(request);
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                return null;

            var json = System.Text.Json.JsonDocument.Parse(response.Content);

            // Correct extraction for: { "embedding": { "values": [ ... ] } }
            if (json.RootElement.TryGetProperty("embedding", out var embeddingElement) &&
                embeddingElement.TryGetProperty("values", out var valuesElement))
            {
                return valuesElement.EnumerateArray().Select(x => x.GetSingle()).ToList();
            }
            return null;
        }

    }

}