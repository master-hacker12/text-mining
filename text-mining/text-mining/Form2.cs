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
using System.IO;

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
        int m_IgnoreTreeChanging = 0;
        bool dsp = false;
        
        bool isDsp (string document)
        {
           string[] str = document.Split('\n');
            for (int i = 0; i<str.Length; i++)
            {
                if ((str[i] == "ДСП") || (str[i] == "Для служебного пользования"))
                    return true;
            }

            return false;
        }

        public bool ProcessAnalize(ref string txt, ref Processor processor)
        {
            if (txt != null && txt.Length > m_MaxTextLengthForShowing)
                textControl1.Text = txt.Substring(0, m_MaxTextLengthForShowing) +
                    "\r\n...\r\nТекст обрезан, так как компонент RichTextBox не отображает длинные тексты";
            else
            {
                textControl1.Text = txt;
                // этот контрол там подправил переходы на новую строку,
                // чтобы корректно вычислялись смещения для подсветки, приходится брать
                // вариант из контрола
                txt = textControl1.Text;
            }
            // подписываемся на событие "бегунка"
            processor.Progress += new ProgressChangedEventHandler(processor_Progress);
           dsp = isDsp(txt);
            try
            {
                textBoxInform.Text = null;
                Cursor = Cursors.WaitCursor;
                AnalysisResult result = processor.Process(new SourceOfAnalysis(txt));

                if (txt != null && txt.Length > m_MaxTextLengthForShowing)
                {
                    MessageBox.Show(string.Format("Внимание, текст слишком длинный ({0}Kb),\r\n" +
                        "поэтому в тексте подсветка найденных сущностей не будет работать\r\n" +
                        "(это из-за ограничений компоненты RichTextEdit в .NET - очень тормозит)",
                        txt.Length / 1024), "!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    m_HideHighlighting = true;
                }
                else
                {
                    // все сущности подсвечиваем зелёным цветом в окне текста
                    foreach (var e in result.Entities)
                        textControl1.AddGreenHighlighting(e.Occurrence);
                    m_HideHighlighting = false;
                }
                BindingList<EntityWrapper> entities = new BindingList<EntityWrapper>();
                foreach (var e in result.Entities) entities.Add(new EntityWrapper(e));
               
                // выводим в таблице
                bindingSource1.DataSource = entities;
                if (entities.Count > 0)
                    bindingSource1.ResetCurrentItem();

                // рисуем дерево токенов
                DrawTokens(result.FirstToken);


                if (dsp)
                    toolStripLabelMessage.Text += ". Данный документ возможно имеет гриф <Для служебного пользования> ";
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка анализа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Текущая сущность, выбранная пользователем в таблице
        /// </summary>
        Referent CurrentEntity
        {
            get
            {
                EntityWrapper ew = bindingSource1.Current as EntityWrapper;
                if (ew == null)
                    return null;
                else
                    return ew.Source;
            }
        }

        /// <summary>
        /// Обработка события выбора текущей сущности в таблице.
        /// Производится подсветка красным её встречаемости в тексте и вывод значений атрибутов
        /// в текстовом информационном окне.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void bindingSource1_CurrentChanged_1(object sender, EventArgs e)
        {
            if (CurrentEntity == null) return;
            Cursor = Cursors.WaitCursor;

            if (!m_HideHighlighting)
            {
                // подсвечиваем в тексте красным цветом вхождение текущей сущности
                textControl1.RedHighlight(CurrentEntity.Occurrence);
            }

            // в нижнем правом текстовом окне выводим подробную информацию о текущей сущности

            StringBuilder info = new StringBuilder();
            info.AppendFormat("Тип объекта: {0}", CurrentEntity.InstanceOf.Caption);
            info.AppendFormat("\r\nКраткое описание: {0}", CurrentEntity.ToString());

            info.Append("\r\nЗначения простых атрибутов: ");
            // сначала выводим значения простых атрибутов
            foreach (var v in CurrentEntity.Slots)
                if (!v.IsInternal)
                    if (!(v.Value is Referent))
                        info.AppendFormat("\r\n\t{0}", v.ToString());

            // теперь выводим значения ссылочных атрибутов (ссылки на другие сущности)
            bool b = false;
            foreach (var v in CurrentEntity.Slots)
                if (!v.IsInternal && (v.Value is Referent))
                {
                    if (!b) { b = true; info.AppendFormat("\r\n\r\nЗначения ссылочных атрибутов (связи с другими сущностями)"); }
                    info.AppendFormat("\r\n\t{0}", v.ToString());
                }

            // здесь показываем текстовые фрагменты
            info.Append("\r\n\r\nФрагменты исходных текстов:\r\n");
            foreach (var v in CurrentEntity.Occurrence)
                info.AppendFormat("'{0}' ", v.ToString());

            textBoxInform.Text = info.ToString();
            Cursor = Cursors.Default;
        }

        void DrawTokens(Token first)
        {
            m_IgnoreTreeChanging++;
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();

            int cou = 0;
            for (Token t = first; t != null; t = t.Next)
            {
                TreeNode tn = _addNode(t, treeView1.Nodes);
                if (++cou > 20000)
                {
                    treeView1.Nodes.Add("Too many nodes ... ").ImageIndex = 4;
                    break;
                }
            }

            treeView1.ExpandAll();
            treeView1.EndUpdate();
            m_IgnoreTreeChanging--;
        }
        TreeNode _addNode(Token t, TreeNodeCollection tnc)
        {
            if (t == null) return null;
            string txt = null; int ind = 0;
            if (t is ReferentToken)
            {
                ind = 3;
                txt = (t as ReferentToken).Referent.ToString();
            }
            else if (t is NumberToken)
            {
                ind = 2;
                txt = (t as NumberToken).Value.ToString();
            }
            else if (t is TextToken)
            {
                ind = 1;
                txt = t.ToString();
            }
            else
                txt = t.ToString();

            TreeNode res = tnc.Add(txt);
            res.ImageIndex = res.SelectedImageIndex = ind;
            res.Tag = t;

            MetaToken mt = t as MetaToken;
            if (mt != null)
            {
                for (Token tt = mt.BeginToken; tt != null; tt = tt.Next)
                {
                    TreeNode tn = _addNode(tt, res.Nodes);
                    if (tt.EndChar > mt.EndChar)
                    {
                        tn.ImageIndex = 4; //ошибочная ситуация, этого не должно быть
                        break;
                    }
                    if (tt == mt.EndToken) break;
                }
            }
            return res;
        }
     

        void processor_Progress(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= 0)
                toolStripProgressBar1.Value = e.ProgressPercentage;
            else
            {
                // если < 0, то это просто информационное сообщение
            }

            if (e.UserState != null)
            {
                toolStripLabelMessage.Text = e.UserState.ToString();
                toolStrip1.Update();
            }
        }

        private void treeView1_AfterSelect_1(object sender, TreeViewEventArgs e)
        {
            if (m_IgnoreTreeChanging > 0) return;
            if (treeView1.SelectedNode == null) return;
            Token cur = treeView1.SelectedNode.Tag as Token; if (cur == null) return;

            // подсветим в тексте
            List<TextAnnotation> tmp = new List<TextAnnotation>();
            tmp.Add(new TextAnnotation() { BeginChar = cur.BeginChar, EndChar = cur.EndChar });
            textControl1.RedHighlight(tmp);

            // выведем информацию в окно
            StringBuilder txt = new StringBuilder();
            txt.AppendFormat("{0}\r\n", cur.ToString());
            if (cur is TextToken)
            {
                foreach (var wf in cur.Morph.Items)
                    txt.AppendFormat("\r\n{0}", wf.ToString());
            }
            else if (cur is NumberToken)
            {
                NumberToken nt = cur as NumberToken;
                txt.AppendFormat("\r\n{0}: {1}", nt.Value, nt.Typ);
            }
            textBoxInform.Text = txt.ToString();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
           
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void textControl1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
