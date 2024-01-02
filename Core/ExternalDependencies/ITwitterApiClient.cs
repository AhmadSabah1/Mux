namespace Core.ExternalDependencies;

public interface ITwitterApiClient
{
    Task<IEnumerable<TweetData>> GetTweetsAsync(string query);
    Task GetBearerTokenAsync();
}