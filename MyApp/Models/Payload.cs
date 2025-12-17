using System.Text.Json.Serialization;

public class Payload
{
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    [JsonPropertyName("commits")]
    public List<Commit>? Commits { get; set; }

    [JsonPropertyName("ref")]
    public string? Ref { get; set; }

    [JsonPropertyName("ref_type")]
    public string? RefType { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("push_id")]
    public long? PushId { get; set; }

    [JsonPropertyName("size")]
    public int? Size { get; set; }

    [JsonPropertyName("distinct_size")]
    public int? DistinctSize { get; set; }

    [JsonPropertyName("issue")]
    public object? Issue { get; set; }  // Could be typed as IssuePayload

    [JsonPropertyName("pull_request")]
    public object? PullRequest { get; set; }  // Could be typed as PullRequestPayload

    [JsonPropertyName("comment")]
    public object? Comment { get; set; }  // Could be typed as CommentPayload
}