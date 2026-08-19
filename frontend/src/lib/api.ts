// lib/api.ts

export enum TimeLeft {
	Short = 'Short',
	Medium = 'Medium',
	Long = 'Long',
	VeryLong = 'Very_Long'
}

export interface ListingResult {
	itemId: number;
	itemName: string;
	isCommodity: boolean;
	cheapestBuyoutCopper: number | null;
	cheapestUnitPriceCopper: number | null;
	quantity: number;
}

const API_BASE_URL = "https://localhost:7049";

export function formatTimeLeft(timeLeft: TimeLeft | string | number): string {
	const timeLeftMap: Record<number, string> = {
		0: 'Short',
		1: 'Medium',
		2: 'Long',
		3: 'Very Long'
	};

	if (typeof timeLeft === 'number') {
		return timeLeftMap[timeLeft] || String(timeLeft);
	}

	const value = String(timeLeft);
	return value.replace(/_/g, ' ');
}

export async function searchListings(connectedRealmId: number, searchQuery: string): Promise<ListingResult[]> {
	const response = await fetch(
		`${API_BASE_URL}/api/listings/${connectedRealmId}?searchQuery=${encodeURIComponent(searchQuery)}`
	);

	if (!response.ok) {
		throw new Error(`Search failed: ${response.status}`);
	}

	const data = (await response.json()) as ListingResult[];
	return data;
}