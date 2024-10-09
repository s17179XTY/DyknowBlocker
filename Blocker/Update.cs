using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blocker
{
    public partial class Bar : Form
    {
        public Bar()
        {
            InitializeComponent();
        }
        public ProgressBar progressBar
        {
            get { return progressBar; }
            set { progressBar = value; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
