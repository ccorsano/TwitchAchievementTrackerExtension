<script lang="ts">
import { EBSVersion } from "../../common/ServerConfig";
import { AchievementsService } from "../../services/EBSAchievementsService";
import { Twitch } from "../../services/TwitchService";
import { ActiveConfig, type ExtensionConfiguration, type RateLimits, type TitleInfo } from "../../common/EBSTypes"
import { ConfigurationService, type ValidationError } from "../../services/EBSConfigurationService"
import CountDown from "../CountDown/CountDown.svelte";
import { onDestroy, onMount } from "svelte";
    
let isLoading: boolean = true
let isRefreshing: boolean = false
let wasModified: boolean = false
let configuration: ExtensionConfiguration | null = null
let titleInfo: TitleInfo = {
    platform: ActiveConfig.None,
    titleId: "",
    productTitle: "",
    productDescription: "",
    logoUri: "",
}
let errors: ValidationError[] = []
let xApiRateLimits: RateLimits = {
    hourlyLimit: 0,
    remaining: 0,
    resetTime: new Date(),
}

let refreshRateLimitsInterval: NodeJS.Timeout | null = null


async function refreshRateLimit()
{
    if (configuration!.activeConfig != ActiveConfig.XBoxLive)
    {
        clearInterval(refreshRateLimitsInterval!)
        refreshRateLimitsInterval = null
        xApiRateLimits = {
            hourlyLimit: 0,
            remaining: 0,
            resetTime: new Date(),
        }
        return;
    }

    let rateLimits = await ConfigurationService.getXApiRateLimits()
    xApiRateLimits = rateLimits
}

async function onForceRefresh()
{
    isRefreshing = true

    let didChange = await ConfigurationService.forceRefresh();
    if (didChange)
    {
        Twitch.send("broadcast", "application/json", {
            "type": "refresh",
            "version": EBSVersion,
        });
    }

    isRefreshing = false
    wasModified = didChange
}

onMount(async () => {
    ConfigurationService.configuredPromise.then(async (_context) => {
        configuration = await ConfigurationService.getConfiguration();
        let validation = await ConfigurationService.validateConfiguration(configuration!);
        if (validation.length == 0)
        {
            titleInfo = await AchievementsService.getTitleInfo();
        }

        isLoading = false
        errors = validation

        if (configuration!.activeConfig == ActiveConfig.XBoxLive)
        {
            refreshRateLimit();
            refreshRateLimitsInterval = setInterval(refreshRateLimit, 1000);
        }

        Twitch.listen("broadcast", async (_target, _contentType, messageStr) => {
            let message = JSON.parse(messageStr);
            let configToken = AchievementsService.configuration!.content;
            
            switch (message.type) {
                case "refresh":
                    break;
                case "set-config":
                    if (message.configToken != configToken)
                    {
                        AchievementsService.configuration!.content = message.configToken;
                        AchievementsService.configuration!.version = message.version;
                        ConfigurationService.configuration!.content = message.configToken;
                        ConfigurationService.configuration!.version = message.version;

                        isLoading = true
                        
                        let config = await ConfigurationService.getConfiguration();
                        let titleInfo = await AchievementsService.getTitleInfo();

                        isLoading = false
                        configuration = config
                        titleInfo = titleInfo
                        errors = validation
                        
                        if (config!.activeConfig == ActiveConfig.XBoxLive)
                        {
                            if (! refreshRateLimitsInterval)
                            {
                                refreshRateLimitsInterval = setInterval(refreshRateLimit, 1000);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        });
    })
})

onDestroy(() => {
    if (refreshRateLimitsInterval)
    {
        clearInterval(refreshRateLimitsInterval)
    }
})

</script>

{#if isLoading}
    <div class="card small">
        <div class="spinner"></div> Loading configuration
    </div>
{:else}
    <div class="card small">
        <h3 class="section">Achievements Tracker</h3>
        <div class="section titleinfo row">
            <div class="col-sm-4" >
                <img src={titleInfo.logoUri} alt={`${titleInfo.productTitle} Logo`} />
            </div>
            <div class="col-sm-8">
                <button class="large primary" on:click={onForceRefresh} disabled={isRefreshing}>Force refresh</button>
            </div>
        </div>
        
        {#if xApiRateLimits != null}
        <div class="section">
            <h4>xApi.us calls</h4>
            <div class="row">
                <div class="col-sm-6">
                    Remaining
                </div>
                <div class="col-sm-6">
                    {xApiRateLimits.remaining}/{xApiRateLimits.hourlyLimit}
                </div>
            </div>
            <div class="row">
                <div class="col-sm-6">
                    Resets in
                </div>
                <div class="col-sm-6">
                    <CountDown targetDate={xApiRateLimits.resetTime} />
                </div>
            </div>
        </div>,
        <div class="section">
            Force refresh typically uses 1 call.<br/>
            15 auto-refresh per hour.
        </div>
        {/if}
    </div>
{/if}