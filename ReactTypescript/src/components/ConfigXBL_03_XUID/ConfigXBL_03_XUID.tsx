import * as React from 'react'
import * as Base from '../../common/ConfigStepBase';
import { AchievementsService, TitleInfo } from '../../services/EBSAchievementsService';
import { ConfigurationState } from '../../services/ConfigurationStateService';

type ConfigXBL_03_XUIDState = {
    xuidSearch: string;
    xuid: string;
}

export default class ConfigXBL_03_XUID extends Base.ConfigStepBase<Base.ConfigStepBaseProps, ConfigXBL_03_XUIDState> {
    state: ConfigXBL_03_XUIDState = {
        xuidSearch: "",
        xuid: null
    }

    constructor(props: Base.ConfigStepBaseProps) {
        super(props);

        this.onChangeGamertagSearch = this.onChangeGamertagSearch.bind(this);
    }

    onChangeGamertagSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({
            xuidSearch: e.currentTarget.value,
            xuid: null,
        })
    }

    onSearch = async (e: React.SyntheticEvent<HTMLInputElement>) => {
        let xuid = await AchievementsService.resolveGamertag(this.state.xuidSearch);

        if (xuid){
            this.setState({
                xuidSearch: this.state.xuidSearch,
                xuid: xuid
            });
        }
    }

    onContinue = (e: React.SyntheticEvent<HTMLInputElement>) => {
        ConfigurationState.currentConfiguration.xBoxLiveConfig.streamerXuid = this.state.xuid;
        this.props.onValid(this, this.props.nextState);
    }

    render(){
        const isContinueEnabled = this.state.xuid;

        return [
            <label htmlFor="xuidSearch">Streamer Id</label>,
            <input name="xuidSearch" type="text" placeholder="Search a Gamertag" onChange={this.onChangeGamertagSearch} />,
            <input type="button" value="Search" onClick={this.onSearch} />,
            <input type="button" value="Continue" disabled={!isContinueEnabled} onClick={this.onContinue} />
        ]
    }
}