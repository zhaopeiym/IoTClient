using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IoTClient.Demo
{
    public partial class IndexForm : Form
    {
        public IndexForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            var form = new ModBusTcpForm();
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ShowDialog();
            Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
            var form = new SiemensForm();
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ShowDialog();
            Show();
        }
    }
}
