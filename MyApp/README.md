# 🚀 Quick Start: Test the GitHub Activity CLI
Option 1: Run as a .NET Application (Easiest for Testing)
If you have .NET 9.0+ installed, clone and run directly:

## 1. Clone the repository

```bash
git clone https://github.com/Siyamukelo00Cebekulu/GitHub-User-Activity.git
cd GitHub-User-Activity
```

## 2. Build the project
```bash
dotnet build
```

## 3. Run with any GitHub username
```bash
dotnet run -- Siyamukelo00Cebekulu
```

## Or using the explicit flag format
```bash
dotnet run -- --username Siyamukelo00Cebekulu
```
## Example Output:
```yaml
text
Fetching recent activity for Siyamukelo00Cebekulu...

Recent GitHub Activity:

- Pushed 5 commits to Siyamukelo00Cebekulu/Siyamukelo00Cebekulu (main)
- Pushed 6 commits to Siyamukelo00Cebekulu/shiny-octo-train (main)
- Created branch 'main' in Siyamukelo00Cebekulu/shiny-octo-train
- Pushed 4 commits to Siyamukelo00Cebekulu/GitHub-User-Activity (master)
- Pushed 12 commits to Siyamukelo00Cebekulu/react-native-technical-assessment (main)
- Made repository Siyamukelo00Cebekulu/react-native-technical-assessment public
```

# Option 2: Install as Global Tool (For Frequent Use)

## 1. Clone the repository
```bash
git clone https://github.com/Siyamukelo00Cebekulu/GitHub-User-Activity.git
cd GitHub-User-Activity
```
## 2. Build and create the tool package
```bash
dotnet pack
```

## 3. Install globally (requires .NET SDK)
```bash
dotnet tool install --global --add-source ./nupkg MyApp
```

## 4. Run from anywhere!
```bash
github-activity Siyamukelo00Cebekulu
💡 Pro Tip: Test with popular GitHub users:
```

## With explicit flag
```bash
github-activity --username Siyamukelo00Cebekulu
```

# Help command
```bash
github-activity --help
```

# Test error handling
```bash
github-activity  # No username (should show usage)
github-activity nonexistentuser12345  # Invalid user
```

# 📝 Quick Reference Card

## Run once	
```bash 
dotnet run -- username
```
## Install globally	
```bash
dotnet tool install --global --add-source ./nupkg MyApp
```

## Update tool	
```bash
dotnet tool update --global --add-source ./nupkg MyApp
```
## Uninstall
```bash	
dotnet tool uninstall -g MyApp
```
## Check version
```bash	
github-activity --version
```
## Get help
```bash	
github-activity --help
```