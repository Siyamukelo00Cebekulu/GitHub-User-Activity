using System.Text.Json.Serialization;

public class GitHubEvent
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("actor")]
    public Actor? Actor { get; set; }

    [JsonPropertyName("repo")]
    public Repository? Repository { get; set; }

    [JsonPropertyName("payload")]
    public Payload? Payload { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("public")]
    public bool IsPublic { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}