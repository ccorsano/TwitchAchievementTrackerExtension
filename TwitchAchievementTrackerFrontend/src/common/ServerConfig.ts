import { TwitchExtHelper } from "./TwitchExtension";

var server = "https://twitchext.conceptoire.com/v5"

var urlParams = new URLSearchParams(window.location.search);
if (urlParams.get('state') == "testing")
{
    server = "http://localhost:8081"
}


export const EBSVersion = "2020.4";
export const EBSBaseUrl = server;