import * as React from 'react';
import * as Base from '../../common/ConfigStepBase';

export default class ConfigXBL_04_Confirm extends Base.ConfigStepBase<Base.ConfigStepBaseProps, any> {
    constructor(props: Base.ConfigStepBaseProps) {
        super(props);
    }

    onSave = (e: React.SyntheticEvent<HTMLInputElement>) => {
        this.props.onValid(this, this.props.nextState);
    }

    render(){
        return [
            <input type="button" value="Save" onClick={this.onSave} />
        ]
    }
}