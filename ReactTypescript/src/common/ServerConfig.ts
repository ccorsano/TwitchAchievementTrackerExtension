var server = "https://twitchext.conceptoire.com/v2"
var intervalTimer = false;

var urlParams = new URLSearchParams(window.location.search);
if (urlParams.get('state') == "testing")
{
    server = "http://localhost:8081"
}

export const EBSBaseUrl = server;