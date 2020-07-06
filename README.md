# Twitch Achievement Tracker Extension : Frontend

Frontend for the Twitch Achievement Tracker Extension.

## How to run
This is a React + TypeScript + WebPack application.
WebPack is used to ease development and bundle the production output.

### Pre-requisites
- NodeJS / NPM
- A code editor that works well with WebPack and TypeScript editing
  - Visual Studio Code
- Twitch Developer Rig to run the extension

### Using Twitch Developer Rig

There is a Twitch Extension manifest included, configured for Rig local file hosting.
The Run Frontend option is configured to launch the correct npm command to run the frontend in the Rig.

### Installing dependencies

Before running the application, you need to restore its dependencies.

- ```npm install``` will resolve and install the frontend dev and prod dependencies.

### Running in development mode

- ```npm run start``` will start the frontend in watch mode, automatically rebuilding and applying changes.

### Building for production

- ```npm run build``` will compile and pack the frontend for production, outputting all files in the dist/ folder.

### Packaging for Twitch upload

For convenience, a windows bat file (pack_assets.bat) is included to produce the zip file to upload as Twitch Extension Assets.
The bat file both runs the npm build and zip the generated files.

## Project structure

```
.
+-- assets                 -> Static assets (images), to be referenced in application code
+-- public                 -> HTML sample pages and mini.css bundle
+-- src
|   +-- common/            -> Shared types and helpers
|   +-- components/        -> React components
|   +-- services/          -> Service classes
|   +-- Config.tsx         -> Root file for the broadcaster Configuration view
|   +-- Mobile.tsx         -> Root file for the Mobile viewer Twitch view
|   +-- VideoOverlay.tsx   -> Root file for the Video Overlay viewer Twitch view
+-- custom.d.ts            -> Custom TypeScript modules to embed image assets
+-- pack_assets.bat        -> Bat file to easily launch the PS script below
+-- pack_assets.ps1        -> PowerShell script to package assets
+-- package.json           -> NPM Dependencies and project configuration
+-- template.html          -> HTML Template to generate the views from
+-- tsconfig.json          -> TypeScript configuration
+-- webpack.config.js      -> WebPack configuration
```


# Twitch Achievement Tracker Extension : Backend
Backend for the Twitch Achievement Tracker Extension.

Grab achievements from xapi.us or Steam for a given streamer and game, and display stats on the stream.

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
dotnet user-secrets set "steam:WebApiKey" "<a_default_steam_api_key>"
```

#### xapi.us configuration
- xapi.XApiKey : Private xapi.us Key, only used for configuration calls where the streamer's key is not yet available.
  ENV var name: xapi__XApiKey

#### Steam configuration
- steam.WebApiKey : Private Steam WebApiKey, only used for configuration calls where the streamer's key is not yet available.
  ENV var name: steam__WebApiKey


#### Twitch configuration
- twitch.ExtensionSecrets : List of enabled Twitch extension secret keys, used to validate Extension calls
  ENV var name: twitch__ExtensionSecrets__0, twitch__ExtensionSecrets__1, ...
  
- config.EncryptionSecret : Secret key used to encrypt the configuration token
  ENV var name: config__EncryptionSecret
