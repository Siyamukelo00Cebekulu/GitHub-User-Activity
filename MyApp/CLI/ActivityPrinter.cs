using System.Text;

public interface IActivityPrinter
{
    
    void PrintActivities(List<GitHubEvent> events);
}

public class ActivityPrinter : IActivityPrinter
{
    private const int MaxActivitiesToShow = 20;

    public void PrintActivities(List<GitHubEvent> events)
    {
        if (events == null || events.Count == 0)
        {
            Console.WriteLine("No recent activity found.");
            return;
        }

        Console.WriteLine($"Recent GitHub Activity:\n");

        // Group activities by type and repository
        var groupedActivities = GroupActivities(events);

        // Print grouped activities
        foreach (var group in groupedActivities)
        {
            Console.WriteLine($"- {group}");
        }

        if (groupedActivities.Count == 0)
        {
            Console.WriteLine("No recognizable activity found in the recent events.");
        }
    }

    private List<string> GroupActivities(List<GitHubEvent> events)
    {
        var results = new List<string>();
        
        // Group push events by repository
        var pushGroups = events
            .Where(e => e.Type == "PushEvent")
            .GroupBy(e => e.Repository?.Name ?? "unknown")
            .Select(g => new
            {
                RepoName = g.Key,
                TotalCommits = g.Sum(e => GetCommitCountForPush(e)),
                Branch = GetMostCommonBranch(g.ToList()),
                EventCount = g.Count()
            })
            .Where(g => g.TotalCommits > 0)
            .ToList();

        // Add grouped push events
        foreach (var pushGroup in pushGroups)
        {
            var branchInfo = string.IsNullOrEmpty(pushGroup.Branch) ? "" : $" ({pushGroup.Branch})";
            results.Add($"Pushed {pushGroup.TotalCommits} commit{(pushGroup.TotalCommits != 1 ? "s" : "")} to {pushGroup.RepoName}{branchInfo}");
        }

        // Handle other event types (non-push events)
        var otherEvents = events
            .Where(e => e.Type != "PushEvent")
            .OrderByDescending(e => e.CreatedAt)
            .Take(MaxActivitiesToShow - pushGroups.Count)
            .ToList();

        foreach (var activity in otherEvents)
        {
            var description = GetSingleActivityDescription(activity);
            if (!string.IsNullOrEmpty(description))
            {
                results.Add(description);
            }
        }

        return results.Take(MaxActivitiesToShow).ToList();
    }

    private int GetCommitCountForPush(GitHubEvent pushEvent)
    {
        // Try multiple ways to get commit count
        if (pushEvent.Payload?.Commits?.Count > 0)
        {
            return pushEvent.Payload.Commits.Count;
        }
        else if (pushEvent.Payload?.Size > 0)
        {
            return pushEvent.Payload.Size.Value;
        }
        else if (pushEvent.Payload?.DistinctSize > 0)
        {
            return pushEvent.Payload.DistinctSize.Value;
        }
        // If we have both head and before SHAs, it's at least 1 commit
        else if (!string.IsNullOrEmpty(pushEvent.Payload?.Head) && 
                 !string.IsNullOrEmpty(pushEvent.Payload?.Before))
        {
            return 1;
        }
        
        return 0;
    }

    private string GetMostCommonBranch(List<GitHubEvent> pushEvents)
    {
        if (!pushEvents.Any())
            return string.Empty;

        // Group by branch and get the most common one
        var branchGroups = pushEvents
            .Where(e => !string.IsNullOrEmpty(e.Payload?.Ref))
            .GroupBy(e => GetBranchName(e.Payload?.Ref))
            .Where(g => !string.IsNullOrEmpty(g.Key))
            .OrderByDescending(g => g.Count())
            .ToList();

        return branchGroups.Any() ? branchGroups.First().Key : string.Empty;
    }

    private string GetSingleActivityDescription(GitHubEvent activity)
    {
        var repoName = activity.Repository?.Name ?? "unknown repository";
        
        try
        {
            return activity.Type switch
            {
                "CreateEvent" => GetCreateEventDescription(activity, repoName),
                "PublicEvent" => $"Made repository {repoName} public",
                "IssuesEvent" => GetIssueEventDescription(activity, repoName),
                "IssueCommentEvent" => $"Commented on an issue in {repoName}",
                "WatchEvent" => $"Starred {repoName}",
                "ForkEvent" => $"Forked {repoName}",
                "PullRequestEvent" => GetPullRequestEventDescription(activity, repoName),
                "PullRequestReviewEvent" => $"Reviewed a pull request in {repoName}",
                "PullRequestReviewCommentEvent" => $"Commented on a pull request review in {repoName}",
                "CommitCommentEvent" => $"Commented on a commit in {repoName}",
                "ReleaseEvent" => GetReleaseEventDescription(activity, repoName),
                "MemberEvent" => GetMemberEventDescription(activity, repoName),
                "GollumEvent" => $"Updated wiki in {repoName}",
                "DeleteEvent" => GetDeleteEventDescription(activity, repoName),
                "ProjectEvent" => $"Updated project in {repoName}",
                "TeamAddEvent" => $"Added to team in {repoName}",
                _ => GetGenericEventDescription(activity, repoName)
            };
        }
        catch (Exception)
        {
            return $"Performed {activity.Type.Replace("Event", "").ToLower()} in {repoName}";
        }
    }

