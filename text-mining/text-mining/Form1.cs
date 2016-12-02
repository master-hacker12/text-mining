using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EP;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
namespace text_mining
{
    public partial class Form1 : Form
    {
        

        private Word.Application wordapp;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EP.ProcessorService.Initialize();
            deleteOfList.Enabled = false;
        }

        private void addFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog OPF = new OpenFileDialog();
            OPF.Multiselect = true;
            OPF.Filter = "Документы Word (*.doc*)|*.doc*|PDF files (*.pdf)|*.pdf";
            if (OPF.ShowDialog()==DialogResult.OK)
            {
                for (int i = 0;i<OPF.FileNames.Length;i++)
                {
                    listBox1.Items.Add(OPF.FileNames[i]);
                }
               
            }
            timer1.Enabled = true;
        }

        private void addFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();

            if (FBD.ShowDialog()==DialogResult.OK)
            {
                string[] filesDoc = Directory.GetFiles(FBD.SelectedPath,"*.doc*");
                string[] filesPdf = Directory.GetFiles(FBD.SelectedPath, "*.pdf");
                if ((filesDoc.Length==0) && (filesPdf.Length==0))
                {
                    MessageBox.Show("В данной папке документы не найдены", "Поиск", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (filesDoc.Length != 0)
                {
                    for (int i = 0; i < filesDoc.Length; i++)
                    {
                        listBox1.Items.Add(filesDoc[i]);
                    }
                }
                if (filesPdf.Length != 0)
                {
                    for (int i = 0; i < filesPdf.Length; i++)
                    {
                        listBox1.Items.Add(filesPdf[i]);
                    }
                }
            }
            timer1.Enabled = true;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            deleteOfList.Enabled = true;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
                timer1.Enabled = false;
            if (listBox1.SelectedIndex == -1)
                deleteOfList.Enabled = false;
        }

        private void deleteOfList_Click(object sender, EventArgs e)
        {
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            listBox1.Update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }
    }
}
