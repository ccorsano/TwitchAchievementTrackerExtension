import { TwitchAuthCallbackContext, TwitchExtensionConfiguration } from "../common/TwitchExtension";

export default class TwitchService {
    onAuthorized: {(context: TwitchAuthCallbackContext):void}[] = [];
    onConfiguration: {(config: TwitchExtensionConfiguration):void}[] = [];
    configuration: TwitchExtensionConfiguration = { content: "", version: "" };
    authToken: TwitchAuthCallbackContext = null;

    constructor()
    {
        this._onAuthorized = this._onAuthorized.bind(this);
        this._onConfiguration = this._onConfiguration.bind(this);

        (<any>window).Twitch.ext.onAuthorized(this._onAuthorized);
        (<any>window).Twitch.ext.configuration.onChanged(this._onConfiguration);
    }

    setConfiguration = (configuration: string, version: string) => {
        (<any>window).Twitch.ext.configuration.set('broadcaster', version, configuration);
        this.configuration = {content: configuration, version: version};
    }

    send = (target: string, contentType: string, message: any) => {
        (<any>window).Twitch.ext.send(target, contentType, message);
    }

    listen = (channel: string, callback: {(target: string, contentType: string, message: string):void}) => {
        (<any>window).Twitch.ext.listen(channel, callback);
    }

    unlisten = (channel: string, callback: {(target: string, contentType: string, message: string):void}) => {
        (<any>window).Twitch.ext.unlisten(channel, callback);
    }

    _onAuthorized = (context: TwitchAuthCallbackContext) => {
        this.authToken = context;
        this.onAuthorized.forEach(handler => {
            handler(context);
        });
    }

    _onConfiguration = (config: TwitchExtensionConfiguration) => {
        this.configuration = config;
        this.onConfiguration.forEach(handler => {
            handler(config);
        })
    }
}

export const Twitch = new TwitchService();