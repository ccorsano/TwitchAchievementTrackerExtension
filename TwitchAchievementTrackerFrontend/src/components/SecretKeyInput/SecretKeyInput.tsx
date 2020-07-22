import * as React from "react";

export interface SecretKeyInputProps {
    onChangeValue: (e: React.SyntheticEvent<HTMLInputElement>) => void;
    keyValue: string,
    isSyntaxValid: boolean,
    pattern: string,
    size: number,
    placeholder: string
}

export interface SecretKeyInputState {
    isKeyVisible: boolean,
}

export default class SecretKeyInput extends React.Component<SecretKeyInputProps, SecretKeyInputState> {
    state: SecretKeyInputState = {
        isKeyVisible: false,
    }

    constructor(props: any){
        super(props);
    }

    onToggleVisibility = () => {
        this.setState({
            isKeyVisible: !this.state.isKeyVisible
        });
    }

    render(){
        return [
            <input name="xapikey" type={this.state.isKeyVisible ? "text" : "password"} pattern={this.props.pattern} size={this.props.size} value={this.props.keyValue} placeholder={this.props.placeholder} onChange={this.props.onChangeValue} className={this.props.isSyntaxValid ? '' : 'sf1-invalid'} />,
            <input type="button" onClick={this.onToggleVisibility} value={this.state.isKeyVisible ? "Hide" : "Show"} />,
        ]
    }
}