using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace DesktopWidget
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeWebView2();
        }

        // Initialize WebView2 control and make it ready
        private async void InitializeWebView2()
        {
         
            await WebView.EnsureCoreWebView2Async(null);
            await LoadGitHubActivityAsync("ignatiosdev");
        }

        public async Task LoadGitHubActivityAsync(string username)
        {
 
            string activityJson = await GetGitHubActivityAsync(username);
            await ShowGitHubActivity(username, activityJson);
        }

        // Fetch activity from GitHub API
        public async Task<string> GetGitHubActivityAsync(string username)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "DesktopWidget");
                string apiUrl = $"https://api.github.com/users/{username}/events/public";
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode(); 
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
