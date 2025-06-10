public class GitHubUser
{
    public string? Login { get; set; }
    public string? Name { get; set; }
    public string? Bio { get; set; }
    public int PublicRepos { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class GitHubEvent
{
    public string? Type { get; set; }
    public GitHubEventRepo? Repo { get; set; }
    public GitHubEventActor? Actor { get; set; }
    public GitHubEventPayload? Payload { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GitHubEventRepo
{
    public string? Name { get; set; }
}

public class GitHubEventActor
{
    public string? Login { get; set; }
}

public class GitHubEventPayload
{
    public string? Action { get; set; }
    public GitHubEventPullRequest? PullRequest { get; set; }
}

public class GitHubEventPullRequest
{
    public string? Title { get; set; }
}