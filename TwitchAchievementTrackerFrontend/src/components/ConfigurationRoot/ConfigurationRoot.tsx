import * as React from 'react'
import ConfigChoosePlatform from '../ConfigChoosePlatform/ConfigChoosePlatform';
import { createPortal } from 'react-dom';

export default class ConfigurationRoot extends React.Component {
    constructor(props : any){
        super(props);

        this.onPlatformChosen = this.onPlatformChosen.bind(this);
    }

    onPlatformChosen = (e: React.SyntheticEvent<HTMLInputElement>) => {
    }

    render(){
        document.getElementById("root")
        return (
            <form>
                <h1>Configuration</h1>
                <ConfigChoosePlatform changeHandler={this.onPlatformChosen} />
            </form>
        )
    }
}