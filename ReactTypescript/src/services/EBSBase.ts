import { version } from "webpack";
import { Twitch } from '../services/TwitchService';


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

    constructor(baseUrl: string){
        this._onAuthorized = this._onAuthorized.bind(this);
        this._onConfiguration = this._onConfiguration.bind(this);
        this.baseUrl = baseUrl;

        Twitch.onAuthorized.push(this._onAuthorized);
        Twitch.onConfiguration.push(this._onConfiguration);
    }

    serviceFetch = async <T>(path: string, init: RequestInit = null): Promise<T> => {
        const opts: RequestInit = {
            method: init?.method ?? 'GET',
            headers: new Headers({
                'Authorization': 'Bearer ' + this.context.token,
                'Content-Type': 'application/json',
                'X-Config-Token': this.configuration?.content,
                'X-Config-Version': this.configuration?.version,
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
            this.configuration = <TwitchExtensionConfiguration>{
                content: "TtWHMmvwfRS2ZMh3w9xhGfn8eUz1wXLa64/L5LIqIwUDUb5aIHdL+eFZwk2+x7WrcF927pckRCly4t7LeOzRfvscuMWdUEWZnuRwBBMp4FNdVs97TAzcvi0qNjnqvnwrSpXP8jnUa19AWULXI6+o1vltgmF85LCa0MuwMqLZu8jUn24m28bhzrVPW0SMZyJyzRlrQJhz61t09kt8BLLUYSAl4gSepp5eydtHMrRofYhFrznYxOaK3600sfR6Bjxb21f/Vbr+Ex2MMg3w6GcAYEX+MslOBOaOKtFspyS7pEo7Jkp6AqlR4WYUTpiKoto8BoVIvCc3Ouhuqi3cituGcw==",
                version: "0.0.2"
            };
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