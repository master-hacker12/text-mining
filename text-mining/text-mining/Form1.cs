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
            toolStripComboBox1.Items.Add("Выделение именованных сущностей");
            toolStripComboBox1.Items.Add("Семантический анализ");
            toolStripComboBox1.Items.Add("Бизнес-объекты");
            toolStripComboBox1.Items.Add("Это заголовок статьи");
            TextType = TextTypes.Ner;
        }

        Processor processor = null;
        Form2 f2 = null;
        #region Типы анализируемого текста
        /// <summary>
        /// Типы анализируемого текста
        /// </summary>
        enum TextTypes
        {
            /// <summary>
            /// Семантический анализ
            /// </summary>
            Semantic,
            /// <summary>
            /// Named entity recognition (выделение именованных сущностей)
            /// </summary>
            Ner,
            /// <summary>
            /// Бизнес-объекты
            /// </summary>
            BusinessObjects,
            /// <summary>
            /// Текст - это заголовок статьи
            /// </summary>
            TitleOfArticle
        }

        TextTypes TextType
        {
            get
            {
                switch (toolStripComboBox1.SelectedIndex)
                {
                    case 0: return TextTypes.Ner;
                    case 1: return TextTypes.Semantic;
                    case 2: return TextTypes.BusinessObjects;
                    case 3: return TextTypes.TitleOfArticle;
                }
                return TextTypes.Ner;
            }
            set
            {
                switch (value)
                {
                    case TextTypes.Ner: toolStripComboBox1.SelectedIndex = 0; break;
                    case TextTypes.Semantic: toolStripComboBox1.SelectedIndex = 1; break;
                    case TextTypes.BusinessObjects: toolStripComboBox1.SelectedIndex = 2; break;
                    case TextTypes.TitleOfArticle: toolStripComboBox1.SelectedIndex = 3; break;
                }
            }
        }

        #endregion




        private void Form1_Load(object sender, EventArgs e)
        {
            EP.Text.Morphology.UnloadLanguages(MorphLang.EN);
            deleteOfList.Enabled = false;
            analizeDocument.Enabled = false;
            
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
            analizeDocument.Enabled = true;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
                timer1.Enabled = false;
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

            processor = new Processor(TextType == TextTypes.TitleOfArticle ? TitlePageAnalyzer.ANALYZER_NAME : // "TITLEPAGE"
                (TextType == TextTypes.BusinessObjects ? BusinessAnalyzer.ANALYZER_NAME : // "BUSINESS" 
                (TextType == TextTypes.Semantic ? SemanticAnalyzer.ANALYZER_NAME : // "SEMANTIC"
                "")));
            f2 = new Form2();
            bool result = f2.ProcessAnalize(ref str, ref processor);
            if (result)
                f2.Visible = true;
            else
            {
                f2.Visible = false;
                f2 = null;
                return;
            }
            

        }
    }
}
