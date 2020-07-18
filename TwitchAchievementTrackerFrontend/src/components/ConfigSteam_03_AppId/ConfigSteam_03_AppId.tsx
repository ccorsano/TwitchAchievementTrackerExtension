import * as React from 'react';
import * as Base from '../../common/ConfigStepBase'
import { ConfigurationService, ValidationError } from '../../services/EBSConfigurationService';
import { TitleInfo, ExtensionConfiguration, ActiveConfig } from '../../common/EBSTypes';
import GameCard from '../GameCard/GameCard';
import ConfigSteam_04_Locale from '../ConfigSteam_04_Locale/ConfigSteam_04_Locale';
import { ConfigSteamConfigStateEnum } from '../../common/ConfigStepBase';
import { EBSVersion } from '../../common/ServerConfig';
import ValidationErrorList from '../ValidationErrorList/ValidationErrorList';


interface ConfigSteam_03_AppIdProps extends Base.ConfigStepBaseProps {
    webApiKey: string,
    steamProfileId: string,
}

type ConfigSteam_03_AppIdState = {
    titleSearch: string;
    isLoading: boolean;
    isConfirmed: boolean;
    ownedApps: TitleInfo[];
    filteredApps: TitleInfo[];
    selectedTitle: TitleInfo;
    errors: ValidationError[];
}

export default class ConfigSteam_03_AppId extends Base.ConfigStepBase<ConfigSteam_03_AppIdProps, ConfigSteam_03_AppIdState> {
    state: ConfigSteam_03_AppIdState = {
        titleSearch: '',
        isLoading: true,
        isConfirmed: false,
        ownedApps: [],
        filteredApps: [],
        selectedTitle: null,
        errors: [],
    }

    constructor(props: ConfigSteam_03_AppIdProps){
        super(props);

        this.onContinue = this.onContinue.bind(this);
        this.onChangeTitleSearch = this.onChangeTitleSearch.bind(this);
        this.onSelectTitle = this.onSelectTitle.bind(this);
    }

    componentDidMount = () => {
        ConfigurationService.getSteamOwnedGames(this.props.steamProfileId, this.props.webApiKey)
        .then(gameList => {
            let gameInfo: TitleInfo = null;
            if (this.state.selectedTitle)
            {
                gameInfo = gameList.find(g => g.titleId == this.state.selectedTitle.titleId);
            }
            
            this.setState({
                isLoading: false,
                filteredApps: gameList,
                ownedApps: gameList,
                selectedTitle: gameInfo,
            });
        });
    }

    onContinue = async () => {
        this.setState({
            isConfirmed: true,
        });
    }

    onChangeTitleSearch = async (e: React.ChangeEvent<HTMLInputElement>) => {
        let titleSearch = e.currentTarget.value.toLowerCase();
        this.setState({
            titleSearch: titleSearch,
            filteredApps: this.state.ownedApps.filter(app => app.productTitle.toLowerCase().search(titleSearch) != -1),
            ownedApps: this.state.ownedApps
        });
    }

    onSelectTitle = async (e: React.MouseEvent<HTMLElement>, titleId: string) => {
        let titleInfo = this.state.ownedApps.find(t => t.titleId == titleId);

        this.setState({
            isLoading: true,
            titleSearch: "",
            filteredApps: this.state.ownedApps,
            ownedApps: this.state.ownedApps,
            selectedTitle: titleInfo,
        });

        // Validate and move on
        const configuration: ExtensionConfiguration = {
            activeConfig: ActiveConfig.Steam,
            steamConfig: {
                steamId: this.props.steamProfileId,
                webApiKey: this.props.webApiKey,
                appId: titleInfo.titleId,
                locale: "english",
            },
            version: EBSVersion,
            xBoxLiveConfig: null
        }

        let errors = await ConfigurationService.validateConfiguration(configuration);
        errors = errors.filter(e => e.path == "SteamConfig.WebApiKey" || e.path == "SteamConfig.SteamId");

        this.setState({
            isLoading: false,
            errors: errors,
            selectedTitle: errors.length == 0 ? titleInfo : null,
        });
    }

    onResetTitle = () => {
        this.setState({
            titleSearch: "",
            filteredApps: this.state.ownedApps,
            ownedApps: this.state.ownedApps,
            selectedTitle: null,
        });
    }

    unvalidate = () => {
        this.setState({
            isConfirmed: false,
        });
    }

    render(){
        const isContinueEnabled = this.state.selectedTitle;
        let selection;

        if (this.state.isConfirmed)
        {
            return (
                <ConfigSteam_04_Locale
                    savedConfiguration={this.props.savedConfiguration}
                    onValidate={this.props.onValidate}
                    onBack={this.unvalidate}
                    webApiKey={this.props.webApiKey}
                    steamProfileId={this.props.steamProfileId}
                    steamAppId={this.state.selectedTitle.titleId} />

            );
        }

        if (this.state.isLoading)
        {
            selection = (
                <div className="card">
                    <div className="spinner"></div>
                </div>
            )
        }
        else if (this.state.selectedTitle)
        {
            selection = (
                <GameCard titleInfo={this.state.selectedTitle} buttonSection={<input type="button" className="section" name="TitleChange" value="Change" onClick={this.onResetTitle} />} />
            )
        }
        else
        {
            selection = [
                <label htmlFor="titleSearch">Owned Game list</label>,
                <input name="titleSearch" type="text" placeholder="Filter your Steam games" onChange={this.onChangeTitleSearch} />,,
                <div className="searchResult container">
                    <div className="row">
                    {
                        this.state.filteredApps.map((titleInfo) => (
                            <GameCard titleInfo={titleInfo} buttonSection={<input className="section" type="button" name="steamTitleChange" value="Select" onClick={(e) => this.onSelectTitle(e, titleInfo.titleId)} />} />
                        ))
                    }
                    </div>
                </div>
            ]
        }

        return [
            <ValidationErrorList errors={this.state.errors} />,
            selection,
            <input type="button" value="Back" onClick={this.onBack} />,
            <input type="button" value="Continue" disabled={!isContinueEnabled} onClick={this.onContinue} />
        ]
    }
}