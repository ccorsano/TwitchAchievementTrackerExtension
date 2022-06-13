<script lang="ts">
import dayjs from 'dayjs'
import duration from 'dayjs/plugin/duration'
import { onDestroy, onMount } from 'svelte';
dayjs.extend(duration)

export let targetDate: Date

let tickInterval: NodeJS.Timeout = null;
let targetMoment: any
$: targetMoment = dayjs(targetDate)
let diffToTarget: any
let timeToTarget: any

onMount(() => {
    tick()
    tickInterval = setInterval(tick, 1000)
})

onDestroy(() => {
    clearInterval(tickInterval)
    tickInterval = null
})

function tick()
{
    diffToTarget = targetMoment.diff(dayjs())
    timeToTarget = dayjs.duration(diffToTarget)
}

</script>

<div>
    <span class="rounded">
        {timeToTarget?.hours()?.toString()?.padStart(2, "0") ?? "00"}
        </span>
        :
        <span class="rounded">
            {timeToTarget?.minutes()?.toString()?.padStart(2, "0") ?? "00"}
        </span>
        :
        <span class="rounded">
            {timeToTarget?.seconds()?.toString()?.padStart(2, "0") ?? "00"}
        </span>
</div>