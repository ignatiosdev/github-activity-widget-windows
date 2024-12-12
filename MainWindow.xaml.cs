using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace DesktopWidget
{
    public partial class MainWindow : Window
    {
        // Constructor for the MainWindow class
        public MainWindow()
        {
            InitializeComponent();
            // Use an async method to load GitHub activity
            InitializeWebView2();
        }

        // Initialize WebView2 control and make it ready
        private async void InitializeWebView2()
        {
            // Wait for WebView2 to initialize
            await WebView.EnsureCoreWebView2Async(null);
            // Load GitHub activity after WebView2 is ready
            await LoadGitHubActivityAsync("ignatiosdev");
        }

        // Method to load GitHub activity for a given username
        public async Task LoadGitHubActivityAsync(string username)
        {
            // Fetch GitHub activity data and handle it
            string activityJson = await GetGitHubActivityAsync(username);

            // Pass the data to WebView2 to display
            await ShowGitHubActivity(username, activityJson);
        }

        // Fetch activity from GitHub API
        public async Task<string> GetGitHubActivityAsync(string username)
        {
            using (HttpClient client = new HttpClient())
            {
                // Add the user-agent header to the request.
                client.DefaultRequestHeaders.Add("User-Agent", "DesktopWidget");

                // GitHub API endpoint for public events
                string apiUrl = $"https://api.github.com/users/{username}/events/public";

                // Send the GET request to GitHub's API and get the response.
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode(); // Ensure a successful response.

                // Return the JSON response as a string.
                string responseData = await response.Content.ReadAsStringAsync();
                return responseData;
            }
        }

        // Display GitHub activity in WebView2 control
        public async Task ShowGitHubActivity(string username, string activityJson)
        {
            string htmlContent = $@"
    <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                }}
                .activity {{
                    margin-bottom: 15px;
                    padding: 10px;
                    border: 1px solid #ddd;
                }}
            </style>
        </head>
        <body>
            <h2>GitHub Activity for {username}</h2>
            <pre>{activityJson}</pre> <!-- Display raw JSON data for now -->
        </body>
    </html>";

            // Display the HTML in WebView2
            WebView.CoreWebView2.NavigateToString(htmlContent);
        }
    }
}
