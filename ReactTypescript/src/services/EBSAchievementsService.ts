import * as EBSConfig from "../common/ServerConfig"
import * as EBS from './EBSBase'
import { ExtensionConfiguration } from '../common/EBSTypes';
import { Twitch } from '../services/TwitchService';

export interface TitleInfo {
    titleId: string;
    productTitle: string;
    productDescription: string;
    logoUri: string;
}

export default class EBSAchievementsService extends EBS.EBSBase {
    constructor(){
        super(EBSConfig.EBSBaseUrl + "/api");
    }

    searchTitleInfo = async(query: string): Promise<TitleInfo[]> => {
        return this.serviceFetch("/title/search/" + query);
    }
}

export const AchievementsService = new EBSAchievementsService();