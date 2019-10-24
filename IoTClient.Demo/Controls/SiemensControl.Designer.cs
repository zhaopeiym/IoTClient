namespace IoTClient.Demo
{
    partial class SiemensControl
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.but_read = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_address = new System.Windows.Forms.TextBox();
            this.but_write = new System.Windows.Forms.Button();
            this.txt_value = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rd_float = new System.Windows.Forms.RadioButton();
            this.rd_double = new System.Windows.Forms.RadioButton();
            this.rd_short = new System.Windows.Forms.RadioButton();
            this.rd_bit = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_port = new System.Windows.Forms.TextBox();
            this.rd_ulong = new System.Windows.Forms.RadioButton();
            this.rd_ushort = new System.Windows.Forms.RadioButton();
            this.rd_long = new System.Windows.Forms.RadioButton();
            this.rd_int = new System.Windows.Forms.RadioButton();
            this.rd_uint = new System.Windows.Forms.RadioButton();
            this.but_server = new System.Windows.Forms.Button();
            this.but_open = new System.Windows.Forms.Button();
            this.txt_ip = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.txt_content = new System.Windows.Forms.TextBox();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // but_read
            // 
            this.but_read.Location = new System.Drawing.Point(178, 14);
            this.but_read.Name = "but_read";
            this.but_read.Size = new System.Drawing.Size(75, 23);
            this.but_read.TabIndex = 7;
            this.but_read.Text = "读取";
            this.but_read.UseVisualStyleBackColor = true;
            this.but_read.Click += new System.EventHandler(this.but_read_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "地址";
            // 
            // txt_address
            // 
            this.txt_address.Location = new System.Drawing.Point(42, 16);
            this.txt_address.Name = "txt_address";
            this.txt_address.Size = new System.Drawing.Size(121, 21);
            this.txt_address.TabIndex = 9;
            // 
            // but_write
            // 
            this.but_write.Location = new System.Drawing.Point(451, 14);
            this.but_write.Name = "but_write";
            this.but_write.Size = new System.Drawing.Size(75, 23);
            this.but_write.TabIndex = 10;
            this.but_write.Text = "写入";
            this.but_write.UseVisualStyleBackColor = true;
            this.but_write.Click += new System.EventHandler(this.but_write_Click);
            // 
            // txt_value
            // 
            this.txt_value.Location = new System.Drawing.Point(335, 15);
            this.txt_value.Name = "txt_value";
            this.txt_value.Size = new System.Drawing.Size(100, 21);
            this.txt_value.TabIndex = 12;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox1);
            this.groupBox3.Controls.Add(this.but_read);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.txt_address);
            this.groupBox3.Controls.Add(this.but_write);
            this.groupBox3.Controls.Add(this.txt_value);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(13, 111);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(855, 47);
            this.groupBox3.TabIndex = 34;
            this.groupBox3.TabStop = false;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(776, 17);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(72, 16);
            this.checkBox1.TabIndex = 13;
            this.checkBox1.Text = "显示报文";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(312, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "值";
            // 
            // rd_float
            // 
            this.rd_float.AutoSize = true;
            this.rd_float.Checked = true;
            this.rd_float.Location = new System.Drawing.Point(436, 23);
            this.rd_float.Name = "rd_float";
            this.rd_float.Size = new System.Drawing.Size(53, 16);
            this.rd_float.TabIndex = 22;
            this.rd_float.TabStop = true;
            this.rd_float.Text = "float";
            this.rd_float.UseVisualStyleBackColor = true;
            // 
            // rd_double
            // 
            this.rd_double.AutoSize = true;
            this.rd_double.Location = new System.Drawing.Point(495, 23);
            this.rd_double.Name = "rd_double";
            this.rd_double.Size = new System.Drawing.Size(59, 16);
            this.rd_double.TabIndex = 23;
            this.rd_double.Text = "double";
            this.rd_double.UseVisualStyleBackColor = true;
            // 
            // rd_short
            // 
            this.rd_short.AutoSize = true;
            this.rd_short.Location = new System.Drawing.Point(70, 23);
            this.rd_short.Name = "rd_short";
            this.rd_short.Size = new System.Drawing.Size(53, 16);
            this.rd_short.TabIndex = 6;
            this.rd_short.Text = "short";
            this.rd_short.UseVisualStyleBackColor = true;
            // 
            // rd_bit
            // 
            this.rd_bit.AutoSize = true;
            this.rd_bit.Location = new System.Drawing.Point(11, 23);
            this.rd_bit.Name = "rd_bit";
            this.rd_bit.Size = new System.Drawing.Size(41, 16);
            this.rd_bit.TabIndex = 16;
            this.rd_bit.Text = "bit";
            this.rd_bit.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(148, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "端口";
            // 
            // txt_port
            // 
            this.txt_port.Location = new System.Drawing.Point(180, 20);
            this.txt_port.Name = "txt_port";
            this.txt_port.Size = new System.Drawing.Size(39, 21);
            this.txt_port.TabIndex = 5;
            this.txt_port.Text = "102";
            // 
            // rd_ulong
            // 
            this.rd_ulong.AutoSize = true;
            this.rd_ulong.Location = new System.Drawing.Point(374, 23);
            this.rd_ulong.Name = "rd_ulong";
            this.rd_ulong.Size = new System.Drawing.Size(53, 16);
            this.rd_ulong.TabIndex = 21;
            this.rd_ulong.Text = "ulong";
            this.rd_ulong.UseVisualStyleBackColor = true;
            // 
            // rd_ushort
            // 
            this.rd_ushort.AutoSize = true;
            this.rd_ushort.Location = new System.Drawing.Point(129, 23);
            this.rd_ushort.Name = "rd_ushort";
            this.rd_ushort.Size = new System.Drawing.Size(59, 16);
            this.rd_ushort.TabIndex = 17;
            this.rd_ushort.Text = "ushort";
            this.rd_ushort.UseVisualStyleBackColor = true;
            // 
            // rd_long
            // 
            this.rd_long.AutoSize = true;
            this.rd_long.Location = new System.Drawing.Point(315, 23);
            this.rd_long.Name = "rd_long";
            this.rd_long.Size = new System.Drawing.Size(47, 16);
            this.rd_long.TabIndex = 20;
            this.rd_long.Text = "long";
            this.rd_long.UseVisualStyleBackColor = true;
            // 
            // rd_int
            // 
            this.rd_int.AutoSize = true;
            this.rd_int.Location = new System.Drawing.Point(194, 23);
            this.rd_int.Name = "rd_int";
            this.rd_int.Size = new System.Drawing.Size(41, 16);
            this.rd_int.TabIndex = 18;
            this.rd_int.Text = "int";
            this.rd_int.UseVisualStyleBackColor = true;
            // 
            // rd_uint
            // 
            this.rd_uint.AutoSize = true;
            this.rd_uint.Location = new System.Drawing.Point(253, 23);
            this.rd_uint.Name = "rd_uint";
            this.rd_uint.Size = new System.Drawing.Size(47, 16);
            this.rd_uint.TabIndex = 19;
            this.rd_uint.Text = "uint";
            this.rd_uint.UseVisualStyleBackColor = true;
            // 
            // but_server
            // 
            this.but_server.Location = new System.Drawing.Point(658, 18);
            this.but_server.Name = "but_server";
            this.but_server.Size = new System.Drawing.Size(99, 23);
            this.but_server.TabIndex = 0;
            this.but_server.Text = "本地模拟服务";
            this.but_server.UseVisualStyleBackColor = true;
            this.but_server.Click += new System.EventHandler(this.but_server_Click);
            // 
            // but_open
            // 
            this.but_open.Location = new System.Drawing.Point(238, 18);
            this.but_open.Name = "but_open";
            this.but_open.Size = new System.Drawing.Size(75, 23);
            this.but_open.TabIndex = 1;
            this.but_open.Text = "连接";
            this.but_open.UseVisualStyleBackColor = true;
            this.but_open.Click += new System.EventHandler(this.but_open_Click);
            // 
            // txt_ip
            // 
            this.txt_ip.Location = new System.Drawing.Point(29, 19);
            this.txt_ip.Name = "txt_ip";
            this.txt_ip.Size = new System.Drawing.Size(100, 21);
            this.txt_ip.TabIndex = 2;
            this.txt_ip.Text = "127.0.0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "IP";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.but_server);
            this.groupBox2.Controls.Add(this.but_open);
            this.groupBox2.Controls.Add(this.txt_ip);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txt_port);
            this.groupBox2.Location = new System.Drawing.Point(13, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(855, 55);
            this.groupBox2.TabIndex = 33;
            this.groupBox2.TabStop = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(763, 18);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "关闭服务";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(319, 18);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "关闭";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.rd_float);
            this.groupBox1.Controls.Add(this.rd_double);
            this.groupBox1.Controls.Add(this.rd_short);
            this.groupBox1.Controls.Add(this.rd_bit);
            this.groupBox1.Controls.Add(this.rd_ulong);
            this.groupBox1.Controls.Add(this.rd_ushort);
            this.groupBox1.Controls.Add(this.rd_long);
            this.groupBox1.Controls.Add(this.rd_int);
            this.groupBox1.Controls.Add(this.rd_uint);
            this.groupBox1.Location = new System.Drawing.Point(13, 60);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(855, 52);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(762, 17);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 24;
            this.button3.Text = "清空数据";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txt_content
            // 
            this.txt_content.Location = new System.Drawing.Point(13, 165);
            this.txt_content.Multiline = true;
            this.txt_content.Name = "txt_content";
            this.txt_content.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_content.Size = new System.Drawing.Size(855, 272);
            this.txt_content.TabIndex = 31;
            // 
            // SiemensControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txt_content);
            this.Name = "SiemensControl";
            this.Size = new System.Drawing.Size(880, 450);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button but_read;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_address;
        private System.Windows.Forms.Button but_write;
        private System.Windows.Forms.TextBox txt_value;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rd_float;
        private System.Windows.Forms.RadioButton rd_double;
        private System.Windows.Forms.RadioButton rd_short;
        private System.Windows.Forms.RadioButton rd_bit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_port;
        private System.Windows.Forms.RadioButton rd_ulong;
        private System.Windows.Forms.RadioButton rd_ushort;
        private System.Windows.Forms.RadioButton rd_long;
        private System.Windows.Forms.RadioButton rd_int;
        private System.Windows.Forms.RadioButton rd_uint;
        private System.Windows.Forms.Button but_server;
        private System.Windows.Forms.Button but_open;
        private System.Windows.Forms.TextBox txt_ip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_content;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}
