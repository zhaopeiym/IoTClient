using System;
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
            //Hide();
            //var form = new ModBusTcpForm();
            //form.StartPosition = FormStartPosition.CenterScreen;
            //form.ShowDialog();
            //Show(); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Hide();
            //var form = new SiemensForm();
            //form.StartPosition = FormStartPosition.CenterScreen;
            //form.ShowDialog();
            //Show(); 
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tab = (sender as TabControl).SelectedTab;
            Text = tab.Name;
            if (tab.Controls.Count <= 0)
            {
                switch (tab.Name)
                {
                    case "ModBusTcp":
                        var modBusTcpControl = new ModBusTcpControl();
                        modBusTcpControl.Dock = DockStyle.Fill;
                        tab.Controls.Add(modBusTcpControl);
                        break;
                    case "Siemens":
                        var siemensControl = new SiemensControl();
                        siemensControl.Dock = DockStyle.Fill;
                        tab.Controls.Add(siemensControl);
                        break;
                }
            }
        }
    }
}
