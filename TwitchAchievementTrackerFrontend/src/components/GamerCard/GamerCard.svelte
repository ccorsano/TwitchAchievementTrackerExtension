<script lang="ts">
import type { PlayerInfoCard } from "../../common/EBSTypes";

export let playerInfo: PlayerInfoCard = null
</script>

<div class="card">
    <h2 class="section">{playerInfo.playerName}</h2>
    <div class="section">Id: {playerInfo.playerId}</div>
    <img src={playerInfo.avatarUrl} class="section media" alt="Avatar" />
    <slot name="buttonSection"></slot>
</div>