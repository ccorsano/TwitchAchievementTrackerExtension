import * as React from 'react';
import * as Base from '../../common/ConfigStepBase'
import { TitleInfo, AchievementsService } from '../../services/EBSAchievementsService';
import { ConfigurationState } from '../../services/ConfigurationStateService';

type ConfigSteam_02_AppIdState = {
    titleSearch: string;
    searchResults: TitleInfo[];
    selectedTitle: TitleInfo;
}

export default class ConfigSteam_02_AppId extends Base.ConfigStepBase<Base.ConfigStepBaseProps, ConfigSteam_02_AppIdState> {
    state: ConfigSteam_02_AppIdState = {
        titleSearch: '',
        searchResults: [],
        selectedTitle: null,
    }

    constructor(props: Base.ConfigStepBaseProps){
        super(props);

        this.onContinue = this.onContinue.bind(this);
        this.onChangeTitleSearch = this.onChangeTitleSearch.bind(this);
        this.onSearch = this.onSearch.bind(this);
        this.onSelectTitle = this.onSelectTitle.bind(this);
    }

    onContinue = async (e: React.SyntheticEvent<HTMLInputElement>) => {
        ConfigurationState.currentConfiguration.steamConfig.appId = this.state.selectedTitle.titleId;
        this.props.onValid(this, this.props.nextState);
    }

    onChangeTitleSearch = async (e: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({
            titleSearch: e.currentTarget.value,
            searchResults: this.state.searchResults,
        });
    }

    onSearch = async (e: React.SyntheticEvent<HTMLInputElement>) => {
        let titleInfos = await AchievementsService.searchSteamTitleInfo(this.state.titleSearch);
        this.setState({
            titleSearch: this.state.titleSearch,
            searchResults: titleInfos,
            selectedTitle: this.state.selectedTitle,
        });
    }

    onSelectTitle = (e: React.MouseEvent<HTMLLIElement>) => {
        let titleId = e.currentTarget.attributes.getNamedItem("itemId").value;
        let titleInfo = this.state.searchResults.find(t => t.titleId == titleId);

        this.setState({
            titleSearch: this.state.titleSearch,
            searchResults: this.state.searchResults,
            selectedTitle: titleInfo,
        });

        // Validate and move on
    }

    render(){
        const isContinueEnabled = this.state.selectedTitle;
        let selection;
        if (this.state.selectedTitle)
        {
            selection = (
                <div className="selectedTitle">
                    <img src={this.state.selectedTitle.logoUri}></img>
                    {this.state.selectedTitle.productTitle}
                </div>
            )
        }
        else
        {
            selection = (
                <ul className="searchResult">
                    {
                        this.state.searchResults.map((titleInfo, i) => (
                            <li itemID={titleInfo.titleId} key={titleInfo.titleId} onClick={this.onSelectTitle}><img src={titleInfo.logoUri}></img> {titleInfo.productTitle}</li>
                        ))
                    }
                </ul>
            )
        }

        return [
            <label htmlFor="titleSearch">Game Title</label>,
            <input name="titleSearch" type="text" placeholder="Search a game title" onChange={this.onChangeTitleSearch} />,
            <input type="button" value="Search" onClick={this.onSearch} />,
            selection,
            <input type="button" value="Continue" disabled={!isContinueEnabled} onClick={this.onContinue} />
        ]
    }
}