<script lang="ts">
import { EBSVersion } from "../../common/ServerConfig";
import { ActiveConfig, ExtensionConfiguration } from "../../common/EBSTypes";
import SecretKeySpan from "../SecretKeySpan/SecretKeySpan.svelte";

export let onValidate: (e: any, configuration: ExtensionConfiguration) => void
export let onBack: (e: any) => void
export let savedConfiguration: ExtensionConfiguration
export let xApiKey: string
export let streamerXuid: string
export let titleId: string
export let locale: string

function onSave(e: any)
{
    let configuration: ExtensionConfiguration = {
        activeConfig: ActiveConfig.XBoxLive,
        version: EBSVersion,
        steamConfig: savedConfiguration?.steamConfig, // Make sure we keep the non-active config saved
        xBoxLiveConfig: {
            xApiKey: xApiKey,
            streamerXuid: streamerXuid,
            titleId: titleId,
            locale: locale,
        }
    };
    onValidate(e, configuration);
}

</script>

<div class="card">
    <div class="section">
        ActiveConfig: XBoxLive
    </div>
    <div class="section">
        XApiKey: <SecretKeySpan keyValue={xApiKey} />
    </div>
    <div class="section">
        StreamerXuid: {streamerXuid}
    </div>
    <div class="section">
        TitleId: {titleId}
    </div>
    <div class="section">
        WebApiKey: {locale}
    </div>
    <div class="section">
        Locale: {locale}
    </div>
</div>
<input type="button" value="Back" on:click={onBack} />
<input type="button" value="Save" on:click={onSave} />