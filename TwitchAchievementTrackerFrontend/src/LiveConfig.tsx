import * as React from 'react';
import * as ReactDOM from 'react-dom';
import '../public/mini-default.min.css';
import LiveConfigRoot from './components/LiveConfigRoot/LiveConfigRoot';

ReactDOM.render(
    <LiveConfigRoot />,
    document.getElementById("root") as HTMLElement
);