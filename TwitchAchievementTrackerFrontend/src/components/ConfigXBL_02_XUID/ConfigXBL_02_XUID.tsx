import * as React from 'react'
import * as Base from '../../common/ConfigStepBase';
import { PlayerInfoCard } from '../../common/EBSTypes';
import { ConfigurationService } from '../../services/EBSConfigurationService';
import GamerCardComponent from '../GamerCard/GamerCard';
import ConfigXBL_03_TitleId from '../ConfigXBL_03_TitleId/ConfigXBL_03_TitleId';

interface ConfigXBL_02_XUIDProps extends Base.ConfigStepBaseProps {
    xApiKey: string,
}

type ConfigXBL_02_XUIDState = {
    xuidSearch: string;
    xuid: string;
    gamerCard: PlayerInfoCard;
    isLoading: boolean,
    isConfirmed: boolean,
}

export default class ConfigXBL_02_XUID extends Base.ConfigStepBase<ConfigXBL_02_XUIDProps, ConfigXBL_02_XUIDState> {
    state: ConfigXBL_02_XUIDState = {
        xuidSearch: "",
        xuid: null,
        gamerCard: null,
        isLoading: true,
        isConfirmed: false,
    }

    constructor(props: ConfigXBL_02_XUIDProps) {
        super(props);

        this.onChangeGamertagSearch = this.onChangeGamertagSearch.bind(this);
        this.onChangeGamertagSearch = this.onChangeGamertagSearch.bind(this);
        this.changeXuid = this.changeXuid.bind(this);
        this.onSearch = this.onSearch.bind(this);
    }

    componentDidMount = () => {
        let currentConfig = this.props.savedConfiguration;
        if (currentConfig?.xBoxLiveConfig?.streamerXuid){
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

        ConfigurationService.resolveXBoxLivePlayerInfo(xuid, this.props.xApiKey)
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

    onSearch = async (_e: React.SyntheticEvent<HTMLInputElement>) => {
        this.setState({
            xuidSearch: this.state.xuidSearch,
            xuid: null,
            gamerCard: null,
            isLoading: true,
        });

        let gamerCard = await ConfigurationService.resolveXBoxLiveGamertag(this.state.xuidSearch, this.props.xApiKey);

        this.setState({
            xuid: gamerCard.playerId,
            gamerCard: gamerCard,
            isLoading: false,
        });
    }

    onResetProfile = (_e: React.SyntheticEvent<HTMLInputElement>) => {
        this.setState({
            xuid: null,
            xuidSearch: '',
            gamerCard: null,
        });
    }

    onContinue = (_e: React.SyntheticEvent<HTMLInputElement>) => {
        this.setState({
            isConfirmed: true,
        });
    }

    unvalidate = (_e: any) => {
        this.setState({
            isConfirmed: false,
        })
    }

    render(){
        const isContinueEnabled = this.state.xuid;

        if (this.state.isConfirmed){
            return (
                <ConfigXBL_03_TitleId
                    savedConfiguration={this.props.savedConfiguration}
                    onValidate={this.props.onValidate}
                    onBack={this.unvalidate}
                    xApiKey={this.props.xApiKey}
                    streamerXuid={this.state.xuid} />
            )
        }

        let content: React.ReactNodeArray = null;
        if (this.state.isLoading)
        {
            content = [
                <div className="spinner"></div>
            ]
        }
        else if (this.state.gamerCard)
        {
            let changeButton = (<input type="button" value="Change" className="section" onClick={this.onResetProfile} />)
            content = [
                <GamerCardComponent playerInfo={this.state.gamerCard} buttonSection={changeButton} />,
                <input type="button" value="Back" onClick={this.onBack} />,
                <input type="button" value="Continue" disabled={!isContinueEnabled} onClick={this.onContinue} />
            ]
        }
        else
        {
            content = [
                <label htmlFor="xuidSearch">Streamer Id</label>,
                <input name="xuidSearch" type="text" placeholder="Search a Gamertag" value={this.state.xuidSearch} onChange={this.onChangeGamertagSearch} />,
                <input type="button" value="Search" onClick={this.onSearch} />,
                <input type="button" value="Back" onClick={this.onBack} />,
            ]
        }

        return [
           content,
        ]
    }
}