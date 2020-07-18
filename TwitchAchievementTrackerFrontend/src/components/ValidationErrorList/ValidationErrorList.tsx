import * as React from "react";
import { ValidationError } from "../../services/EBSConfigurationService";

type ValidationErrorListProps = {
    errors: ValidationError[]
}

export default class ValidationErrorList extends React.Component<ValidationErrorListProps, any>
{
    constructor(props: ValidationErrorListProps){
        super(props);
    }

    render(){
        return (
            <ul>
                {this.props.errors.map((error, i) => (
                    <li key={error.path + '_' + i}>
                        <mark className="secondary"><span className="icon-alert secondary"></span>Error</mark> {error.path} : {error.errorDescription}
                    </li>
                ))}
            </ul>
        )
    }
}