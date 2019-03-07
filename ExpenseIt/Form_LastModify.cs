using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATA
{
    public partial class Form_LastModify : Form
    {
        public Form_LastModify(string fileName, string date)
        {
            InitializeComponent();
            label1.Text = fileName;
            label2.Text = date;
            this.Width = label1.Width + 100;
        }

    }
}
