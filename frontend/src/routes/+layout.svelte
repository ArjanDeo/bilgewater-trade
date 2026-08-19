<script lang="ts">
	import './layout.css';
	import favicon from '$lib/assets/favicon.svg';
	import { goto } from '$app/navigation';

	let { children } = $props();
</script>

<svelte:head><link rel="icon" href={favicon} /></svelte:head>
<div class="bg-taupe-800 text-white relative flex min-h-screen flex-col">
	<div class="flex w-full flex-col items-center pt-4">
		{@render children()}
	</div>
	<div class="absolute bottom-4 right-4">
		<button onclick={async () => await goto('/about')} class="text-gray-400 hover:text-gray-300 cursor-pointer">About</button>
	</div>
</div>
