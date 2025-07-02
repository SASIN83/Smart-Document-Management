using System;
using System.Net;
using System.Text.Json;

public class Structures
{
    public class Document
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Author { get; set; }
    }
    public class GeminiResponse
    {
        public List<Candidate> candidates { get; set; }
        public UsageMetadata usageMetadata { get; set; }
        public string modelVersion { get; set; }
        public string responseId { get; set; }
    }

    public class Candidate
    {
        public Content content { get; set; }
        public string finishReason { get; set; }
        public int index { get; set; }
    }

    public class Content
    {
        public List<Part> parts { get; set; }
        public string role { get; set; }
    }

    public class Part
    {
        public string text { get; set; }
    }

    public class UsageMetadata
    {
        public int promptTokenCount { get; set; }
        public int totalTokenCount { get; set; }
        public List<PromptTokensDetail> promptTokensDetails { get; set; }
    }

    public class PromptTokensDetail
    {
        public string modality { get; set; }
        public int tokenCount { get; set; }
    }

    public class TextResponse
    {
        public string Summary { get; set; }
        public string Category { get; set; }
        public string Reasoning { get; set; }
    }

    public class Gen_Response
    {
        public HttpStatusCode statusCode { get; set; }
        public TextResponse response { get; set; }
        public string Message { get; set; }
    }
    public class DocumentWithEmbedding
    {
        public string ID { get; set; }
        public string FileName { get; set; }
        public string Summary { get; set; }
        public string Category { get; set; }
        public string Reasoning { get; set; }
        public string Content { get; set; }
        public string EmbeddingJson { get; set; }

        // Not mapped: parsed embedding
        public List<float> Embedding => string.IsNullOrEmpty(EmbeddingJson)
            ? new List<float>()
            : JsonSerializer.Deserialize<List<float>>(EmbeddingJson);
    }

    public class DocumentList
    {
        public string ID { get; set; }
        public string FileName { get; set; }
        public string Summary { get; set; }
        public string Category { get; set; }
        public DateTime InsertDate { get; set; }
    }
}