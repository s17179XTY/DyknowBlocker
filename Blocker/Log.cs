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
        //public Blocker2()
        //{
        //    InitializeComponent();
        //}

        private readonly Blocker1 mainForm;

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
            mainForm.Show();
        }

        public void AppendLog(string logMessage)
        {
            Box.AppendText(logMessage + Environment.NewLine);
        }

        private void box_TextChanged(object sender, EventArgs e)
        {

        }

        private void Blocker2_Load(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private bool isDragging = false;
        private Point dragStart;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
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

        private void panel1_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPos = PointToScreen(e.Location);
                Location = new Point(currentPos.X - dragStart.X, currentPos.Y - dragStart.Y);
            }
        }

        private void panel1_MouseUp_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                Capture = false;
            }
        }
    }
}