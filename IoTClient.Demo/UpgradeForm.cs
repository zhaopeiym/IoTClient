using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IoTClient.Demo
{
    public partial class UpgradeForm : Form
    {
        public UpgradeForm()
        {
            InitializeComponent();
            TopMost = true;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            CheckForIllegalCrossThreadCalls = false;
            Task.Run(async () =>
            {
                await DownloadAsync();
                DialogResult = DialogResult.OK;
                Close();
            });
        }

        public async Task DownloadAsync()
        {
            long downloadSize = 0;//已经下载大小
            long downloadSpeed = 0;//下载速度
            using (HttpClient http = new HttpClient())
            {
                var httpResponseMessage = await http.GetAsync("https://download.haojima.net/api/IoTClient/Download", HttpCompletionOption.ResponseHeadersRead);//发送请求
                var contentLength = httpResponseMessage.Content.Headers.ContentLength;   //文件大小    
                if (contentLength == null)
                {
                    MessageBox.Show("服务器忙，请稍后再试。");
                    return;
                }
                using (var stream = await httpResponseMessage.Content.ReadAsStreamAsync())
                {
                    var readLength = 102400;//100K
                    byte[] bytes = new byte[readLength];
                    int writeLength;
                    var beginSecond = DateTime.Now.Second;//当前时间秒
                    //使用追加方式打开一个文件流
                    var filePath = Application.StartupPath + @"\temp." + Path.GetFileName(Application.ExecutablePath);
                    using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                    {
                        while ((writeLength = stream.Read(bytes, 0, readLength)) > 0)
                        {
                            fs.Write(bytes, 0, writeLength);
                            downloadSize += writeLength;
                            downloadSpeed += writeLength;
                            progressBar1.Invoke((Action)(() =>
                            {
                                var endSecond = DateTime.Now.Second;
                                if (beginSecond != endSecond)//计算速度
                                {
                                    downloadSpeed = downloadSpeed / (endSecond - beginSecond);
                                    Text = "下载速度" + downloadSpeed / 1024 + "KB/S";
                                    beginSecond = DateTime.Now.Second;
                                    downloadSpeed = 0;//清空
                                }
                                progressBar1.Value = Math.Max((int)(downloadSize * 100 / contentLength), 1);
                            }));
                        }
                    }
                }
            }
        }
    }
}
