<script lang="ts">
import type { ValidationError } from "../../services/EBSConfigurationService";

export let errors:ValidationError[] = []
</script>

<ul>
    {#each errors as error,i}
    {#key error.path + '_' + i}
    <li>
        <mark class="secondary"><span class="icon-alert secondary"></span>Error</mark> {error.path} : {error.errorDescription}
    </li>
    {/key}
    {/each}
</ul>