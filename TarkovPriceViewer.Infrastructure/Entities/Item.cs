namespace TarkovPriceViewer.Infrastructure.Entities
{
    public class Item
    {
        public string? NameDisplay;
        public char[]? NameCompare;
        public string? NameDisplay2;
        public char[]? NameCompare2;
        public bool IsName2 = false;
        public string? MarketAddress;
        public string? WikiAddress;
        public string? PriceLast;
        public string? PriceDay;
        public string? PriceWeek;
        public string? SellToTrader;
        public string? SellToTraderPrice;
        public string? BuyFromTrader;
        public string? BuyFromTraderPrice;
        public string? LastUpdate;

        public string? Needs;
        public string? BartersNCrafts;
        public string? Type;
        public string? Recoil;
        public string? Accuracy;
        public string? Ergo;

        public DateTime LastFetch;
        public Ballistic? Ballistic = null;

        public string[] Data()
        {
            return new string[] {
                IsName2 ? NameDisplay2 : NameDisplay
                , Recoil ?? string.Empty
                , Accuracy ?? string.Empty
                , Ergo ?? string.Empty
                , PriceLast ?? string.Empty
                , BuyFromTraderPrice != null ? BuyFromTraderPrice.Replace(" ", string.Empty).Replace(@"~", @" ≈") : string.Empty
                , BuyFromTrader ?? string.Empty
            };
        }
    }
}
