<script lang="ts">
 import goldCoin from "$lib/assets/coin-gold.png";
 import silverCoin from "$lib/assets/coin-silver.png";
 import copperCoin from "$lib/assets/coin-copper.png";

 let { copperPrice }: { copperPrice: number } = $props();

 const formatNumber = new Intl.NumberFormat("en-US");
 const gold = $derived(Math.floor(copperPrice / 10000));
 const silver = $derived(Math.floor((copperPrice % 10000) / 100));
 const copper = $derived(copperPrice % 100);
</script>

<div class="flex items-center gap-1">
 <span class="inline-flex items-center gap-0.5">
  <img src={goldCoin} alt="Gold" class="size-4" />
  {formatNumber.format(gold)}
 </span>
 <span class="inline-flex items-center gap-0.5">
  <img src={silverCoin} alt="Silver" class="size-4" />
  {formatNumber.format(silver)}
 </span>
 <span class="inline-flex items-center gap-0.5">
  <img src={copperCoin} alt="Copper" class="size-4" />
  {formatNumber.format(copper)}
 </span>
</div>
