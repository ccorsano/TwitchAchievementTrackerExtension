<script lang="ts">
import { Achievement, AchievementSummary, ActiveConfig, TitleInfo } from '../../common/EBSTypes';
import AchievementsList from '../AchievementsList/AchievementsList.svelte'

import Cup from '../../../assets/cup.svg';
import Logo from '../../../assets/logo.png';
import { AchievementsService } from '../../services/EBSAchievementsService'

let isCollapsed: boolean = true
let isConfigurationValid: boolean = false
let titleInfo: TitleInfo = null
let achievementsSummary: AchievementSummary = null
let achievementsDetails: Achievement[] = []

let refreshAll = async () => {
    let titleInfoPromise = AchievementsService.getTitleInfo()
    AchievementsService.getSummary().then(summary => achievementsSummary = summary)
    AchievementsService.getAchievements().then(achievements => achievementsDetails = achievements)

    try {
        titleInfo = await titleInfoPromise
        isConfigurationValid = true
    }
    catch(error)
    {
        isConfigurationValid = false
    }
}

let refreshSummary = async () => {
    let summaryPromise = AchievementsService.getSummary();
    let achievementsPromise = isCollapsed ? Promise.resolve(achievementsDetails) : AchievementsService.getAchievements();

    achievementsSummary = await summaryPromise
    achievementsDetails = await achievementsPromise
}

function togglePanel()
{
    if (! isConfigurationValid) return;

    isCollapsed = !isCollapsed
    if (! isCollapsed)
    {
        refreshSummary();
    }
}

refreshAll()
setInterval(refreshSummary, 60000)

let percentage:number = 0.0
let completedCount:number = 0
let totalCount:number = 0
let platformClass = ""
let logoClassName = ""
let logoUri = `url(${Logo})`

$:{
    percentage = achievementsSummary ? (achievementsSummary.completed / achievementsSummary.total) * 100.0 : 0.0
    completedCount = achievementsSummary?.completed ?? 0
    totalCount = achievementsSummary?.total ?? 0
    platformClass = titleInfo?.platform == ActiveConfig.Steam ?  "steam" : "xboxlive"
    logoClassName = "gameLogo noselect " + platformClass
    logoUri = `url(${titleInfo?.logoUri ?? Logo})`
}
</script>

<style type="text/scss">
:global{
    @import "./VideoOverlay.scss";
}
</style>

<div class={isCollapsed ? "overlayBox collapsed" : "overlayBox open"}>
    <div class={logoClassName} on:click="{togglePanel}" style:background-image={logoUri}>
        <div class="summaryWidget">
            { achievementsSummary ? percentage.toFixed(0) + '%' : ''}
        </div>
    </div>
    <div id="achievementsPanel" class={platformClass}>
        <div class="card-container">
            <h2 id="gameTitle">
                {titleInfo?.productTitle ?? "Game"}
            </h2>
            <div id="completionHeadline">
                <span class="percentage">{percentage.toFixed(0)}%</span>
                <img src={Cup} alt="achievements" />
                <span class="completedCount">{completedCount}</span>/<span class="totalCount">{totalCount}</span> 
            </div>
        </div>
        <AchievementsList achievements={achievementsDetails} platform={titleInfo?.platform} />
    </div>
</div>