import './ConfigXBL_03_TitleId.scss';
import * as React from 'react'
import * as Base from '../../common/ConfigStepBase';
import { AchievementsService } from '../../services/EBSAchievementsService';
import { TitleInfo, ExtensionConfiguration, ActiveConfig } from '../../common/EBSTypes';
import { ConfigurationService, ValidationError } from '../../services/EBSConfigurationService';
import GameCard from '../GameCard/GameCard';
import ConfigXBL_04_Locale from '../ConfigXBL_04_Locale/ConfigXBL_04_Locale';
import { EBSVersion } from '../../common/ServerConfig';
import ValidationErrorList from '../ValidationErrorList/ValidationErrorList';

interface ConfigXBL_03_TitleIdProps extends Base.ConfigStepBaseProps {
    xApiKey: string,
    streamerXuid: string,
}

type ConfigXBL_03_TitleIdState = {
    titleSearch: string;
    searchResults: TitleInfo[];
    selectedTitle: TitleInfo;
    isLoading: boolean;
    isConfirmed: boolean;
    errors: ValidationError[];
}

export default class ConfigXBL_03_TitleId extends Base.ConfigStepBase<ConfigXBL_03_TitleIdProps, ConfigXBL_03_TitleIdState> {
    state: ConfigXBL_03_TitleIdState = {
        titleSearch: "",
        searchResults: [],
        selectedTitle: null,
        isLoading: true,
        isConfirmed: false,
        errors: [],
    }

    constructor(props: ConfigXBL_03_TitleIdProps) {
        super(props);

        this.onContinue = this.onContinue.bind(this);
        this.onChangeTitleSearch = this.onChangeTitleSearch.bind(this);
        this.onSearch = this.onSearch.bind(this);
        this.onSelectTitle = this.onSelectTitle.bind(this);
        this.onResetTitle = this.onResetTitle.bind(this);
    }

    componentDidMount = () => {
        let currentConfig = this.props.savedConfiguration;
        if (currentConfig?.xBoxLiveConfig?.titleId){
            ConfigurationService.resolveXBoxLiveTitleInfo(currentConfig.xBoxLiveConfig.titleId, currentConfig.xBoxLiveConfig.xApiKey)
            .then(titleInfo => {
                this.validateTitle(titleInfo);
            })
            .catch(() => {
                this.setState({
                    selectedTitle: null,
                    isLoading: false,
                });
            })
        }
        else
        {
            this.fetchRecentTitles(this.props.streamerXuid, this.props.xApiKey);
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

    onContinue = async () => {
        this.setState({
            isConfirmed: true,
        });
    }

    onChangeTitleSearch = async (e: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({
            titleSearch: e.currentTarget.value,
            searchResults: this.state.searchResults,
        });
    }

    onSearch = async () => {
        this.setState({
            searchResults: [],
            selectedTitle: null,
            isLoading: true,
            errors: [],
        });
        let titleInfos = await AchievementsService.searchXApiTitleInfo(this.state.titleSearch, this.props.xApiKey);
        this.setState({
            titleSearch: this.state.titleSearch,
            searchResults: titleInfos,
            selectedTitle: this.state.selectedTitle,
            isLoading: false,
        });
    }

    onSelectTitle = async (_e: React.MouseEvent<HTMLElement>, titleId: string) => {
        let titleInfo = this.state.searchResults.find(t => t.titleId == titleId);

        this.setState({
            titleSearch: this.state.titleSearch,
            searchResults: this.state.searchResults,
            selectedTitle: titleInfo,
            isLoading: true,
            errors: [],
        });

        // Validate and move on
        await this.validateTitle(titleInfo);
    }

    validateTitle = async (titleInfo: TitleInfo) => {
        this.setState({
            isLoading: true,
            errors: [],
        });

        const configuration: ExtensionConfiguration = {
            activeConfig: ActiveConfig.XBoxLive,
            steamConfig: null,
            version: EBSVersion,
            xBoxLiveConfig: {
                xApiKey: this.props.xApiKey,
                locale: this.props.savedConfiguration?.xBoxLiveConfig?.locale ?? "en",
                streamerXuid: this.props.streamerXuid,
                titleId: titleInfo.titleId,
            }
        }

        let errors: ValidationError[] = [];
        try
        {
            errors = await ConfigurationService.validateTitle(configuration);
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
            isLoading: false,
            selectedTitle: titleInfo,
        });
    }

    onResetTitle = () => {
        this.setState({
            titleSearch: this.state.titleSearch,
            searchResults: this.state.searchResults,
            selectedTitle: null,
        });
        this.fetchRecentTitles(this.props.streamerXuid, this.props.xApiKey);
    }

    unvalidate = () => {
        this.setState({
            isConfirmed: false,
        });
    }

    render(){
        const isContinueEnabled = this.state.selectedTitle && this.state.errors?.length == 0;
        let content;

        if (this.state.isConfirmed){
            return (
                <ConfigXBL_04_Locale
                    savedConfiguration={this.props.savedConfiguration}
                    onValidate={this.props.onValidate}
                    onBack={this.unvalidate}
                    xApiKey={this.props.xApiKey}
                    streamerXuid={this.props.streamerXuid}
                    titleId={this.state.selectedTitle.titleId} />
            )
        }

        if (this.state.isLoading)
        {
            content = (<div className="spinner"></div>);
        }
        else if (this.state.selectedTitle)
        {
            let changeButton = (<input className="section" type="button" name="xBoxTitleChange" value="Change" onClick={this.onResetTitle} />);
            content = (
                <React.Fragment>
                    <ValidationErrorList errors={this.state.errors} />
                    <GameCard titleInfo={this.state.selectedTitle} buttonSection={changeButton} />
                </React.Fragment>
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
                        this.state.searchResults.map((titleInfo) => (
                            <GameCard titleInfo={titleInfo} buttonSection={<input className="section" type="button" name="xBoxTitleChange" value="Select" onClick={(e) => this.onSelectTitle(e, titleInfo.titleId)} />} />
                        ))
                    }
                    </div>
                </div>
            ];
        }

        return [
            content,
            <input type="button" value="Back" onClick={this.onBack} />,
            <input type="button" value="Continue" disabled={!isContinueEnabled} onClick={this.onContinue} />
        ]
    }
}