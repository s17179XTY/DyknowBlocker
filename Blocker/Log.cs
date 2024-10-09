using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Blocker;

namespace Blocker
{
    public partial class Blocker2 : Form
    {
        private readonly Blocker1 mainForm;

        public string Send_log { get; set; }

        public Blocker2(Blocker1 mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        public RichTextBox LogBox
        {
            get { return Box; }
        }

        private void Exit2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Send_log = "0";
            Box.Clear();
            if (mainForm.NotifyIconV.Visible != true)
            {
                mainForm.Show();
            }
        }

        public void AppendLog(string logMessage)
        {
            Box.AppendText(logMessage + Environment.NewLine);
        }

        private void Box_TextChanged(object sender, EventArgs e)
        {

        }

        private void Blocker2_Load(object sender, EventArgs e)
        {
            
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private bool isDragging = false;
        private Point dragStart;

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStart = e.Location;
                Capture = true;
            }
        }

        private void Blocker2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStart = e.Location;
                Capture = true;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }

        private void Blocker2_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPos = PointToScreen(e.Location);
                Location = new Point(currentPos.X - dragStart.X, currentPos.Y - dragStart.Y);
            }
        }

        private void Blocker2_MouseUp_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                Capture = false;
            }
        }

        private void Panel1_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPos = PointToScreen(e.Location);
                Location = new Point(currentPos.X - dragStart.X, currentPos.Y - dragStart.Y);
            }
        }

        private void Panel1_MouseUp_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                Capture = false;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}