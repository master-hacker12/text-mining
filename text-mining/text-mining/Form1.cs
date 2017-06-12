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
using EP.Text;
using EP.Semantix;
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
            timer1.Enabled = true;
            label1.Text = " Обычные анализаторы (по умолчанию)";
            label1.Text += '\n';
            Processor p = new Processor();
            int i = 0;
            foreach (Analyzer a in p.Analyzers)
            {
                i++;
                label1.Text += i.ToString() + '.';
                label1.Text += a.ToString();
                label1.Text += '\n';
            }

        }

        Processor processor = null;
        Form2 f2 = null;


        private void Form1_Load(object sender, EventArgs e)
        {
            EP.Text.Morphology.UnloadLanguages(MorphLang.EN);
            EP.Text.Morphology.UnloadLanguages(MorphLang.UA);
            deleteOfList.Enabled = false;
            analizeDocument.Enabled = false;
            
        }

        private void addFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog OPF = new OpenFileDialog();
            OPF.Multiselect = true;
            OPF.Filter = "Документы Word (*.doc*)|*.doc*";
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
                if (filesDoc.Length==0)
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
            }
            timer1.Enabled = true;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            deleteOfList.Enabled = true;
            analizeDocument.Enabled = true;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (listBox1.SelectedIndex == -1)
            {
                deleteOfList.Enabled = false;
                analizeDocument.Enabled = false;
            }

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

        private void analizeDocument_Click(object sender, EventArgs e)
        {
             if (listBox1.SelectedIndex<0)
            {
                MessageBox.Show("Выберете файл!", "Ошибка файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Word.Application app = new Word.Application();
            var document = app.Documents.Open(listBox1.SelectedItem,Visible: false);
            var range = document.Content;
            string str = range.Text;
            document.Close();
            app.Quit();

            if (!checkBox2.Checked && !checkBox3.Checked && !checkBox4.Checked)
                processor = new Processor();
            else
            {
                string type = null;
                if (checkBox2.Checked)
                    type = "SEMANTIC";
                if (checkBox3.Checked)
                    type = "BUSINESS";
                if (checkBox4.Checked)
                    type = "TITLEPAGE";
                if (checkBox2.Checked && checkBox3.Checked)
                    type = "SEMANTIC,BUSINESS";
                if (checkBox2.Checked && checkBox4.Checked)
                    type = "SEMANTIC,TITLEPAGE";
                if (checkBox3.Checked && checkBox4.Checked)
                    type = "BUSINESS,TITLEPAGE";
                if (checkBox2.Checked && checkBox3.Checked && checkBox4.Checked)
                    type = "SEMANTIC,BUSINESS,TITLEPAGE";
                processor = new Processor(type);
            }  
            f2 = new Form2();

         
                f2.Visible = true;
            bool result = f2.ProcessAnalize(ref str, ref processor,getNameFile(listBox1.SelectedItem.ToString()));
            if (!result)
                f2.Visible = false;
            f2 = null;
           
            

        }

        public string getNameFile(string path)
        {
            string[] result = path.Split('\\');
            return result[result.Length-1];

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Visible = true;
            f2.importResult();
          
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.Visible = true;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBox1_MouseHover(object sender, EventArgs e)
        {
            
        }
    }
}
