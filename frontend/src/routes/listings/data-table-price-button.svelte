<script lang="ts">
 import type { ComponentProps } from "svelte";
 import ArrowUpDownIcon from "@lucide/svelte/icons/arrow-up-down";
 import { Button } from "$lib/components/ui/button/index.js";
 
 let { variant = "ghost", ...restProps }: ComponentProps<typeof Button> =
  $props();
</script>
 
<Button {variant} {...restProps}>
<span class="text-lg">Price</span>
 <ArrowUpDownIcon class="ms-2 size-5" />
</Button>