    private string GetCreateEventDescription(GitHubEvent activity, string repoName)
    {
        var refType = activity.Payload?.RefType?.ToLower() ?? "resource";
        var refName = GetRefName(activity.Payload?.Ref);
        
        if (!string.IsNullOrEmpty(refName))
        {
            if (refType == "branch")
            {
                return $"Created {refType} '{refName}' in {repoName}";
            }
            else if (refType == "repository")
            {
                return $"Created new repository {repoName}";
            }
            return $"Created {refType} '{refName}' in {repoName}";
        }
        
        return $"Created {refType} in {repoName}";
    }

    private string GetDeleteEventDescription(GitHubEvent activity, string repoName)
    {
        var refType = activity.Payload?.RefType?.ToLower() ?? "resource";
        var refName = GetRefName(activity.Payload?.Ref);
        
        if (!string.IsNullOrEmpty(refName))
        {
            return $"Deleted {refType} '{refName}' from {repoName}";
        }
        
        return $"Deleted {refType} from {repoName}";
    }

    private string GetIssueEventDescription(GitHubEvent activity, string repoName)
    {
        var action = activity.Payload?.Action?.ToLower() ?? "modified";
        
        return action switch
        {
            "opened" => $"Opened a new issue in {repoName}",
            "closed" => $"Closed an issue in {repoName}",
            "reopened" => $"Reopened an issue in {repoName}",
            "edited" => $"Edited an issue in {repoName}",
            "assigned" => $"Assigned an issue in {repoName}",
            "unassigned" => $"Unassigned an issue in {repoName}",
            "labeled" => $"Added label to issue in {repoName}",
            "unlabeled" => $"Removed label from issue in {repoName}",
            _ => $"{CapitalizeFirstLetter(action)} an issue in {repoName}"
        };
    }

    private string GetPullRequestEventDescription(GitHubEvent activity, string repoName)
    {
        var action = activity.Payload?.Action?.ToLower() ?? "modified";
        
        return action switch
        {
            "opened" => $"Opened a pull request in {repoName}",
            "closed" => $"Closed a pull request in {repoName}",
            "merged" => $"Merged a pull request in {repoName}",
            "reopened" => $"Reopened a pull request in {repoName}",
            "edited" => $"Edited a pull request in {repoName}",
            "assigned" => $"Assigned a pull request in {repoName}",
            "unassigned" => $"Unassigned a pull request in {repoName}",
            "review_requested" => $"Requested review for pull request in {repoName}",
            "review_request_removed" => $"Removed review request from pull request in {repoName}",
            _ => $"{CapitalizeFirstLetter(action)} a pull request in {repoName}"
        };
    }

    private string GetReleaseEventDescription(GitHubEvent activity, string repoName)
    {
        var action = activity.Payload?.Action?.ToLower() ?? "published";
        
        return action switch
        {
            "published" => $"Published a release in {repoName}",
            "created" => $"Created a release in {repoName}",
            "edited" => $"Edited a release in {repoName}",
            "deleted" => $"Deleted a release in {repoName}",
            "prereleased" => $"Published a prerelease in {repoName}",
            _ => $"{CapitalizeFirstLetter(action)} a release in {repoName}"
        };
    }

    private string GetMemberEventDescription(GitHubEvent activity, string repoName)
    {
        var action = activity.Payload?.Action?.ToLower() ?? "added";
        
        return action switch
        {
            "added" => $"Added a member to {repoName}",
            "removed" => $"Removed a member from {repoName}",
            "edited" => $"Edited member permissions in {repoName}",
            _ => $"{CapitalizeFirstLetter(action)} a member in {repoName}"
        };
    }

    private string GetGenericEventDescription(GitHubEvent activity, string repoName)
    {
        var readableType = activity.Type.Replace("Event", "").ToLower();
        
        // Add spaces before capital letters (camelCase to words)
        readableType = System.Text.RegularExpressions.Regex.Replace(
            readableType, 
            "([a-z])([A-Z])", 
            "$1 $2"
        );
        
        return $"{CapitalizeFirstLetter(readableType)} in {repoName}";
    }

    private string GetBranchName(string? refString)
    {
        if (string.IsNullOrEmpty(refString))
            return string.Empty;

        // refs/heads/main -> main
        // refs/tags/v1.0 -> v1.0
        var parts = refString.Split('/');
        return parts.Length >= 3 ? parts[2] : refString;
    }

    private string GetRefName(string? refString)
    {
        if (string.IsNullOrEmpty(refString))
            return string.Empty;

        var branchName = GetBranchName(refString);
        return branchName == refString ? "" : branchName;
    }

    private string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        return char.ToUpper(input[0]) + input[1..];
    }
}