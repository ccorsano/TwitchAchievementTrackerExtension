import * as React from 'react';
import * as Base from '../../common/ConfigStepBase';
import * as ServerConfig from '../../common/ServerConfig';
import { ActiveConfig, ExtensionConfiguration } from '../../common/EBSTypes';
import { ConfigurationService, ValidationError } from '../../services/EBSConfigurationService';
import { AchievementsService } from '../../services/EBSAchievementsService';
import { ConfigSteamConfigStateEnum } from '../../common/ConfigStepBase';
import ConfigXBL_02_XUID from '../ConfigXBL_02_XUID/ConfigXBL_02_XUID';
import ValidationErrorList from '../ValidationErrorList/ValidationErrorList';

type ConfigXBL_01_XApiKeyState = {
    isSyntaxValid: boolean,
    isSyntaxError: boolean,
    isKeyActive: boolean,
    isValidating: boolean,
    enteredApiKey: string,
    errors: ValidationError[],
    isConfirmed: boolean,
}

export default class ConfigXBL_01_XApiKey extends Base.ConfigStepBase<Base.ConfigStepBaseProps, ConfigXBL_01_XApiKeyState> {
    state: ConfigXBL_01_XApiKeyState = {
        isSyntaxValid: false,
        isSyntaxError: false,
        isKeyActive: false,
        isValidating: false,
        enteredApiKey: '',
        errors: [],
        isConfirmed: false,
    }

    formatRegexp = /^[0-9a-f]+$/i;
    formatFullMatchRegexp = (/^[0-9a-f]{40}$/i);

    constructor(props: Base.ConfigStepBaseProps) {
        super(props);

        this.onChangeXApiValue = this.onChangeXApiValue.bind(this);
        this.changeXApiValue = this.changeXApiValue.bind(this);
    }

    componentDidMount= () => {
        let currentConfig = this.props.savedConfiguration;
        if (currentConfig?.xBoxLiveConfig?.xApiKey)
        {
            this.changeXApiValue(currentConfig.xBoxLiveConfig.xApiKey);
        }
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

        let errors: ValidationError[] = [];
        try
        {
            errors = await ConfigurationService.validateConfiguration(configuration);
            errors = errors.filter(e => e.path == "XBoxLiveConfig.XApiKey");
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
            isConfirmed: errors.length == 0,
        });

        if (this.state.errors.length == 0)
        {
            let newConfig = await ConfigurationService.setConfiguration(configuration);
        }
    }

    unvalidate = (e: any) => {
        this.setState({
            isConfirmed: false,
        })
    }

    render(){
        let helpMessage;

        if (this.state.isConfirmed)
        {
            return (
                <ConfigXBL_02_XUID
                    savedConfiguration={this.props.savedConfiguration}
                    onValidate={this.props.onValidate}
                    onBack={this.unvalidate}
                    xApiKey={this.state.enteredApiKey} />
            );
        }

        if (this.state.errors.some(e => e.errorCode == "ExpiredXBLToken"))
        {
            helpMessage = (
                <div>
                    Your XApi.us Microsoft token has expired.<br/>
                    <a href='https://xapi.us/profile'>Log into your xapi.us account</a> and refresh your token using the "Sign in to XBox LIVE" button.
                </div>
            )
        }
        else
        {
            helpMessage =(
                <div>
                    <span className="icon-info"></span> You can register and get your xapi.us API Key on <a href="https://xapi.us/" target="_blank">https://xapi.us/</a>.<br/>
                    A free account should be enough in most use case, and once configured the extension will limit its request frequency.<br/>
                    Note: This extension is not affiliated with xapi.us in any way or form.
                </div>
            )
        }
        const isContinueEnabled = this.state.isSyntaxValid && !this.state.isValidating;
        return [
            <label htmlFor="xapikey">XApi Key</label>,
            <input name="xapikey" type="text" pattern="[0-9a-f]{40}" size={45} value={this.state.enteredApiKey} placeholder="Enter your XApi.us key" onChange={this.onChangeXApiValue} className={this.state.isSyntaxValid ? '' : 'sf1-invalid'} />,
            helpMessage,
            <ValidationErrorList errors={this.state.errors} />,
            <input type="button" value="Cancel" onClick={this.props.onBack} />,
            <input type="button" value="Continue" disabled={!isContinueEnabled} onClick={this.onContinue} />
        ]
    }
}