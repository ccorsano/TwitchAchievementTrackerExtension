<script lang="ts">
import ConfigChoosePlatform from "../ConfigChoosePlatform/ConfigChoosePlatform.svelte";

function onPlatformChosen(){}

</script>

<form>
    <h1>Configuration</h1>
    <ConfigChoosePlatform changeHandler={onPlatformChosen} />
</form>