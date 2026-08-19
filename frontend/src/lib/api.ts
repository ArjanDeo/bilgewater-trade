// lib/api.ts

export enum TimeLeft {
	Short = 'Short',
	Medium = 'Medium',
	Long = 'Long',
	VeryLong = 'Very_Long'
}

export interface AuctionResult {
	itemId: number;
	itemName: string;
	isCommodity: boolean;
	buyoutCopper: number | null;
	unitPriceCopper: number | null;
	quantity: number;
	timeLeft: TimeLeft;
}

const API_BASE_URL = "https://localhost:7049"; // adjust to your actual Api port

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

export interface PriceParts {
	gold: number;
	silver: number;
	copper: number;
}

export function getPriceBreakdown(copperAmount: number | null): PriceParts {
	if (!copperAmount) return { gold: 0, silver: 0, copper: 0 };

	const gold = Math.floor(copperAmount / 10000);
	const remaining = copperAmount % 10000;
	const silver = Math.floor(remaining / 100);
	const copper = remaining % 100;

	return { gold, silver, copper };
}

export function formatPrice(copperAmount: number | null): string {
	if (!copperAmount) return 'N/A';

	const { gold, silver, copper } = getPriceBreakdown(copperAmount);
	const parts: string[] = [];
	if (gold > 0) parts.push(`${gold}g`);
	if (silver > 0) parts.push(`${silver}s`);
	if (copper > 0) parts.push(`${copper}c`);

	return parts.length > 0 ? parts.join(' ') : '0c';
}

export interface AggregatedAuction {
	itemId: number;
	itemName: string;
	totalQuantity: number;
	lowestBuyoutCopper: number | null;
	lowestUnitPriceCopper: number | null;
	isCommodity: boolean;
}

export function aggregateAuctions(auctions: AuctionResult[]): AggregatedAuction[] {
	const grouped = new Map<number, AuctionResult[]>();

	// Group auctions by itemId
	for (const auction of auctions) {
		if (!grouped.has(auction.itemId)) {
			grouped.set(auction.itemId, []);
		}
		grouped.get(auction.itemId)!.push(auction);
	}

	// Aggregate the grouped auctions
	const aggregated: AggregatedAuction[] = [];
	for (const [itemId, itemAuctions] of grouped) {
		const totalQuantity = itemAuctions.reduce((sum, a) => sum + a.quantity, 0);
		const lowestBuyout = itemAuctions
			.filter(a => a.buyoutCopper !== null)
			.reduce((min, a) => (a.buyoutCopper! < min ? a.buyoutCopper! : min), Infinity);
		const lowestUnitPrice = itemAuctions
			.filter(a => a.unitPriceCopper !== null)
			.reduce((min, a) => (a.unitPriceCopper! < min ? a.unitPriceCopper! : min), Infinity);

		aggregated.push({
			itemId,
			itemName: itemAuctions[0].itemName,
			totalQuantity,
			lowestBuyoutCopper: lowestBuyout === Infinity ? null : lowestBuyout,
			lowestUnitPriceCopper: lowestUnitPrice === Infinity ? null : lowestUnitPrice,
			isCommodity: itemAuctions[0].isCommodity
		});
	}

	return aggregated;
}

export async function searchAuctions(connectedRealmId: number, item: string): Promise<AuctionResult[]> {
	const response = await fetch(
		`${API_BASE_URL}/api/auctions/${connectedRealmId}?item=${encodeURIComponent(item)}`
	);

	if (!response.ok) {
		throw new Error(`Search failed: ${response.status}`);
	}

	const data = (await response.json()) as AuctionResult[];
	return data;
}