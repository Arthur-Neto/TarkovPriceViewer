namespace TarkovPriceViewer.Infrastructure.Settings
{
    public class AppSettings
    {
        public bool MinimizetoTrayWhenStartup { get; set; } = false;
        public bool CloseOverlayWhenMouseMoved { get; set; } = true;
        public bool RandomItem { get; set; } = false;
        public string ShowOverlayKey { get; set; } = "120";
        public string HideOverlayKey { get; set; } = "121";
        public string CompareOverlayKey { get; set; } = "119";
        public string OverlayTransparency { get; set; } = "80";
        public bool ShowLastPrice { get; set; } = true;
        public bool ShowDayPrice { get; set; } = true;
        public bool ShowWeekPrice { get; set; } = true;
        public bool SellToTrader { get; set; } = true;
        public bool BuyFromTrader { get; set; } = true;
        public bool Needs { get; set; } = true;
        public bool BartersNCrafts { get; set; } = true;
    }
}
