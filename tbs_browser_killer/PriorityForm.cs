using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tbs_browser_killer
{
    public partial class PriorityForm : Form
    {
        public PriorityForm()
        {
            InitializeComponent();
        }

        public string SelectedValue = "";

        private void button1_Click(object sender, EventArgs e)
        {
            SelectedValue = "High";
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SelectedValue = "RealTime";
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SelectedValue = "Cancel";
            this.Close();
        }
    }
}
