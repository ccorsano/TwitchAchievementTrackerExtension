export enum ActiveConfig {
    None = "",
    XBoxLive = 'XBoxLive',
    Steam = 'Steam',
}

export interface XApiConfiguration {
    xApiKey: string;
    streamerXuid: string;
    titleId: string;
    locale: string;
}

export interface SteamConfiguration {
    webApiKey: string;
    steamId: string;
    appId: string;
    locale: string;
}

export interface ExtensionConfiguration {
    version: string;
    activeConfig: ActiveConfig;
    xBoxLiveConfig: XApiConfiguration;
    steamConfig: SteamConfiguration;
}

export interface SupportedLanguage {
    langCode: string;
    displayName: string;
}

export interface PlayerInfoCard {
    playerId: string;
    playerName: string;
    avatarUrl: string;
}

export interface TitleInfo {
    platform: ActiveConfig;
    titleId: string;
    productTitle: string;
    productDescription: string;
    logoUri: string;
}

export interface Achievement {
    id: string;
    name: string;
    completed: boolean;
    unlockTime: Date;
    description: string;
}

export interface AchievementSummary {
    gameName: string;
    total: number;
    completed: number;
    inPrograss: number;
    notStarter: number;
    currentPoints: number;
    totalPoints: number;
}

export interface RateLimits {
    hourlyLimit: number,
    remaining: number,
    resetTime: Date,
}