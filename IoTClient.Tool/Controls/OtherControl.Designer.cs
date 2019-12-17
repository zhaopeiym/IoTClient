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
            this.SuspendLayout();
            // 
            // but_crc16calculate
            // 
            this.but_crc16calculate.Location = new System.Drawing.Point(183, 11);
            this.but_crc16calculate.Name = "but_crc16calculate";
            this.but_crc16calculate.Size = new System.Drawing.Size(75, 23);
            this.but_crc16calculate.TabIndex = 0;
            this.but_crc16calculate.Text = "CRC16计算";
            this.but_crc16calculate.UseVisualStyleBackColor = true;
            this.but_crc16calculate.Click += new System.EventHandler(this.but_crc16calculate_Click);
            // 
            // txt_crcstr
            // 
            this.txt_crcstr.Location = new System.Drawing.Point(12, 12);
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
            this.but_crc16validation.Location = new System.Drawing.Point(264, 12);
            this.but_crc16validation.Name = "but_crc16validation";
            this.but_crc16validation.Size = new System.Drawing.Size(75, 23);
            this.but_crc16validation.TabIndex = 3;
            this.but_crc16validation.Text = "CRC16验证";
            this.but_crc16validation.UseVisualStyleBackColor = true;
            this.but_crc16validation.Click += new System.EventHandler(this.but_crc16validation_Click);
            // 
            // OtherControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.but_crc16validation);
            this.Controls.Add(this.txt_content);
            this.Controls.Add(this.txt_crcstr);
            this.Controls.Add(this.but_crc16calculate);
            this.Name = "OtherControl";
            this.Size = new System.Drawing.Size(880, 450);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button but_crc16calculate;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox txt_crcstr;
        private System.Windows.Forms.TextBox txt_content;
        private System.Windows.Forms.Button but_crc16validation;
    }
}
