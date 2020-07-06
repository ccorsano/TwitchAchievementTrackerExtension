import * as React from "react";
import { PlayerInfoCard } from "../../common/EBSTypes";
import '../../../public/mini-default.min.css';

type GamerCardComponentProps = {
    playerInfo: PlayerInfoCard,
    buttonSection: React.ReactNode,
}

export default class GamerCardComponent extends React.Component<GamerCardComponentProps, any> {
    constructor(props: GamerCardComponentProps){
        super(props);
    }

    render(){
        return (
            <div className="card">
                <h2 className="section">{this.props.playerInfo.playerName}</h2>
                <div className="section">Id: {this.props.playerInfo.playerId}</div>
                <img src={this.props.playerInfo.avatarUrl} className="section media" />
                { this.props.buttonSection }
            </div>
        );
    }
}