namespace IoTClient.Tool.Controls
{
    partial class ModBusAsciiControl
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
            this.cb_portNameSend_server = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_stopBit = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_dataBit = new System.Windows.Forms.TextBox();
            this.cb_portNameSend = new System.Windows.Forms.ComboBox();
            this.but_close = new System.Windows.Forms.Button();
            this.but_server_close = new System.Windows.Forms.Button();
            this.but_server_open = new System.Windows.Forms.Button();
            this.but_open = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_stationNumber = new System.Windows.Forms.TextBox();
            this.rd_ulong = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.rd_short = new System.Windows.Forms.RadioButton();
            this.rd_bit = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.rd_discrete = new System.Windows.Forms.RadioButton();
            this.button6 = new System.Windows.Forms.Button();
            this.rd_float = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_baudRate = new System.Windows.Forms.TextBox();
            this.rd_double = new System.Windows.Forms.RadioButton();
            this.rd_ushort = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.but_sendData = new System.Windows.Forms.Button();
            this.txt_dataPackage = new System.Windows.Forms.TextBox();
            this.chb_show_package = new System.Windows.Forms.CheckBox();
            this.but_read = new System.Windows.Forms.Button();
            this.lab_address = new System.Windows.Forms.Label();
            this.txt_address = new System.Windows.Forms.TextBox();
            this.but_write = new System.Windows.Forms.Button();
            this.txt_value = new System.Windows.Forms.TextBox();
            this.lab_value = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rd_long = new System.Windows.Forms.RadioButton();
            this.rd_int = new System.Windows.Forms.RadioButton();
            this.rd_uint = new System.Windows.Forms.RadioButton();
            this.txt_content = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cb_portNameSend_server
            // 
            this.cb_portNameSend_server.FormattingEnabled = true;
            this.cb_portNameSend_server.Location = new System.Drawing.Point(614, 21);
            this.cb_portNameSend_server.Name = "cb_portNameSend_server";
            this.cb_portNameSend_server.Size = new System.Drawing.Size(47, 20);
            this.cb_portNameSend_server.TabIndex = 24;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(250, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 21;
            this.label7.Text = "停止位";
            // 
            // txt_stopBit
            // 
            this.txt_stopBit.Location = new System.Drawing.Point(292, 19);
            this.txt_stopBit.Name = "txt_stopBit";
            this.txt_stopBit.Size = new System.Drawing.Size(24, 21);
            this.txt_stopBit.TabIndex = 22;
            this.txt_stopBit.Text = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(176, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 19;
            this.label6.Text = "数据位";
            // 
            // txt_dataBit
            // 
            this.txt_dataBit.Location = new System.Drawing.Point(217, 20);
            this.txt_dataBit.Name = "txt_dataBit";
            this.txt_dataBit.Size = new System.Drawing.Size(24, 21);
            this.txt_dataBit.TabIndex = 20;
            this.txt_dataBit.Text = "8";
            // 
            // cb_portNameSend
            // 
            this.cb_portNameSend.FormattingEnabled = true;
            this.cb_portNameSend.Location = new System.Drawing.Point(34, 20);
            this.cb_portNameSend.Name = "cb_portNameSend";
            this.cb_portNameSend.Size = new System.Drawing.Size(47, 20);
            this.cb_portNameSend.TabIndex = 18;
            // 
            // but_close
            // 
            this.but_close.Location = new System.Drawing.Point(466, 19);
            this.but_close.Name = "but_close";
            this.but_close.Size = new System.Drawing.Size(75, 23);
            this.but_close.TabIndex = 17;
            this.but_close.Text = "断开";
            this.but_close.UseVisualStyleBackColor = true;
            // 
            // but_server_close
            // 
            this.but_server_close.Location = new System.Drawing.Point(768, 19);
            this.but_server_close.Name = "but_server_close";
            this.but_server_close.Size = new System.Drawing.Size(75, 23);
            this.but_server_close.TabIndex = 16;
            this.but_server_close.Text = "关闭服务";
            this.but_server_close.UseVisualStyleBackColor = true;
            this.but_server_close.Click += new System.EventHandler(this.but_server_close_Click);
            // 
            // but_server_open
            // 
            this.but_server_open.Location = new System.Drawing.Point(665, 19);
            this.but_server_open.Name = "but_server_open";
            this.but_server_open.Size = new System.Drawing.Size(99, 23);
            this.but_server_open.TabIndex = 0;
            this.but_server_open.Text = "本地模拟服务";
            this.but_server_open.UseVisualStyleBackColor = true;
            this.but_server_open.Click += new System.EventHandler(this.but_server_open_Click);
            // 
            // but_open
            // 
            this.but_open.Location = new System.Drawing.Point(385, 18);
            this.but_open.Name = "but_open";
            this.but_open.Size = new System.Drawing.Size(75, 23);
            this.but_open.TabIndex = 1;
            this.but_open.Text = "连接";
            this.but_open.UseVisualStyleBackColor = true;
            this.but_open.Click += new System.EventHandler(this.but_open_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(324, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 15;
            this.label5.Text = "站号";
            // 
            // txt_stationNumber
            // 
            this.txt_stationNumber.Location = new System.Drawing.Point(353, 19);
            this.txt_stationNumber.Name = "txt_stationNumber";
            this.txt_stationNumber.Size = new System.Drawing.Size(25, 21);
            this.txt_stationNumber.TabIndex = 14;
            this.txt_stationNumber.Text = "1";
            // 
            // rd_ulong
            // 
            this.rd_ulong.AutoSize = true;
            this.rd_ulong.Location = new System.Drawing.Point(424, 23);
            this.rd_ulong.Name = "rd_ulong";
            this.rd_ulong.Size = new System.Drawing.Size(53, 16);
            this.rd_ulong.TabIndex = 21;
            this.rd_ulong.Text = "ulong";
            this.rd_ulong.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(587, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 23;
            this.label8.Text = "端口";
            // 
            // rd_short
            // 
            this.rd_short.AutoSize = true;
            this.rd_short.Checked = true;
            this.rd_short.Location = new System.Drawing.Point(120, 23);
            this.rd_short.Name = "rd_short";
            this.rd_short.Size = new System.Drawing.Size(53, 16);
            this.rd_short.TabIndex = 6;
            this.rd_short.TabStop = true;
            this.rd_short.Text = "short";
            this.rd_short.UseVisualStyleBackColor = true;
            // 
            // rd_bit
            // 
            this.rd_bit.AutoSize = true;
            this.rd_bit.Location = new System.Drawing.Point(11, 23);
            this.rd_bit.Name = "rd_bit";
            this.rd_bit.Size = new System.Drawing.Size(47, 16);
            this.rd_bit.TabIndex = 16;
            this.rd_bit.Text = "线圈";
            this.rd_bit.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(86, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "波特率";
            // 
            // rd_discrete
            // 
            this.rd_discrete.AutoSize = true;
            this.rd_discrete.Location = new System.Drawing.Point(64, 23);
            this.rd_discrete.Name = "rd_discrete";
            this.rd_discrete.Size = new System.Drawing.Size(47, 16);
            this.rd_discrete.TabIndex = 24;
            this.rd_discrete.Text = "离散";
            this.rd_discrete.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(768, 16);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 18;
            this.button6.Text = "清空数据";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // rd_float
            // 
            this.rd_float.AutoSize = true;
            this.rd_float.Location = new System.Drawing.Point(486, 23);
            this.rd_float.Name = "rd_float";
            this.rd_float.Size = new System.Drawing.Size(53, 16);
            this.rd_float.TabIndex = 22;
            this.rd_float.Text = "float";
            this.rd_float.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cb_portNameSend_server);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txt_stopBit);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txt_dataBit);
            this.groupBox2.Controls.Add(this.cb_portNameSend);
            this.groupBox2.Controls.Add(this.but_close);
            this.groupBox2.Controls.Add(this.but_server_close);
            this.groupBox2.Controls.Add(this.but_server_open);
            this.groupBox2.Controls.Add(this.but_open);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txt_stationNumber);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txt_baudRate);
            this.groupBox2.Location = new System.Drawing.Point(13, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(855, 50);
            this.groupBox2.TabIndex = 37;
            this.groupBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "端口";
            // 
            // txt_baudRate
            // 
            this.txt_baudRate.Location = new System.Drawing.Point(127, 19);
            this.txt_baudRate.Name = "txt_baudRate";
            this.txt_baudRate.Size = new System.Drawing.Size(42, 21);
            this.txt_baudRate.TabIndex = 5;
            this.txt_baudRate.Text = "9600";
            // 
            // rd_double
            // 
            this.rd_double.AutoSize = true;
            this.rd_double.Location = new System.Drawing.Point(545, 23);
            this.rd_double.Name = "rd_double";
            this.rd_double.Size = new System.Drawing.Size(59, 16);
            this.rd_double.TabIndex = 23;
            this.rd_double.Text = "double";
            this.rd_double.UseVisualStyleBackColor = true;
            // 
            // rd_ushort
            // 
            this.rd_ushort.AutoSize = true;
            this.rd_ushort.Location = new System.Drawing.Point(179, 23);
            this.rd_ushort.Name = "rd_ushort";
            this.rd_ushort.Size = new System.Drawing.Size(59, 16);
            this.rd_ushort.TabIndex = 17;
            this.rd_ushort.Text = "ushort";
            this.rd_ushort.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.but_sendData);
            this.groupBox3.Controls.Add(this.txt_dataPackage);
            this.groupBox3.Controls.Add(this.chb_show_package);
            this.groupBox3.Controls.Add(this.but_read);
            this.groupBox3.Controls.Add(this.lab_address);
            this.groupBox3.Controls.Add(this.txt_address);
            this.groupBox3.Controls.Add(this.but_write);
            this.groupBox3.Controls.Add(this.txt_value);
            this.groupBox3.Controls.Add(this.lab_value);
            this.groupBox3.Location = new System.Drawing.Point(13, 114);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(855, 47);
            this.groupBox3.TabIndex = 38;
            this.groupBox3.TabStop = false;
            // 
            // but_sendData
            // 
            this.but_sendData.Location = new System.Drawing.Point(616, 20);
            this.but_sendData.Name = "but_sendData";
            this.but_sendData.Size = new System.Drawing.Size(75, 23);
            this.but_sendData.TabIndex = 16;
            this.but_sendData.Text = "发送报文";
            this.but_sendData.UseVisualStyleBackColor = true;
            this.but_sendData.Click += new System.EventHandler(this.but_sendData_Click);
            // 
            // txt_dataPackage
            // 
            this.txt_dataPackage.Location = new System.Drawing.Point(424, 21);
            this.txt_dataPackage.Name = "txt_dataPackage";
            this.txt_dataPackage.Size = new System.Drawing.Size(186, 21);
            this.txt_dataPackage.TabIndex = 17;
            // 
            // chb_show_package
            // 
            this.chb_show_package.AutoSize = true;
            this.chb_show_package.Location = new System.Drawing.Point(771, 19);
            this.chb_show_package.Name = "chb_show_package";
            this.chb_show_package.Size = new System.Drawing.Size(72, 16);
            this.chb_show_package.TabIndex = 13;
            this.chb_show_package.Text = "显示报文";
            this.chb_show_package.UseVisualStyleBackColor = true;
            // 
            // but_read
            // 
            this.but_read.Location = new System.Drawing.Point(133, 18);
            this.but_read.Name = "but_read";
            this.but_read.Size = new System.Drawing.Size(75, 23);
            this.but_read.TabIndex = 7;
            this.but_read.Text = "读取";
            this.but_read.UseVisualStyleBackColor = true;
            this.but_read.Click += new System.EventHandler(this.but_read_Click);
            // 
            // lab_address
            // 
            this.lab_address.AutoSize = true;
            this.lab_address.Location = new System.Drawing.Point(9, 22);
            this.lab_address.Name = "lab_address";
            this.lab_address.Size = new System.Drawing.Size(29, 12);
            this.lab_address.TabIndex = 8;
            this.lab_address.Text = "地址";
            // 
            // txt_address
            // 
            this.txt_address.Location = new System.Drawing.Point(39, 17);
            this.txt_address.Name = "txt_address";
            this.txt_address.Size = new System.Drawing.Size(88, 21);
            this.txt_address.TabIndex = 9;
            // 
            // but_write
            // 
            this.but_write.Location = new System.Drawing.Point(337, 20);
            this.but_write.Name = "but_write";
            this.but_write.Size = new System.Drawing.Size(75, 23);
            this.but_write.TabIndex = 10;
            this.but_write.Text = "写入";
            this.but_write.UseVisualStyleBackColor = true;
            this.but_write.Click += new System.EventHandler(this.but_write_Click);
            // 
            // txt_value
            // 
            this.txt_value.Location = new System.Drawing.Point(256, 20);
            this.txt_value.Name = "txt_value";
            this.txt_value.Size = new System.Drawing.Size(74, 21);
            this.txt_value.TabIndex = 12;
            // 
            // lab_value
            // 
            this.lab_value.AutoSize = true;
            this.lab_value.Location = new System.Drawing.Point(228, 24);
            this.lab_value.Name = "lab_value";
            this.lab_value.Size = new System.Drawing.Size(17, 12);
            this.lab_value.TabIndex = 11;
            this.lab_value.Text = "值";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rd_discrete);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.rd_float);
            this.groupBox1.Controls.Add(this.rd_double);
            this.groupBox1.Controls.Add(this.rd_short);
            this.groupBox1.Controls.Add(this.rd_bit);
            this.groupBox1.Controls.Add(this.rd_ulong);
            this.groupBox1.Controls.Add(this.rd_ushort);
            this.groupBox1.Controls.Add(this.rd_long);
            this.groupBox1.Controls.Add(this.rd_int);
            this.groupBox1.Controls.Add(this.rd_uint);
            this.groupBox1.Location = new System.Drawing.Point(13, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(855, 50);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            // 
            // rd_long
            // 
            this.rd_long.AutoSize = true;
            this.rd_long.Location = new System.Drawing.Point(365, 23);
            this.rd_long.Name = "rd_long";
            this.rd_long.Size = new System.Drawing.Size(47, 16);
            this.rd_long.TabIndex = 20;
            this.rd_long.Text = "long";
            this.rd_long.UseVisualStyleBackColor = true;
            // 
            // rd_int
            // 
            this.rd_int.AutoSize = true;
            this.rd_int.Location = new System.Drawing.Point(244, 23);
            this.rd_int.Name = "rd_int";
            this.rd_int.Size = new System.Drawing.Size(41, 16);
            this.rd_int.TabIndex = 18;
            this.rd_int.Text = "int";
            this.rd_int.UseVisualStyleBackColor = true;
            // 
            // rd_uint
            // 
            this.rd_uint.AutoSize = true;
            this.rd_uint.Location = new System.Drawing.Point(303, 23);
            this.rd_uint.Name = "rd_uint";
            this.rd_uint.Size = new System.Drawing.Size(47, 16);
            this.rd_uint.TabIndex = 19;
            this.rd_uint.Text = "uint";
            this.rd_uint.UseVisualStyleBackColor = true;
            // 
            // txt_content
            // 
            this.txt_content.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_content.Location = new System.Drawing.Point(13, 168);
            this.txt_content.Multiline = true;
            this.txt_content.Name = "txt_content";
            this.txt_content.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_content.Size = new System.Drawing.Size(855, 272);
            this.txt_content.TabIndex = 35;
            // 
            // ModBusAsciiControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txt_content);
            this.Name = "ModBusAsciiControl";
            this.Size = new System.Drawing.Size(880, 450);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cb_portNameSend_server;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_stopBit;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_dataBit;
        private System.Windows.Forms.ComboBox cb_portNameSend;
        private System.Windows.Forms.Button but_close;
        private System.Windows.Forms.Button but_server_close;
        private System.Windows.Forms.Button but_server_open;
        private System.Windows.Forms.Button but_open;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_stationNumber;
        private System.Windows.Forms.RadioButton rd_ulong;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton rd_short;
        private System.Windows.Forms.RadioButton rd_bit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rd_discrete;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.RadioButton rd_float;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_baudRate;
        private System.Windows.Forms.RadioButton rd_double;
        private System.Windows.Forms.RadioButton rd_ushort;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button but_sendData;
        private System.Windows.Forms.TextBox txt_dataPackage;
        private System.Windows.Forms.CheckBox chb_show_package;
        private System.Windows.Forms.Button but_read;
        private System.Windows.Forms.Label lab_address;
        private System.Windows.Forms.TextBox txt_address;
        private System.Windows.Forms.Button but_write;
        private System.Windows.Forms.TextBox txt_value;
        private System.Windows.Forms.Label lab_value;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rd_long;
        private System.Windows.Forms.RadioButton rd_int;
        private System.Windows.Forms.RadioButton rd_uint;
        private System.Windows.Forms.TextBox txt_content;
    }
}
