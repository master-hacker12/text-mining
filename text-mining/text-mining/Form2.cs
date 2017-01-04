using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EP.Semantix;
using EP;
using EP.Text;

namespace text_mining
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }


        bool m_HideHighlighting;
        int m_MaxTextLengthForShowing = 2000000;
        public bool ProcessAnalize(ref string txt, ref Processor processor)
        {
            try
            {


                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка анализа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
           
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
