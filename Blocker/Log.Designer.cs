namespace Blocker
{
    partial class Blocker2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Blocker2));
            this.Exit2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Box = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Exit2
            // 
            this.Exit2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Exit2.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.Exit2.Location = new System.Drawing.Point(353, 3);
            this.Exit2.Name = "Exit2";
            this.Exit2.Size = new System.Drawing.Size(22, 23);
            this.Exit2.TabIndex = 1;
            this.Exit2.Text = "X";
            this.Exit2.UseVisualStyleBackColor = true;
            this.Exit2.Click += new System.EventHandler(this.Exit2_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.Exit2);
            this.panel1.Location = new System.Drawing.Point(-4, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(381, 32);
            this.panel1.TabIndex = 1;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseMove_1);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseUp_1);
            // 
            // Box
            // 
            this.Box.Location = new System.Drawing.Point(12, 32);
            this.Box.Name = "Box";
            this.Box.Size = new System.Drawing.Size(349, 252);
            this.Box.TabIndex = 3;
            this.Box.Text = "";
            this.Box.TextChanged += new System.EventHandler(this.Box_TextChanged);
            // 
            // Blocker2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(373, 296);
            this.Controls.Add(this.Box);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Blocker2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Log";
            this.Load += new System.EventHandler(this.Blocker2_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Blocker2_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Blocker2_MouseMove_1);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Blocker2_MouseUp_1);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Exit2;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.RichTextBox Box;
    }
}

