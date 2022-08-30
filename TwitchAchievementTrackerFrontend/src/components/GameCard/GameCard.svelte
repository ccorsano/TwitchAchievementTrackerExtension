<script lang="ts">
import type { TitleInfo } from "../../common/EBSTypes";

export let titleInfo: TitleInfo

</script>

<div class="card small">
    <h2 class="section">{titleInfo.productTitle}</h2>
    <img class="section media" src={titleInfo.logoUri} alt={`${titleInfo.productTitle} Logo`} />
    <slot name="buttonSection"></slot>
</div>