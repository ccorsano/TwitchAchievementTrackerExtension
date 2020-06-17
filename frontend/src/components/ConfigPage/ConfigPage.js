import React from 'react'
import Configuration from '../../util/Configuration/Configuration'

import './Config.css'
import '../../../public/mini-default.min.css'

export default class ConfigPage extends React.Component{
    constructor(props){
        super(props)

        this.Configuration = new Configuration();

        //if the extension is running on twitch or dev rig, set the shorthand here. otherwise, set to null. 
        this.twitch = window.Twitch ? window.Twitch.ext : null
        this.state={
            finishedLoading:false,
            theme:'light',
            xBoxLive: {
                gameQuery: ""
            },
            steam: {
                gameQuery: ""
            }
        }

        this.handleSubmit = this.handleSubmit.bind(this);
        this.searchXboxTitle = this.searchXboxTitle.bind(this);
        this.handleXBoxLiveConfigChange = this.handleXBoxLiveConfigChange.bind(this);
    }

    handleSubmit(event) {

    }

    searchXboxTitle(event) {

    }

    contextUpdate(context, delta){
        if(delta.includes('theme')){
            this.setState(()=>{
                return {theme:context.theme}
            })
        }
    }

    componentDidMount(){
        // do config page setup as needed here
        if(this.twitch){
            this.twitch.onAuthorized((auth)=>{
                this.Configuration.setToken(auth.token)
                if(!this.state.finishedLoading){
                    // if the component hasn't finished loading (as in we've not set up after getting a token), let's set it up now.
    
                    // now we've done the setup for the component, let's set the state to true to force a rerender with the correct data.
                    this.setState(()=>{
                        return {finishedLoading:true}
                    })
                }
            })
    
            this.twitch.onContext((context,delta)=>{
                this.contextUpdate(context,delta)
            })
        }
    }

    handleXBoxLiveConfigChange(event) {
        const target = event.target;
        const value = target.value;
        const name = target.name;
        this.setState({ xBoxLive: { [name]: value } });
    }

    handleSteamConfigChange(event) {
        const target = event.target;
        const value = target.value;
        const name = target.name;
        this.setState({ steam: { [name]: value } });
    }

    render(){
        return(
            <form onSubmit={this.handleSubmit}>
                <div class="collapse">
                    <input type="radio" id="collapse-xbl" checked aria-hidden="true" name="platform" />
                    <label for="collapse-xbl" aria-hidden="true">XBoxLive</label>
                    <div>
                        <h2>XBoxLive Achievement tracker configuration</h2>
                        <fieldset class="utilities container">
                            <legend>Search the Game XBL identifier (TitleId)</legend>
                            <div class="row col-md-12">
                                <div class="row col-md-6">
                                    <div class="col-sm-12 col-md-6">
                                        <label for="gameQuery">Game name</label>
                                    </div>
                                    <div class="col-sm-12 col-md">
                                        <input type="text" name="gameQuery" value={this.state.xBoxLiveGameQuery} onChange={this.handleXBoxLiveConfigChange} ></input>
                                    </div>
                                </div>
                                <div class="col-sm-12 col-md-6" id="gameQueryResult">
                                    <ul>
                                    </ul>
                                </div>
                            </div>
                            <input type="button" value="Search" onClick={this.searchXboxTitle} />
                        </fieldset>
                        <fieldset class="utilities container">
                            <legend>Get the XBoxLive Id (XUID) from a gamertag</legend>
                            <div class="row">
                                <div class="col-sm-12 col-md-3">
                                    <label for="gamertag">Gamertag</label>
                                </div>
                                <div class="col-sm-12 col-md">
                                    <input type="text" name="gamertag"></input>
                                    <span id="resolvedXuid"></span>
                                </div>
                            </div>
                            <input type="button" id="resolveXuid" value="Resolve" />
                        </fieldset>
                        <fieldset class="configuration container">
                            <legend>Configuration</legend>
                            <strong>Make sure you connect to xapi.us and refresh your XBoxLive token there before going live !</strong>
                            <div class="row">
                                <div class="col-sm-12 col-md-3">
                                    <label for="xapiKey">xapi.us private key</label>
                                </div>
                                <div class="col-sm-12 col-md">
                                    <input type="text" name="xapiKey"></input>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-md-3">
                                    <label for="streamerXuid">Streamer XBoxLive identifier (XUID)</label>
                                </div>
                                <div class="col-sm-12 col-md">
                                    <input type="text" name="streamerXuid"></input>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-md-3">
                                    <label for="titleId">Game identifier (TitleId)</label>
                                </div>
                                <div class="col-sm-12 col-md">
                                    <input type="text" name="titleId"></input>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-md-3">
                                    <label for="titleId">Locale</label>
                                </div>
                                <div class="col-sm-12 col-md">
                                    <input type="text" name="locale" placeholder="fr-FR"></input>
                                </div>
                            </div>
                            <input type="button" id="saveConfig" value="Save"></input>
                        </fieldset>
                    </div>
                </div>
                <div class="collapse">
                    <input type="radio" id="collapse-steam" aria-hidden="true" name="platform" />
                    <label for="collapse-steam" aria-hidden="true">Steam</label>
                    <div>
                        <h2>Steam Achievement tracker configuration</h2>
                        <fieldset class="utilities container">
                            <legend>Search the Game Steam identifier (AppId)</legend>
                            <div class="row col-md-12">
                                <div class="row col-md-6">
                                    <div class="col-sm-12 col-md-6">
                                        <label for="steamGameQuery">Game name</label>
                                    </div>
                                    <div class="col-sm-12 col-md">
                                        <input type="text" name="steamGameQuery" id="#steamGameQueryInput"></input>
                                    </div>
                                </div>
                                <div class="col-sm-12 col-md-6" id="steamGameQueryResult">
                                    <ul>
                                    </ul>
                                </div>
                            </div>
                            <input type="button" id="steamSearchTitle" value="Search" />
                        </fieldset>
                        <fieldset class="configuration container">
                            <legend>Configuration</legend>
                            <div class="row">
                                <div class="col-sm-12 col-md-3">
                                    <label for="steamWebApiKey">Steam WebApi private key</label>
                                </div>
                                <div class="col-sm-12 col-md">
                                    <input type="text" name="steamWebApiKey"></input>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-md-3">
                                    <label for="streamerSteamId">Streamer Steam identifier (SteamID)</label>
                                </div>
                                <div class="col-sm-12 col-md">
                                    <input type="text" name="streamerSteamId"></input>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-md-3">
                                    <label for="steamAppId">Game identifier (AppId)</label>
                                </div>
                                <div class="col-sm-12 col-md">
                                    <input type="text" name="steamAppId"></input>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-md-3">
                                    <label for="steamLocale">Locale</label>
                                </div>
                                <div class="col-sm-12 col-md">
                                    <input type="text" name="steamLocale" placeholder="french"></input>
                                </div>
                            </div>
                            <input type="button" id="saveSteamConfig" value="Save" />
                        </fieldset>
                    </div>
                </div>
                <input type="button" id="saveActiveConfig" value="Save"></input>
            </form>
        )
    }
}