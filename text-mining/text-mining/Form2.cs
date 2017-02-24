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
using System.Xml;

namespace text_mining
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            toolStripLabel1.Text = null;
            timer1.Enabled = true;
        }


        bool m_HideHighlighting;
        int m_MaxTextLengthForShowing = 2000000;
        int m_IgnoreTreeChanging = 0;
        bool dsp = false;
        string starttext;
        AnalysisResult export;
        Processor pr = null;

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

        string NowTime () // Текущая дата и время
        {
            return DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
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
                pr = processor;
                export = result;
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
                starttext = txt;
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
            MoneyReferent money = t.GetReferent() as MoneyReferent;
            NumberExToken num = NumberHelper.TryParseNumberWithPostfix(t);
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
            else if  (money!=null )
            {
                ind = 4;
                txt = money.ToString();
            }
            else if (num!=null)
            {
                ind = 5;
                double sumLengthMeter = 0;
                if ((num.ExTyp == NumberExToken.ExTyps.Kilometer ||
                        num.ExTyp == NumberExToken.ExTyps.Meter ||
                        num.ExTyp == NumberExToken.ExTyps.Santimeter ||
                        num.ExTyp == NumberExToken.ExTyps.Millimeter) &&
                        num.ExTyp2 == NumberExToken.ExTyps.Undefined)
                {
                    NumberExToken.ExTyps normTyp = NumberExToken.ExTyps.Meter;
                    double normVal = num.NormalizeValue(ref normTyp);
                    if (normTyp == NumberExToken.ExTyps.Meter)
                        sumLengthMeter += normVal;
                }
                // обязательно нужно перемещаться на конец метатокена, чтобы не делались
                // привязки с середины. Например, пусть "-20м", если взять просто следующий, то получим "20м" противоположное по знаку число.
                t = num.EndToken;
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

        public string getNameFile(string file)
        {
            string f = null;
            for (int i = 0; i < file.Length; i++)
            {
                if (file[i] == '.')
                {
                    f = file.Substring(0, i);
                    break;
                }
            }
            return f;
        }
        private void button1_Click(object sender, EventArgs e) //сериализация реультатов (экспорт)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Файлы XML (*.xml) | *.xml";
            if (sfd.ShowDialog() == DialogResult.OK)
            {

                string filetxt = getNameFile(sfd.FileName) + ".txt";

                File.WriteAllText(filetxt, textControl1.Text);
                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    Indent = true,
                    OmitXmlDeclaration = true,

                };
                XmlWriter textWritter = XmlWriter.Create(sfd.FileName, settings);

                textWritter.WriteStartDocument();

                textWritter.WriteStartElement("xml");
                textWritter.WriteAttributeString("version", "1.0");
                textWritter.WriteAttributeString("Encoding", "utf-8");

                textWritter.WriteStartElement("processors");
                int i = 1;
                
               foreach(var p in pr.Analyzers)
                {
                    textWritter.WriteStartElement("processor");
                    textWritter.WriteAttributeString("id",i.ToString() );
                    textWritter.WriteValue(p.ToString());
                    textWritter.WriteEndElement();
                    i++;
                }


                textWritter.WriteEndElement();


                textWritter.WriteStartElement("Entities");
                textWritter.WriteAttributeString("Language", "Ru");
                 i = 1;
                foreach (var ec in export.Entities)
                {
                    try
                    {
                        textWritter.WriteStartElement("Object");
                        textWritter.WriteAttributeString("id", i.ToString());
                        textWritter.WriteAttributeString("Type_object", ec.InstanceOf.Caption);
                        textWritter.WriteValue(ec.ToString());
                        textWritter.WriteStartElement("Value_Simple_Attributte");
                        string x = "";
                        foreach (var v in ec.Slots)
                        {
                            if (!v.IsInternal)
                                if (!(v.Value is Referent))
                                    x += v.ToString();


                        }

                        textWritter.WriteValue(x.ToString());

                        textWritter.WriteEndElement();

                        textWritter.WriteStartElement("Value_Link_Attributte");

                        bool b = false;
                        x = "";
                        foreach (var v in ec.Slots)
                            if (!v.IsInternal && (v.Value is Referent))
                            {

                                x += v.ToString();
                            }
                        textWritter.WriteValue(x.ToString());
                        textWritter.WriteEndElement();
                        textWritter.WriteStartElement("Part_text");
                        x = "";
                        foreach (var v in CurrentEntity.Occurrence)
                            x += v.ToString();
                        textWritter.WriteValue(x.ToString());
                        textWritter.WriteEndElement();
                        textWritter.WriteEndElement();
                        i++;
                    }
                    catch (Exception eeeee)
                    {
                        string m = eeeee.Message;
                    }
                }

                i = 1;
                textWritter.WriteEndElement();

                textWritter.WriteStartElement("Tokens");
                for (Token t = export.FirstToken; t != null; t = t.Next)
                {
                    textWritter.WriteStartElement("Token");
                    textWritter.WriteAttributeString("id", i.ToString());
                    textWritter.WriteAttributeString("BeginChar", t.BeginChar.ToString());

                    textWritter.WriteAttributeString("EndChar", t.EndChar.ToString());
                    textWritter.WriteAttributeString("LengthChar", t.LengthChar.ToString());
                    textWritter.WriteValue(t.ToString());
                    textWritter.WriteEndElement();
                    i++;
                }
                textWritter.WriteEndElement();

                textWritter.WriteEndElement();
                textWritter.Close();


            }

        }
       

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripLabel1.Text = NowTime();
        }
        public void importResult ()
        {
            MessageBox.Show("Xml файл должен иметь такое же имя как и имя файла исходного текста", "Вниманиие!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            OpenFileDialog OPF = new OpenFileDialog();
            OPF.Filter = "Документы xml (*.xml)|*.xml";
            if (OPF.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(getNameFile(OPF.FileName) + ".txt"))
                {
                    MessageBox.Show("Отстутствует исходный файл" + getNameFile(OPF.FileName) + ".txt", "Ошибка файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string[] type = new string[16];
                string text = null;
                try
                {

                    text = File.ReadAllText(getNameFile(OPF.FileName) + ".txt");
                    if ((text == null) || (text == "  "))
                    {
                        MessageBox.Show("Исходный текст отстуствует", "Ошибка файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    XmlTextReader reader = new XmlTextReader(OPF.FileName);
                    bool find = false;
                    while (reader.Read())
                    {
                        if (reader.Name == "processors")
                        {
                            find = true;
                            reader.Close();
                            reader = null;
                            break;
                        }
                    }
                    if (!find)
                    {
                        MessageBox.Show("Отстуствует информация об способах анализа текста", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    int i = 0;
                    reader = new XmlTextReader(OPF.FileName);
                    find = false;
                    while (reader.Read())
                    {
                        if ((reader.Name == "processors") && (!find))
                        {
                            find = true;
                        }

                        if (reader.NodeType == XmlNodeType.Text)
                        {
                            type[i] = reader.Value;
                            i++;
                        }

                        if (reader.Name == "Entities")
                            break;

                    }

                    reader.Close();
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message, "Ошибка файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                bool semnatic = false;
                bool business = false;
                bool title = false;
                for (int i = 12; i < 16; i++)
                {
                    if (type[i] == null)
                        break;

                    switch (type[i])
                    {
                        case "Бизнес-объекты (BUSINESS)": { business = true; break; };
                        case "Семантический анализ (SEMANTIC)": { semnatic = true; break; };
                        case "Титульный лист (TITLEPAGE)": { title = true; break; };
                    }
                }
                Processor import = null;
                string analizator = null;
                if ((!semnatic) && (!business) && (!title))
                {
                    import = new Processor();
                }
                else
                {
                    if (semnatic)
                        analizator = "SEMANTIC";
                    if (business)
                        analizator = "BUSINESS";
                    if (title)
                        analizator = "TITLEPAGE";
                    if (semnatic && business)
                        analizator = "SEMANTIC,BUSINESS";
                    if (semnatic && title)
                        analizator = "SEMANTIC,TITLEPAGE";
                    if (business && title)
                        analizator = "BUSINESS,TITLEPAGE";
                    if (semnatic && business && title)
                        analizator = "SEMANTIC,BUSINESS,TITLEPAGE";
                    import = new Processor(analizator);
                }
                bool result = ProcessAnalize(ref text, ref import);
                if (!result)
                {
                    MessageBox.Show("Импорт завершился не успешно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();

                }

            }
        }

        private void button2_Click(object sender, EventArgs e) //десериализация результатов (импорт)
        {
            importResult();
        }
    }
}
