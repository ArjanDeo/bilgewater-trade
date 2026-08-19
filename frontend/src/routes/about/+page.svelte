<script lang="ts">
	function navigateHome() {
		window.location.href = '/';
	}
</script>

<title>About - bilgewater.trade</title>

<div class="min-h-screen bg-gray-950 text-gray-100">
	<!-- Header -->
	<div class="bg-linear-to-r from-amber-900 to-amber-800 border-b-2 border-amber-700 py-8">
		<div class="max-w-4xl mx-auto px-6">
			<h1 class="text-4xl font-bold text-amber-300 mb-2">About bilgewater.trade</h1>
			<p class="text-amber-100">Performant Insight and Data Analysis of World of Warcraft's Auction House</p>
		</div>
	</div>

	<!-- Main Content -->
	<div class="max-w-4xl mx-auto px-6 py-12">
		<!-- What is bilgewater.trade? -->
		<section class="mb-12">
			<h2 class="text-3xl font-bold text-amber-300 mb-4">What is bilgewater.trade?</h2>
			<div class="bg-gray-900 border-l-4 border-amber-600 p-6 rounded">
				<p class="text-gray-300 leading-relaxed mb-4">
					bilgewater.trade is a high-performance tool designed to help World of Warcraft players analyze and track auction house data across multiple realms. Whether you're a gold goblin, a casual trader, or just curious about market trends, we provide real-time insights into WoW's economy.
				</p>
				<p class="text-gray-300 leading-relaxed">
					By aggregating auction data and presenting it in an easy-to-understand format, we help you make informed decisions about buying, selling, and investing in auction house items.
				</p>
			</div>
		</section>

		<!-- Features -->
		<section class="mb-12">
			<h2 class="text-3xl font-bold text-amber-300 mb-6">Features</h2>
			<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
				<div class="bg-gray-900 border border-amber-700 p-6 rounded hover:border-amber-500 transition">
					<div class="flex items-center mb-3">
						<div class="w-2 h-2 bg-amber-400 rounded-full mr-3"></div>
						<h3 class="text-xl font-bold text-amber-300">Real-time Data</h3>
					</div>
					<p class="text-gray-400">Access the latest auction house prices and availability instantly.</p>
				</div>

				<div class="bg-gray-900 border border-amber-700 p-6 rounded hover:border-amber-500 transition">
					<div class="flex items-center mb-3">
						<div class="w-2 h-2 bg-amber-400 rounded-full mr-3"></div>
						<h3 class="text-xl font-bold text-amber-300">Aggregated Results</h3>
					</div>
					<p class="text-gray-400">See total quantities and lowest prices for each unique item across all auctions.</p>
				</div>

				<div class="bg-gray-900 border border-amber-700 p-6 rounded hover:border-amber-500 transition">
					<div class="flex items-center mb-3">
						<div class="w-2 h-2 bg-amber-400 rounded-full mr-3"></div>
						<h3 class="text-xl font-bold text-amber-300">WoWhead Integration</h3>
					</div>
					<p class="text-gray-400">Click on any item to view details on WoWhead directly.</p>
				</div>

				<div class="bg-gray-900 border border-amber-700 p-6 rounded hover:border-amber-500 transition">
					<div class="flex items-center mb-3">
						<div class="w-2 h-2 bg-amber-400 rounded-full mr-3"></div>
						<h3 class="text-xl font-bold text-amber-300">Clean UI</h3>
					</div>
					<p class="text-gray-400">Designed with WoW's aesthetic in mind for a familiar experience.</p>
				</div>
			</div>
		</section>

		<!-- How It Works -->
		<section class="mb-12">
			<h2 class="text-3xl font-bold text-amber-300 mb-4">How It Works</h2>
			<div class="bg-gray-900 border-l-4 border-amber-600 p-6 rounded">
				<ol class="space-y-4 text-gray-300">
					<li class="flex gap-4">
						<span class="text-amber-400 font-bold shrink-0">1.</span>
						<span>Enter the name of an item you want to search for in the search bar.</span>
					</li>
					<li class="flex gap-4">
						<span class="text-amber-400 font-bold shrink-0">2.</span>
						<span>Press Enter or click the Search button to retrieve auction data.</span>
					</li>
					<li class="flex gap-4">
						<span class="text-amber-400 font-bold shrink-0">3.</span>
						<span>View aggregated results showing total quantity and lowest price for each item.</span>
					</li>
					<li class="flex gap-4">
						<span class="text-amber-400 font-bold shrink-0">4.</span>
						<span>Click on any item name to view more details on WoWhead.</span>
					</li>
				</ol>
			</div>
		</section>

		<!-- About the Data -->
		<section class="mb-12">
			<h2 class="text-3xl font-bold text-amber-300 mb-4">About the Data</h2>
			<div class="bg-gray-900 border-l-4 border-amber-600 p-6 rounded">
				<p class="text-gray-300 leading-relaxed mb-4">
					Our data is sourced from World of Warcraft's official auction house API. We focus on the Bilgewater Realm and provide real-time updates to ensure you have the most accurate market information available.
				</p>
				<p class="text-gray-400 text-sm">
					Note: Prices are displayed in gold, silver, and copper (1 gold = 100 silver = 10,000 copper).
				</p>
			</div>
		</section>

		<!-- Footer Action -->
		<div class="border-t border-gray-700 pt-8 text-center">
			<button onclick={navigateHome} class="bg-amber-600 hover:bg-amber-700 text-white font-bold py-3 px-8 rounded transition cursor-pointer">
				← Back to Search
			</button>
		</div>
	</div>
</div>

<style>
	:global(body) {
		background-color: rgb(3, 7, 18);
	}
</style>