import * as React from 'react';
import * as Base from '../../common/ConfigStepBase';
import * as ServerConfig from '../../common/ServerConfig';
import { ActiveConfig, ExtensionConfiguration } from '../../common/EBSTypes';
import { ConfigurationService, ValidationError } from '../../services/EBSConfigurationService';
import { AchievementsService } from '../../services/EBSAchievementsService';
import { ConfigurationState } from '../../services/ConfigurationStateService'

type ConfigXBL_01_XApiKeyState = {
    isSyntaxValid: boolean,
    isSyntaxError: boolean,
    isKeyActive: boolean,
    isValidating: boolean,
    enteredApiKey: string,
    errors: ValidationError[],
}

export default class ConfigXBL_01_XApiKey extends Base.ConfigStepBase<Base.ConfigStepBaseProps, ConfigXBL_01_XApiKeyState> {
    state: ConfigXBL_01_XApiKeyState = {
        isSyntaxValid: false,
        isSyntaxError: false,
        isKeyActive: false,
        isValidating: false,
        enteredApiKey: '',
        errors: []
    }

    formatRegexp = /^[0-9a-f]+$/i;
    formatFullMatchRegexp = (/^[0-9a-f]{40}$/i);

    constructor(props: Base.ConfigStepBaseProps) {
        super(props);

        this.onChangeXApiValue = this.onChangeXApiValue.bind(this);
        this.changeXApiValue = this.changeXApiValue.bind(this);
    }

    componentDidMount= () => {
        let currentConfig = ConfigurationState.currentConfiguration;
        this.changeXApiValue(currentConfig.xBoxLiveConfig.xApiKey);
    }

    onChangeXApiValue = (e: React.SyntheticEvent<HTMLInputElement>) => {
        this.changeXApiValue(e.currentTarget.value);
    }

    changeXApiValue = (value: string) => {
        let formatCheck = this.formatFullMatchRegexp.test(value);
        let isSyntaxValid = this.formatRegexp.test(value);
        this.setState({
            isSyntaxValid: formatCheck,
            isSyntaxError: !isSyntaxValid,
            enteredApiKey: value,
        });
    }

    hasValue = () => {

    }

    isValid = () => {
        return false;
    }

    onContinue = async (e: React.SyntheticEvent<HTMLInputElement>) => {
        this.setState({
            isValidating: true
        });

        const configuration: ExtensionConfiguration = {
            activeConfig: ActiveConfig.XBoxLive,
            steamConfig: null,
            version: ServerConfig.EBSVersion,
            xBoxLiveConfig: {
                xApiKey: this.state.enteredApiKey,
                locale: null,
                streamerXuid: null,
                titleId: null,
            }
        }

        let errors = await ConfigurationService.validateConfiguration(configuration);

        this.setState({
            errors: errors,
            isValidating: false,
        });

        if (this.state.errors.length == 0)
        {
            this.props.onValid(this, this.props.nextState);
            let newConfig = await ConfigurationService.setConfiguration(configuration);
            
            ConfigurationState.currentConfiguration.xBoxLiveConfig.xApiKey = this.state.enteredApiKey;

            ConfigurationService.configuration.content = newConfig.configToken;
            AchievementsService.configuration.content = newConfig.configToken;
        }

    }

    render(){
        let helpMessage;
        if (this.state.errors.some(e => e.errorCode == "ExpiredXBLToken"))
        {
            helpMessage = (
                <div>
                    Your XApi.us Microsoft token has expired.<br/>
                    <a href='https://xapi.us/profile'>Log into your xapi.us account</a> and refresh your token using the "Sign in to XBox LIVE" button.
                </div>
            )
        }
        const isContinueEnabled = this.state.isSyntaxValid && !this.state.isValidating;
        return [
            <label htmlFor="xapikey">XApi Key</label>,
            <input name="xapikey" type="text" pattern="[0-9a-f]{40}" value={this.state.enteredApiKey} placeholder="Enter your XApi.us key" onChange={this.onChangeXApiValue} className={this.state.isSyntaxValid ? '' : 'sf1-invalid'} />,
            helpMessage,
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