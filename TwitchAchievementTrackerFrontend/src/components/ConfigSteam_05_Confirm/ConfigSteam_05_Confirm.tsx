import * as React from 'react';
import * as Base from '../../common/ConfigStepBase';
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
            xBoxLiveConfig: this.props.savedConfiguration?.xBoxLiveConfig, // Make sure we keep the non-active config saved
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
        return [
            <div className="card">
                <div className="section">
                    ActiveConfig: Steam
                </div>
                <div className="section">
                    WebApiKey: {this.props.webApiKey}
                </div>
                <div className="section">
                    StreamerSteamId: {this.props.steamProfileId}
                </div>
                <div className="section">
                    AppId: {this.props.steamAppId}
                </div>
                <div className="section">
                    WebApiKey: {this.props.locale}
                </div>
            </div>,
            <input type="button" value="Back" onClick={this.onBack} />,
            <input type="button" value="Save" onClick={this.onSave} />
        ]
    }
}