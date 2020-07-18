import * as React from 'react';
import * as Base from '../../common/ConfigStepBase';
import { ExtensionConfiguration, ActiveConfig } from '../../common/EBSTypes';
import EBSAchievementsService from '../../services/EBSAchievementsService';
import { EBSVersion } from '../../common/ServerConfig';
import { ConfigurationService, ValidationError } from '../../services/EBSConfigurationService';
import ValidationErrorList from '../ValidationErrorList/ValidationErrorList';

interface ConfigSteam_05_ConfirmProps extends Base.ConfigStepBaseProps {
    webApiKey: string,
    steamProfileId: string,
    steamAppId: string,
    locale: string,
}

interface ConfigSteam_05_ConfirmState extends Base.ConfigStepBaseState {
    isLoading: boolean,
    errors: ValidationError[]
}

export default class ConfigSteam_05_Confirm extends Base.ConfigStepBase<ConfigSteam_05_ConfirmProps, ConfigSteam_05_ConfirmState> {
    state: ConfigSteam_05_ConfirmState = {
        isLoading: true,
        errors: [],
        isValid: false,
    }

    constructor(props: ConfigSteam_05_ConfirmProps){
        super(props);
    }

    componentDidMount = async () => {
            
        const configuration: ExtensionConfiguration = {
            activeConfig: ActiveConfig.Steam,
            steamConfig: {
                steamId: this.props.steamProfileId,
                webApiKey: this.props.webApiKey,
                appId: this.props.steamAppId,
                locale: this.props.locale
            },
            version: EBSVersion,
            xBoxLiveConfig: null
        }

        let errors = await ConfigurationService.validateConfiguration(configuration);
        errors = errors.filter(e => e.path == "SteamConfig.WebApiKey" || e.path == "SteamConfig.SteamId");

        this.setState({
            isLoading: false,
            errors: errors,
            isValid: errors.length == 0
        });
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
        let selection;
        let isSaveDisabled = this.state.isLoading || this.state.errors.length > 0;

        if (this.state.isLoading)
        {
            selection = (
                <div className="card">
                    <div className="spinner"></div>
                </div>
            )
        }
        else
        {
            selection = (
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
                </div>
            )
        }
        return [
            selection,
            <ValidationErrorList errors={this.state.errors} />,
            <input type="button" value="Back" onClick={this.onBack} />,
            <input type="button" value="Save" onClick={this.onSave} disabled={isSaveDisabled} />
        ]
    }
}