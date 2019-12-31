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
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // but_crc16calculate
            // 
            this.but_crc16calculate.Location = new System.Drawing.Point(177, 14);
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
            this.txt_crcstr.Size = new System.Drawing.Size(165, 21);
            this.txt_crcstr.TabIndex = 1;
            // 
            // txt_content
            // 
            this.txt_content.Location = new System.Drawing.Point(3, 169);
            this.txt_content.Multiline = true;
            this.txt_content.Name = "txt_content";
            this.txt_content.Size = new System.Drawing.Size(874, 268);
            this.txt_content.TabIndex = 2;
            // 
            // but_crc16validation
            // 
            this.but_crc16validation.Location = new System.Drawing.Point(258, 15);
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
            this.groupBox1.Size = new System.Drawing.Size(346, 44);
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
            this.groupBox2.Location = new System.Drawing.Point(356, 3);
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
            this.groupBox3.Controls.Add(this.textBox4);
            this.groupBox3.Controls.Add(this.textBox5);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.textBox6);
            this.groupBox3.Controls.Add(this.button4);
            this.groupBox3.Controls.Add(this.button5);
            this.groupBox3.Controls.Add(this.button6);
            this.groupBox3.Location = new System.Drawing.Point(616, 3);
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
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(33, 41);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(214, 34);
            this.textBox4.TabIndex = 7;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(193, 14);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(53, 21);
            this.textBox5.TabIndex = 6;
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
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(33, 14);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(128, 21);
            this.textBox6.TabIndex = 3;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(173, 81);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 2;
            this.button4.Text = "发送";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(89, 81);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 1;
            this.button5.Text = "关闭连接";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(5, 81);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 0;
            this.button6.Text = "打开连接";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // OtherControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
    }
}
