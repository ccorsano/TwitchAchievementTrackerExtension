import * as React from 'react'
import '../../common/ConfigStepBase'
import * as Base from '../../common/ConfigStepBase';
import { ValidationError, ConfigurationService } from '../../services/EBSConfigurationService';
import { ConfigurationState } from '../../services/ConfigurationStateService';
import { ActiveConfig, ExtensionConfiguration } from '../../common/EBSTypes';
import * as ServerConfig from '../../common/ServerConfig';
import { AchievementsService } from '../../services/EBSAchievementsService';

type ConfigSteam_01_WebApiKeyState = {
    isSyntaxValid: boolean,
    isSyntaxError: boolean,
    isValidating: boolean,
    isKeyValid: boolean,
    enteredApiKey: string,
    errors: ValidationError[],
}

export default class ConfigSteam_01_WebAPIKey extends Base.ConfigStepBase<Base.ConfigStepBaseProps, ConfigSteam_01_WebApiKeyState> {
    state: ConfigSteam_01_WebApiKeyState = {
        isSyntaxValid: false,
        isSyntaxError: false,
        isValidating: false,
        isKeyValid: false,
        enteredApiKey: '',
        errors: [],
    }

    formatRegexp = /^[0-9a-f]+$/i;
    formatFullMatchRegexp = (/^[0-9a-f]{32}$/i);

    constructor(props: Base.ConfigStepBaseProps) {
        super(props);

        this.onChangeWebApiValue = this.onChangeWebApiValue.bind(this);
        this.changeWebApiValue = this.changeWebApiValue.bind(this);
    }

    componentDidMount = () => {
        let currentConfig = ConfigurationState.currentConfiguration;
        this.changeWebApiValue(currentConfig.steamConfig.webApiKey);
    }

    onChangeWebApiValue = (e: React.SyntheticEvent<HTMLInputElement>) => {
        this.changeWebApiValue(e.currentTarget.value);
    }

    changeWebApiValue = (value: string) => {
        let formatCheck = this.formatFullMatchRegexp.test(value);
        let isSyntaxValid = this.formatRegexp.test(value);
        this.setState({
            isSyntaxValid: formatCheck,
            isSyntaxError: !isSyntaxValid,
            enteredApiKey: value,
        });
    }

    onContinue = async (e: React.SyntheticEvent<HTMLInputElement>) => {
        this.setState({
            isValidating: true
        });

        const configuration: ExtensionConfiguration = {
            activeConfig: ActiveConfig.Steam,
            xBoxLiveConfig: null,
            version: ServerConfig.EBSVersion,
            steamConfig: {
                webApiKey: this.state.enteredApiKey,
                locale: null,
                steamId: null,
                appId: null,
            }
        }

        let errors = await ConfigurationService.validateConfiguration(configuration);
        errors = errors.filter(e => e.path == "SteamConfig.WebApiKey");

        this.setState({
            errors: errors,
            isValidating: false,
        });

        if (this.state.errors.length == 0)
        {
            this.props.onValid(this, this.props.nextState);
            let newConfig = await ConfigurationService.setConfiguration(configuration);
            
            ConfigurationState.currentConfiguration.steamConfig.webApiKey = this.state.enteredApiKey;
        }
    }

    render(){
        let isContinueEnabled = this.state.isSyntaxValid && !this.state.isValidating;
        return [
            <label htmlFor="webApiKey">Steam WebApi Key</label>,
            <input name="webApiKey" type="text" pattern="[0-9a-fA-F]{32}" value={this.state.enteredApiKey} placeholder="Enter your Steam WebAPI key" onChange={this.onChangeWebApiValue} className={this.state.isSyntaxValid ? '' : 'sf1-invalid'} />,
            <ul>
                {this.state.errors.map((error, i) => (
                    <li key={error.path + '_' + i}>
                        {error.path}: {error.errorDescription}
                    </li>
                ))}
            </ul>,
            <input type="button" value="Continue" disabled={!isContinueEnabled} onClick={this.onContinue} />
        ]
    }
}