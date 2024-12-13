using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL;

namespace GithubWidget
{
    public class GithubApiClient
    {
        private readonly GraphQLHttpClient _client;

        public GithubApiClient(string token)
        {
            _client = new GraphQLHttpClient("https://api.github.com/graphql", new NewtonsoftJsonSerializer());
            _client.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            _client.HttpClient.DefaultRequestHeaders.Add("User-Agent", "CSharp-GithubApiClient"); // Send github auth token
        }

        public async Task<ContributionCalendar> GetUserContributions(string userName)
        {
            var query = new GraphQLRequest
            {
                Query = @"
                query($userName: String!) { 
                  user(login: $userName){
                    contributionsCollection {
                      contributionCalendar {
                        totalContributions
                        weeks {
                          contributionDays {
                            contributionCount
                            date
                          }
                        }
                      }
                    }
                  }
                }",
                Variables = new
                {
                    userName
                }
            };

            try
            {
                var response = await _client.SendQueryAsync<GitHubResponse>(query);

                if (response.Errors != null && response.Errors.Any())
                {
                    throw new Exception(string.Join(", ", response.Errors.Select(e => e.Message)));
                }

                return response.Data.User.ContributionsCollection.ContributionCalendar;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }

    // This class models the entire response from the GitHub API
    public class GitHubResponse
    {
        public User User { get; set; } = new User(); // User data (contribution information)
    }

    // Represents a GitHub user and their associated data
    public class User
    {
        public ContributionsCollection ContributionsCollection { get; set; } = new ContributionsCollection();  // User's contributions collection
    }

    // Contains the contribution calendar of the user
    public class ContributionsCollection
    {
        public ContributionCalendar ContributionCalendar { get; set; } = new ContributionCalendar(); // Contribution data for the user
    }

    // Represents the calendar of contributions (e.g., total contributions, weeks)
    public class ContributionCalendar
    {
        public int TotalContributions { get; set; }  // Total contributions made by the user
        public List<Week> Weeks { get; set; } = new List<Week>(); // List of weeks with contribution data
    }

    // Represents a week in the user's contribution calendar
    public class Week
    {
        public List<ContributionDay> ContributionDays { get; set; } = new List<ContributionDay>(); // List of contribution days in the week
    }

    // Represents a single day in the contribution calendar
    public class ContributionDay
    {
        public int ContributionCount { get; set; }  // Number of contributions made on this day
        public string Date { get; set; } = string.Empty;  // Date of the contributions in the format YYYY-MM-DD
    }
}