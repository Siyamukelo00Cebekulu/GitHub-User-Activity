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

1. ✍ **User Input**

    The user runs the program and enters a GitHub username.

2. ⌯⌲**Send API Request**

    The program constructs a request to the GitHub API endpoint and sends it using HttpClient:

```bash
https://api.github.com/users/{username}/events
```

3. 📥**Receive JSON Response**

    The GitHub API returns a JSON array containing a list of recent events (e.g., pushes, pull requests, stars).

4. ☰**Parse & Filter Data**

    The JSON data is deserialized and filtered to show only relevant fields such as:
    - Event type (PushEvent, PullRequestEvent, etc.)
    - Repository name
    - Event creation date

5. 📜 **Display Results**

    The program formats and prints the data neatly to the terminal, providing a clear summary of the user’s latest activity.

## 🧰 Technologies and Libraries Used

- C# (.NET 6 or later)
- HttpClient for sending HTTP requests
- System.Text.Json for parsing JSON data
- System.Text.Json.Serialization
- System.CommandLine
- Command-line interface (CLI) for user interaction

## 🧑‍💻 Example Output

```yaml
Fetching recent activity for Siyamukelo00Cebekulu...

Recent GitHub Activity:

- Pushed 5 commits to Siyamukelo00Cebekulu/Siyamukelo00Cebekulu (main)
- Pushed 6 commits to Siyamukelo00Cebekulu/shiny-octo-train (main)
- Created branch 'main' in Siyamukelo00Cebekulu/shiny-octo-train
- Pushed 4 commits to Siyamukelo00Cebekulu/GitHub-User-Activity (master)
- Pushed 12 commits to Siyamukelo00Cebekulu/react-native-technical-assessment (main)
- Made repository Siyamukelo00Cebekulu/react-native-technical-assessment public
```

# Submission URL

[label](https://roadmap.sh/projects/github-user-activity)