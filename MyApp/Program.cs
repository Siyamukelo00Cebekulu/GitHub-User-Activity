using System.CommandLine;

class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            var usernameOption = new Option<string>(
                name: "--username",
                description: "GitHub username to fetch activity for",
                getDefaultValue: () => string.Empty);

            var rootCommand = new RootCommand("GitHub Activity CLI - Fetch recent GitHub user activity");
            rootCommand.AddOption(usernameOption);

            rootCommand.SetHandler(async (username) =>
            {
                await HandleCommand(username);
            }, usernameOption);

            // For backward compatibility with the format: github-activity <username>
            if (args.Length > 0 && !args[0].StartsWith("--"))
            {
                args = new[] { "--username", args[0] };
            }

            return await rootCommand.InvokeAsync(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return 1;
        }
    }

    static async Task HandleCommand(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("Error: GitHub username is required.");
            Console.WriteLine("Usage: github-activity <username>");
            Console.WriteLine("       github-activity --username <username>");
            return;
        }

        try
        {
            Console.WriteLine($"Fetching recent activity for {username}...\n");

            var gitHubService = new GitHubService();
            var activityPrinter = new ActivityPrinter();

            var activities = await gitHubService.GetUserActivityAsync(username);
            
            if (activities != null && activities.Any())
            {
                activityPrinter.PrintActivities(activities);
                Console.WriteLine($"\nShowing {Math.Min(activities.Count, 10)} most recent activities.");
            }
            else
            {
                Console.WriteLine("No activity found or user does not exist.");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Network error: {ex.Message}");
            Console.WriteLine("Please check your internet connection and try again.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}