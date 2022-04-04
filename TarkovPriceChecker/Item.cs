namespace TarkovPriceViewer
{
    public class Item
    {
        public string name_display;
        public char[] name_compare;
        public string name_display2;
        public char[] name_compare2;
        public bool isname2 = false;
        public string market_address;
        public string wiki_address;
        public string price_last;
        public string price_day;
        public string price_week;
        public string sell_to_trader;
        public string sell_to_trader_price;
        public string buy_from_trader;
        public string buy_from_trader_price;
        public string last_updated;

        public string needs;
        public string bartersandcrafts;
        public string type;
        public string recoil;
        public string accuracy;
        public string ergo;

        public DateTime last_fetch;
        public Ballistic ballistic = null;

        public string[] Data()
        {
            return new string[] {
                isname2 ? name_display2 : name_display
                , recoil != null ? recoil : ""
                , accuracy != null ? accuracy : ""
                , ergo != null ? ergo : ""
                , price_last != null ? price_last : ""
                , buy_from_trader_price != null ? buy_from_trader_price.Replace(" ", "").Replace(@"~", @" ≈") : ""
                , buy_from_trader != null ? buy_from_trader : ""
            };
        }
    }
}
