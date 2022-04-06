using TarkovPriceViewer.Infrastructure.JsonWriter;
using TarkovPriceViewer.Infrastructure.Settings;

namespace TarkovPriceViewer.Forms
{
    public partial class KeyPressCheck : Form
    {
        public int Button { get; set; }

        private readonly IWritableOptions<SettingsOptions> _writableOptions;

        public KeyPressCheck(
            IWritableOptions<SettingsOptions> writableOptions
        )
        {
            InitializeComponent();

            _writableOptions = writableOptions;
        }

        private void KeyPressCheckFormClosed(object sender, FormClosedEventArgs e)
        {
            if (Owner != null)
            {
                ((MainForm)Owner).ChangePressKeyData(null);
            }
        }

        private void KeyPressCheckKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode is not Keys.ShiftKey and not Keys.Menu and not Keys.ControlKey)
            {
                if (e.KeyCode != Keys.Escape)
                {
                    var keyCode = ((int)e.KeyCode).ToString();
                    switch (Button)
                    {
                        case 1:
                            _writableOptions.Update(opt =>
                            {
                                opt.ShowOverlayKey = keyCode;
                            });
                            break;
                        case 2:
                            _writableOptions.Update(opt =>
                            {
                                opt.HideOverlayKey = keyCode;
                            });
                            break;
                        case 3:
                            _writableOptions.Update(opt =>
                            {
                                opt.CompareOverlayKey = keyCode;
                            });
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
