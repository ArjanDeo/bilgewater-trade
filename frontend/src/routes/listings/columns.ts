import { createColumnHelper, renderComponent } from "@tanstack/svelte-table";
import type { DataTableFeatures } from "./data-table-features.js";
import type { ListingResult } from "$lib/api.js";
import DataTablePriceButton from "./data-table-price-button.svelte";
import DataTableQuantityButton from "./data-table-quantity-button.svelte";
import DataTableWowPrice from "./data-table-price.svelte";

// This type is used to define the shape of our data.
// You can use a Zod schema here if you want.
// export type Payment = {
//  id: string;
//  amount: number;
//  status: "pending" | "processing" | "success" | "failed";
//  email: string;
// };

// Use `accessor` for data columns and `display` for columns without one.
const columnHelper = createColumnHelper<DataTableFeatures, ListingResult>();

export const columns = columnHelper.columns([
	columnHelper.accessor("itemName", {
		header: "Item",
	}),
	columnHelper.accessor("cheapestUnitPriceCopper", {
		header: ({ column }) =>
			renderComponent(DataTablePriceButton, {
				onclick: column.getToggleSortingHandler(),
			}),
            
		cell: ({ getValue }) => {
			const price = getValue();
			return price != null
				? renderComponent(DataTableWowPrice, { copperPrice: price })
				: "—";
		},
		sortUndefined: "last",
	}),
	columnHelper.accessor("quantity", {
		header: ({ column }) =>
			renderComponent(DataTableQuantityButton, {
				onclick: column.getToggleSortingHandler(),
			}),
		cell: ({ getValue }) => new Intl.NumberFormat("en-US").format(getValue()),
	}),
]);