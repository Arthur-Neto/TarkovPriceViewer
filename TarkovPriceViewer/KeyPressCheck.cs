using Microsoft.Extensions.Options;
using TarkovPriceViewer.Infrastructure.Settings;

namespace TarkovPriceChecker
{
    public partial class KeyPressCheck : Form
    {
        public int Button { get; set; }

        private readonly AppSettings _appSettings;

        public KeyPressCheck(IOptions<AppSettings> options)
        {
            InitializeComponent();

            _appSettings = options.Value;
        }

        private void KeyPressCheckFormClosed(object sender, FormClosedEventArgs e)
        {
            if (Owner != null)
            {
                ((MainForm)Owner).ChangePressKeyData(null);
            }
        }

        private void KeyPressCheck_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode is not Keys.ShiftKey and not Keys.Menu and not Keys.ControlKey)
            {
                if (e.KeyCode != Keys.Escape)
                {
                    switch (Button)
                    {
                        case 1:
                            //Program.Settings["ShowOverlay_Key"] = ((int)e.KeyCode).ToString();
                            break;
                        case 2:
                            //Program.Settings["HideOverlay_Key"] = ((int)e.KeyCode).ToString();
                            break;
                        case 3:
                            //Program.Settings["CompareOverlay_Key"] = ((int)e.KeyCode).ToString();
                            break;
                    }

                    if (Owner != null)
                    {
                        ((MainForm)Owner).ChangePressKeyData(e.KeyCode);
                    }
                }

                Close();
            }
        }
    }
}
