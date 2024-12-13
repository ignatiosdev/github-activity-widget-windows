using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace GithubWidget
{
    public partial class MainWindow : Window
    {
        private const string GitHubToken = "";
        private const string GitHubUsername = "ignatiosdev";


        public MainWindow()
        {
            InitializeComponent();
            InitializeWebView2();
            RedirectConsoleToWebView();
            GetGithubActivity();  
        }

        // Initialize WebView2 control and make it ready
        private async void InitializeWebView2()
        {
            await WebView.EnsureCoreWebView2Async(null);
        }

        // CONSOLE TO WEBVIEW LOGIC

        private void RedirectConsoleToWebView()
        {
            // Ensure CoreWebView2 is initialized before setting the custom TextWriter
            WebView.CoreWebView2InitializationCompleted += (sender, args) =>
            {
                // Pass CoreWebView2 to WebView2TextWriter after initialization
                Console.SetOut(new WebView2TextWriter(WebView.CoreWebView2));
            };
        }

        public class WebView2TextWriter : TextWriter
        {
            private readonly CoreWebView2 _coreWebView2;
            public WebView2TextWriter(CoreWebView2 coreWebView2)
            {
                _coreWebView2 = coreWebView2;
            }
            public override void Write(string value)
            {
                // Use WebView2's ExecuteScriptAsync to log to browser's console
                if (_coreWebView2 != null)
                {
                    string script = $"console.log('{value.Replace("'", "\\'")}');";
                    _coreWebView2.ExecuteScriptAsync(script);
                }
            }

            public override Encoding Encoding => Encoding.UTF8;
        }

       
        // GITHUB ACTIVITY REQUEST
        private static async void GetGithubActivity()
        {
            try
            {
                // Create an instance of the GitHubGraphQLClient with the token
                var gitHubClient = new GithubApiClient(GitHubToken);

                // Fetch user contributions using the GraphQL client
                var contributions = await gitHubClient.GetUserContributions(GitHubUsername);

                // Log the total contributions to the console
                Console.WriteLine($"GitHub Activity for {GitHubUsername}:");
                Console.WriteLine($"Total Contributions: {contributions.TotalContributions}");

                // Log the contributions for each week
                foreach (var week in contributions.Weeks)
                {
                    foreach (var day in week.ContributionDays)
                    {
                        string message = ($"Date: {day.Date}, Contributions: {day.ContributionCount}");
                        Console.WriteLine(message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors (e.g., network issues, invalid token)
                Console.WriteLine($"An error occurred: {ex.Message}");
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
