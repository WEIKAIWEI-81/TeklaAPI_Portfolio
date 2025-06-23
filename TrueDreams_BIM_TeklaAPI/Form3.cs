using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TrueDreams_BIM_TeklaAPI
{
    

    public partial class Form3 : Form
    {

        public Form3()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void newtext(string n1)
        {
            richTextBox1.AppendText(n1 + "\n");

        }
    }
}
