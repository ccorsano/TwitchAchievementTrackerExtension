# TwitchAchievementTrackerExtension
Backend for the Twitch XBox Achievement Tracker Extension.

Grab achievements from xapi.us for a given streamer and game, and display stats on the stream.

## How to run
This is a dotnetcore3.1 asp.net application, with a Dockerfile.

### Pre-requisites
- dotnetcore 3.1
- A code editor that works well with dotnetcore
  - Visual Studio Code
  - Visual Studio
- optionally, docker
- Twitch Developer Rig to run the extension

### Development mode
#### Visual Studio
Launch the application TwitchAchievementBackend, it will start and listen on https 8080 / http 8081 (which is what the Twitch Developer Rig uses with the extension project).

#### dotnet cli (multiplatform)

cd to the project directory (TwitchAchievementTrackerExtension/TwitchAchievementTrackerExtension)
`dotnet run` 

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
