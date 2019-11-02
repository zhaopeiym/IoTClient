namespace IoTClient.Tool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IndexForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.ModBusTcp = new System.Windows.Forms.TabPage();
            this.modBusTcpControl1 = new IoTClient.Tool.ModBusTcpControl();
            this.ModBusRtu = new System.Windows.Forms.TabPage();
            this.Siemens = new System.Windows.Forms.TabPage();
            this.BACnet = new System.Windows.Forms.TabPage();
            this.Ports = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemBlogPath = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.ModBusTcp.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.ModBusTcp);
            this.tabControl1.Controls.Add(this.ModBusRtu);
            this.tabControl1.Controls.Add(this.Siemens);
            this.tabControl1.Controls.Add(this.BACnet);
            this.tabControl1.Controls.Add(this.Ports);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(894, 474);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // ModBusTcp
            // 
            this.ModBusTcp.Controls.Add(this.modBusTcpControl1);
            this.ModBusTcp.Location = new System.Drawing.Point(4, 22);
            this.ModBusTcp.Name = "ModBusTcp";
            this.ModBusTcp.Padding = new System.Windows.Forms.Padding(3);
            this.ModBusTcp.Size = new System.Drawing.Size(886, 448);
            this.ModBusTcp.TabIndex = 0;
            this.ModBusTcp.Text = "ModBusTcp";
            this.ModBusTcp.UseVisualStyleBackColor = true;
            // 
            // modBusTcpControl1
            // 
            this.modBusTcpControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modBusTcpControl1.Location = new System.Drawing.Point(3, 3);
            this.modBusTcpControl1.Name = "modBusTcpControl1";
            this.modBusTcpControl1.Size = new System.Drawing.Size(880, 442);
            this.modBusTcpControl1.TabIndex = 0;
            // 
            // ModBusRtu
            // 
            this.ModBusRtu.Location = new System.Drawing.Point(4, 22);
            this.ModBusRtu.Name = "ModBusRtu";
            this.ModBusRtu.Padding = new System.Windows.Forms.Padding(3);
            this.ModBusRtu.Size = new System.Drawing.Size(886, 448);
            this.ModBusRtu.TabIndex = 4;
            this.ModBusRtu.Text = "ModBusRtu";
            this.ModBusRtu.UseVisualStyleBackColor = true;
            // 
            // Siemens
            // 
            this.Siemens.Location = new System.Drawing.Point(4, 22);
            this.Siemens.Name = "Siemens";
            this.Siemens.Padding = new System.Windows.Forms.Padding(3);
            this.Siemens.Size = new System.Drawing.Size(886, 448);
            this.Siemens.TabIndex = 1;
            this.Siemens.Text = "西门子S7-200Smar";
            this.Siemens.UseVisualStyleBackColor = true;
            // 
            // BACnet
            // 
            this.BACnet.Location = new System.Drawing.Point(4, 22);
            this.BACnet.Name = "BACnet";
            this.BACnet.Padding = new System.Windows.Forms.Padding(3);
            this.BACnet.Size = new System.Drawing.Size(886, 448);
            this.BACnet.TabIndex = 2;
            this.BACnet.Text = " BACnet ";
            this.BACnet.UseVisualStyleBackColor = true;
            // 
            // Ports
            // 
            this.Ports.Location = new System.Drawing.Point(4, 22);
            this.Ports.Name = "Ports";
            this.Ports.Padding = new System.Windows.Forms.Padding(3);
            this.Ports.Size = new System.Drawing.Size(886, 448);
            this.Ports.TabIndex = 3;
            this.Ports.Text = "  串口  ";
            this.Ports.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem5,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItemBlogPath,
            this.toolStripMenuItem4,
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(894, 25);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(104, 21);
            this.toolStripMenuItem5.Text = "擎呐设备云平台";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(68, 21);
            this.toolStripMenuItem2.Text = "开源地址";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(67, 21);
            this.toolStripMenuItem3.Text = "提交Bug";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItemBlogPath
            // 
            this.toolStripMenuItemBlogPath.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItemBlogPath.Name = "toolStripMenuItemBlogPath";
            this.toolStripMenuItemBlogPath.Size = new System.Drawing.Size(68, 21);
            this.toolStripMenuItemBlogPath.Text = "博客地址";
            this.toolStripMenuItemBlogPath.Click += new System.EventHandler(this.toolStripMenuItemBlogPath_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(56, 21);
            this.toolStripMenuItem4.Text = "交流群";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(87, 21);
            this.toolStripMenuItem1.Text = "测试版 0.0.3";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // IndexForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 501);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "IndexForm";
            this.Text = "IoTClient Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IndexForm_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.ModBusTcp.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage ModBusTcp;
        private System.Windows.Forms.TabPage Siemens;
        private ModBusTcpControl modBusTcpControl1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBlogPath;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.TabPage BACnet;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.TabPage Ports;
        private System.Windows.Forms.TabPage ModBusRtu;
    }
}