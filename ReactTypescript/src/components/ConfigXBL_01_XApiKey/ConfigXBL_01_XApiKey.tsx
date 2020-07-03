import * as React from 'react';
import * as Base from '../../common/ConfigStepBase';
import { ActiveConfig, ExtensionConfiguration } from '../../common/EBSTypes';
import { ConfigurationService, ValidationError } from '../../services/EBSConfigurationService';
import { AchievementsService } from '../../services/EBSAchievementsService';

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
    }

    onChangeXApiValue = (e: React.SyntheticEvent<HTMLInputElement>) => {
        let formatCheck = this.formatFullMatchRegexp.test(e.currentTarget.value);
        let isSyntaxValid = this.formatRegexp.test(e.currentTarget.value);
        this.setState({
            isSyntaxValid: formatCheck,
            isSyntaxError: !isSyntaxValid,
            enteredApiKey: e.currentTarget.value,
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
            ActiveConfig: ActiveConfig.XBoxLive,
            SteamConfig: null,
            Version: "0.0.2",
            XBoxLiveConfig: {
                XApiKey: this.state.enteredApiKey,
                Locale: null,
                StreamerXuid: null,
                TitleId: null,
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
            <input name="xapikey" type="text" pattern="[0-9a-f]{40}" placeholder="Enter your XApi.us key" onChange={this.onChangeXApiValue} className={this.state.isSyntaxValid ? '' : 'sf1-invalid'} />,
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