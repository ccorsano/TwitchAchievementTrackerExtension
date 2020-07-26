using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NBomber;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Services;

namespace TwitchAchievementTrackerLoadTestAgent
{
    public class TwitchUser
    {
        public string opaque_id { get; set; }
        public string jwtToken { get; set; }
    }

    public class TestAgent : IHostedService
    {
        private TwitchEBSService _twitchEBSService;
        private string _secret;
        public int _userIndex = 0;

        public TestAgent(IConfiguration configuration, TwitchEBSService twitchEBSService)
        {
            _twitchEBSService = twitchEBSService;
            _secret = configuration.GetValue<string>("config:EncryptionSecret");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var serverBase = "https://twitchext.conceptoire.com/v4";
            //var serverBase = "https://localhost:32768";
            var titleUri = new Uri($"{serverBase}/api/title");
            var summaryUri = new Uri($"{serverBase}/api/achievements/summary");
            var achievementsUri = new Uri($"{serverBase}/api/achievements/list");

            var testVersion = "2020.4";
            var testConfiguration = "dj4ftiN0Y8+XCxbqPOjpvLJCVTKcgWwpR7VMzyDzhJWuGoKXD3UMlWXowjNaFuWyJU3xKSJoIQpEzFHooOUpr5XyNHaXug0Evxo64UDpeOOR6iWPmcwPJF37UzSJXVchcjaqUeS7R6WNZ1+NQFBR4Qy8kE8xyxH+AcBQr1ejj9goU4CsWJrCQL/jfzi33ZbuW4/shh3c3OamqIgGNXf5gXJEqk+fkLYAg+mOSHzQMZ3Eq9xk11/sYfzHxjcG/qXgs0TB3wXz3zbrvSvrzQFm9LosVD5BQsGoNZU530dn6Yhl35aHFBo7QqXSJQOodb7fkJeDajuhBJZZ+aMv/q1+VQ==";

            var feedData = FeedData.FromSeq(GetUsers("158511925"));
            var feed = Feed.CreateConstant("user", feedData);

            var activateExtensionStep = HttpStep.Create("GetTitleInfo request", feed, context =>
                Http.CreateRequest("GET", titleUri.AbsoluteUri)
                .WithHeader("X-Config-Version", testVersion)
                .WithHeader("X-Config-Token", testConfiguration)
                .WithHeader("Authorization", $"Bearer {context.FeedItem.jwtToken}")
                .WithCheck(response => Task.FromResult(response.IsSuccessStatusCode))
            );
            var getSummaryStep = HttpStep.Create("Get Summary request", feed, context =>
                Http.CreateRequest("GET", summaryUri.AbsoluteUri)
                .WithHeader("X-Config-Version", testVersion)
                .WithHeader("X-Config-Token", testConfiguration)
                .WithHeader("Authorization", $"Bearer {context.FeedItem.jwtToken}")
                .WithCheck(response => Task.FromResult(response.IsSuccessStatusCode))
            );
            var getAchievementsStep = HttpStep.Create("Get Achievements request", feed, context =>
                Http.CreateRequest("GET", achievementsUri.AbsoluteUri)
                .WithHeader("X-Config-Version", testVersion)
                .WithHeader("X-Config-Token", testConfiguration)
                .WithHeader("Authorization", $"Bearer {context.FeedItem.jwtToken}")
                .WithCheck(response => Task.FromResult(response.IsSuccessStatusCode))
            );

            var viewerScenario = ScenarioBuilder.CreateScenario("Viewer", new NBomber.Contracts.IStep[]
            {
                activateExtensionStep,
                getSummaryStep,
                getAchievementsStep,
                Step.CreatePause(TimeSpan.FromMinutes(1)),
                getSummaryStep,
                Step.CreatePause(TimeSpan.FromMinutes(1)),
                getSummaryStep,
                Step.CreatePause(TimeSpan.FromMinutes(1)),
                getSummaryStep,
                getAchievementsStep,
            });

            

            return Task.Run(() => NBomberRunner
                .RegisterScenarios(viewerScenario)
                .Run()
            );
        }

        IEnumerable<TwitchUser> GetUsers(string channelId)
        {
            if (_userIndex >= 50)
            {
                yield break;
            }
            var newUser = new TwitchUser
            {
                opaque_id = "loadtest_" + _userIndex,
                jwtToken = _twitchEBSService.GetUserJWTToken("loadtest_" + _userIndex, channelId, "viewer"),
            };
            yield return newUser;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
