<script lang="ts">
	import './layout.css';
	import favicon from '$lib/assets/goblinfacelogo.png';
	import { goto } from '$app/navigation';
	import { ModeWatcher } from "mode-watcher";
	import MoonIcon from "@lucide/svelte/icons/moon";
	import SunIcon from "@lucide/svelte/icons/sun";
	import { toggleMode } from "mode-watcher";
	import { Button } from "$lib/components/ui/button/index.js";
	import { resolve } from '$app/paths';
	import { page } from '$app/state';

	let { children } = $props();
const title = $derived(
    page.route.id === '/'
        ? 'Home'
        : page.route.id?.slice(1).split('/').pop()?.replace(/^\w/, c => c.toUpperCase()) ?? 'Page'
);
</script>

<svelte:head><link rel="icon" href={favicon}/></svelte:head>
<title>{title} - bilgewater.trade</title>

<div class="relative flex min-h-screen flex-col">
	<div class="flex w-full flex-col items-center pt-4">
		<ModeWatcher defaultMode="dark"/>
		{@render children()}
	</div>
	<div class="absolute bottom-4 right-4">
	{#if page.route.id == "/about"}
		<button onclick={() =>goto(resolve('/'))} class="text-gray-400 hover:text-gray-300 cursor-pointer">Home</button>
	{:else}
		<button onclick={() => goto(resolve('/about'))} class="text-gray-400 hover:text-gray-300 cursor-pointer">About</button>
	{/if}
		<Button onclick={toggleMode} variant="outline" size="icon" class="ml-2 cursor-pointer">
		<SunIcon
			class="h-[1.2rem] w-[1.2rem] scale-100 rotate-0 transition-all! dark:scale-0 dark:-rotate-90"
		/>
		<MoonIcon
			class="absolute h-[1.2rem] w-[1.2rem] scale-0 rotate-90 transition-all! dark:scale-100 dark:rotate-0"
		/>
		<span class="sr-only">Toggle theme</span>
		</Button>
	</div>
</div>
