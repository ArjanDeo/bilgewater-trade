<script lang="ts">
	import { searchListings, type ListingResult } from '$lib/api';
	import Button from '$lib/components/ui/button/button.svelte';
	import Input from '$lib/components/ui/input/input.svelte';
	import Spinner from '$lib/components/ui/spinner/spinner.svelte';
	import { columns } from './listings/columns';
	import DataTable from './listings/data-table.svelte';
	import wowLogo from '$lib/assets/world-of-warcraft-logo.svg';
	let searchQuery: string = $state('');
	let searching: boolean = $state(false);
	let searchResults: ListingResult[] = $state([]);

	const search = async () => {
		if (searching || !searchQuery.trim()) return;

		searching = true;

		try {
			searchResults = await searchListings(11, searchQuery);
			console.log(searchResults);
		} finally {
			searching = false;
		}
	};

	const handleKeydown = (e: KeyboardEvent) => {
		if (e.key === 'Enter') {
			search();
		}
	};
</script>

<div class="flex flex-col items-center py-2">
	<h1 class="text-4xl font-bold text-amber-400">bilgewater.trade</h1>
	<h3 class="text-xl"><img src={wowLogo} alt="World of Warcraft Logo" class="h-6 w-6 inline-block mr-0.5" /> Auction House Insights and Analysis, with an emphasis on performance</h3>
</div>

<div class="flex flex-col gap-2 py-4">
	<div class="flex flex-row gap-2">
		<Input
			id="searchQuery"
			name="searchQuery"
			bind:value={searchQuery}
			type="text"
			placeholder="Search for an item..."
			class="w-full max-w-lg"
			onkeydown={handleKeydown}
		/>

		<Button
			onclick={search}
			disabled={searching}
			class="cursor-pointer bg-amber-500 hover:bg-amber-600"
		>
			{#if searching}
				<Spinner class="mr-2 h-4 w-4" />
			{/if}
			Search
		</Button>
	</div>
</div>

<div>
	{#if searchResults.length > 0}
		<DataTable data={searchResults} {columns} />
	{/if}
</div>