using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace text_mining
{
    public partial class Form4 : Form
    {
        
        public Form4()
        {
            InitializeComponent();
            if (!File.Exists("settings.xml"))
            {
                setting = new Settings();
                textBox1.Text = setting.fioup;
                textBox2.Text = setting.statusup;
                textBox3.Text = setting.type;
                textBox4.Text = setting.title;
                textBox5.Text = setting.fiodown;
                textBox6.Text = setting.statusdown;
            }
            else
            {
                XmlSerializer formatter = new XmlSerializer(typeof(Settings));
                using (FileStream fs = new FileStream("settings.xml", FileMode.Open))
                {
                    setting = (Settings)formatter.Deserialize(fs);
                    textBox1.Text = setting.fioup;
                    textBox2.Text = setting.statusup;
                    textBox3.Text = setting.type;
                    textBox4.Text = setting.title;
                    textBox5.Text = setting.fiodown;
                    textBox6.Text = setting.statusdown;
                    File.Delete("settings.xml");
                }

            }
        }

         Settings setting = null;
        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(Settings));

            if ( (textBox1.Text==null) || (textBox2.Text == null) || (textBox3.Text == null) || (textBox4.Text == null) || (textBox5.Text == null) || (textBox6.Text == null) )
            {
                MessageBox.Show("Некоторые поля пустые. Заполните их", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
                using (FileStream fs = new FileStream("settings.xml", FileMode.CreateNew))
                {
                    formatter.Serialize(fs, setting);
                }
            }

        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Все несохраненные изменения будут утеряны. Выйти?", "Выход", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                return;
            }
            else
                e.Cancel = true;
        }
    }
}
