<script lang="ts">
    import { Achievement, ActiveConfig } from "../../common/EBSTypes"

    export let achievements: Achievement[]
    export let platform: ActiveConfig

    let platformClass:string = ""
    $:{
        platformClass = platform == ActiveConfig.XBoxLive ?  "steam" : "xboxlive"
    }
</script>

<ul id="list" class={platformClass}>
    {#each achievements as achievement,i}
    {#key "achievement_" + i + "_" + achievement.id}
    <li class={achievement.completed ? "completed" : "notCompleted"}>
        <div class="achievementTitle">
            {achievement.name}
        </div>
        <div class="achievementDescription">
            {achievement.description}
        </div>
    </li>
    {/key}
    {/each}
</ul>