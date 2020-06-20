var token = undefined;
var tuid = "";
var ebs = "";
var detailsVisible = false;
var server = "twitchext.conceptoire.com/v2"
var intervalTimer = false;

var urlParams = new URLSearchParams(this.location.search);
if (urlParams.get('state') == "testing")
{
    server = "localhost:8081"
}

// because who wants to type this every time?
var twitch = window.Twitch.ext;

// create the request options for our Twitch API calls
var requests = {
    titleInfo: createAchievementsRequest('GET', 'title', updateTitle),
    summary: createAchievementsRequest('GET', 'summary', updateSummary),
    listAchievement: createAchievementsRequest('GET', 'list', updateBlock)
};

function createAchievementsRequest(type, method, callback) {
    twitch.rig.log('Create request ' + location.protocol + '//' + server + '/api/achievements/' + method);
    return {
        type: type,
        url: location.protocol + '//' + server + '/api/achievements/' + method,
        success: callback,
        error: logError
    }
}

function setAuth(token, config, version) {
    Object.keys(requests).forEach((req) => {
        twitch.rig.log('Setting auth headers for request ' + req);
        requests[req].headers = {
            'Authorization': 'Bearer ' + token,
            'X-Config-Token': config,
            'X-Config-Version': version,
        }
    });
}

twitch.onContext(function(context) {
    twitch.rig.log(context);
});

function tryRefreshConfiguration() {
    if (! token || ! twitch.configuration.broadcaster)
    {
        twitch.rig.log('Delaying config refresh, extension not ready yet');
        return false;
    }

    var configToken = twitch.configuration.broadcaster.content;
    var configVersion = twitch.configuration.broadcaster.version;

    setAuth(token, configToken, configVersion);
    $.ajax(requests.titleInfo);
    $.ajax(requests.summary);

    // Refresh the data every minute
    if (intervalTimer)
    {
        clearInterval(intervalTimer);
    }
    intervalTimer = setInterval(function() {
        $.ajax(requests.summary);
        if (detailsVisible)
        {
            $.ajax(requests.listAchievement);
        }
    }, 60000);

    return true;
}

twitch.configuration.onChanged(function(){
    if (! twitch.configuration.broadcaster)
    {
        throw 'Could not load broadcaster config';
    }
    tryRefreshConfiguration();
})

twitch.onAuthorized(function(auth) {
    // save our credentials
    token = auth.token;
    tuid = auth.userId;

    tryRefreshConfiguration();
});

function updateTitle(titleInfo) {
    $('#gameTitle').text(titleInfo.productTitle);
    $('#gameLogo').attr('src', titleInfo.logoUri);
}

function updateSummary(summary) {
    twitch.rig.log('Updating summary');
    twitch.rig.log('Summary : ' + summary.completed + '/' + summary.total + ', ' + summary.currentPoints + '/' + summary.totalPoints + ' gamerscore');
    var percentage = (summary.completed / summary.total) * 100.0;
    $('#completionHeadline').text('Achievements: ' + summary.completed + '/' + summary.total + ' ' + percentage.toFixed(2) + '%');
}

function updateBlock(achievements) {
    var container = $('#list');
    achievements.forEach(achievement => {
        var listItemId = 'achievement_' + achievement.id;
        var listItem = $('#' + listItemId);
        var nameDiv = $(listItem).children('div.achievementTitle');
        var descriptionDiv = $(listItem).children('div.achievementDescription');
        if (! listItem.length)
        {
            listItem = document.createElement("li");
            $(listItem).attr('id', listItemId);
            nameDiv = document.createElement("div");
            nameDiv.className = "achievementTitle";
            descriptionDiv = document.createElement("div");
            descriptionDiv.className = "achievementDescription";
            $(descriptionDiv).text(achievement.description);
            $(listItem).append(nameDiv);
            $(listItem).append(descriptionDiv);
            container.append(listItem);
        }
        listItem.className = achievement.completed ? "completed" : "notCompleted";
        
        $(nameDiv).text(achievement.name);
    });
}

function logError(_, error, status) {
  twitch.rig.log('EBS request returned '+status+' ('+error+')');
}

function logSuccess(hex, status) {
  // we could also use the output to update the block synchronously here,
  // but we want all views to get the same broadcast response at the same time.
  twitch.rig.log('EBS request returned '+hex+' ('+status+')');
}

$(function() {
    $('#showDetails').click(function() {
        $("#showDetails").addClass(detailsVisible ? "collapsed" : "open");
        $("#showDetails").removeClass(detailsVisible ? "open" : "collapsed");
        if (detailsVisible)
        {
            $("#list").css('display', 'none')
            detailsVisible = false;
        }
        else
        {
            $("#list").css('display', 'block')
            $.ajax(requests.listAchievement)
            detailsVisible = true;
        }
    });
});
