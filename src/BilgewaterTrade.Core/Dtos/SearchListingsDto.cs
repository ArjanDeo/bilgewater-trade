using BilgewaterTrade.DataAccess;

namespace BilgewaterTrade.Core.Dtos;

public class SearchListingsDto
{
        public int ItemId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public bool IsCommodity { get; set; }

        public long? CheapestBuyoutCopper { get; set; }

        public long? CheapestUnitPriceCopper { get; set; }

        public int Quantity { get; set; }
}