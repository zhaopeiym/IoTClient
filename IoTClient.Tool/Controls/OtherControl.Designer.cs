namespace IoTClient.Tool.Controls
{
    partial class OtherControl
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.but_crc16calculate = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txt_crcstr = new System.Windows.Forms.TextBox();
            this.txt_content = new System.Windows.Forms.TextBox();
            this.but_crc16validation = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_tcpmsg = new System.Windows.Forms.TextBox();
            this.txt_tcpport = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_tcpip = new System.Windows.Forms.TextBox();
            this.but_tcpsend = new System.Windows.Forms.Button();
            this.but_tcpclose = new System.Windows.Forms.Button();
            this.but_tcpopen = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_udpmsg = new System.Windows.Forms.TextBox();
            this.txt_udpport = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_udpip = new System.Windows.Forms.TextBox();
            this.but_udpsend = new System.Windows.Forms.Button();
            this.but_udpclose = new System.Windows.Forms.Button();
            this.but_udpopen = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.txt_redisValue = new System.Windows.Forms.TextBox();
            this.txt_redisKey = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txt_address = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // but_crc16calculate
            // 
            this.but_crc16calculate.Location = new System.Drawing.Point(349, 14);
            this.but_crc16calculate.Name = "but_crc16calculate";
            this.but_crc16calculate.Size = new System.Drawing.Size(75, 23);
            this.but_crc16calculate.TabIndex = 0;
            this.but_crc16calculate.Text = "CRC16计算";
            this.but_crc16calculate.UseVisualStyleBackColor = true;
            this.but_crc16calculate.Click += new System.EventHandler(this.but_crc16calculate_Click);
            // 
            // txt_crcstr
            // 
            this.txt_crcstr.Location = new System.Drawing.Point(6, 15);
            this.txt_crcstr.Name = "txt_crcstr";
            this.txt_crcstr.Size = new System.Drawing.Size(337, 21);
            this.txt_crcstr.TabIndex = 1;
            // 
            // txt_content
            // 
            this.txt_content.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txt_content.Location = new System.Drawing.Point(3, 169);
            this.txt_content.Multiline = true;
            this.txt_content.Name = "txt_content";
            this.txt_content.Size = new System.Drawing.Size(874, 268);
            this.txt_content.TabIndex = 2;
            // 
            // but_crc16validation
            // 
            this.but_crc16validation.Location = new System.Drawing.Point(430, 14);
            this.but_crc16validation.Name = "but_crc16validation";
            this.but_crc16validation.Size = new System.Drawing.Size(75, 23);
            this.but_crc16validation.TabIndex = 3;
            this.but_crc16validation.Text = "CRC16验证";
            this.but_crc16validation.UseVisualStyleBackColor = true;
            this.but_crc16validation.Click += new System.EventHandler(this.but_crc16validation_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.but_crc16calculate);
            this.groupBox1.Controls.Add(this.but_crc16validation);
            this.groupBox1.Controls.Add(this.txt_crcstr);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(512, 44);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CRC";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txt_tcpmsg);
            this.groupBox2.Controls.Add(this.txt_tcpport);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txt_tcpip);
            this.groupBox2.Controls.Add(this.but_tcpsend);
            this.groupBox2.Controls.Add(this.but_tcpclose);
            this.groupBox2.Controls.Add(this.but_tcpopen);
            this.groupBox2.Location = new System.Drawing.Point(3, 53);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(253, 110);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "TCP";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "消息";
            // 
            // txt_tcpmsg
            // 
            this.txt_tcpmsg.Location = new System.Drawing.Point(33, 41);
            this.txt_tcpmsg.Multiline = true;
            this.txt_tcpmsg.Name = "txt_tcpmsg";
            this.txt_tcpmsg.Size = new System.Drawing.Size(214, 34);
            this.txt_tcpmsg.TabIndex = 7;
            // 
            // txt_tcpport
            // 
            this.txt_tcpport.Location = new System.Drawing.Point(193, 14);
            this.txt_tcpport.Name = "txt_tcpport";
            this.txt_tcpport.Size = new System.Drawing.Size(53, 21);
            this.txt_tcpport.TabIndex = 6;
            this.txt_tcpport.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(164, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "端口";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "IP";
            // 
            // txt_tcpip
            // 
            this.txt_tcpip.Location = new System.Drawing.Point(33, 14);
            this.txt_tcpip.Name = "txt_tcpip";
            this.txt_tcpip.Size = new System.Drawing.Size(128, 21);
            this.txt_tcpip.TabIndex = 3;
            // 
            // but_tcpsend
            // 
            this.but_tcpsend.Location = new System.Drawing.Point(173, 81);
            this.but_tcpsend.Name = "but_tcpsend";
            this.but_tcpsend.Size = new System.Drawing.Size(75, 23);
            this.but_tcpsend.TabIndex = 2;
            this.but_tcpsend.Text = "发送";
            this.but_tcpsend.UseVisualStyleBackColor = true;
            this.but_tcpsend.Click += new System.EventHandler(this.but_tcpSend_Click);
            // 
            // but_tcpclose
            // 
            this.but_tcpclose.Location = new System.Drawing.Point(89, 81);
            this.but_tcpclose.Name = "but_tcpclose";
            this.but_tcpclose.Size = new System.Drawing.Size(75, 23);
            this.but_tcpclose.TabIndex = 1;
            this.but_tcpclose.Text = "关闭连接";
            this.but_tcpclose.UseVisualStyleBackColor = true;
            this.but_tcpclose.Click += new System.EventHandler(this.but_tcpClose_Click);
            // 
            // but_tcpopen
            // 
            this.but_tcpopen.Location = new System.Drawing.Point(5, 81);
            this.but_tcpopen.Name = "but_tcpopen";
            this.but_tcpopen.Size = new System.Drawing.Size(75, 23);
            this.but_tcpopen.TabIndex = 0;
            this.but_tcpopen.Text = "打开连接";
            this.but_tcpopen.UseVisualStyleBackColor = true;
            this.but_tcpopen.Click += new System.EventHandler(this.but_tcpOpen_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txt_udpmsg);
            this.groupBox3.Controls.Add(this.txt_udpport);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.txt_udpip);
            this.groupBox3.Controls.Add(this.but_udpsend);
            this.groupBox3.Controls.Add(this.but_udpclose);
            this.groupBox3.Controls.Add(this.but_udpopen);
            this.groupBox3.Location = new System.Drawing.Point(262, 53);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(253, 110);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "UDP";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "消息";
            // 
            // txt_udpmsg
            // 
            this.txt_udpmsg.Location = new System.Drawing.Point(33, 41);
            this.txt_udpmsg.Multiline = true;
            this.txt_udpmsg.Name = "txt_udpmsg";
            this.txt_udpmsg.Size = new System.Drawing.Size(214, 34);
            this.txt_udpmsg.TabIndex = 7;
            // 
            // txt_udpport
            // 
            this.txt_udpport.Location = new System.Drawing.Point(193, 14);
            this.txt_udpport.Name = "txt_udpport";
            this.txt_udpport.Size = new System.Drawing.Size(53, 21);
            this.txt_udpport.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(164, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "端口";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "IP";
            // 
            // txt_udpip
            // 
            this.txt_udpip.Location = new System.Drawing.Point(33, 14);
            this.txt_udpip.Name = "txt_udpip";
            this.txt_udpip.Size = new System.Drawing.Size(128, 21);
            this.txt_udpip.TabIndex = 3;
            // 
            // but_udpsend
            // 
            this.but_udpsend.Location = new System.Drawing.Point(173, 81);
            this.but_udpsend.Name = "but_udpsend";
            this.but_udpsend.Size = new System.Drawing.Size(75, 23);
            this.but_udpsend.TabIndex = 2;
            this.but_udpsend.Text = "发送";
            this.but_udpsend.UseVisualStyleBackColor = true;
            this.but_udpsend.Click += new System.EventHandler(this.but_udpSend_Click);
            // 
            // but_udpclose
            // 
            this.but_udpclose.Location = new System.Drawing.Point(89, 81);
            this.but_udpclose.Name = "but_udpclose";
            this.but_udpclose.Size = new System.Drawing.Size(75, 23);
            this.but_udpclose.TabIndex = 1;
            this.but_udpclose.Text = "关闭连接";
            this.but_udpclose.UseVisualStyleBackColor = true;
            this.but_udpclose.Click += new System.EventHandler(this.but_udpClose_Click);
            // 
            // but_udpopen
            // 
            this.but_udpopen.Location = new System.Drawing.Point(5, 81);
            this.but_udpopen.Name = "but_udpopen";
            this.but_udpopen.Size = new System.Drawing.Size(75, 23);
            this.but_udpopen.TabIndex = 0;
            this.but_udpopen.Text = "打开连接";
            this.but_udpopen.UseVisualStyleBackColor = true;
            this.but_udpopen.Click += new System.EventHandler(this.but_udpOpen_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button4);
            this.groupBox4.Controls.Add(this.button3);
            this.groupBox4.Controls.Add(this.txt_redisValue);
            this.groupBox4.Controls.Add(this.txt_redisKey);
            this.groupBox4.Controls.Add(this.button2);
            this.groupBox4.Controls.Add(this.button1);
            this.groupBox4.Controls.Add(this.txt_address);
            this.groupBox4.Location = new System.Drawing.Point(521, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(356, 160);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Redis";
            this.groupBox4.Enter += new System.EventHandler(this.groupBox4_Enter);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(275, 122);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "写入";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.redisSet_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(275, 48);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "读取";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.redisRead_Click);
            // 
            // txt_redisValue
            // 
            this.txt_redisValue.Location = new System.Drawing.Point(6, 85);
            this.txt_redisValue.Multiline = true;
            this.txt_redisValue.Name = "txt_redisValue";
            this.txt_redisValue.Size = new System.Drawing.Size(263, 60);
            this.txt_redisValue.TabIndex = 4;
            // 
            // txt_redisKey
            // 
            this.txt_redisKey.Location = new System.Drawing.Point(6, 50);
            this.txt_redisKey.Name = "txt_redisKey";
            this.txt_redisKey.Size = new System.Drawing.Size(263, 21);
            this.txt_redisKey.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(275, 14);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "关闭";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(194, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "连接";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.redisOpen_Click);
            // 
            // txt_address
            // 
            this.txt_address.Location = new System.Drawing.Point(6, 16);
            this.txt_address.Name = "txt_address";
            this.txt_address.Size = new System.Drawing.Size(182, 21);
            this.txt_address.TabIndex = 0;
            this.txt_address.Text = "127.0.0.1:6379";
            // 
            // OtherControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txt_content);
            this.Name = "OtherControl";
            this.Size = new System.Drawing.Size(880, 450);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button but_crc16calculate;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox txt_crcstr;
        private System.Windows.Forms.TextBox txt_content;
        private System.Windows.Forms.Button but_crc16validation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button but_tcpsend;
        private System.Windows.Forms.Button but_tcpclose;
        private System.Windows.Forms.Button but_tcpopen;
        private System.Windows.Forms.TextBox txt_tcpport;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_tcpip;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_tcpmsg;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_udpmsg;
        private System.Windows.Forms.TextBox txt_udpport;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_udpip;
        private System.Windows.Forms.Button but_udpsend;
        private System.Windows.Forms.Button but_udpclose;
        private System.Windows.Forms.Button but_udpopen;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txt_redisValue;
        private System.Windows.Forms.TextBox txt_redisKey;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txt_address;
    }
}
