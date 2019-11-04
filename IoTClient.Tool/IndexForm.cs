using IoTClient.Tool.Controls;
using IoTClient.Tool.Model;
using IoTServer.Common;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IoTClient.Tool
{
    public partial class IndexForm : Form
    {
        public IndexForm()
        {

            #region 检查版本升级
#if !DEBUG
            Task.Run(async () =>
            {
                while (true)
                {
                    //检查版本升级
                    await CheckUpgradeAsync();
                    await Task.Delay(1000 * 60 * 60 * 1);//一小时
                }
            });
#endif 
            #endregion

            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            //加载模拟服务的历史数据
            DataPersist.LoadData();

            #region 初始化设置上次选择的tab
            var tabName = GetTabName();
            if (!string.IsNullOrWhiteSpace(tabName))
            {
                foreach (TabPage item in tabControl1.TabPages)
                {
                    if (item.Name == tabName?.Trim())
                    {
                        //设置上次选择的tab
                        tabControl1.SelectedTab = item;
                    }
                    //还未实现，先隐藏
                    else if (item.Name == "OmronFinsTcp")
                    {
                        tabControl1.TabPages.Remove(item);
                    }
                    //加载用户控件
                    SelectedTab(item);
                }
            }
            //切换到上次选择的Tab
            SelectedTab(tabControl1.SelectedTab);
            #endregion

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
            Text = "IoTClient Tool - " + tab.Text?.Trim();
            SaveTabName(tab.Name);
            if (tab.Controls.Count <= 0)
            {
                switch (tab.Name)
                {
                    case "ModBusTcp":
                        var modBusTcp = new ModBusTcpControl();
                        modBusTcp.Dock = DockStyle.Fill;
                        tab.Controls.Add(modBusTcp);
                        break;
                    case "ModBusRtu":
                        var modBusRtu = new ModBusRtuControl();
                        modBusRtu.Dock = DockStyle.Fill;
                        tab.Controls.Add(modBusRtu);
                        break;
                    case "Siemens":
                        var siemens = new SiemensControl();
                        siemens.Dock = DockStyle.Fill;
                        tab.Controls.Add(siemens);
                        break;
                    case "BACnet":
                        var bacnet = new BACnetControl();
                        bacnet.Dock = DockStyle.Fill;
                        tab.Controls.Add(bacnet);
                        break;
                    case "Ports":
                        var ports = new PortsControl();
                        ports.Dock = DockStyle.Fill;
                        tab.Controls.Add(ports);
                        break;
                    case "MitsubishiMC":
                        var mitsubishiMC = new MitsubishiMCControl();
                        mitsubishiMC.Dock = DockStyle.Fill;
                        tab.Controls.Add(mitsubishiMC);
                        break;
                    case "OmronFinsTcp":
                        var omronFinsTcp = new OmronFinsTcpControl();
                        omronFinsTcp.Dock = DockStyle.Fill;
                        tab.Controls.Add(omronFinsTcp);
                        break;
                }
            }
            else
            {
                switch (tab.Controls[0].Name)
                {
                    case nameof(PortsControl):
                        var portsControl = tab.Controls[0] as PortsControl;
                        portsControl?.UpdatePortNames();
                        break;
                    case nameof(ModBusRtuControl):
                        var modBusRtuControl = tab.Controls[0] as ModBusRtuControl;
                        modBusRtuControl?.UpdatePortNames();
                        break;
                }
            }
        }

        private void SaveTabName(string tagName)
        {
            var path = @"C:\IoTClient";
            var filePath = path + @"\TabName.Data";
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fileStream))
                {
                    sw.Write(tagName);
                }
            }
        }

        private string GetTabName()
        {
            var dataString = string.Empty;
            var path = @"C:\IoTClient";
            var filePath = path + @"\TabName.Data";
            if (File.Exists(filePath))
                dataString = File.ReadAllText(filePath);
            else
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                File.SetAttributes(path, FileAttributes.Hidden);
            }
            return dataString;
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

        private void toolStripMenuItemBlogPath_Click(object sender, EventArgs e)
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

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://qingnakeji.com");
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 检查当前是否需要升级
        /// </summary>
        private async Task CheckUpgradeAsync()
        {
            UpgradeFileManage();
            HttpClient http = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(new VersionCheckInput()));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await http.PostAsync("https://download.haojima.net/api/IoTClient/VersionCheck", content);
            var result = await response.Content.ReadAsStringAsync();
            var VersionObj = JsonConvert.DeserializeObject<ResultBase<VersionCheckOutput>>(result);
            //VersionObj.IsSuccess  有问题 TODO
            if (VersionObj.Code == 200 && VersionObj.Data.Code == 1)
            {
                if (MessageBox.Show("IoTClient有新版本，是否升级到最新版本？", "版本升级", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (new UpgradeForm().ShowDialog() != DialogResult.OK) return;
                    var newApp = Application.StartupPath + @"\temp." + Path.GetFileName(Application.ExecutablePath);
                    //1、检查更新文件 复制临时文件
                    //File.Copy(Application.ExecutablePath, newApp, true);
                    //2、打开临时文件 关闭并旧版本
                    Process.Start(newApp);
                    //https://blog.csdn.net/qq_33204100/article/details/83144271
                    Close();
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// 升级文件处理
        /// </summary>
        private void UpgradeFileManage()
        {
            //如果启动的升级临时文件，
            //则1、删除旧版本 2、复制当前临时文件为新版本 3、启动新版本 4、关闭当前打开的临时版本
            if (Path.GetFileName(Application.ExecutablePath).Contains("temp."))
            {
                var filePath = Path.Combine(Application.StartupPath, Path.GetFileName(Application.ExecutablePath).Replace("temp.", ""));
                var newFilePath = filePath;
                try
                {
                    try
                    {
                        //2.1删除旧版本
                        if (File.Exists(filePath)) File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        //如果因为进程正在使用中则休眠后再重试
                        //出现此问题的原因是，上一个程序还没关闭，这个程序就启动了，启动后会执行删除上一个程序，所以报错。
                        Thread.Sleep(500);
                        if (File.Exists(filePath)) File.Delete(filePath);
                    }
                    //3、复制临时文件为新的文件 打开新的文件       
                    File.Copy(Application.ExecutablePath, newFilePath);
                    //3、打开新的文件
                    Process.Start(filePath);
                    //4、关闭临时文件   
                    //Close();
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("升级失败 " + ex.Message);
                }
            }
            //4.2如果当前启动的不是临时文件，则删除临时文件。
            else
            {
                var filePath = Path.Combine(Application.StartupPath, "temp." + Path.GetFileName(Application.ExecutablePath));
                try
                {
                    if (File.Exists(filePath)) File.Delete(filePath);
                }
                catch (Exception)
                {
                    Thread.Sleep(500);
                    if (File.Exists(filePath)) File.Delete(filePath);
                }
            }
        }
    }
}
