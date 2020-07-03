import { EBSBase } from "./EBSBase";

import * as EBS from './EBSBase'

export enum ActiveConfig {
    XBoxLive = 'XBoxLive',
    Steam = 'Steam',
}

export interface XApiConfiguration {
    XApiKey: string;
    StreamerXuid: string;
    TitleId: string;
    Locale: string;
}

export interface SteamConfiguration {
    WebApiKey: string;
    SteamId: string;
    AppId: string;
    Locale: string;
}


export interface ExtensionConfiguration {
    Version: string;
    ActiveConfig: ActiveConfig;
    XBoxLiveConfig: XApiConfiguration;
    SteamConfig: SteamConfiguration;
}

export interface ValidationError {
    path: string;
    errorCode: string;
    errorDescription: string;
}


var server = "https://twitchext.conceptoire.com/v2"
var intervalTimer = false;

var urlParams = new URLSearchParams(window.location.search);
if (urlParams.get('state') == "testing")
{
    server = "http://localhost:8081"
}

export default class EBSConfigurationService extends EBS.EBSBase {
    constructor(){
        super(server + "/api/configuration");
    }

    setConfiguration = async(config: ExtensionConfiguration): Promise<void> => {
        return this.serviceFetch("/", {
            method: 'POST',
            body: JSON.stringify(config)
        });
    }

    getConfiguration = async (): Promise<ExtensionConfiguration> => {
        return this.serviceFetch("/");
    }

    validateConfiguration = async (config: ExtensionConfiguration): Promise<ValidationError[]> => {
        return this.serviceFetch("/validate", {
            method: 'POST',
            body: JSON.stringify(config)
        });
    }
}

export const ConfigurationService = new EBSConfigurationService();