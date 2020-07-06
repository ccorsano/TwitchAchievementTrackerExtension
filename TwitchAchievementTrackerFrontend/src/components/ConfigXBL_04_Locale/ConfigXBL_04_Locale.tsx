import * as React from 'react';
import * as Base from '../../common/ConfigStepBase';
import { ConfigurationService } from '../../services/EBSConfigurationService';
import { ConfigurationState } from '../../services/ConfigurationStateService';
import { ActiveConfig, SupportedLanguage } from '../../common/EBSTypes';


type ConfigXBL_04_LocaleState = {
    supportedLanguages: SupportedLanguage[],
    selectedLanguage: SupportedLanguage
}

export default class ConfigXBL_04_Locale extends Base.ConfigStepBase<Base.ConfigStepBaseProps, ConfigXBL_04_LocaleState> {
    state: ConfigXBL_04_LocaleState = {
        supportedLanguages: [ { langCode: '', displayName: "Loading..." } ],
        selectedLanguage: null,
    };

    constructor(props: Base.ConfigStepBaseProps) {
        super(props);
        this.onContinue = this.onContinue.bind(this);
    }

    componentDidMount = () => {
        ConfigurationService.getSupportedLanguages(ActiveConfig.XBoxLive, ConfigurationState.currentConfiguration.xBoxLiveConfig.titleId)
            .then(languages => {
                this.setState({
                    supportedLanguages: [{ langCode: '', displayName: "Select language ..."}].concat(languages),
                    selectedLanguage: null,
                });
            });
    }

    onSelect = (e: React.ChangeEvent<HTMLSelectElement>) => {
        this.setState({
            supportedLanguages: this.state.supportedLanguages,
            selectedLanguage: this.state.supportedLanguages.find(l => l.langCode == e.currentTarget.value),
        })
    }

    onContinue = (e: React.SyntheticEvent<HTMLInputElement>) => {
        ConfigurationState.currentConfiguration.xBoxLiveConfig.locale = this.state.selectedLanguage.langCode;
        this.props.onValid(this, this.props.nextState);
    }

    render(){
        const isContinueEnabled = this.state.selectedLanguage;
        let selection;
        if (this.state.selectedLanguage)
        {
            selection = (
                <div className="selectedLanguage">
                    {this.state.selectedLanguage.displayName}
                </div>
            )
        }
        else if (this.state.supportedLanguages)
        {
            selection = (
                <select name="language" className="selectLanguage" onChange={this.onSelect}>
                    {
                        this.state.supportedLanguages.map((language, i) => (
                            <option value={language.langCode}>{language.displayName}</option>
                        ))
                    }
                </select>
            )
        }

        return [
            selection,
            <input type="button" disabled={!isContinueEnabled} value="Continue" onClick={this.onContinue} />
        ]
    }
}