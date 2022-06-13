<script lang="ts">
import { EBSVersion } from "../../common/ServerConfig";
import { ConfigXBLConfigStateEnum } from "../../common/ConfigStepBase";
import { ActiveConfig, ExtensionConfiguration } from "../../common/EBSTypes"
import type { TwitchExtensionConfiguration } from "../../common/TwitchExtension"
import { ConfigurationService } from "../../services/EBSConfigurationService";
import { Twitch } from "../../services/TwitchService";
import ConfigXBL_01_XApiKey from "../ConfigXBL_01_XApiKey/ConfigXBL_01_XApiKey.svelte"


export let onSaved: (savedConfig: TwitchExtensionConfiguration, configObject: ExtensionConfiguration) => void
export let savedConfiguration: ExtensionConfiguration
export let onCancel: () => void;

let currentState: ConfigXBLConfigStateEnum = ConfigXBLConfigStateEnum.XApiUsKey
let isValid: boolean = false

async function onValidateStep(_e: any, config: ExtensionConfiguration)
{
    let result = await ConfigurationService.setConfiguration(config);
    Twitch.setConfiguration(result.configToken, EBSVersion);
    
    console.log(JSON.stringify(result))

    Twitch.send("broadcast", "application/json", {
        "type": "set-config",
        "version": EBSVersion,
        "configToken": result.configToken
    });

    isValid = true
    onSaved({content: result.configToken, version: EBSVersion}, config)
}

function onCancelStep()
{
    onCancel()
}

let currentConfig: ExtensionConfiguration
$:{
    currentConfig = savedConfiguration ?? {activeConfig: ActiveConfig.XBoxLive, xBoxLiveConfig: null, steamConfig: null, version: EBSVersion}
    currentConfig.activeConfig = ActiveConfig.XBoxLive
    if (! currentConfig.xBoxLiveConfig)
    {
        currentConfig.xBoxLiveConfig = {
            xApiKey: null,
            streamerXuid: null,
            locale: null,
            titleId: null,
        }
    }
}
</script>

<h2>Configure XBoxLive Achievements</h2>
{#if isValid}
<div class="card">
    <h2 class="section">Saved !</h2>
</div>
{:else}
<ConfigXBL_01_XApiKey savedConfiguration={savedConfiguration} onValidate={onValidateStep} onBack={onCancelStep} />
{/if}