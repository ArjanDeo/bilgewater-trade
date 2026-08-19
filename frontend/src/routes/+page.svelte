<script lang="ts">
	import { searchAuctions, aggregateAuctions, type AggregatedAuction, getPriceBreakdown } from '$lib/api';
	import Button from '$lib/components/ui/button/button.svelte';
	import Input from '$lib/components/ui/input/input.svelte';
	import Spinner from '$lib/components/ui/spinner/spinner.svelte';
	import * as Table from '$lib/components/ui/table';

	let searchQuery: string = $state('');
	let searching: boolean = $state(false);
    let data: AggregatedAuction[] = $state([]);
	const search = async () => {
		searching = true;
        const searchResult = await searchAuctions(11, searchQuery);
        data = aggregateAuctions(searchResult);
		searching = false;
	};
	const handleKeydown = (e: KeyboardEvent) => {
		if (e.key === 'Enter') {
			search();
		}
	};
</script>

<title>bilgewater.trade</title>
<div class="flex flex-col items-center py-2">
	<h1 class="text-4xl font-bold text-amber-400">bilgewater.trade</h1>
	<h3 class="text-xl">Performant Insight and Data Analysis of World of Warcraft's Auction House</h3>
</div>

<div class="flex flex-row items-center gap-2 py-4">
	<div>
		<label for="searchQuery" class="">Search Query</label>
		<div class="flex flex-row gap-2">
			<Input
				name="searchQuery"
				bind:value={searchQuery}
				type="text"
				placeholder="Search for an item..."
				class="w-full max-w-lg"
				onkeydown={handleKeydown}
			/>

			<Button onclick={search} class="bg-amber-500 hover:bg-amber-600 text-white cursor-pointer">
				{#if searching}
					<Spinner class="w-4 h-4 mr-2" />
				{/if}
				Search
			</Button>
		</div>
	</div>
</div>
<div>
    <!-- Display the search results -->
    {#if data.length > 0}
        <h2 class="text-2xl font-bold text-amber-400 mb-4">Search Results</h2>
        <div class="rounded-lg border-2 border-amber-700 bg-gray-900 shadow-2xl overflow-hidden">
            <Table.Root class="bg-gray-900">
                <Table.Header class="bg-linear-to-r from-amber-900 to-amber-800 border-b-2 border-amber-700">
                    <Table.Row class="hover:bg-linear-to-r hover:from-amber-900 hover:to-amber-800">
                        <Table.Head class="text-amber-300 font-bold text-lg">Item</Table.Head>
                        <Table.Head class="text-right text-amber-300 font-bold text-lg">Total Quantity</Table.Head>
                        <Table.Head class="text-right text-amber-300 font-bold text-lg">Lowest Price</Table.Head>
                    </Table.Row>
                </Table.Header>
                <Table.Body>
                    {#each data as result, i (i)}
                        {@const effectiveLowestPrice = result.lowestBuyoutCopper ?? result.lowestUnitPriceCopper}
                        {@const lowestPrice = getPriceBreakdown(effectiveLowestPrice)}
                        <Table.Row class="border-b border-gray-700 hover:bg-gray-800 transition-colors duration-200">
                            <Table.Cell class="text-gray-100 font-semibold"><a href="https://www.wowhead.com/item={result.itemId}" target="_blank" data-wowhead="item={result.itemId}">{result.itemName}</a></Table.Cell>
                            <Table.Cell class="text-right text-gray-300">{result.totalQuantity}</Table.Cell>
                            <Table.Cell class="text-right">
                                <span class="text-yellow-400 font-bold">{lowestPrice.gold}</span><span class="text-yellow-400">g</span>
                                <span class="text-gray-300 font-bold ml-1">{lowestPrice.silver}</span><span class="text-gray-300">s</span>
                                <span class="text-orange-600 font-bold ml-1">{lowestPrice.copper}</span><span class="text-orange-600">c</span>
                            </Table.Cell>
                        </Table.Row>
                    {/each}
                </Table.Body>
            </Table.Root>
        </div>
    {:else if !searching && searchQuery}
        <p>No results found for "{searchQuery}".</p>
    {/if}
</div>
