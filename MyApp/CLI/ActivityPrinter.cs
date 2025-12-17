using System.Text;


public interface IActivityPrinter
{
    void PrintActivities(List<GitHubEvent> events);
}

public class ActivityPrinter : IActivityPrinter
{
    private const int MaxActivitiesToShow = 15;

    public void PrintActivities(List<GitHubEvent> events)
    {
        if (events == null || events.Count == 0)
        {
            Console.WriteLine("No recent activity found.");
            return;
        }

        Console.WriteLine($"Recent GitHub Activity:\n");

        int count = 0;
        foreach (var activity in events)
        {
            if (count >= MaxActivitiesToShow)
                break;

            var description = GetActivityDescription(activity);
            if (!string.IsNullOrEmpty(description))
            {
                Console.WriteLine($"- {description}");
                count++;
            }
        }

        if (count == 0)
        {
            Console.WriteLine("No recognizable activity found in the recent events.");
        }
    }

    private string GetActivityDescription(GitHubEvent activity)
    {
        var repoName = activity.Repository?.Name ?? "unknown repository";
        var actorName = activity.Actor?.Login ?? "someone";
        
        try
        {
            return activity.Type switch
            {
                "PushEvent" => GetPushEventDescription(activity, repoName),
                "IssuesEvent" => GetIssueEventDescription(activity, repoName),
                "IssueCommentEvent" => GetIssueCommentEventDescription(activity, repoName),
                "WatchEvent" => $"⭐ Starred {repoName}",
                "CreateEvent" => GetCreateEventDescription(activity, repoName),
                "DeleteEvent" => GetDeleteEventDescription(activity, repoName),
                "ForkEvent" => $"🍴 Forked {repoName}",
                "PullRequestEvent" => GetPullRequestEventDescription(activity, repoName),
                "PullRequestReviewEvent" => GetPullRequestReviewEventDescription(activity, repoName),
                "PullRequestReviewCommentEvent" => $"💬 Reviewed a pull request in {repoName}",
                "CommitCommentEvent" => $"💬 Commented on a commit in {repoName}",
                "ReleaseEvent" => GetReleaseEventDescription(activity, repoName),
                "MemberEvent" => GetMemberEventDescription(activity, repoName),
                "PublicEvent" => $"📢 Made {repoName} public",
                "GollumEvent" => $"📝 Updated wiki in {repoName}",
                "ProjectCardEvent" => $"📋 Updated project card in {repoName}",
                "ProjectColumnEvent" => $"📋 Updated project column in {repoName}",
                "ProjectEvent" => $"📋 Updated project in {repoName}",
                "TeamAddEvent" => $"👥 Added to team in {repoName}",
                "OrganizationEvent" => GetOrganizationEventDescription(activity, repoName),
                _ => GetGenericEventDescription(activity, repoName)
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing activity: {ex.Message}");
            return $"Performed {activity.Type.ToLower()} in {repoName}";
        }
    }

    private string GetPushEventDescription(GitHubEvent activity, string repoName)
    {
        // GitHub API returns commit count in 'size' property for PushEvent
        var commitCount = activity.Payload?.Size ?? 0;
        var distinctCount = activity.Payload?.DistinctSize ?? 0;
        
        if (commitCount == 0)
        {
            // Sometimes commits might be in the Commits list instead
            commitCount = activity.Payload?.Commits?.Count ?? 0;
        }

        var branch = GetBranchName(activity.Payload?.Ref);
        var branchInfo = string.IsNullOrEmpty(branch) ? "" : $" ({branch})";

        if (commitCount == 0)
        {
            return $"Pushed to {repoName}{branchInfo}";
        }
        else if (distinctCount > 0 && distinctCount != commitCount)
        {
            return $"Pushed {commitCount} commits ({distinctCount} distinct) to {repoName}{branchInfo}";
        }
        else
        {
            return $"Pushed {commitCount} commit{(commitCount != 1 ? "s" : "")} to {repoName}{branchInfo}";
        }
    }

    private string GetIssueEventDescription(GitHubEvent activity, string repoName)
    {
        var action = activity.Payload?.Action?.ToLower() ?? "modified";
        var actionText = action switch
        {
            "opened" => "📝 Opened",
            "closed" => "✅ Closed",
            "reopened" => "🔄 Reopened",
            "assigned" => "👤 Assigned",
            "unassigned" => "👤 Unassigned",
            "labeled" => "🏷️ Labeled",
            "unlabeled" => "🏷️ Unlabeled",
            "edited" => "✏️ Edited",
            "milestoned" => "🎯 Milestoned",
            "demilestoned" => "🎯 Demilestoned",
            "locked" => "🔒 Locked",
            "unlocked" => "🔓 Unlocked",
            _ => CapitalizeFirstLetter(action)
        };
        
        return $"{actionText} an issue in {repoName}";
    }

    private string GetIssueCommentEventDescription(GitHubEvent activity, string repoName)
    {
        var action = activity.Payload?.Action?.ToLower() ?? "commented";
        var actionText = action switch
        {
            "created" => "💬 Commented",
            "edited" => "✏️ Edited comment",
            "deleted" => "🗑️ Deleted comment",
            _ => CapitalizeFirstLetter(action)
        };
        
        return $"{actionText} on an issue in {repoName}";
    }

    private string GetCreateEventDescription(GitHubEvent activity, string repoName)
    {
        var refType = activity.Payload?.RefType?.ToLower() ?? "resource";
        var emoji = refType switch
        {
            "branch" => "🌿",
            "tag" => "🏷️",
            "repository" => "📁",
            _ => "📝"
        };
        
        var refName = GetRefName(activity.Payload?.Ref);
        var refInfo = string.IsNullOrEmpty(refName) ? "" : $" '{refName}'";
        
        return $"{emoji} Created {refType}{refInfo} in {repoName}";
    }

    private string GetDeleteEventDescription(GitHubEvent activity, string repoName)
    {
        var refType = activity.Payload?.RefType?.ToLower() ?? "resource";
        var refName = GetRefName(activity.Payload?.Ref);
        var refInfo = string.IsNullOrEmpty(refName) ? "" : $" '{refName}'";
        
        return $"🗑️ Deleted {refType}{refInfo} in {repoName}";
    }

    private string GetPullRequestEventDescription(GitHubEvent activity, string repoName)
    {
        var action = activity.Payload?.Action?.ToLower() ?? "modified";
        var actionText = action switch
        {
            "opened" => "📦 Opened",
            "closed" => action == "closed" && activity.Payload?.PullRequest != null 
                ? (IsPullRequestMerged(activity.Payload.PullRequest) ? "✅ Merged" : "❌ Closed")
                : "❌ Closed",
            "reopened" => "🔄 Reopened",
            "edited" => "✏️ Edited",
            "assigned" => "👤 Assigned",
            "unassigned" => "👤 Unassigned",
            "review_requested" => "👁️ Review requested",
            "review_request_removed" => "👁️ Review request removed",
            "ready_for_review" => "👁️ Ready for review",
            "labeled" => "🏷️ Labeled",
            "unlabeled" => "🏷️ Unlabeled",
            "synchronize" => "🔄 Updated",
            _ => CapitalizeFirstLetter(action)
        };
        
        return $"{actionText} a pull request in {repoName}";
    }

    private string GetPullRequestReviewEventDescription(GitHubEvent activity, string repoName)
    {
        var action = activity.Payload?.Action?.ToLower() ?? "reviewed";
        var actionText = action switch
        {
            "submitted" => "👁️ Reviewed",
            "edited" => "✏️ Edited review",
            "dismissed" => "❌ Dismissed review",
            _ => CapitalizeFirstLetter(action)
        };
        
        return $"{actionText} a pull request in {repoName}";
    }

    private string GetReleaseEventDescription(GitHubEvent activity, string repoName)
    {
        var action = activity.Payload?.Action?.ToLower() ?? "published";
        var actionText = action switch
        {
            "published" => "🚀 Published",
            "unpublished" => "🚀 Unpublished",
            "created" => "🚀 Created",
            "edited" => "✏️ Edited",
            "deleted" => "🗑️ Deleted",
            "prereleased" => "🚀 Pre-released",
            "released" => "🚀 Released",
            _ => CapitalizeFirstLetter(action)
        };
        
        return $"{actionText} a release in {repoName}";
    }

    private string GetMemberEventDescription(GitHubEvent activity, string repoName)
    {
        var action = activity.Payload?.Action?.ToLower() ?? "added";
        var actionText = action switch
        {
            "added" => "👥 Added member to",
            "removed" => "👥 Removed member from",
            "edited" => "👥 Edited member in",
            _ => CapitalizeFirstLetter(action)
        };
        
        return $"{actionText} {repoName}";
    }

    private string GetOrganizationEventDescription(GitHubEvent activity, string repoName)
    {
        var action = activity.Payload?.Action?.ToLower() ?? "updated";
        return $"{CapitalizeFirstLetter(action)} organization {repoName}";
    }

    private string GetGenericEventDescription(GitHubEvent activity, string repoName)
    {
        var readableType = activity.Type
            .Replace("Event", "")
            .Replace("([A-Z])", " $1")
            .ToLower();
        
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

    private bool IsPullRequestMerged(object? pullRequest)
    {
        // This is a simplified check. In a real implementation,
        // you'd want to deserialize the pull_request object properly
        try
        {
            var prJson = pullRequest?.ToString();
            return prJson?.Contains("\"merged\": true") == true || 
                   prJson?.Contains("'merged': True") == true;
        }
        catch
        {
            return false;
        }
    }

    private string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        return char.ToUpper(input[0]) + input[1..];
    }
}