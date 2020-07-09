import * as React from 'react';
import * as Base from '../../common/ConfigStepBase'
import { SupportedLanguage, ActiveConfig } from '../../common/EBSTypes';
import { ConfigurationService } from '../../services/EBSConfigurationService';
import ConfigSteam_05_Confirm from '../ConfigSteam_05_Confirm/ConfigSteam_05_Confirm';
import { ConfigSteamConfigStateEnum } from '../../common/ConfigStepBase';

interface ConfigSteam_04_LocaleProps extends Base.ConfigStepBaseProps {
    webApiKey: string,
    steamProfileId: string,
    steamAppId: string,
}

type ConfigSteam_04_LocaleState = {
    isLoading: boolean,
    supportedLanguages: SupportedLanguage[],
    selectedLanguage: SupportedLanguage,
    isConfirmed: boolean,
}

export default class ConfigSteam_04_Locale extends Base.ConfigStepBase<ConfigSteam_04_LocaleProps, ConfigSteam_04_LocaleState> {
    state: ConfigSteam_04_LocaleState = {
        isLoading: true,
        supportedLanguages: [ { langCode: '', displayName: "Loading..." } ],
        selectedLanguage: null,
        isConfirmed: false,
    };

    constructor(props: ConfigSteam_04_LocaleProps) {
        super(props);
        this.onContinue = this.onContinue.bind(this);
    }

    componentDidMount = () => {
        let currentLanguage: string = this.props.savedConfiguration?.steamConfig?.locale;

        ConfigurationService.getSteamSupportedLanguages(this.props.steamAppId)
            .then(languages => {
                this.setState({
                    isLoading: false,
                    supportedLanguages: [{ langCode: '', displayName: "Select language ..."}].concat(languages),
                    selectedLanguage: currentLanguage ? languages.find(l => l.langCode == currentLanguage) : null,
                });
            });
    }

    onSelect = (e: React.ChangeEvent<HTMLSelectElement>) => {
        this.setState({
            supportedLanguages: this.state.supportedLanguages,
            selectedLanguage: this.state.supportedLanguages.find(l => l.langCode == e.currentTarget.value),
        })
    }

    onResetLanguage = (e: React.SyntheticEvent<HTMLInputElement>) => {
        this.setState({
            selectedLanguage: null,
        });
    }

    onContinue = (e: React.SyntheticEvent<HTMLInputElement>) => {
        this.setState({
            isConfirmed: true
        })
    }

    unvalidate = (e: React.SyntheticEvent<HTMLElement>) => {
        this.setState({
            isConfirmed: false
        });
    }
    
    render(){
        const isContinueEnabled = this.state.selectedLanguage != null;
        let selection;

        if (this.state.isConfirmed)
        {
            return (
                <ConfigSteam_05_Confirm
                    savedConfiguration={this.props.savedConfiguration}
                    onValidate={this.props.onValidate}
                    onBack={this.unvalidate}
                    webApiKey={this.props.webApiKey}
                    steamProfileId={this.props.steamProfileId}
                    steamAppId={this.props.steamAppId}
                    locale={this.state.selectedLanguage.langCode} />
            )
        }

        if (this.state.isLoading)
        {
            selection = (
                <div className="card">
                    <div className="spinner"></div>
                </div>
            )
        }
        else if (this.state.selectedLanguage)
        {
            selection = (
                <div className="card">
                    <h2>{this.state.selectedLanguage.displayName}</h2>
                    <input type="button" className="section" name="LanguageChange" value="Change" onClick={this.onResetLanguage} />
                </div>
            )
        }
        else if (this.state.supportedLanguages)
        {
            selection = (
                <div className="card">
                    <select name="language" className="selectLanguage" onChange={this.onSelect}>
                        {
                            this.state.supportedLanguages.map((language, i) => (
                                <option value={language.langCode}>{language.displayName}</option>
                            ))
                        }
                    </select>
                </div>
            )
        }

        return [
            selection,
            <input type="button" value="Back" onClick={this.onBack} />,
            <input type="button" disabled={!isContinueEnabled} value="Continue" onClick={this.onContinue} />
        ]
    }
}