import { Twitch } from '../services/TwitchService';
import * as ServerConfig from '../common/ServerConfig';
import { TwitchAuthCallbackContext, TwitchExtensionConfiguration } from "../common/TwitchExtension";
require('../common/TwitchExtension')

var urlParams = new URLSearchParams(window.location.search);

if (urlParams.get('state') == "testing")
{
}

export class EBSBase {
    context: TwitchAuthCallbackContext;
    configuration: TwitchExtensionConfiguration;
    baseUrl: string;
    onConfigured: (auth: TwitchAuthCallbackContext, config: TwitchExtensionConfiguration) => void;
    onAuthorized: (context: TwitchAuthCallbackContext) => void;
    onConfiguration: (configuration: TwitchExtensionConfiguration) => void;

    configuredPromise: Promise<[TwitchAuthCallbackContext, TwitchExtensionConfiguration]>;
    contextPromise: Promise<TwitchAuthCallbackContext>;
    configurationPromise: Promise<TwitchExtensionConfiguration>;

    constructor(baseUrl: string){
        this._onAuthorized = this._onAuthorized.bind(this);
        this._onConfiguration = this._onConfiguration.bind(this);
        this.baseUrl = baseUrl;

        Twitch.onAuthorized.push(this._onAuthorized);
        Twitch.onConfiguration.push(this._onConfiguration);

        this.contextPromise = new Promise<TwitchAuthCallbackContext>((resolve, _reject) => {
            Twitch.onAuthorized.push(context => {
                this._onAuthorized(context);
                resolve(context);
            });
        });

        this.configurationPromise = new Promise<TwitchExtensionConfiguration>((resolve, _reject) => {
            Twitch.onConfiguration.push(_config => {
                this._onConfiguration();
                resolve(this.configuration);
            });
        });

        this.configuredPromise = Promise.all([this.contextPromise, this.configurationPromise]);
        this.configuredPromise.then(([authContext, configContext]) => {
            if (this.onConfigured)
            {
                this.onConfigured(authContext, configContext);
            }
        });
        
        Twitch.listen("broadcast", (_target, _contentType, messageStr) => {
            let message = JSON.parse(messageStr);

            if (message.type != "set-config") return;

            this.configuration = message;
            this.configurationPromise = new Promise<TwitchExtensionConfiguration>((resolve, _reject) => {
                resolve(message);
            });
            this.configuredPromise = Promise.all([this.contextPromise, this.configurationPromise]);
            this._onConfiguration();
        });
    }

    serviceFetch = async <T>(path: string, init: RequestInit = null, configTokenOverride: string = null, versionOverride: string = null): Promise<T> => {
        let [] = await this.configuredPromise;

        const opts: RequestInit = {
            method: init?.method ?? 'GET',
            headers: new Headers({
                'Authorization': 'Bearer ' + this.context.token,
                'Content-Type': 'application/json',
                'X-Config-Token': configTokenOverride ?? this.configuration?.content ?? '',
                'X-Config-Version': versionOverride ?? this.configuration?.version ?? ServerConfig.EBSVersion,
            }),
            body: init?.body,
        };

        var response = await fetch(this.baseUrl + path, opts);

        if (!response.ok) {
            throw new Error(response.statusText)
        }
        
        return response.json();
    }

    _onAuthorized = (context: TwitchAuthCallbackContext) => {
        this.context = context;
        if (this.onAuthorized)
        {
            this.onAuthorized(this.context);
        }
        if (this.configuration && this.onConfigured)
        {
            this.onConfigured(this.context, this.configuration);
        }
    }

    _onConfiguration = () => {
        this.configuration = (<TwitchExtensionConfiguration> ((<any>window).Twitch.ext.configuration.broadcaster));

        if (this.onConfiguration)
        {
            this.onConfiguration(this.configuration);
        }
        if (this.context && this.onConfigured)
        {
            this.onConfigured(this.context, this.configuration);
        }
    }
}