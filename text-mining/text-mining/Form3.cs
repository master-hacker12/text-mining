using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace text_mining
{
    public partial class TableForm : Form
    {
        Person[] persona = null;
        int pos = 0;
        bool edit = false;
        public TableForm()
        {
            InitializeComponent();
            timer1.Enabled = true;
        }
        bool dsp;
        public void UpdateTable (Person[] data, bool DSP)
        {
            if (table.Rows.Count > 0)
                table.Rows.Clear();
            table.Rows.Add(data.Length);
            for (int i =0;i<table.Rows.Count;i++)
            {
                table.Rows[i].Cells[0].Value = (i + 1).ToString();
                table.Rows[i].Cells[1].Value = data[i].surname;
                table.Rows[i].Cells[2].Value = data[i].name;
                table.Rows[i].Cells[3].Value = data[i].secname;
                table.Rows[i].Cells[4].Value = data[i].gender;
                table.Rows[i].Cells[5].Value = data[i].birthday;
                table.Rows[i].Cells[6].Value = data[i].phone;
                table.Rows[i].Cells[7].Value = data[i].addres;
                table.Rows[i].Cells[8].Value = data[i].status;
                table.Rows[i].Cells[9].Value = data[i].crytical;
                if (data[i].crytical)
                {
                    table.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
            }
            dsp = DSP;
            persona = data;
        }

        private void TableForm_Load(object sender, EventArgs e)
        {

        }

        private void table_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (edit)
            {
                if(MessageBox.Show("Сохранить изменения?", "Внимание!!!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)==DialogResult.OK)
                {
                    SaveChange();
                }

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            edit = true;
            button1.Enabled = false;
            button2.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            textBox5.Enabled = true;
            textBox6.Enabled = true;
            textBox7.Enabled = true;
            textBox8.Enabled = true;
            radioButton1.Enabled = true;
            radioButton2.Enabled = true;
            listBox1.SetSelected(0, true);
            if ((listBox1.SelectedItem.ToString() != "Нет данных") && (listBox1.Text != null) || (dsp))
            {
                radioButton3.Enabled = false;
                radioButton4.Enabled = false;
            }
            else
            {
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!edit)
            {
                int index = Convert.ToInt32(table.SelectedRows[0].Cells[0].Value) - 1;
                textBox1.Text = persona[index].surname;
                textBox2.Text = persona[index].name;
                textBox3.Text = persona[index].secname;
                if (persona[index].gender == "Мужской")
                    radioButton1.Checked = true;
                if (persona[index].gender == "Женский")
                    radioButton2.Checked = true;
                textBox4.Text = persona[index].birthday;
                textBox5.Text = persona[index].phone;
                textBox6.Text = persona[index].addres;
                textBox7.Text = persona[index].status;
                textBox8.Text = persona[index].link;
                listBox1.Items.Clear();
                listBox1.Items.Add(persona[index].document);
                if (persona[index].crytical)
                    radioButton3.Checked = true;
                else
                    radioButton4.Checked = true;
                pos = index;
            }
        }

         private void SaveChange ()
        {
            if (textBox1.Text!=null)
            persona[pos].surname = textBox1.Text;
            if (textBox2.Text != null)
                persona[pos].name = textBox2.Text;
            if (textBox3.Text != null)
                persona[pos].secname = textBox3.Text;
            if (radioButton1.Checked)
                persona[pos].gender = "Мужской";
            if (radioButton2.Checked)
                persona[pos].gender = "Женский";
            if (textBox4.Text != null)
                persona[pos].birthday = textBox4.Text;
            if (textBox5.Text != null)
                persona[pos].phone = textBox5.Text;
            if (textBox6.Text != null)
                persona[pos].addres = textBox6.Text;
            if (textBox7.Text != null)
                persona[pos].status = textBox7.Text;
            if (radioButton3.Checked)
                persona[pos].crytical = true;
            if (radioButton4.Checked)
                persona[pos].crytical = false;
            if (textBox8.Text != null)
                persona[pos].link = textBox8.Text;
            UpdateTable(persona, dsp);
            edit = false;
            button1.Enabled = true;
            button2.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;
            textBox7.Enabled = false;
            textBox8.Enabled = false;
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            SaveChange();
        }

        private void TableForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (edit)
            {
                if (MessageBox.Show("Сохранить изменения?", "Внимание!!!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    SaveChange();
                }

            }
            XmlSerializer formatter = new XmlSerializer(typeof(Person[]));

                using (FileStream fs = new FileStream("save.xml", FileMode.CreateNew))
                {
                    formatter.Serialize(fs, persona);
                }

        }
    }
}
