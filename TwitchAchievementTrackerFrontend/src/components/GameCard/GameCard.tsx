import * as React from "react";
import { TitleInfo } from "../../common/EBSTypes";
import '../../../public/mini-default.min.css';

type GameCardProps = {
    titleInfo: TitleInfo,
    buttonSection: React.ReactNode,
}

export default class GameCard extends React.Component<GameCardProps, any> {
    constructor(props: GameCardProps){
        super(props);
    }

    render(){
        return (
            <div className="card small">
                <h2 className="section">{this.props.titleInfo.productTitle}</h2>
                <img className="section media" src={this.props.titleInfo.logoUri}></img>
                {this.props.buttonSection}
            </div>
        );
    }
}