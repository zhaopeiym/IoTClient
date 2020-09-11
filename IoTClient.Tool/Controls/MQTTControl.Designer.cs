namespace IoTClient.Tool.Controls
{
    partial class MQTTControl
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.but_Start = new System.Windows.Forms.Button();
            this.txt_msg = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.but_Subscribe = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.but_Publish = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.but_Stop = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.txt_pfx_file = new IoTClient.Tool.Controls.TextBoxEx();
            this.txt_ca_file = new IoTClient.Tool.Controls.TextBoxEx();
            this.txt_Password = new IoTClient.Tool.Controls.TextBoxEx();
            this.txt_UserName = new IoTClient.Tool.Controls.TextBoxEx();
            this.txt_ClientID = new IoTClient.Tool.Controls.TextBoxEx();
            this.txt_Port = new IoTClient.Tool.Controls.TextBoxEx();
            this.txt_Address = new IoTClient.Tool.Controls.TextBoxEx();
            this.txt_subscribe_topic = new IoTClient.Tool.Controls.TextBoxEx();
            this.txt_publish_payload = new IoTClient.Tool.Controls.TextBoxEx();
            this.txt_publish_topic = new IoTClient.Tool.Controls.TextBoxEx();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // but_Start
            // 
            this.but_Start.Location = new System.Drawing.Point(560, 15);
            this.but_Start.Name = "but_Start";
            this.but_Start.Size = new System.Drawing.Size(75, 23);
            this.but_Start.TabIndex = 0;
            this.but_Start.Text = "启动";
            this.but_Start.UseVisualStyleBackColor = true;
            this.but_Start.Click += new System.EventHandler(this.but_start_ClickAsync);
            // 
            // txt_msg
            // 
            this.txt_msg.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txt_msg.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txt_msg.Location = new System.Drawing.Point(3, 57);
            this.txt_msg.Multiline = true;
            this.txt_msg.Name = "txt_msg";
            this.txt_msg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_msg.Size = new System.Drawing.Size(866, 281);
            this.txt_msg.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl1.Location = new System.Drawing.Point(0, 83);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(880, 367);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txt_subscribe_topic);
            this.tabPage1.Controls.Add(this.but_Subscribe);
            this.tabPage1.Controls.Add(this.txt_msg);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(872, 341);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "订阅";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // but_Subscribe
            // 
            this.but_Subscribe.Location = new System.Drawing.Point(414, 16);
            this.but_Subscribe.Name = "but_Subscribe";
            this.but_Subscribe.Size = new System.Drawing.Size(75, 23);
            this.but_Subscribe.TabIndex = 8;
            this.but_Subscribe.Text = "订阅";
            this.but_Subscribe.UseVisualStyleBackColor = true;
            this.but_Subscribe.Click += new System.EventHandler(this.but_Subscribe_ClickAsync);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txt_publish_payload);
            this.tabPage2.Controls.Add(this.txt_publish_topic);
            this.tabPage2.Controls.Add(this.but_Publish);
            this.tabPage2.Controls.Add(this.textBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(872, 341);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "发布";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // but_Publish
            // 
            this.but_Publish.Location = new System.Drawing.Point(673, 50);
            this.but_Publish.Name = "but_Publish";
            this.but_Publish.Size = new System.Drawing.Size(75, 23);
            this.but_Publish.TabIndex = 10;
            this.but_Publish.Text = "发布";
            this.but_Publish.UseVisualStyleBackColor = true;
            this.but_Publish.Click += new System.EventHandler(this.but_Publish_Click);
            // 
            // textBox2
            // 
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.textBox2.Location = new System.Drawing.Point(3, 92);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(866, 246);
            this.textBox2.TabIndex = 9;
            // 
            // but_Stop
            // 
            this.but_Stop.Location = new System.Drawing.Point(641, 15);
            this.but_Stop.Name = "but_Stop";
            this.but_Stop.Size = new System.Drawing.Size(75, 23);
            this.but_Stop.TabIndex = 8;
            this.but_Stop.Text = "停止";
            this.but_Stop.UseVisualStyleBackColor = true;
            this.but_Stop.Click += new System.EventHandler(this.but_Stop_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(724, 19);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(90, 16);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "开启SSL/TLS";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // txt_pfx_file
            // 
            this.txt_pfx_file.Location = new System.Drawing.Point(232, 42);
            this.txt_pfx_file.Name = "txt_pfx_file";
            this.txt_pfx_file.PlaceHolder = "双击选择pfx文件";
            this.txt_pfx_file.Size = new System.Drawing.Size(224, 21);
            this.txt_pfx_file.TabIndex = 11;
            this.txt_pfx_file.DoubleClick += new System.EventHandler(this.txt_pfx_file_Click);
            // 
            // txt_ca_file
            // 
            this.txt_ca_file.Location = new System.Drawing.Point(2, 42);
            this.txt_ca_file.Name = "txt_ca_file";
            this.txt_ca_file.PlaceHolder = "双击选择ca.crt";
            this.txt_ca_file.Size = new System.Drawing.Size(224, 21);
            this.txt_ca_file.TabIndex = 10;
            this.txt_ca_file.DoubleClick += new System.EventHandler(this.txt_ca_file_Click);
            // 
            // txt_Password
            // 
            this.txt_Password.Location = new System.Drawing.Point(444, 15);
            this.txt_Password.Name = "txt_Password";
            this.txt_Password.PasswordChar = '*';
            this.txt_Password.PlaceHolder = "Password";
            this.txt_Password.Size = new System.Drawing.Size(100, 21);
            this.txt_Password.TabIndex = 7;
            // 
            // txt_UserName
            // 
            this.txt_UserName.Location = new System.Drawing.Point(338, 15);
            this.txt_UserName.Name = "txt_UserName";
            this.txt_UserName.PlaceHolder = "UserName";
            this.txt_UserName.Size = new System.Drawing.Size(100, 21);
            this.txt_UserName.TabIndex = 6;
            // 
            // txt_ClientID
            // 
            this.txt_ClientID.Location = new System.Drawing.Point(232, 15);
            this.txt_ClientID.Name = "txt_ClientID";
            this.txt_ClientID.PlaceHolder = "ClientID";
            this.txt_ClientID.Size = new System.Drawing.Size(100, 21);
            this.txt_ClientID.TabIndex = 5;
            // 
            // txt_Port
            // 
            this.txt_Port.Location = new System.Drawing.Point(163, 15);
            this.txt_Port.Name = "txt_Port";
            this.txt_Port.PlaceHolder = "Port";
            this.txt_Port.Size = new System.Drawing.Size(63, 21);
            this.txt_Port.TabIndex = 4;
            this.txt_Port.Text = "1883";
            // 
            // txt_Address
            // 
            this.txt_Address.Location = new System.Drawing.Point(1, 15);
            this.txt_Address.Name = "txt_Address";
            this.txt_Address.PlaceHolder = "Address";
            this.txt_Address.Size = new System.Drawing.Size(156, 21);
            this.txt_Address.TabIndex = 3;
            // 
            // txt_subscribe_topic
            // 
            this.txt_subscribe_topic.Location = new System.Drawing.Point(3, 17);
            this.txt_subscribe_topic.Name = "txt_subscribe_topic";
            this.txt_subscribe_topic.PlaceHolder = "Topic";
            this.txt_subscribe_topic.Size = new System.Drawing.Size(406, 21);
            this.txt_subscribe_topic.TabIndex = 8;
            // 
            // txt_publish_payload
            // 
            this.txt_publish_payload.Location = new System.Drawing.Point(3, 37);
            this.txt_publish_payload.Multiline = true;
            this.txt_publish_payload.Name = "txt_publish_payload";
            this.txt_publish_payload.PlaceHolder = "Payload";
            this.txt_publish_payload.Size = new System.Drawing.Size(630, 49);
            this.txt_publish_payload.TabIndex = 12;
            // 
            // txt_publish_topic
            // 
            this.txt_publish_topic.Location = new System.Drawing.Point(3, 10);
            this.txt_publish_topic.Name = "txt_publish_topic";
            this.txt_publish_topic.PlaceHolder = "Topic";
            this.txt_publish_topic.Size = new System.Drawing.Size(630, 21);
            this.txt_publish_topic.TabIndex = 11;
            // 
            // MQTTControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txt_pfx_file);
            this.Controls.Add(this.txt_ca_file);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.but_Stop);
            this.Controls.Add(this.txt_Password);
            this.Controls.Add(this.txt_UserName);
            this.Controls.Add(this.txt_ClientID);
            this.Controls.Add(this.txt_Port);
            this.Controls.Add(this.txt_Address);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.but_Start);
            this.Name = "MQTTControl";
            this.Size = new System.Drawing.Size(880, 450);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button but_Start;
        private System.Windows.Forms.TextBox txt_msg;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private TextBoxEx txt_Address;
        private TextBoxEx txt_Port;
        private TextBoxEx txt_ClientID;
        private TextBoxEx txt_UserName;
        private TextBoxEx txt_Password;
        private TextBoxEx txt_subscribe_topic;
        private System.Windows.Forms.Button but_Subscribe;
        private TextBoxEx txt_publish_topic;
        private System.Windows.Forms.Button but_Publish;
        private System.Windows.Forms.TextBox textBox2;
        private TextBoxEx txt_publish_payload;
        private System.Windows.Forms.Button but_Stop;
        private System.Windows.Forms.CheckBox checkBox1;
        private TextBoxEx txt_ca_file;
        private TextBoxEx txt_pfx_file;
    }
}
