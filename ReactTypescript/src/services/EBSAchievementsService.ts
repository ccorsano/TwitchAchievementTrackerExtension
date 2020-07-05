import * as EBSConfig from "../common/ServerConfig"
import * as EBS from './EBSBase'
import { ExtensionConfiguration, TitleInfo } from '../common/EBSTypes';
import { Twitch } from '../services/TwitchService';

export default class EBSAchievementsService extends EBS.EBSBase {
    constructor(){
        super(EBSConfig.EBSBaseUrl + "/api");
    }

    searchTitleInfo = async(query: string): Promise<TitleInfo[]> => {
        return this.serviceFetch("/title/search/" + encodeURIComponent(query));
    }

    searchSteamTitleInfo = async(query: string): Promise<TitleInfo[]> => {
        return this.serviceFetch("/title/steam/search/" + encodeURIComponent(query));
    }

    searchXApiTitleInfo = async(query: string, xApiKey: string): Promise<TitleInfo[]> => {
        return this.serviceFetch("/title/xapi/search/" + encodeURIComponent(query) + "?xApiKey=" + encodeURIComponent(xApiKey));
    }
}

export const AchievementsService = new EBSAchievementsService();