import * as React from 'react';
import * as Base from '../../common/ConfigStepBase'
import { ConfigurationState } from '../../services/ConfigurationStateService';
import { ExtensionConfiguration, ActiveConfig } from '../../common/EBSTypes';
import EBSAchievementsService from '../../services/EBSAchievementsService';
import { EBSVersion } from '../../common/ServerConfig';

interface ConfigSteam_05_ConfirmProps extends Base.ConfigStepBaseProps {
    webApiKey: string,
    steamProfileId: string,
    steamAppId: string,
    locale: string,
}

export default class ConfigSteam_05_Confirm extends Base.ConfigStepBase<ConfigSteam_05_ConfirmProps, any> {
    constructor(props: ConfigSteam_05_ConfirmProps){
        super(props);
    }

    onSave = (e: React.SyntheticEvent<HTMLInputElement>) => {
        let configuration: ExtensionConfiguration = {
            activeConfig: ActiveConfig.Steam,
            version: EBSVersion,
            xBoxLiveConfig: null,
            steamConfig: {
                webApiKey: this.props.webApiKey,
                steamId: this.props.steamProfileId,
                appId: this.props.steamAppId,
                locale: this.props.locale,
            }
        };
        this.props.onValidate(this, configuration);
    }

    render(){
        let configuration = ConfigurationState.currentConfiguration;
        return [
            <ul>
                <li>ActiveConfig: Steam</li>
                <li>WebApiKey: {this.props.webApiKey}</li>
                <li>AppId: {this.props.steamAppId}</li>
                <li>StreamerSteamId: {this.props.steamProfileId}</li>
                <li>Locale: {this.props.locale}</li>
            </ul>,
            <input type="button" value="Back" onClick={this.onBack} />,
            <input type="button" value="Save" onClick={this.onSave} />
        ]
    }
}