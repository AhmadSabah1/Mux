using System.Net.Http.Headers;
using System.Text;
using Core.ExternalDependencies;
using Newtonsoft.Json.Linq;

public class TwitterApiClient : ITwitterApiClient
{
    private readonly string _apiKey;
    private readonly string _apiKeySecret;
    private readonly string _tokenEndpoint;
    private string _bearerToken;
    
    public TwitterApiClient(string apiKey, string apiKeySecret, string tokenEndpoint)
    {
        _apiKey = apiKey;
        _apiKeySecret = apiKeySecret;
        _tokenEndpoint = tokenEndpoint;
    }
    
    public async Task<IEnumerable<TweetData>> GetTweetsAsync(string query)
    {
        try
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);

            var response = await httpClient.GetAsync(
                $"https://api.twitter.com/2/tweets/search/recent?query={query}&expansions=author_id&user.fields=name");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);

            var authors = json["includes"]["users"]
                .ToDictionary(u => u["id"].ToString(), u => u["name"].ToString());

            var tweets = json["data"].Select(t =>
                new TweetData(
                    authors[t["author_id"].ToString()],
                    t["text"].ToString(),
                    t["attachments"]["media_keys"].Select(m => $"https://twitter.com/i/status/{t["id"]}/photo/{m}")
                )
            );

            return tweets;
        }
        catch (Exception e)
        {
            var fakeTweets = new List<TweetData>
            {
                new("Mux", "Hej med dig Mux", new List<string>
                {
                    "google.com"
                }),
                new("Ahmad", "Hej med dig Ahmad", new List<string>
                {
                    "google.com"
                })
            };
            Console.WriteLine(e.Message);

            return fakeTweets;
        }
    }

    public async Task GetBearerTokenAsync()
    {
        using (var client = new HttpClient())
        {
            var combinedKeys = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_apiKey}:{_apiKeySecret}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", combinedKeys);

            var content = new StringContent("grant_type=client_credentials", Encoding.UTF8,
                "application/x-www-form-urlencoded");
            var response = await client.PostAsync(_tokenEndpoint, content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var jObject = JObject.Parse(jsonResponse);
            var bearerToken = jObject["access_token"].ToString();

            _bearerToken = bearerToken;
        }
    }
}