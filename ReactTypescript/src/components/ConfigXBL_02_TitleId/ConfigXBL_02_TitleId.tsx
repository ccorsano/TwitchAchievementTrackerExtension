import './ConfigXBL_02_TitleId.scss';
import * as React from 'react'
import * as Base from '../../common/ConfigStepBase';
import { AchievementsService, TitleInfo } from '../../services/EBSAchievementsService'

type ConfigXBL_02_TitleIdState = {
    titleSearch: string;
    searchResults: TitleInfo[];
    selectedTitleId: string;
}

export default class ConfigXBL_02_TitleId extends Base.ConfigStepBase<Base.ConfigStepBaseProps, ConfigXBL_02_TitleIdState> {
    state: ConfigXBL_02_TitleIdState = {
        titleSearch: "",
        searchResults: [],
        selectedTitleId: null,
    }

    constructor(props: Base.ConfigStepBaseProps) {
        super(props);
        this.onSelectTitle = this.onSelectTitle.bind(this);
    }

    onContinue = async (e: React.SyntheticEvent<HTMLInputElement>) => {
    }

    onChangeTitleSearch = async (e: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({
            titleSearch: e.currentTarget.value,
            searchResults: this.state.searchResults,
        });
    }

    onSearch = async (e: React.SyntheticEvent<HTMLInputElement>) => {
        let titleInfos = await AchievementsService.searchTitleInfo(this.state.titleSearch);
        this.setState({
            titleSearch: this.state.titleSearch,
            searchResults: titleInfos,
            selectedTitleId: this.state.selectedTitleId,
        });
    }

    onSelectTitle = (e: React.MouseEvent<HTMLLIElement>) => {
        let titleId = e.currentTarget.attributes.getNamedItem("itemId").value;
        this.setState({
            titleSearch: this.state.titleSearch,
            searchResults: this.state.searchResults,
            selectedTitleId: titleId,
        });

        // Validate and move on
    }

    render(){
        const isContinueEnabled = false;

        return [
            <label htmlFor="titleSearch">Game Title</label>,
            <input name="titleSearch" type="text" placeholder="Search a game title" onChange={this.onChangeTitleSearch} />,
            <input type="button" value="Search" onClick={this.onSearch} />,
            <ul className="searchResult">
                {
                    this.state.searchResults.map((titleInfo, i) => (
                        <li itemID={titleInfo.titleId} key={titleInfo.titleId} onClick={this.onSelectTitle}><img src={titleInfo.logoUri}></img> {titleInfo.productTitle}</li>
                    ))
                }
            </ul>,
            <input type="button" value="Continue" disabled={!isContinueEnabled} onClick={this.onContinue} />
        ]
    }
}