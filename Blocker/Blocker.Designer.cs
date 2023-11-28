namespace Blocker
{
    partial class Blocker1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Blocker1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.Minimize = new System.Windows.Forms.Button();
            this.Exit = new System.Windows.Forms.Button();
            this.button = new System.Windows.Forms.Button();
            this.Minimize1 = new System.Windows.Forms.Button();
            this.Exit1 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.OpenLog = new System.Windows.Forms.Button();
            this.ButtonClick = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.Minimize);
            this.panel1.Controls.Add(this.Exit);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(134, 32);
            this.panel1.TabIndex = 0;
            // 
            // Minimize
            // 
            this.Minimize.Font = new System.Drawing.Font("Segoe UI Variable Display", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Minimize.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Minimize.Location = new System.Drawing.Point(85, 3);
            this.Minimize.Name = "Minimize";
            this.Minimize.Size = new System.Drawing.Size(22, 23);
            this.Minimize.TabIndex = 2;
            this.Minimize.Text = "_";
            this.Minimize.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Minimize.UseVisualStyleBackColor = true;
            // 
            // Exit
            // 
            this.Exit.Font = new System.Drawing.Font("Segoe UI Variable Display", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Exit.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.Exit.Location = new System.Drawing.Point(107, 3);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(22, 23);
            this.Exit.TabIndex = 1;
            this.Exit.Text = "X";
            this.Exit.UseVisualStyleBackColor = true;
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // button
            // 
            this.button.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F);
            this.button.Location = new System.Drawing.Point(12, 86);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(107, 29);
            this.button.TabIndex = 2;
            this.button.Text = "Pause";
            this.button.UseVisualStyleBackColor = true;
            // 
            // Minimize1
            // 
            this.Minimize1.Font = new System.Drawing.Font("Segoe UI Variable Display", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Minimize1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Minimize1.Location = new System.Drawing.Point(136, 3);
            this.Minimize1.Name = "Minimize1";
            this.Minimize1.Size = new System.Drawing.Size(22, 23);
            this.Minimize1.TabIndex = 2;
            this.Minimize1.Text = "_";
            this.Minimize1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Minimize1.UseVisualStyleBackColor = true;
            this.Minimize1.Click += new System.EventHandler(this.Minimize1_Click);
            // 
            // Exit1
            // 
            this.Exit1.Font = new System.Drawing.Font("Segoe UI Variable Display", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Exit1.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.Exit1.Location = new System.Drawing.Point(158, 3);
            this.Exit1.Name = "Exit1";
            this.Exit1.Size = new System.Drawing.Size(22, 23);
            this.Exit1.TabIndex = 1;
            this.Exit1.Text = "X";
            this.Exit1.UseVisualStyleBackColor = true;
            this.Exit1.Click += new System.EventHandler(this.button2_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.Minimize1);
            this.panel2.Controls.Add(this.Exit1);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(187, 32);
            this.panel2.TabIndex = 2;
            this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseDown);
            // 
            // OpenLog
            // 
            this.OpenLog.BackColor = System.Drawing.Color.White;
            this.OpenLog.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F);
            this.OpenLog.Location = new System.Drawing.Point(27, 38);
            this.OpenLog.Name = "OpenLog";
            this.OpenLog.Size = new System.Drawing.Size(131, 31);
            this.OpenLog.TabIndex = 3;
            this.OpenLog.Text = "OpenLog";
            this.OpenLog.UseVisualStyleBackColor = false;
            this.OpenLog.Click += new System.EventHandler(this.OpenLog_Click);
            // 
            // ButtonClick
            // 
            this.ButtonClick.BackColor = System.Drawing.Color.White;
            this.ButtonClick.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F);
            this.ButtonClick.Location = new System.Drawing.Point(27, 86);
            this.ButtonClick.Name = "ButtonClick";
            this.ButtonClick.Size = new System.Drawing.Size(131, 31);
            this.ButtonClick.TabIndex = 4;
            this.ButtonClick.Text = "Pause";
            this.ButtonClick.UseVisualStyleBackColor = false;
            this.ButtonClick.Click += new System.EventHandler(this.Pause_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Blocker";
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.logToolStripMenuItem.Text = "Log";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.logToolStripMenuItem_Click);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.pauseToolStripMenuItem.Text = "Pause";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.logToolStripMenuItem,
            this.pauseToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowItemToolTips = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(108, 92);
            this.contextMenuStrip1.MouseHover += new System.EventHandler(this.contextMenuStrip1_MouseHover);
            // 
            // Blocker1
            // 
            this.ClientSize = new System.Drawing.Size(184, 143);
            this.Controls.Add(this.ButtonClick);
            this.Controls.Add(this.OpenLog);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Blocker1";
            this.Load += new System.EventHandler(this.Log_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Blocker1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Blocker1_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Blocker1_MouseUp);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Minimize;
        private System.Windows.Forms.Button Exit;
        private System.Windows.Forms.Button button;
        private System.Windows.Forms.Button Minimize1;
        private System.Windows.Forms.Button Exit1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button OpenLog;
        private System.Windows.Forms.Button ButtonClick;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}

