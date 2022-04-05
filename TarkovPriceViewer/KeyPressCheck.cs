using TarkovPriceViewer;

namespace TarkovPriceChecker
{
    public partial class KeyPressCheck : Form
    {
        private readonly int _button;

        public KeyPressCheck(int button)
        {
            _button = button;

            InitializeComponent();
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
                    switch (_button)
                    {
                        case 1:
                            Program.Settings["ShowOverlay_Key"] = ((int)e.KeyCode).ToString();
                            break;
                        case 2:
                            Program.Settings["HideOverlay_Key"] = ((int)e.KeyCode).ToString();
                            break;
                        case 3:
                            Program.Settings["CompareOverlay_Key"] = ((int)e.KeyCode).ToString();
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
