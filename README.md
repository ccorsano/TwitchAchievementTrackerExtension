# TwitchAchievementTrackerExtension
Backend for the Twitch XBox Achievement Tracker Extension.

Grab achievements from xapi.us for a given streamer and game, and display stats on the stream.

## How to run
This is a dotnetcore3.1 asp.net application, with a Dockerfile.

### Secrets
Secrets are loaded as configuration.
For development, use the User Secrets feature or the Development appsetting.json (but careful not to submit).
For deployment, Env variables will be loaded for configuration.

#### xapi.us configuration
- xapi.XApiKey : Private xapi.us Key
  ENV var name: xapi__XApiKey

#### Twitch configuration
- twitch.ExtensionSecret : Twitch extension secret key, used to validate Extension calls
  ENV var name: twitch__ExtensionSecret
