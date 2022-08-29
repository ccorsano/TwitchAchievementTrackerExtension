<script lang="ts">
    export let keyValue: string
    
    let isKeyVisible: boolean = false

    function toggleVisibility()
    {
        isKeyVisible = !isKeyVisible
    }
</script>

<span style="cursor: pointer;" on:click={(e) => toggleVisibility()}>
{#if isKeyVisible}
    {keyValue}
{:else}
    Click to show secret
{/if}
</span>
    