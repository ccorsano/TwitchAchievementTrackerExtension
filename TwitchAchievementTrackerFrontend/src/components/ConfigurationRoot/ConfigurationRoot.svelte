<script lang="ts">
import type { PublicAnnouncement } from "../../common/EBSTypes";
import { ConfigurationService } from "../../services/EBSConfigurationService";

import ConfigChoosePlatform from "../ConfigChoosePlatform/ConfigChoosePlatform.svelte";

function onPlatformChosen(){}

let publicAnnouncements = new Array<PublicAnnouncement>(0);

ConfigurationService.getMessages().then(messages => {
    messages.forEach(message => {
        publicAnnouncements = [...publicAnnouncements, message]
    });
})

function severityToClass(severity: string)
{
    switch (severity) {
        case "Error":
            return "error"
        case "Warning":
            return "warning"
        default:
            return ""
    }
}

</script>

<style lang="scss">
    :global{
        @import './ConfigurationRoot.scss';
    }
</style>

<form>
    <h1>Configuration</h1>
    {#each publicAnnouncements as announcement}
        <div class="card {severityToClass(announcement.severity)}">
            <h4>{announcement.title}</h4>
            <p>{announcement.message}</p>
        </div>
    {/each}
    <ConfigChoosePlatform changeHandler={onPlatformChosen} />
</form>