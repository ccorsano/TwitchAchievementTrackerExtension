import * as React from 'react';
import * as Base from '../../common/ConfigStepBase'
import { AchievementsService } from '../../services/EBSAchievementsService';
import { ConfigurationState } from '../../services/ConfigurationStateService';
import { ConfigurationService } from '../../services/EBSConfigurationService';
import * as webpack from 'webpack';
import { TitleInfo } from '../../common/EBSTypes';
import GameCard from '../GameCard/GameCard';

type ConfigSteam_02_AppIdState = {
    titleSearch: string;
    isLoading: boolean;
    ownedApps: TitleInfo[];
    filteredApps: TitleInfo[];
    selectedTitle: TitleInfo;
}

export default class ConfigSteam_02_AppId extends Base.ConfigStepBase<Base.ConfigStepBaseProps, ConfigSteam_02_AppIdState> {
    state: ConfigSteam_02_AppIdState = {
        titleSearch: '',
        isLoading: true,
        ownedApps: [],
        filteredApps: [],
        selectedTitle: null,
    }

    constructor(props: Base.ConfigStepBaseProps){
        super(props);

        this.onContinue = this.onContinue.bind(this);
        this.onChangeTitleSearch = this.onChangeTitleSearch.bind(this);
        this.onSelectTitle = this.onSelectTitle.bind(this);
    }

    componentDidMount = () => {
        let steamId = ConfigurationState.currentConfiguration.steamConfig.steamId;
        let currentTitle = ConfigurationState.currentConfiguration.steamConfig.appId;

        ConfigurationService.getSteamOwnedGames(steamId, ConfigurationState.currentConfiguration.steamConfig.webApiKey)
        .then(gameList => {
            let gameInfo: TitleInfo = null;
            if (currentTitle)
            {
                gameInfo = gameList.find(g => g.titleId == currentTitle);
            }
            
            this.setState({
                isLoading: false,
                filteredApps: gameList,
                ownedApps: gameList,
                selectedTitle: gameInfo,
            });
        });
    }

    onContinue = async (e: React.SyntheticEvent<HTMLInputElement>) => {
        ConfigurationState.currentConfiguration.steamConfig.appId = this.state.selectedTitle.titleId;
        this.props.onValid(this, this.props.nextState);
    }

    onChangeTitleSearch = async (e: React.ChangeEvent<HTMLInputElement>) => {
        let titleSearch = e.currentTarget.value.toLowerCase();
        this.setState({
            titleSearch: titleSearch,
            filteredApps: this.state.ownedApps.filter(app => app.productTitle.toLowerCase().search(titleSearch) != -1),
            ownedApps: this.state.ownedApps
        });
    }

    onSelectTitle = (e: React.MouseEvent<HTMLElement>, titleId: string) => {
        let titleInfo = this.state.ownedApps.find(t => t.titleId == titleId);

        this.setState({
            titleSearch: "",
            filteredApps: this.state.ownedApps,
            ownedApps: this.state.ownedApps,
            selectedTitle: titleInfo,
        });

        // Validate and move on
    }

    onResetTitle = (e: React.MouseEvent<HTMLInputElement>) => {
        this.setState({
            titleSearch: "",
            filteredApps: this.state.ownedApps,
            ownedApps: this.state.ownedApps,
            selectedTitle: null,
        });
    }

    render(){
        const isContinueEnabled = this.state.selectedTitle;
        let selection;
        if (this.state.selectedTitle)
        {
            selection = (
                <GameCard titleInfo={this.state.selectedTitle} buttonSection={<input type="button" className="section" name="TitleChange" value="Change" onClick={this.onResetTitle} />} />
            )
        }
        else
        {
            selection = [
                <input name="titleSearch" type="text" placeholder="Filter your Steam games" onChange={this.onChangeTitleSearch} />,,
                <div className="searchResult container">
                    <div className="row">
                    {
                        this.state.filteredApps.map((titleInfo, i) => (
                            <GameCard titleInfo={titleInfo} buttonSection={<input className="section" type="button" name="steamTitleChange" value="Select" onClick={(e) => this.onSelectTitle(e, titleInfo.titleId)} />} />
                        ))
                    }
                    </div>
                </div>
            ]
        }

        return [
            <label htmlFor="titleSearch">Game Title</label>,
            selection,
            <input type="button" value="Continue" disabled={!isContinueEnabled} onClick={this.onContinue} />
        ]
    }
}