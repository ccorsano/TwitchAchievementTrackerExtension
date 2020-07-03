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