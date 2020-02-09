using IndustrialParkHans.BlockTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustrialParkHans
{
    public partial class AddSectionDialog : Form
    {
        public AddSectionDialog()
        {
            InitializeComponent();

            foreach (Section s in Enum.GetValues(typeof(Section)))
                comboBox1.Items.Add(s);
        }

        public Section section;
        public bool OKed;

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            section = (Section)comboBox1.SelectedItem;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            OKed = true;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            OKed = false;
            Close();
        }
    }
}
