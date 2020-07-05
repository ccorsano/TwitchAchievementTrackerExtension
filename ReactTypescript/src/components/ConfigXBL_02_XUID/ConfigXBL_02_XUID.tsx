import * as React from 'react'
import * as Base from '../../common/ConfigStepBase';
import { AchievementsService } from '../../services/EBSAchievementsService';
import { ConfigurationState } from '../../services/ConfigurationStateService';
import { PlayerInfoCard } from '../../common/EBSTypes';
import { ConfigurationService } from '../../services/EBSConfigurationService';
import GamerCardComponent from '../GamerCard/GamerCard';

type ConfigXBL_02_XUIDState = {
    xuidSearch: string;
    xuid: string;
    gamerCard: PlayerInfoCard;
    isLoading: boolean,
}

export default class ConfigXBL_02_XUID extends Base.ConfigStepBase<Base.ConfigStepBaseProps, ConfigXBL_02_XUIDState> {
    state: ConfigXBL_02_XUIDState = {
        xuidSearch: "",
        xuid: null,
        gamerCard: null,
        isLoading: true,
    }

    constructor(props: Base.ConfigStepBaseProps) {
        super(props);

        this.onChangeGamertagSearch = this.onChangeGamertagSearch.bind(this);
        this.onChangeGamertagSearch = this.onChangeGamertagSearch.bind(this);
        this.changeXuid = this.changeXuid.bind(this);
        this.onSearch = this.onSearch.bind(this);
    }

    componentDidMount = () => {
        let currentConfig = ConfigurationState.currentConfiguration;
        if (currentConfig.xBoxLiveConfig?.streamerXuid){
            this.changeXuid(currentConfig.xBoxLiveConfig.streamerXuid);
        }
        else
        {
            this.setState({
                isLoading: false,
            });
        }
    }

    changeXuid = (xuid: string) => {
        this.setState({
            xuid: null,
            isLoading: true,
        });

        ConfigurationService.resolveXBoxLivePlayerInfo(xuid, ConfigurationState.currentConfiguration.xBoxLiveConfig.xApiKey)
        .then(playerInfo => {
            this.setState({
                xuid: xuid,
                gamerCard: playerInfo,
                isLoading: false,
            });
        });
    }

    onChangeGamertagSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({
            xuidSearch: e.currentTarget.value,
        })
    }

    onSearch = async (e: React.SyntheticEvent<HTMLInputElement>) => {
        this.setState({
            xuidSearch: this.state.xuidSearch,
            xuid: null,
            gamerCard: null,
            isLoading: true,
        });

        let xuid = await AchievementsService.resolveGamertag(this.state.xuidSearch);

        if (xuid){
            this.setState({
                xuidSearch: this.state.xuidSearch,
                xuid: xuid,
            });
            this.changeXuid(xuid);
        }
    }

    onResetProfile = (e: React.SyntheticEvent<HTMLInputElement>) => {
        this.setState({
            xuid: null,
            xuidSearch: '',
            gamerCard: null,
        });
    }

    onContinue = (e: React.SyntheticEvent<HTMLInputElement>) => {
        ConfigurationState.currentConfiguration.xBoxLiveConfig.streamerXuid = this.state.xuid;
        this.props.onValid(this, this.props.nextState);
    }

    render(){
        const isContinueEnabled = this.state.xuid;

        let content: React.ReactNodeArray = null;
        if (this.state.isLoading)
        {
            content = [
                <div>Loading ...</div>
            ]
        }
        else if (this.state.gamerCard)
        {
            let changeButton = (<input type="button" value="Change" className="section" onClick={this.onResetProfile} />)
            content = [
                <GamerCardComponent playerInfo={this.state.gamerCard} buttonSection={changeButton} />,
                <input type="button" value="Continue" disabled={!isContinueEnabled} onClick={this.onContinue} />
            ]
        }
        else
        {
            content = [
                <label htmlFor="xuidSearch">Streamer Id</label>,
                <input name="xuidSearch" type="text" placeholder="Search a Gamertag" onChange={this.onChangeGamertagSearch} />,
                <input type="button" value="Search" onClick={this.onSearch} />,
            ]
        }

        return [
           content
        ]
    }
}