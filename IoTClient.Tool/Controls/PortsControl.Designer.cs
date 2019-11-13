namespace IoTClient.Tool.Controls
{
    partial class PortsControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_encoding = new System.Windows.Forms.ComboBox();
            this.cb_parity = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cb_stopBit = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cb_dataBit = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cb_baudRate = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cb_portNameSend = new System.Windows.Forms.ComboBox();
            this.but_open = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.but_close = new System.Windows.Forms.Button();
            this.txt_content = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txt_msg = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_encoding);
            this.groupBox1.Controls.Add(this.cb_parity);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cb_stopBit);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cb_dataBit);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cb_baudRate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cb_portNameSend);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(279, 102);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "设置";
            // 
            // cb_encoding
            // 
            this.cb_encoding.FormattingEnabled = true;
            this.cb_encoding.Location = new System.Drawing.Point(190, 72);
            this.cb_encoding.Name = "cb_encoding";
            this.cb_encoding.Size = new System.Drawing.Size(83, 20);
            this.cb_encoding.TabIndex = 12;
            this.cb_encoding.SelectedValueChanged += new System.EventHandler(this.cb_encoding_SelectedValueChanged);
            // 
            // cb_parity
            // 
            this.cb_parity.FormattingEnabled = true;
            this.cb_parity.Location = new System.Drawing.Point(190, 46);
            this.cb_parity.Name = "cb_parity";
            this.cb_parity.Size = new System.Drawing.Size(83, 20);
            this.cb_parity.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(148, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "编  码";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(148, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "校验位";
            // 
            // cb_stopBit
            // 
            this.cb_stopBit.FormattingEnabled = true;
            this.cb_stopBit.Location = new System.Drawing.Point(190, 20);
            this.cb_stopBit.Name = "cb_stopBit";
            this.cb_stopBit.Size = new System.Drawing.Size(83, 20);
            this.cb_stopBit.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(148, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "停止位";
            // 
            // cb_dataBit
            // 
            this.cb_dataBit.FormattingEnabled = true;
            this.cb_dataBit.Location = new System.Drawing.Point(48, 72);
            this.cb_dataBit.Name = "cb_dataBit";
            this.cb_dataBit.Size = new System.Drawing.Size(83, 20);
            this.cb_dataBit.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "数据位";
            // 
            // cb_baudRate
            // 
            this.cb_baudRate.FormattingEnabled = true;
            this.cb_baudRate.Location = new System.Drawing.Point(48, 46);
            this.cb_baudRate.Name = "cb_baudRate";
            this.cb_baudRate.Size = new System.Drawing.Size(83, 20);
            this.cb_baudRate.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "波特率";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "端  口";
            // 
            // cb_portNameSend
            // 
            this.cb_portNameSend.FormattingEnabled = true;
            this.cb_portNameSend.Location = new System.Drawing.Point(48, 20);
            this.cb_portNameSend.Name = "cb_portNameSend";
            this.cb_portNameSend.Size = new System.Drawing.Size(83, 20);
            this.cb_portNameSend.TabIndex = 0;
            // 
            // but_open
            // 
            this.but_open.Location = new System.Drawing.Point(30, 19);
            this.but_open.Name = "but_open";
            this.but_open.Size = new System.Drawing.Size(75, 23);
            this.but_open.TabIndex = 11;
            this.but_open.Text = "打开串口";
            this.but_open.UseVisualStyleBackColor = true;
            this.but_open.Click += new System.EventHandler(this.butopen_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.but_close);
            this.groupBox2.Controls.Add(this.but_open);
            this.groupBox2.Location = new System.Drawing.Point(288, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(589, 54);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            // 
            // but_close
            // 
            this.but_close.Location = new System.Drawing.Point(111, 19);
            this.but_close.Name = "but_close";
            this.but_close.Size = new System.Drawing.Size(75, 23);
            this.but_close.TabIndex = 12;
            this.but_close.Text = "关闭串口";
            this.but_close.UseVisualStyleBackColor = true;
            this.but_close.Click += new System.EventHandler(this.butclose_Click);
            // 
            // txt_content
            // 
            this.txt_content.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_content.Location = new System.Drawing.Point(3, 111);
            this.txt_content.Multiline = true;
            this.txt_content.Name = "txt_content";
            this.txt_content.Size = new System.Drawing.Size(874, 327);
            this.txt_content.TabIndex = 13;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txt_msg);
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Location = new System.Drawing.Point(288, 56);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(589, 49);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            // 
            // txt_msg
            // 
            this.txt_msg.Location = new System.Drawing.Point(30, 18);
            this.txt_msg.Name = "txt_msg";
            this.txt_msg.Size = new System.Drawing.Size(240, 21);
            this.txt_msg.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(276, 16);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "发送";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.butwrite_Click);
            // 
            // PortsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.txt_content);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "PortsControl";
            this.Size = new System.Drawing.Size(880, 450);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cb_portNameSend;
        private System.Windows.Forms.ComboBox cb_parity;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cb_stopBit;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cb_dataBit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cb_baudRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button but_open;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txt_content;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txt_msg;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button but_close;
        private System.Windows.Forms.ComboBox cb_encoding;
        private System.Windows.Forms.Label label6;
    }
}
