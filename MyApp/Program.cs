using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;

public class Program
{

    private static readonly HttpClient client = new HttpClient();

    public static async Task Main(string[] args)
    {

        string username = "<placeholder>";

        string url = $"https://api.github.com/users/Siyamukelo00Cebekulu/events";

        try
        {
            client.DefaultRequestHeaders.Add("User-Agent", "CSharpCLIApp");

            // Add your token
           
           

            Console.WriteLine($"Fetching recent activity for '{username}'...\n");

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();


            var events = JsonSerializer.Deserialize<List<GitHubEvent>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


            // Display simple formatted output
            foreach (var e in events)
            {
                switch (e.Type)
                {
                    case "PushEvent":
                        // Calculate number of commits
                        var pushEvents = events.GroupBy(e => e.Repo.Name);
                        foreach (var repoGroup in pushEvents)
                        {
                            int totalCommits = repoGroup.Sum(e => e.Payload.Commits?.Count ?? 0);
                            Console.WriteLine($"Pushed {totalCommits} commit(s) to {repoGroup.Key}");
                        }
                        break;

                    case "IssuesEvent":
                        Console.WriteLine($"Opened a new issue in {e.Repo.Name}");
                        break;

                    case "WatchEvent":
                        Console.WriteLine($"Starred {e.Repo.Name}");
                        break;

                    default:
                        Console.WriteLine($"{e.Actor.Login} did {e.Type} in {e.Repo.Name}");
                        break;
                }
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error fetching data: {e.Message}");
        }
    }
}