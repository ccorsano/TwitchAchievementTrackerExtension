using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TwitchAchievementTrackerBackend.Configuration;
using TwitchAchievementTrackerBackend.Model;
using TwitchAchievementTrackerBackend.Services;

string channelId = args[0];

DateTimeOffset EPOCH = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);


ConfigurationBuilder builder = new();
builder.AddUserSecrets<Program>();
IConfiguration config = builder.Build();

ServiceCollection services = new ServiceCollection();
services.AddHttpClient();
services.AddLogging();
services.Configure<ConfigurationTokenOptions>(config);
services.AddSingleton<ConfigurationTokenService>();

ServiceProvider provider = services.BuildServiceProvider();

var options = config.Get<Settings>()!;

var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(options.ExtensionSecret!));
var jwtSigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

var exp = DateTimeOffset.UtcNow - EPOCH;

var tokenObj = new JwtSecurityToken(null, null, null, null, DateTime.UtcNow.AddDays(1), jwtSigningCredentials);
tokenObj.Payload["channel_id"] = channelId;
tokenObj.Payload["user_id"] = channelId;
tokenObj.Payload["role"] = "external";
tokenObj.Header.Remove("typ");

string token = new JwtSecurityTokenHandler().WriteToken(tokenObj);

Console.WriteLine(token);

var httpClient = provider.GetRequiredService<HttpClient>();
httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
httpClient.DefaultRequestHeaders.Add("Client-Id",options.ExtensionId);

var response = await httpClient.GetAsync($"https://api.twitch.tv/helix/extensions/configurations?extension_id={options.ExtensionId}&segment=broadcaster&broadcaster_id={channelId}");

response.EnsureSuccessStatusCode();
var segmentResponse = await response.Content.ReadFromJsonAsync(SourceGenerationContext.Default.ExtensionSegmentResponse);
var segment = segmentResponse!.data[0];

Console.WriteLine(segmentResponse);

ConfigurationTokenService tokenService = provider.GetRequiredService<ConfigurationTokenService>();
var tokenContent = tokenService.DecodeConfigurationToken(Convert.FromBase64String(segment.content));

Console.WriteLine(JsonSerializer.Serialize(tokenContent, SourceGenerationContext.Default.ExtensionConfiguration));

tokenContent.SteamConfig.Locale = "english";

string newContent = Convert.ToBase64String(tokenService.EncodeConfigurationToken(tokenContent));
Console.WriteLine(newContent);

ExtensionSegmentUpdate segmentUpdate = new(
    extension_id: options.ExtensionId,
    broadcaster_id: segment.broadcaster_id,
    segment: segment.segment,
    version: segment.version,
    content: newContent);

var editResponse = await httpClient.PutAsJsonAsync($"https://api.twitch.tv/helix/extensions/configurations", segmentUpdate, SourceGenerationContext.Default.ExtensionSegmentUpdate);

editResponse.EnsureSuccessStatusCode();

record Settings(string ExtensionId, string ExtensionSecret);
record ExtensionConfigSegment (string segment, string broadcaster_id, string content, string version);
record ExtensionSegmentResponse (ExtensionConfigSegment[] data);
record ExtensionSegmentUpdate(string extension_id, string segment, string broadcaster_id, string version, string content);

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ExtensionSegmentResponse))]
[JsonSerializable(typeof(ExtensionSegmentUpdate))]
[JsonSerializable(typeof(ExtensionConfiguration))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}