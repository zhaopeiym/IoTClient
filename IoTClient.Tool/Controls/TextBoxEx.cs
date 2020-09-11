using System.Drawing;
using System.Windows.Forms;

namespace IoTClient.Tool.Controls
{
    public class TextBoxEx : TextBox
    {
        public string PlaceHolder { get; set; }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0xF || m.Msg == 0x133)
            {
                WmPaint(ref m);
            }
        }
        private void WmPaint(ref Message m)
        {
            Graphics g = Graphics.FromHwnd(Handle);
            if (!string.IsNullOrEmpty(PlaceHolder) && string.IsNullOrEmpty(Text))
                g.DrawString(PlaceHolder, Font, new SolidBrush(Color.LightGray), 2, 2);
        }
    }
}
