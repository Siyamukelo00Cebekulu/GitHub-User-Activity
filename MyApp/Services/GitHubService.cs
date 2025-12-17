using System.Net.Http.Json;


public interface IGitHubService
{
    Task<List<GitHubEvent>?> GetUserActivityAsync(string username);
}

public class GitHubService : IGitHubService
{
    private readonly HttpClient _httpClient;

    public GitHubService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.github.com/"),
            DefaultRequestHeaders =
            {
                {"User-Agent", "GitHubActivityCLI/1.0"},
                {"Accept", "application/vnd.github.v3+json"}
            }
        };
    }

    public async Task<List<GitHubEvent>?> GetUserActivityAsync(string username)
    {
        try
        {
            var response = await _httpClient.GetAsync($"users/{username}/events");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to fetch GitHub activity: {response.StatusCode} - {error}");
            }

            var events = await response.Content.ReadFromJsonAsync<List<GitHubEvent>>();
            return events;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error fetching GitHub activity: {ex.Message}", ex);
        }
    }
}