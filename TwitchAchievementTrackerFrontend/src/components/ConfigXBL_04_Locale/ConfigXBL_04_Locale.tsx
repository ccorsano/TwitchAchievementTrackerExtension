import * as React from 'react';
import * as Base from '../../common/ConfigStepBase';
import { ConfigurationService } from '../../services/EBSConfigurationService';
import { SupportedLanguage } from '../../common/EBSTypes';
import ConfigXBL_05_Confirm from '../ConfigXBL_05_Confirm/ConfigXBL_05_Confirm';


interface ConfigXBL_04_LocaleProps extends Base.ConfigStepBaseProps {
    xApiKey: string,
    streamerXuid: string,
    titleId: string,
}

type ConfigXBL_04_LocaleState = {
    supportedLanguages: SupportedLanguage[],
    selectedLanguage: SupportedLanguage,
    isLoading: boolean,
    isConfirmed: boolean,
}

export default class ConfigXBL_04_Locale extends Base.ConfigStepBase<ConfigXBL_04_LocaleProps, ConfigXBL_04_LocaleState> {
    state: ConfigXBL_04_LocaleState = {
        supportedLanguages: [ { langCode: '', displayName: "Loading..." } ],
        selectedLanguage: null,
        isLoading: true,
        isConfirmed: false,
    };

    constructor(props: ConfigXBL_04_LocaleProps) {
        super(props);
        this.onContinue = this.onContinue.bind(this);
    }

    componentDidMount = () => {
        let currentLocale = this.props.savedConfiguration?.xBoxLiveConfig?.locale;
        
        ConfigurationService.getXBoxLiveSupportedLanguages(this.props.titleId, this.props.xApiKey)
            .then(languages => {
                this.setState({
                    supportedLanguages: [{ langCode: '', displayName: "Select language ..."}].concat(languages),
                    selectedLanguage: currentLocale ? languages.find(l => l.langCode == currentLocale) : null,
                    isLoading: false,
                });
            });
    }

    onSelect = (e: React.ChangeEvent<HTMLSelectElement>) => {
        this.setState({
            supportedLanguages: this.state.supportedLanguages,
            selectedLanguage: this.state.supportedLanguages.find(l => l.langCode == e.currentTarget.value),
        })
    }

    onResetLanguage = () => {
        this.setState({
            selectedLanguage: null,
        });
    }

    onContinue = () => {
        this.setState({
            isConfirmed: true,
        });
    }

    unvalidate = () => {
        this.setState({
            isConfirmed: false,
        });
    }

    render(){
        const isContinueEnabled = this.state.selectedLanguage;
        let selection;

        if (this.state.isConfirmed)
        {
            return (
                <ConfigXBL_05_Confirm
                    savedConfiguration={this.props.savedConfiguration}
                    onValidate={this.props.onValidate}
                    onBack={this.unvalidate}
                    xApiKey={this.props.xApiKey}
                    streamerXuid={this.props.streamerXuid}
                    titleId={this.props.titleId}
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
                            this.state.supportedLanguages.map((language) => (
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