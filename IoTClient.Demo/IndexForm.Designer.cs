namespace IoTClient.Demo
{
    partial class IndexForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.ModBusTcp = new System.Windows.Forms.TabPage();
            this.Siemens = new System.Windows.Forms.TabPage();
            this.modBusTcpControl1 = new IoTClient.Demo.ModBusTcpControl();
            this.tabControl1.SuspendLayout();
            this.ModBusTcp.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.ModBusTcp);
            this.tabControl1.Controls.Add(this.Siemens);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(894, 501);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // ModBusTcp
            // 
            this.ModBusTcp.Controls.Add(this.modBusTcpControl1);
            this.ModBusTcp.Location = new System.Drawing.Point(4, 22);
            this.ModBusTcp.Name = "ModBusTcp";
            this.ModBusTcp.Padding = new System.Windows.Forms.Padding(3);
            this.ModBusTcp.Size = new System.Drawing.Size(886, 475);
            this.ModBusTcp.TabIndex = 0;
            this.ModBusTcp.Text = "ModBusTcp";
            this.ModBusTcp.UseVisualStyleBackColor = true;
            // 
            // Siemens
            // 
            this.Siemens.Location = new System.Drawing.Point(4, 22);
            this.Siemens.Name = "Siemens";
            this.Siemens.Padding = new System.Windows.Forms.Padding(3);
            this.Siemens.Size = new System.Drawing.Size(886, 475);
            this.Siemens.TabIndex = 1;
            this.Siemens.Text = "Siemens";
            this.Siemens.UseVisualStyleBackColor = true;
            // 
            // modBusTcpControl1
            // 
            this.modBusTcpControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modBusTcpControl1.Location = new System.Drawing.Point(3, 3);
            this.modBusTcpControl1.Name = "modBusTcpControl1";
            this.modBusTcpControl1.Size = new System.Drawing.Size(880, 469);
            this.modBusTcpControl1.TabIndex = 0;
            // 
            // IndexForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 501);
            this.Controls.Add(this.tabControl1);
            this.Name = "IndexForm";
            this.Text = "IndexForm";
            this.tabControl1.ResumeLayout(false);
            this.ModBusTcp.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage ModBusTcp;
        private System.Windows.Forms.TabPage Siemens;
        private ModBusTcpControl modBusTcpControl1;
    }
}