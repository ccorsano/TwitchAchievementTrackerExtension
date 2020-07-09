import { version } from "webpack";
import { Twitch } from '../services/TwitchService';
import * as ServerConfig from '../common/ServerConfig';
import { TwitchAuthCallbackContext, TwitchExtensionConfiguration } from "../common/TwitchExtension";
require('../common/TwitchExtension')

var urlParams = new URLSearchParams(window.location.search);
let isDebug: boolean = false;

if (urlParams.get('state') == "testing")
{
    isDebug = true;
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

        this.contextPromise = new Promise<TwitchAuthCallbackContext>((resolve, reject) => {
            Twitch.onAuthorized.push(context => {
                this._onAuthorized(context);
                resolve(context);
            });
        });

        this.configurationPromise = new Promise<TwitchExtensionConfiguration>((resolve, reject) => {
            Twitch.onConfiguration.push(config => {
                this._onConfiguration();
                resolve(this.configuration);
            });
        });

        this.configuredPromise = Promise.all([this.contextPromise, this.configurationPromise]);
        this.configuredPromise.then(() => {
            if (this.onConfigured)
            {
                this.onConfigured(this.context, this.configuration);
            }
        });
    }

    serviceFetch = async <T>(path: string, init: RequestInit = null, configTokenOverride: string = null, versionOverride: string = null): Promise<T> => {
        await this.configuredPromise;

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

        if (isDebug && !this.configuration)
        {
            // this.configuration = <TwitchExtensionConfiguration>{
            //     content: "M4dKt7OfjxV2Qg6hNDfUILxVxcM4R2ibLq8MfDI42Yq2QtX8DUNsw/6A3tX+3zX1QKstugDUncqGEQf1+WE7NCK1Izw+AVIbdpUKUnJMPXvVh2i61drG+i+d+wksr015Yb3NCPdQ4ULKYxQuFyTuSSpPkC55L0AYUhKjbA7P8MTq/Erywgj/weVkgXey7At2RGKTMD8AR5FaLYlsyZa8Oxp2DUEN3pa2fc466IZt1HMnn8Rj3QR178SvwEa7r7K8Tl26P7dRzr6TO/9e1iDuX2PVsnhRbicsNKHprOLapLEOoMifzVBzOk1RYqRDvhE3Jw0+xZS7Km79Sq85Q9J8GA==",
            //     version: "0.0.3"
            // };
        }

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