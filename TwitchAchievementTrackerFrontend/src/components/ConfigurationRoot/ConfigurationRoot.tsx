import * as React from 'react'
import ConfigChoosePlatform from '../ConfigChoosePlatform/ConfigChoosePlatform';

export default class ConfigurationRoot extends React.Component {
    constructor(props : any){
        super(props);

        this.onPlatformChosen = this.onPlatformChosen.bind(this);
    }

    onPlatformChosen = () => {
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