namespace TarkovPriceViewer
{
    public class Item
    {
        public string NameDisplay;
        public char[] NameCompare;
        public string NameDisplay2;
        public char[] NameCompare2;
        public bool IsName2 = false;
        public string MarketAddress;
        public string WikiAddress;
        public string PriceLast;
        public string PriceDay;
        public string PriceWeek;
        public string SellToTrader;
        public string SellToTraderPrice;
        public string BuyFromTrader;
        public string BuyFromTraderPrice;
        public string LastUpdate;

        public string Needs;
        public string Bartersandcrafts;
        public string Type;
        public string Recoil;
        public string Accuracy;
        public string Ergo;

        public DateTime LastFetch;
        public Ballistic Ballistic = null;

        public string[] Data()
        {
            return new string[] {
                IsName2 ? NameDisplay2 : NameDisplay
                , Recoil != null ? Recoil : ""
                , Accuracy != null ? Accuracy : ""
                , Ergo != null ? Ergo : ""
                , PriceLast != null ? PriceLast : ""
                , BuyFromTraderPrice != null ? BuyFromTraderPrice.Replace(" ", "").Replace(@"~", @" ≈") : ""
                , BuyFromTrader != null ? BuyFromTrader : ""
            };
        }
    }
}
