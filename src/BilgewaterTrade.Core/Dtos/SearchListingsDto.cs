using BilgewaterTrade.DataAccess;

namespace BilgewaterTrade.Core.Dtos;

public class SearchListingsDto
{
        public int ItemId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public bool IsCommodity { get; set; }

        public long? BuyoutCopper { get; set; }

        public long? UnitPriceCopper { get; set; }

        public int Quantity { get; set; }

        public Common.TimeLeft TimeLeft { get; set; }
}