public class GitHubEvent
{
    public string Id { get; set; }
    public string Type { get; set; }
    public Actor Actor { get; set; }
    public Repo Repo { get; set; }
    public Payload Payload { get; set; }
    public bool Public { get; set; }
    public DateTime Created_At { get; set; }
}