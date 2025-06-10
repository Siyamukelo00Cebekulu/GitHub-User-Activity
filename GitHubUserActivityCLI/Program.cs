class Program
{
    static readonly HttpClient client = new HttpClient();
    
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a GitHub username as an argument.");
            return;
        }

        string username = args[0];
        await DisplayUserActivity(username);
    }

    static async Task DisplayUserActivity(string username)
    {
        try
        {
            Console.WriteLine($"Fetching GitHub activity for {username}...");
            
            // We'll implement the API calls here
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error fetching data: {e.Message}");
        }
    }
}