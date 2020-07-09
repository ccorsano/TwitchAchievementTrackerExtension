import * as React from 'react';
import * as Base from '../../common/ConfigStepBase';
import { ConfigurationService, ValidationError, EncryptedConfigurationResponse } from '../../services/EBSConfigurationService';
import { ConfigurationState } from '../../services/ConfigurationStateService'
import ConfigXBL_01_XApiKey from '../ConfigXBL_01_XApiKey/ConfigXBL_01_XApiKey';
import ConfigXBL_02_XUID from '../ConfigXBL_02_XUID/ConfigXBL_02_XUID';
import ConfigXBL_03_TitleId from '../ConfigXBL_03_TitleId/ConfigXBL_03_TitleId';
import ConfigXBL_04_Locale from '../ConfigXBL_04_Locale/ConfigXBL_04_Locale';
import ConfigXBL_05_Confirm from '../ConfigXBL_05_Confirm/ConfigXBL_05_Confirm';
import { ActiveConfig, ExtensionConfiguration } from '../../common/EBSTypes';
import { Twitch } from '../../services/TwitchService';
import * as ServerConfig from '../../common/ServerConfig'
import { TwitchExtensionConfiguration } from '../../common/TwitchExtension';
import { EBSBase } from '../../services/EBSBase';
import { ConfigXBLConfigStateEnum } from '../../common/ConfigStepBase';


type ConfigXBLConfigProps = {
    onSaved: (savedConfig: TwitchExtensionConfiguration, configObject: ExtensionConfiguration) => void;
}

type ConfigXBLConfigState = {
    currentState: ConfigXBLConfigStateEnum,
    isValid: boolean
}

export default class ConfigXBLConfigRoot extends React.Component<ConfigXBLConfigProps, ConfigXBLConfigState> {
    state: ConfigXBLConfigState = {
        currentState: ConfigXBLConfigStateEnum.XApiUsKey,
        isValid: false,
    };
    
    constructor(props:ConfigXBLConfigProps){
        super(props);

        this.onValidateStep = this.onValidateStep.bind(this);
        this.onCancelStep = this.onCancelStep.bind(this);
    }

    // Using componentWillMoubt: componentDidMount will be executed on child elements first, and we need to setup the current config first
    componentDidMount = () => {
        let currentConfig: ExtensionConfiguration = ConfigurationState.currentConfiguration ?? {activeConfig: ActiveConfig.XBoxLive, xBoxLiveConfig: null, steamConfig: null, version: ServerConfig.EBSVersion};
        currentConfig.activeConfig = ActiveConfig.XBoxLive;
        if (! currentConfig.xBoxLiveConfig)
        {
            currentConfig.xBoxLiveConfig = {
                xApiKey: null,
                streamerXuid: null,
                locale: null,
                titleId: null,
            }
        }
    }

    onValidateStep = async (e: React.Component, config: ExtensionConfiguration) => {
        ConfigurationService.setConfiguration(config).then(result => {
            Twitch.setConfiguration(result.configToken, ServerConfig.EBSVersion);

            Twitch.send("broadcast", "application/json", {
                "type": "set-config",
                "version": ServerConfig.EBSVersion,
                "configToken": result.configToken
            });

            this.setState({
                currentState: this.state.currentState,
                isValid: true,
            });

            this.props.onSaved({content: result.configToken, version: ServerConfig.EBSVersion}, ConfigurationState.currentConfiguration);
        });
    }

    onCancelStep = (previousState: any) => {
        if (previousState !== null)
        {
            this.setState({
                currentState: previousState
            });
        }
    }

    render(){
        let config = <div>Loading...</div>

        switch (this.state.currentState) {
            case ConfigXBLConfigStateEnum.XApiUsKey:
                config = <ConfigXBL_01_XApiKey onValidate={this.onValidateStep} onBack={this.onCancelStep} nextState={ConfigXBLConfigStateEnum.XUIDSearch} previousState={null} />
                break;
            // case ConfigXBLConfigStateEnum.XUIDSearch:
            //     config = <ConfigXBL_02_XUID onValid={this.onValidateStep} onBack={this.onCancelStep} nextState={ConfigXBLConfigStateEnum.TitleIdSearch} previousState={ConfigXBLConfigStateEnum.XApiUsKey} />
            //     break;
            // case ConfigXBLConfigStateEnum.TitleIdSearch:
            //     config = <ConfigXBL_03_TitleId onValid={this.onValidateStep} onBack={this.onCancelStep} nextState={ConfigXBLConfigStateEnum.Locale} previousState={ConfigXBLConfigStateEnum.XUIDSearch} />
            //     break;
            // case ConfigXBLConfigStateEnum.Locale:
            //     config = <ConfigXBL_04_Locale onValid={this.onValidateStep} onBack={this.onCancelStep} nextState={ConfigXBLConfigStateEnum.Confirm} previousState={ConfigXBLConfigStateEnum.TitleIdSearch} />
            //     break;
            // case ConfigXBLConfigStateEnum.Confirm:
            //     config = <ConfigXBL_05_Confirm onValid={this.onValidateStep} onBack={this.onCancelStep} nextState={null} previousState={ConfigXBLConfigStateEnum.Locale} />
            //     break;
            default:
                break;
        }

        if (this.state.isValid){
            config = (
                <div className="card">
                    <h2 className="section">Saved !</h2>
                </div>
            )
        }

        return [
            <h2>Configure XBoxLive Achievements</h2>,
            config
        ]
    }
}