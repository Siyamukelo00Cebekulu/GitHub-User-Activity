

public interface IActivityPrinter
{
    void PrintActivities(List<GitHubEvent> events);
}

public class ActivityPrinter : IActivityPrinter
{
    public void PrintActivities(List<GitHubEvent> events)
    {
        if (events == null || events.Count == 0)
        {
            Console.WriteLine("No recent activity found.");
            return;
        }

        foreach (var activity in events.Take(10)) // Show only last 10 activities
        {
            var description = GetActivityDescription(activity);
            if (!string.IsNullOrEmpty(description))
            {
                Console.WriteLine($"- {description}");
            }
        }
    }

    private string GetActivityDescription(GitHubEvent activity)
    {
        var repoName = activity.Repository?.Name ?? "unknown repository";

        return activity.Type switch
        {
            "PushEvent" => GetPushEventDescription(activity, repoName),
            "IssuesEvent" => GetIssueEventDescription(activity, repoName),
            "WatchEvent" => GetWatchEventDescription(repoName),
            "CreateEvent" => GetCreateEventDescription(activity, repoName),
            "DeleteEvent" => GetDeleteEventDescription(activity, repoName),
            "ForkEvent" => GetForkEventDescription(repoName),
            "PullRequestEvent" => GetPullRequestEventDescription(activity, repoName),
            "IssueCommentEvent" => GetIssueCommentEventDescription(activity, repoName),
            "CommitCommentEvent" => GetCommitCommentEventDescription(repoName),
            "ReleaseEvent" => GetReleaseEventDescription(activity, repoName),
            _ => GetGenericEventDescription(activity, repoName)
        };
    }

    private string GetPushEventDescription(GitHubEvent activity, string repoName)
    {
        var commitCount = activity.Payload?.Commits?.Count ?? 0;
        var branch = activity.Payload?.Ref?.Split('/').LastOrDefault() ?? "default";
        return $"Pushed {commitCount} commit{(commitCount != 1 ? "s" : "")} to {repoName} ({branch})";
    }

    private string GetIssueEventDescription(GitHubEvent activity, string repoName)
    {
        var action = activity.Payload?.Action ?? "modified";
        return $"{CapitalizeFirstLetter(action)} an issue in {repoName}";
    }

    private string GetWatchEventDescription(string repoName)
    {
        return $"Starred {repoName}";
    }

    private string GetCreateEventDescription(GitHubEvent activity, string repoName)
    {
        var refType = activity.Payload?.RefType ?? "resource";
        return $"Created a new {refType} in {repoName}";
    }

    private string GetDeleteEventDescription(GitHubEvent activity, string repoName)
    {
        var refType = activity.Payload?.RefType ?? "resource";
        return $"Deleted a {refType} in {repoName}";
    }

    private string GetForkEventDescription(string repoName)
    {
        return $"Forked {repoName}";
    }

    private string GetPullRequestEventDescription(GitHubEvent activity, string repoName)
    {
        var action = activity.Payload?.Action ?? "modified";
        return $"{CapitalizeFirstLetter(action)} a pull request in {repoName}";
    }

    private string GetIssueCommentEventDescription(GitHubEvent activity, string repoName)
    {
        var action = activity.Payload?.Action ?? "commented";
        return $"{CapitalizeFirstLetter(action)} on an issue in {repoName}";
    }

    private string GetCommitCommentEventDescription(string repoName)
    {
        return $"Commented on a commit in {repoName}";
    }

    private string GetReleaseEventDescription(GitHubEvent activity, string repoName)
    {
        var action = activity.Payload?.Action ?? "published";
        return $"{CapitalizeFirstLetter(action)} a release in {repoName}";
    }

    private string GetGenericEventDescription(GitHubEvent activity, string repoName)
    {
        var readableType = activity.Type.Replace("Event", "").ToLower();
        return $"{CapitalizeFirstLetter(readableType)} in {repoName}";
    }

    private string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        return char.ToUpper(input[0]) + input[1..];
    }
}