export default class TwitchService {
    onAuthorized: {(context: TwitchAuthCallbackContext):void}[] = [];
    onConfiguration: {(config: TwitchExtensionConfiguration):void}[] = [];
    configuration: TwitchExtensionConfiguration = { content: "", version: "" };

    constructor()
    {
        this._onAuthorized = this._onAuthorized.bind(this);
        this._onConfiguration = this._onConfiguration.bind(this);

        (<any>window).Twitch.ext.onAuthorized(this._onAuthorized);
        (<any>window).Twitch.ext.configuration.onChanged(this._onConfiguration);
    }

    setConfiguration = (configuration: string, version: string) => {
        (<any>window).Twitch.ext.configuration.set('broadcaster', version, configuration);
    }

    _onAuthorized = (context: TwitchAuthCallbackContext) => {
        this.onAuthorized.forEach(handler => {
            handler(context);
        });
    }

    _onConfiguration = (config: TwitchExtensionConfiguration) => {
        this.onConfiguration.forEach(handler => {
            handler(config);
        })
    }
}

export const Twitch = new TwitchService();