using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitHub_User_Activity
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: github-activity <username>");
                return;
            }

            string username = args[0];
            string url = $"https://api.github.com/users/{username}/events/public";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "CSharpCLIApp");

                try
                {
                    Console.WriteLine($"Fetching recent activity for '{username}'...\n");

                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                    JsonElement root = doc.RootElement;

                    if (root.ValueKind != JsonValueKind.Array)
                    {
                        Console.WriteLine("Unexpected response format.");
                        return;
                    }

                    Console.WriteLine("Output:");

                    int count = 0;
                    foreach (JsonElement ev in root.EnumerateArray())
                    {
                        count++;
                        string type = ev.TryGetProperty("type", out JsonElement typeElement)
                            ? typeElement.GetString() ?? "UnknownEvent"
                            : "UnknownEvent";

                        string repo = ev.TryGetProperty("repo", out JsonElement repoElement) &&
                                      repoElement.TryGetProperty("name", out JsonElement nameElement)
                            ? nameElement.GetString() ?? "UnknownRepo"
                            : "UnknownRepo";

                        string message = FormatEvent(type, ev, repo);
                        Console.WriteLine($"- {message}");
                    }

                    if (count == 0)
                        Console.WriteLine("No recent activity found.");
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Error fetching data: {e.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e.Message}");
                }
            }
        }

        static string FormatEvent(string type, JsonElement ev, string repo)
        {
            return type switch
            {
                "PushEvent" => FormatPushEvent(ev, repo),
                "IssuesEvent" => FormatIssuesEvent(ev, repo),
                "WatchEvent" => $"Starred {repo}",
                "ForkEvent" => $"Forked {repo}",
                "CreateEvent" => $"Created something new in {repo}",
                "PullRequestEvent" => FormatPullRequestEvent(ev, repo),
                _ => $"Did {type.Replace("Event", "").ToLower()} in {repo}"
            };
        }

        static string FormatPushEvent(JsonElement ev, string repo)
        {
            if (ev.TryGetProperty("payload", out JsonElement payload) &&
                payload.TryGetProperty("size", out JsonElement size))
            {
                int commitCount = size.GetInt32();
                return $"Pushed {commitCount} commit{(commitCount == 1 ? "" : "s")} to {repo}";
            }
            else
            {

                return $"Pushed commit to {repo}";
            }
        }

        static string FormatIssuesEvent(JsonElement ev, string repo)
        {
            if (ev.TryGetProperty("payload", out JsonElement payload) &&
                payload.TryGetProperty("action", out JsonElement action))
            {
                string actionType = action.GetString() ?? "performed an action on";
                return $"{Capitalize(actionType)} a new issue in {repo}";
            }
            return $"Opened an issue in {repo}";
        }

        static string FormatPullRequestEvent(JsonElement ev, string repo)
        {
            if (ev.TryGetProperty("payload", out JsonElement payload) &&
                payload.TryGetProperty("action", out JsonElement action))
            {
                string actionType = action.GetString() ?? "performed";
                return $"{Capitalize(actionType)} a pull request in {repo}";
            }
            return $"Worked on a pull request in {repo}";
        }

        static string Capitalize(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return char.ToUpper(text[0]) + text.Substring(1);
        }
    }
}
