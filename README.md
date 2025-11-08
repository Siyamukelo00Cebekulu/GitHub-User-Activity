# 🧠 GitHub-User-Activity

## 📘 Overview

GitHub Activity CLI is a simple command-line interface (CLI) tool that fetches and displays the recent public activity of any GitHub user directly in your terminal.

This project demonstrates my ability to work with REST APIs, handle JSON data, and build clean, functional CLI applications using C#.
It’s designed as a small but practical tool to showcase programming fundamentals and real-world API integration.

## ⚙️ Features

- Fetches a user’s recent GitHub activity via the GitHub Events API
- Parses and filters the JSON response to extract relevant information.
- Displays the activity in a readable and organized format in the terminal.
- Handles network errors and invalid usernames gracefully.

## 🔁 Program Flow

1. ✍ User Input

The user runs the program and enters a GitHub username.

2. ⌯⌲⌯⌲Send API Request

The program constructs a request to the GitHub API endpoint and sends it using HttpClient :

```bash
https://api.github.com/users/{username}/events
```

3. 📥Receive JSON Response
- The GitHub API returns a JSON array containing a list of recent events (e.g., pushes, pull requests, stars).

4. ☰☰Parse & Filter Data
The JSON data is deserialized and filtered to show only relevant fields such as:

- Event type (PushEvent, PullRequestEvent, etc.)
- Repository name
- Event creation date

5. 📜 Display Results
The program formats and prints the data neatly to the terminal, providing a clear summary of the user’s latest activity.

## 🧰 Technologies Used

- C# (.NET 6 or later)
- HttpClient for sending HTTP requests
- System.Text.Json for parsing JSON data
- Command-line interface (CLI) for user interaction

## 🧑‍💻 Example Output

```yaml
nter GitHub username: Siyamukelo00Cebekulu
Fetching recent activity...

[1] PushEvent on repo: Siyamukelo00Cebekulu/TaskTracker
    Date: 2025-11-08T14:20:11Z

[2] CreateEvent on repo: Siyamukelo00Cebekulu/Portfolio
    Date: 2025-11-07T18:05:49Z

[3] WatchEvent on repo: dotnet/runtime
    Date: 2025-11-07T16:22:33Z
```