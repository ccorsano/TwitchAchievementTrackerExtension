<script lang="ts">
import { type Achievement, type AchievementSummary, ActiveConfig, type TitleInfo } from "../../common/EBSTypes";
import { AchievementsService } from "../../services/EBSAchievementsService";
import { Twitch } from "../../services/TwitchService";
import AchievementsList from "../AchievementsList/AchievementsList.svelte";
import Cup from '../../../assets/cup.svg';
import Logo from '../../../assets/logo.png';

let isConfigurationValid: boolean = false
let titleInfo: TitleInfo = {
    platform: ActiveConfig.None,
    titleId: "",
    productTitle: "",
    productDescription: "",
    logoUri: "",
}
let achievementsSummary: AchievementSummary | null = null
let achievementsDetails: Achievement[] = []

let refreshAll = async () => {
    let titleInfoPromise = AchievementsService.getTitleInfo();
    AchievementsService.getSummary().then(summary => achievementsSummary = summary)
    AchievementsService.getAchievements().then(achievements => achievementsDetails = achievements)

    try {
        titleInfo = await titleInfoPromise;
        isConfigurationValid = true
    }
    catch(error)
    {
        isConfigurationValid = false
    }
}

let refreshSummary = async () => {
    let summaryPromise = AchievementsService.getSummary();
    let achievementsPromise = AchievementsService.getAchievements();

    achievementsSummary = await summaryPromise
    achievementsDetails = await achievementsPromise
}

refreshAll();
setInterval(refreshSummary, 60000);

Twitch.listen("broadcast", (_target, _contentType, messageStr) => {
    let message = JSON.parse(messageStr);
    let configToken = AchievementsService.configuration!.content;
    
    switch (message.type) {
        case "refresh":
            refreshAll();
            break;
        case "set-config":
            if (message.configToken != configToken)
            {
                AchievementsService.configuration!.content = message.configToken;
                AchievementsService.configuration!.version = message.version;
                refreshAll();
            }
            break;
        default:
            break;
    }
});

let percentage:number = 0.0
let completedCount:number = 0
let totalCount:number = 0
let platformClass = ""
let logoClassName = ""
let logoUri = `url(${Logo})`

$:{
    percentage = achievementsSummary ? (achievementsSummary.completed / achievementsSummary.total) * 100.0 : 0.0;
    completedCount = achievementsSummary?.completed ?? 0;
    totalCount = achievementsSummary?.total ?? 0;
    platformClass = titleInfo?.platform == ActiveConfig.Steam ?  "steam" : "xboxlive";
    logoClassName = "gameLogo noselect " + platformClass;
    logoUri = `url(${titleInfo?.logoUri ?? Logo})`
}

</script>

<style lang="scss">
    :global{
        @import "./Mobile.scss";
    }
</style>

<div class="overlayBox open">
    <div id="achievementsPanel" class={platformClass}>
        <div class={logoClassName} style:background-image={logoUri}>
        </div>
        <div class="card-container">
            <div id="completionHeadline">
                <span class="percentage">{percentage.toFixed(0)}%</span>
                <img src={Cup} alt="achievements" />
                <span class="completedCount">{completedCount}</span>/<span class="totalCount">{totalCount}</span> 
            </div>
            <h2 id="gameTitle" class={platformClass}>
                {titleInfo?.productTitle ?? "Game"}
            </h2>
        </div>
    </div>
    <AchievementsList achievements={achievementsDetails} platform={titleInfo?.platform} />
</div>