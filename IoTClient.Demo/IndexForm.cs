using IoTServer.Common;
using System;
using System.Diagnostics;
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
            FormBorderStyle = FormBorderStyle.FixedSingle;
            DataPersist.LoadData();
            SelectedTab(tabControl1.SelectedTab);
            Task.Run(async () =>
            {
                await Task.Delay(1000 * 60 * 1);//1分钟自动保存一次
                DataPersist.SaveData();
            });
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tab = (sender as TabControl).SelectedTab;
            SelectedTab(tab);
        }

        /// <summary>
        /// 切换Tab
        /// </summary>
        /// <param name="tab"></param>
        private void SelectedTab(TabPage tab)
        {
            Text = "IoTClient - " + tab.Name;
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

        private void IndexForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataPersist.SaveData();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://github.com/zhaopeiym/IoTClient");
            }
            catch (Exception) { }
        }

        private void 博客地址ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://www.cnblogs.com/zhaopei/p/11651790.html");
            }
            catch (Exception) { }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://github.com/zhaopeiym/IoTClient/issues");
            }
            catch (Exception) { }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://jq.qq.com/?_wv=1027&k=5bz0ne5");
            }
            catch (Exception) { }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://github.com/zhaopeiym/IoTClient/releases");
            }
            catch (Exception) { }
        }
    }
}
