<script lang="ts">
	import { Button as ButtonPrimitive } from "bits-ui";
	import { type Events, type Props, buttonVariants } from "./index.js";
	import { cn } from "$lib/shadcn/utils.js.js";

	import LoadingSpinner from "$lib/components/loadingSpinner.svelte";
	
	type $$Props = Props;
	type $$Events = Events;

	let className: $$Props["class"] = undefined;
	export let variant: $$Props["variant"] = "default";
	export let size: $$Props["size"] = "default";
	export let builders: $$Props["builders"] = [];
	export let loading: $$Props["loading"] = undefined;
	export let disabled: $$Props["disabled"] = undefined;
	export { className as class };

</script>

<ButtonPrimitive.Root
	{builders}
	class={cn(buttonVariants({ variant, size }), className)}
	type="button"
	{...$$restProps}
	on:click
	on:keydown
	aria-busy={loading}
	disabled={disabled || loading}
>
	{#if loading}
		<span class="opacity-0">
			<slot />
		</span>
		<LoadingSpinner class="absolute text-gray-200"/>
	{:else}
		<slot />
	{/if}
</ButtonPrimitive.Root>
