import './ConfigXBL_03_TitleId.scss';
import * as React from 'react'
import * as Base from '../../common/ConfigStepBase';
import { AchievementsService } from '../../services/EBSAchievementsService'
import { ConfigurationState } from '../../services/ConfigurationStateService';
import { TitleInfo } from '../../common/EBSTypes';
import { ConfigurationService } from '../../services/EBSConfigurationService';
import GameCard from '../GameCard/GameCard';

type ConfigXBL_03_TitleIdState = {
    titleSearch: string;
    searchResults: TitleInfo[];
    selectedTitle: TitleInfo;
    isLoading: boolean;
}

export default class ConfigXBL_03_TitleId extends Base.ConfigStepBase<Base.ConfigStepBaseProps, ConfigXBL_03_TitleIdState> {
    state: ConfigXBL_03_TitleIdState = {
        titleSearch: "",
        searchResults: [],
        selectedTitle: null,
        isLoading: true,
    }

    constructor(props: Base.ConfigStepBaseProps) {
        super(props);

        this.onContinue = this.onContinue.bind(this);
        this.onChangeTitleSearch = this.onChangeTitleSearch.bind(this);
        this.onSearch = this.onSearch.bind(this);
        this.onSelectTitle = this.onSelectTitle.bind(this);
        this.onResetTitle = this.onResetTitle.bind(this);
    }

    componentDidMount = () => {
        let currentConfig = ConfigurationState.currentConfiguration;
        if (currentConfig.xBoxLiveConfig?.titleId){
            ConfigurationService.resolveXBoxLiveTitleInfo(currentConfig.xBoxLiveConfig.titleId, currentConfig.xBoxLiveConfig.xApiKey)
            .then(titleInfo => {
                this.setState({
                    selectedTitle: titleInfo,
                    isLoading:false,
                });
            })
            .catch(error => {
                this.setState({
                    selectedTitle: null,
                    isLoading: false,
                });
            })
        }
        else
        {
            this.fetchRecentTitles(currentConfig.xBoxLiveConfig.streamerXuid, currentConfig.xBoxLiveConfig.xApiKey);
        }
    }

    fetchRecentTitles = (xuid: string, xApiKey: string) => {
        this.setState({
            searchResults: [],
            selectedTitle: null,
            isLoading: true,
        });
        ConfigurationService.getRecentXBoxLiveTitleInfo(xuid, xApiKey)
        .then(titleList => {
            this.setState({
                searchResults: titleList,
                selectedTitle: null,
                isLoading: false,
            })
        })
        .catch( () => {
            this.setState({
                isLoading: false,
            });
        });
    }

    onContinue = async (e: React.SyntheticEvent<HTMLInputElement>) => {
        ConfigurationState.currentConfiguration.xBoxLiveConfig.titleId = this.state.selectedTitle.titleId;
        this.props.onValid(this, this.props.nextState);
    }

    onChangeTitleSearch = async (e: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({
            titleSearch: e.currentTarget.value,
            searchResults: this.state.searchResults,
        });
    }

    onSearch = async (e: React.SyntheticEvent<HTMLInputElement>) => {
        this.setState({
            searchResults: [],
            selectedTitle: null,
            isLoading: true,
        });
        let titleInfos = await AchievementsService.searchXApiTitleInfo(this.state.titleSearch, ConfigurationState.currentConfiguration.xBoxLiveConfig.xApiKey);
        this.setState({
            titleSearch: this.state.titleSearch,
            searchResults: titleInfos,
            selectedTitle: this.state.selectedTitle,
            isLoading: false,
        });
    }

    onSelectTitle = (e: React.MouseEvent<HTMLElement>, titleId: string) => {
        let titleInfo = this.state.searchResults.find(t => t.titleId == titleId);

        this.setState({
            titleSearch: this.state.titleSearch,
            searchResults: this.state.searchResults,
            selectedTitle: titleInfo,
        });

        // Validate and move on
    }

    onResetTitle = (e: React.MouseEvent<HTMLInputElement>) => {
        this.setState({
            titleSearch: this.state.titleSearch,
            searchResults: this.state.searchResults,
            selectedTitle: null,
        });
        let currentConfig = ConfigurationState.currentConfiguration;
        this.fetchRecentTitles(currentConfig.xBoxLiveConfig.streamerXuid, currentConfig.xBoxLiveConfig.xApiKey);
    }

    render(){
        const isContinueEnabled = this.state.selectedTitle;
        let content;
        if (this.state.isLoading)
        {
            content = (<div className="spinner"></div>);
        }
        else if (this.state.selectedTitle)
        {
            let changeButton = (<input className="section" type="button" name="xBoxTitleChange" value="Change" onClick={this.onResetTitle} />);
            content = (
                <GameCard titleInfo={this.state.selectedTitle} buttonSection={changeButton} />
            );
        }
        else
        {
            content = [
                <label htmlFor="titleSearch">Search Game</label>,
                <input name="titleSearch" type="text" placeholder="Search a game title" onChange={this.onChangeTitleSearch} />,
                <input type="button" value="Search" onClick={this.onSearch} />,
                <div className="searchResult container">
                    <div className="row">
                    {
                        this.state.searchResults.map((titleInfo, i) => (
                            <GameCard titleInfo={titleInfo} buttonSection={<input className="section" type="button" name="xBoxTitleChange" value="Select" onClick={(e) => this.onSelectTitle(e, titleInfo.titleId)} />} />
                        ))
                    }
                    </div>
                </div>
            ];
        }

        return [
            content,
            <input type="button" value="Continue" disabled={!isContinueEnabled} onClick={this.onContinue} />
        ]
    }
}