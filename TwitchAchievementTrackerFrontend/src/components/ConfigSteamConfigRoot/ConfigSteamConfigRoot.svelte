<script lang="ts">
import type { TwitchExtensionConfiguration } from "../../common/TwitchExtension";
import { ActiveConfig, type ExtensionConfiguration } from "../../common/EBSTypes";
import { ConfigurationService } from "../../services/EBSConfigurationService";
import { Twitch } from "../../services/TwitchService";
import { EBSVersion } from "../../common/ServerConfig";
import ConfigSteam_01_WebAPIKey from "../ConfigSteam_01_WebAPIKey/ConfigSteam_01_WebAPIKey.svelte"

export let savedConfiguration: ExtensionConfiguration
export let onSaved: (savedConfig: TwitchExtensionConfiguration, configObject: ExtensionConfiguration) => void
export let onCancel: () => void

let isValid:boolean = false


async function onValidateStep(_e: any, config: ExtensionConfiguration)
{
    let result = await ConfigurationService.setConfiguration(config);
    Twitch.setConfiguration(result.configToken, EBSVersion);

    Twitch.send("broadcast", "application/json", {
        "type": "set-config",
        "version": EBSVersion,
        "configToken": result.configToken
    });

    isValid = true

    onSaved({content: result.configToken, version: EBSVersion}, config);
}

function onCancelStep()
{
    onCancel();
}


let currentConfig: ExtensionConfiguration = savedConfiguration ?? {activeConfig: ActiveConfig.Steam, xBoxLiveConfig: null, steamConfig: null, version: EBSVersion}
currentConfig.activeConfig = ActiveConfig.Steam

if (! currentConfig.steamConfig)
{
    currentConfig.steamConfig = {}
}
</script>

<h2>Steam</h2>
{#if isValid}
<div class="card">
    <h2 class="section">Saved !</h2>
</div>
{:else}
<ConfigSteam_01_WebAPIKey savedConfiguration={savedConfiguration} onValidate={onValidateStep} onBack={onCancelStep} />
{/if}