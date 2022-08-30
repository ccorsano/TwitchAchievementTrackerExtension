import * as EBSConfig from "../common/ServerConfig"
import * as EBS from './EBSBase'
import { TitleInfo, Achievement, AchievementSummary } from '../common/EBSTypes';

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

    getTitleInfo = (): Promise<TitleInfo> => {
        return this.serviceFetch("/title");
    }

    getSummary = (): Promise<AchievementSummary> => {
        return this.serviceFetch("/achievements/summary");
    }

    getAchievements = (): Promise<Achievement[]> => {
        return this.serviceFetch("/achievements/list");
    }
}

export const AchievementsService = new EBSAchievementsService();