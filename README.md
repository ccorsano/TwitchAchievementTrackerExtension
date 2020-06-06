# TwitchAchievementTrackerExtension : Frontend

TODO: document Twitch extension frontend, just static html / js files...

# TwitchAchievementTrackerExtension : Backend
Backend for the Twitch XBox Achievement Tracker Extension.

Grab achievements from xapi.us for a given streamer and game, and display stats on the stream.

## How to run
This is a dotnetcore3.1 asp.net application, with a Dockerfile to ease deployments.
As it is completely stateless it would be well-suited for a serverless / SaaS deployment, but the project is currently not structured for that.

### Pre-requisites
- dotnetcore 3.1
- A code editor that works well with dotnetcore
  - Visual Studio Code
  - Visual Studio
- optionally, docker
- Twitch Developer Rig to run the extension

### Using Twitch Developer Rig

There is a Twitch Extension manifest included, configured for Rig local file hosting, and to launch the dotnet backend as a backend.
Just run the local files and backend, after having configured the secrets, and you should be set.

### Development mode

Check the section about secrets before being able to run the backend !

#### Visual Studio
Launch the application TwitchAchievementBackend, it will start and listen on https 8080 / http 8081 (which is what the Twitch Developer Rig uses with the extension project).

#### dotnet cli (multiplatform)

```
cd TwitchAchievementTrackerBackend/TwitchAchievementTrackerBackend
dotnet run
```

### Secrets
Secrets are loaded as configuration.
For development, use the User Secrets feature or the Development appsetting.json (but careful not to submit).
For deployment, Env variables will be loaded for configuration.

#### Setting required secrets for development
This is the crossplatform way, using the dotnet cli:
```
cd TwitchAchievementTrackerBackend/TwitchAchievementTrackerBackend
dotnet user-secrets set "twitch:ExtensionSecrets:0" "<your_twitch_extension_secret>"
dotnet user-secrets set "config:EncryptionSecret" "<a_random_string_used_as_encryption_password>"
dotnet user-secrets set "xapi:XApiKey" "<a_default_xapius_api_key>"
```

#### xapi.us configuration
- xapi.XApiKey : Private xapi.us Key, only used for configuration calls where the streamer's key is not yet available.
  ENV var name: xapi__XApiKey

#### Twitch configuration
- twitch.ExtensionSecrets : List of enabled Twitch extension secret keys, used to validate Extension calls
  ENV var name: twitch__ExtensionSecrets__0, twitch__ExtensionSecrets__1, ...
  
- config.EncryptionSecret : Secret key used to encrypt the configuration token
  ENV var name: config__EncryptionSecret
