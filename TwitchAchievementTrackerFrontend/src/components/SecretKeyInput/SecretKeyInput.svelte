<script lang="ts">
export let onChangeValue: (e: any) => void
export let keyValue: string
export let isSyntaxValid: boolean
export let pattern: string
export let size: number
export let placeholder: string

let isKeyVisible: boolean = false

console.log(`isSyntaxValid: ${isSyntaxValid}`)
</script>

<input name="xapikey" type={isKeyVisible ? "text" : "password"} pattern={pattern} size={size} value={keyValue} placeholder={placeholder} on:change={onChangeValue} class={isSyntaxValid ? '' : 'sf1-invalid'} />
<input type="button" on:click={() => isKeyVisible = !isKeyVisible} value={isKeyVisible ? "Hide" : "Show"} />