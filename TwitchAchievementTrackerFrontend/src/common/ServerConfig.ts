var server = "https://twitchext.conceptoire.com/v3"
var intervalTimer = false;

var urlParams = new URLSearchParams(window.location.search);
if (urlParams.get('state') == "testing")
{
    server = "http://localhost:8081"
}


export const EBSVersion = "0.0.3";
export const EBSBaseUrl = server;