using System.CommandLine;

class Program
{
    /*
    The main entry point for the GitHub Activity CLI application.
    
    This method initializes the command-line interface, parses arguments, and
    executes the appropriate logic to fetch and display GitHub user activity.
    
    Returns: An integer exit code (0 for success, non-zero for errors).
    Parameters: args - Command-line arguments passed to the application.
    */
    static async Task<int> Main(string[] args)
    {
        try
        {
            /*
            Create a command-line option for the GitHub username.
            This defines how users can specify which GitHub user's activity to fetch.
            
            Properties:
                name: The command-line flag name ("--username")
                description: Help text shown to users
                getDefaultValue: Default value when no username is provided
            */
            var usernameOption = new Option<string>(
                name: "--username",
                description: "GitHub username to fetch activity for",
                getDefaultValue: () => string.Empty);

            /*
            Create the root command for the application.
            This represents the main command that users invoke.
            */
            var rootCommand = new RootCommand("GitHub Activity CLI - Fetch recent GitHub user activity");
            
            // Add the username option to the root command
            rootCommand.AddOption(usernameOption);

            /*
            Set up the handler that executes when the command is invoked.
            This connects the parsed username to the business logic handler.
            */
            rootCommand.SetHandler(async (username) =>
            {
                await HandleCommand(username);
            }, usernameOption);

            /*
            Handle backward compatibility for the original command format.
            If the first argument doesn't start with "--", assume it's a username
            and reformat it to use the --username flag.
            
            This allows both formats to work:
                github-activity Siyamukelo00Cebekulu
                github-activity --username Siyamukelo00Cebekulu
            */
            if (args.Length > 0 && !args[0].StartsWith("--"))
            {
                args = new[] { "--username", args[0] };
            }

            /*
            Execute the command with the provided arguments.
            This parses the arguments, validates them, and runs the handler.
            */
            return await rootCommand.InvokeAsync(args);
        }
        catch (Exception ex)
        {
            /*
            Catch any unexpected exceptions at the top level.
            This ensures the application doesn't crash silently and provides
            a user-friendly error message.
            */
            Console.WriteLine($"Unexpected error: {ex.Message}");
            
            /*
            Return error code 1 to indicate failure.
            Standard convention: 0 = success, non-zero = failure.
            */
            return 1;
        }
    }

    /*
    Handles the business logic for fetching and displaying GitHub activity.
    
    This method validates the username, calls the GitHub API, processes the response,
    and displays the results to the user.
    
    Parameters: username - The GitHub username to fetch activity for.
    */
    static async Task HandleCommand(string username)
    {
        /*
        Validate that a username was provided.
        Check for null, empty, or whitespace-only strings.
        */
        if (string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("Error: GitHub username is required");
            Console.WriteLine("Usage: github-activity <username>");
            Console.WriteLine("       github-activity --username <username>");
            return; // Exit early if no username provided
        }

        try
        {
            /*
            Inform the user that we're fetching their data.
            This provides feedback that the application is working.
            */
            Console.WriteLine($"Fetching recent activity for {username}...\n");

            /*
            Create instances of the services needed to fetch and display activities.
            
            GitHubService: Responsible for making API calls to GitHub
            ActivityPrinter: Responsible for formatting and displaying the results
            */
            var gitHubService = new GitHubService();
            var activityPrinter = new ActivityPrinter();

            /*
            Fetch the user's activity data from GitHub.
            This is an asynchronous call that may take some time depending on
            network speed and GitHub API response time.
            */
            var activities = await gitHubService.GetUserActivityAsync(username);
            
            /*
            Check if we successfully retrieved any activities.
            The activities list could be null or empty for various reasons:
                - User doesn't exist
                - User has no public activity
                - API rate limit exceeded
                - Network issues
            */
            if (activities != null && activities.Any())
            {
                /*
                Display the formatted activities to the user.
                The ActivityPrinter handles grouping, formatting, and output.
                */
                activityPrinter.PrintActivities(activities);
                
                /*
                Provide feedback on how many activities are being shown.
                We limit display to prevent overwhelming the user with too much data.
                */
                Console.WriteLine($"\nShowing {Math.Min(activities.Count, 10)} most recent activities.");
            }
            else
            {
                /*
                Handle the case where no activities were found.
                This could mean the user doesn't exist or has no public activity.
                */
                Console.WriteLine("No activity found or user does not exist.");
            }
        }
        catch (HttpRequestException ex)
        {
            /*
            Handle network-related errors specifically.
            HttpRequestException is thrown when there are issues with the HTTP request,
            such as network connectivity problems or DNS resolution failures.
            */
            Console.WriteLine($"Network error: {ex.Message}");
            Console.WriteLine("Please check your internet connection and try again.");
        }
        catch (Exception ex)
        {
            /*
            Catch any other unexpected errors.
            This is a safety net to prevent unhandled exceptions.
            */
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}