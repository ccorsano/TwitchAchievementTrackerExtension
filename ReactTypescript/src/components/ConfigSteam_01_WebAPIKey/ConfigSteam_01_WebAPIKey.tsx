import * as React from 'react'
import '../../common/ConfigStepBase'
import * as Base from '../../common/ConfigStepBase';

export default class ConfigSteam_01_WebAPIKey extends Base.ConfigStepBase<Base.ConfigStepBaseProps, any> {
    constructor(props: Base.ConfigStepBaseProps) {
        super(props);
    }

    render(){
        return [
            <label htmlFor="webApiKey">Steam WebApi Key</label>,
            <input type="text" name="webApiKey" placeholder="Enter your Steal WebApi key" />
        ]
    }
}