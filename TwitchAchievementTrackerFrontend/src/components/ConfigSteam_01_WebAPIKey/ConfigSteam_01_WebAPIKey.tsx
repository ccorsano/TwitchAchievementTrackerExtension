import * as React from 'react'
import '../../common/ConfigStepBase'
import * as Base from '../../common/ConfigStepBase';
import { ValidationError, ConfigurationService } from '../../services/EBSConfigurationService';
import { ActiveConfig, ExtensionConfiguration, SteamConfiguration } from '../../common/EBSTypes';
import * as ServerConfig from '../../common/ServerConfig';
import { AchievementsService } from '../../services/EBSAchievementsService';
import ConfigSteam_02_SteamID from '../ConfigSteam_02_SteamID/ConfigSteam_02_SteamID'
import { ConfigSteamConfigStateEnum } from '../../common/ConfigStepBase';
import ValidationErrorList from '../ValidationErrorList/ValidationErrorList';

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
        let currentConfig: SteamConfiguration = this.props.savedConfiguration?.steamConfig;
        if (currentConfig?.webApiKey)
        {
            this.changeWebApiValue(currentConfig.webApiKey);
        }
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

        let errors: ValidationError[] = [];
        try
        {
            errors = await ConfigurationService.validateConfiguration(configuration);
            errors = errors.filter(e => e.path == "SteamConfig.WebApiKey");
        }
        catch(e)
        {
            errors.push({
                errorCode: "EBSError",
                errorDescription: "Error validating WebApiKey",
                path: "",
            });
        }

        this.setState({
            errors: errors,
            isValidating: false,
            isKeyValid: errors.length == 0,
        });

        if (this.state.errors.length == 0)
        {
            let newConfig = await ConfigurationService.setConfiguration(configuration);
        }
    }

    unvalidate = (e: React.SyntheticEvent<HTMLElement>) => {
        this.setState({
            isKeyValid: false,
        });
    }

    render(){
        let isContinueEnabled = this.state.isSyntaxValid && !this.state.isValidating;

        if (this.state.isKeyValid)
        {
            return <ConfigSteam_02_SteamID
                    savedConfiguration={this.props.savedConfiguration}
                    onValidate={this.props.onValidate}
                    onBack={this.unvalidate}
                    webApiKey={this.state.enteredApiKey} />
        }
        
        return [
            <label htmlFor="webApiKey">Steam WebApi Key</label>,
            <input name="webApiKey" type="text" pattern="[0-9a-fA-F]{32}" value={this.state.enteredApiKey} placeholder="Enter your Steam WebAPI key" onChange={this.onChangeWebApiValue} className={this.state.isSyntaxValid ? '' : 'sf1-invalid'} />,
            <div>
                <span className="icon-info"></span> You can apply for a WebAPI Key on <a href="https://steamcommunity.com/dev/apikey" target="_blank">https://steamcommunity.com/dev/apikey</a>.
            </div>,
            <ValidationErrorList errors={this.state.errors} />,
            <input type="button" value="Cancel" onClick={this.props.onBack} />,
            <input type="button" value="Continue" disabled={!isContinueEnabled} onClick={this.onContinue} />
        ]
    }
}