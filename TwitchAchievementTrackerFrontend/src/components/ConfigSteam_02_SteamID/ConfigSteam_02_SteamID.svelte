<script lang="ts">
import { ActiveConfig, ExtensionConfiguration, PlayerInfoCard } from "../../common/EBSTypes"
import { ConfigurationService, ValidationError } from "../../services/EBSConfigurationService"
import { EBSVersion } from "../../common/ServerConfig"
import GamerCard from "../GamerCard/GamerCard.svelte"
import ValidationErrorList from "../ValidationErrorList/ValidationErrorList.svelte"
import ConfigSteam_03_AppId from "../ConfigSteam_03_AppId/ConfigSteam_03_AppId.svelte"
import type { SvelteComponent } from "svelte/internal";

export let onValidate: (e: any, configuration: ExtensionConfiguration) => void
export let onBack: (e: any) => void
export let savedConfiguration: ExtensionConfiguration
export let webApiKey:string = ""

const formatRegexp: RegExp = /^https:\/\/steamcommunity\.com\/(profiles|id)\/([^\/]+)(\/.*)?$/i
const steamIdRegexp: RegExp = /^[0-9]+$/

let isLoading: boolean = true
let isProfileUrlFormatValid: boolean = false
let isProfileValid: boolean = false
let isConfirmed: boolean = false
let unvalidatedUrl: string = null
let steamProfileUrl: string = null
let steamProfileId: string = null
let steamProfile: PlayerInfoCard = null
let errors: ValidationError[] = []


function onChangeProfileUrl(value: string)
{
    let formatCheck = this.formatRegexp.test(value);
    isProfileUrlFormatValid = formatCheck
    isProfileValid = false
    unvalidatedUrl = value
    steamProfileUrl = formatCheck ? value : null
}

async function doValidate(_e: any)
{
    if (isProfileValid)
    {
        isConfirmed = true
    }

    isLoading = true

    try {
        let split = this.formatRegexp.exec(steamProfileUrl);
        let profileId = split[2];
        let resolvedProfile: PlayerInfoCard = null;

        if (! steamIdRegexp.test(profileId))
        {
            resolvedProfile = await ConfigurationService.resolveSteamVanityUrl(profileId, webApiKey);
            profileId = resolvedProfile.playerId;
        }
        else
        {
            resolvedProfile = await ConfigurationService.resolveSteamPlayerInfo(profileId, webApiKey);
        }
        
        const configuration: ExtensionConfiguration = {
            activeConfig: ActiveConfig.Steam,
            steamConfig: {
                steamId: profileId,
                webApiKey: webApiKey,
                appId: null,
                locale: null
            },
            version: EBSVersion,
            xBoxLiveConfig: null
        }
        
        let newErrors = await ConfigurationService.validateConfiguration(configuration)
        errors = newErrors.filter(e => e.path == "SteamConfig.WebApiKey" || e.path == "SteamConfig.SteamId")

        if (errors.length == 0)
        {
            isProfileValid = true
            steamProfileId = profileId
            steamProfile = resolvedProfile
        }
    }
    finally
    {
        isLoading = false
    }
}

function onResetProfile(_e: any)
{
    isProfileUrlFormatValid = true
    isProfileValid = false
    steamProfileId = null           
    steamProfile = null
}

function unvalidate(_e: any)
{
    isConfirmed = false
}

let steamId:string
$:{
    steamId = savedConfiguration?.steamConfig?.steamId
    if (steamId && steamIdRegexp.test(steamId)) {
        ConfigurationService.resolveSteamPlayerInfo(steamId, webApiKey)
            .then(playerInfo => {
                isLoading = false
                isProfileUrlFormatValid = false
                isProfileValid = true
                steamProfileId = steamId
                steamProfile = playerInfo
            });
    }
    else
    {
        isLoading = false
    }
}

let isContinueEnabled:boolean
$: isContinueEnabled = isProfileUrlFormatValid || isProfileValid;

</script>


{#if isConfirmed}
<ConfigSteam_03_AppId
    savedConfiguration={savedConfiguration}
    onValidate={onValidate}
    onBack={unvalidate}
    webApiKey={webApiKey}
    steamProfileId={steamProfileId} />
{:else}
    {#if !isLoading}
        {#if steamProfile}
            <GamerCard playerInfo={steamProfile}>
                <input slot="buttonSection" type="button" value="Change" class="section" on:click={onResetProfile} />
            </GamerCard>
        {:else}
            <label for="vanityUrl">Steam Profile URL</label>,
            <input name="vanityUrl" type="text"
                pattern="https:\/\/steamcommunity\.com\/(profiles|id)\/([^\/]+)(\/.*)?$"
                placeholder="Enter your Steam profile URL"
                value={unvalidatedUrl}
                on:change={e => onChangeProfileUrl(e.currentTarget.value)} />,
            <div>
                <span class="icon-info"></span> You can copy your Profile URL by right-clicking on your profile page on the Steam application, and selecting 'Copy Page URL'.
            </div>,
            <ValidationErrorList errors={errors} />
        {/if}
    {:else}
        <div class="card">
            <div class="spinner"></div>
        </div>
    {/if}
    <input type="button" value="Back" onClick={onBack} />,
    <input type="button" value="Continue" disabled={!isContinueEnabled} on:click={doValidate} />
{/if}