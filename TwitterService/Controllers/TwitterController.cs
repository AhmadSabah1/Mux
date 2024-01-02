using System.Net.Http.Headers;
using System.Text;
using Core.ExternalDependencies;
using Microsoft.AspNetCore.Mvc;
using Tweetinvi;
using Tweetinvi.Parameters.V2;

namespace TwitterService.Controllers;

[ApiController]
[Route("[controller]")]
public class TwitterController : ControllerBase
{
    private readonly TwitterApiClient _twitterApiClient;

    public TwitterController(TwitterApiClient twitterApiClient)
    {
        _twitterApiClient = twitterApiClient;
    }

    [HttpGet(Name = "Search")]
    public async Task<IActionResult> Search()
    {
        await _twitterApiClient.GetBearerTokenAsync();

        var tweets = await _twitterApiClient.GetTweetsAsync("hej men mux");

        return Ok(tweets);
    }
}