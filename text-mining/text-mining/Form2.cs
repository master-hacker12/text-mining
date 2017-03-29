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
using System.Xml.Serialization;
using System.Text.RegularExpressions;

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
        //AnalysisResult export;
        Processor pr = null;
        Person[] persondata = null;


        bool isDsp (string document)
        {
           // string[] c = { "\r\n" };
           //string[] str = document.Split( c, StringSplitOptions.None);
            string[] str = document.Split('\n');
            for (int i = 0; i < str.Length; i++)
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
               // export = result;
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
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка анализа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor = Cursors.Default;
                starttext = txt;
                return false;
            }
           
            
            string[] sentences = GetSentences(textControl1.Text);
            if (sentences!=null)
            {
                for (int i=0;i<sentences.Length;i++)
                {
                 persondata = Person.Summa(persondata, FindPerson(sentences[i]));
                }
            }
            label1.Text = "";
            button3.Enabled = false;
            if (persondata != null)
                label1.Text = "Текст содержит персональные данные см. подробнее------>";
            return true;
        }

        public string[] GetSentences (string text)
        {
            string str = textControl1.Text;
            string result = string.Join(" ", str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            char[] tochka = { '.', '?', '!' };
            string[] words = result.Split(tochka);
            return words;
        }

      
        public int CountPerson (AnalysisResult ar)
        {
            int count = 0;

            foreach (var e in ar.Entities)
           {
                if (e.InstanceOf.Caption == "Персона")
                    count++;

            }
            return count;
        }

        public string[] GetWords(string sentence)
        {
            string[] result = sentence.Split(' ', ',','\n');
            return result;
        }

        public int InformationInPerson(string word,string[] sentence, string[] person,int length)
        {
            int pos = 0;
            int col = 0;
            bool zap = false;
            for (int i=0;i<sentence.Length;i++)
            {
                if (word == sentence[i])
                {
                    pos = i;
                    if ((i + 2) < sentence.Length)
                    {
                        if (sentence[i + 2] == "")
                            zap = true;
                    }
                    col++;
                }
            }

            if (col>=length)
                return 0;
            if (zap)
                return 1;

            int posPers = 0;
            person[0] = person[0].ToLower();
            if (person[1].Length>2)
            person[1] = person[1].ToLower();
            if (person[2].Length > 2)
                person[2] = person[2].ToLower();
            person[0] = person[0].Remove(person[0].Length - 2, 2);
            if (person[1].Length > 2)
                person[1] = person[1].Remove(person[1].Length - 2, 2);
            if (person[2].Length > 2)
                person[2] = person[2].Remove(person[2].Length - 2, 2);
            for (int i = 0; i < sentence.Length; i++)
            {
                
                   sentence[i].StartsWith(person[0], StringComparison.CurrentCultureIgnoreCase);
                

                if ( sentence[i].StartsWith(person[0], StringComparison.CurrentCultureIgnoreCase) || sentence[i].StartsWith(person[1], StringComparison.CurrentCultureIgnoreCase) || sentence[i].StartsWith(person[2], StringComparison.CurrentCultureIgnoreCase))

                {
                    posPers = i;
                }
            }

            return Math.Abs(posPers-pos) ;
        }

        private Person[] FindPerson(string sentence)
        {
            string name = null;
            string surname = null;
            string secname = null;
            string birthday = null;
            string gender = null;
            string status = null;
            string addres = null;
            string phone = null;
            string document = null;
            string link = null;

            AnalysisResult res = pr.Process(new SourceOfAnalysis(sentence));// разбить предложение на слова и дальше анализировать высчитывать расстояния от одной персоны до другой
            int person = CountPerson(res);
            Person[] pers = new Person[person];

            int j = 0;
            foreach (var e in res.Entities)
            {

                name = null;
                surname = null;
                secname = null;
                birthday = null;
                gender = null;
                status = null;
                addres = null;
                phone = null;
                document = null;
                link = null;
                if (e.InstanceOf.Caption == "Персона")
                {
                    foreach (var f in e.Slots)
                    {

                        if (f.DefiningFeature.Caption=="Пол")
                        {
                            if (f.Value.ToString() == "MALE")
                            {
                                gender = "Мужской";
                            }
                            if (f.Value.ToString() == "FEMALE")
                            {
                                gender = "Женский";
                            }
                            continue;
                        }

                        if (f.DefiningFeature.Caption == "Фамилия")
                        {
                            surname = f.Value.ToString();
                            continue;
                        }
                        if (f.DefiningFeature.Caption == "Имя")
                        {
                            name = f.Value.ToString();
                            continue;
                        }
                        if (f.DefiningFeature.Caption == "Отчество")
                        {
                            secname = f.Value.ToString();
                            continue;
                        }
                        if (f.DefiningFeature.Caption == "Свойство")
                        {
                            status = f.Value.ToString();
                            continue;
                        }
                        if (f.DefiningFeature.Caption == "Родился")
                        {
                            birthday = f.Value.ToString();
                            continue;
                        }
                        if (f.DefiningFeature.Caption == "Контактные данные")
                        {
                            link = f.Value.ToString();
                            continue;
                        }
                        if (f.DefiningFeature.Caption == "Удостоверение личности")
                        {
                            document = f.Value.ToString();
                            continue;
                        }
                    }

                    pers[j] = new Person();
                    pers[j].AddPerson(name, surname, secname, birthday, null, gender, status, null);
                    j++;
                }

            }
            string[] words = GetWords(sentence);
            int countPhone = 0;
            int countAdress = 0;
            Regex street = new Regex(@"жив(\w*)");
            Regex phoneNum = new Regex(@"телефон(\w*)");


            for (int i=0;i<words.Length;i++)
            {
                if (street.IsMatch(words[i]))
                {
                    countAdress++;
                }
                if (phoneNum.IsMatch(words[i]))
                {
                    countPhone++;
                }
            }            
            int[,] pPhone = new int[person,countPhone];
            int[,] pAdress = new int[person, countAdress];
            string[] listPhone = new string[countPhone];
            string[] listAdress = new string[countAdress];
            int ppp = 0;
            int pp = 0;
            for (int i = 0;i<person;i++)
            {             
                    foreach(var e in res.Entities)
                    {
                        if (e.InstanceOf.Caption=="Адрес")
                        {

                        if (listAdress == null)
                        break;
                        if (ppp >= listAdress.Length)
                            break;
                            listAdress[ppp] = e.ToString();
                            ppp++; 
                        }
                        if (e.InstanceOf.Caption=="Телефонный номер")
                    {
                        if (listPhone == null)
                            break;
                        if (pp >= listPhone.Length)
                            break;
                        listPhone[pp] = e.Slots[0].Value.ToString();
                        pp++;
                    }
                        if (countPhone>0)
                    {
                        if (listPhone[0]==null)
                        {
                            pp = 0;
                            for (int gg=0;gg<words.Length;gg++)
                            {
                                if (phoneNum.IsMatch(words[gg]))
                                {
                                    if (pp >= listPhone.Length)
                                        break;
                                    try
                                    {
                                        double tel = Convert.ToDouble(words[gg + 1]);
                                        listPhone[pp] = tel.ToString();
                                        pp++;
                                    }
                                    catch(Exception ex)
                                    {

                                    }
                                }
                            }
                        }
                    }
                        
                    }
                if (countAdress != 0)
                {
                    for (int jj = 0; jj < listAdress.Length; jj++)
                    {
                        string[] data = pers[i].Get();
                        if (pAdress == null)
                        {
                            pers[i].Append(null, null, null, null, null, null, null, listAdress[jj], false);
                            break;
                        }
                        pAdress[i, jj] = InformationInPerson(listAdress[jj], words, data, person);
                        if (pAdress[i, jj] == 0)
                        {
                            pAdress = null;
                            pers[i].Append(null, null, null, null, null, null, null, listAdress[jj], false);
                            break;
                        }
                    }
                }
                if (countPhone != 0)
                {
                    for (int jj = 0; jj < listPhone.Length; jj++)
                    {
                        if (pPhone == null)
                        {
                            pers[i].Append(null, null, null, null, null, phone, null, null, false);
                            break;
                        }
                        string[] data = pers[i].Get();
                        pPhone[i, jj] = InformationInPerson(listPhone[jj], words, data, person);
                        if (pPhone[i, jj] == 0)
                        {
                            pPhone = null;
                            pers[i].Append(null, null, null, null, null, phone, null, null, false);
                            break;
                        }
                    }
                }
            }

            if ((pAdress != null) && (countAdress!=0))
            {
                int[] pMin = new int[person];
                int p = 0;
                for (int jj = 0; jj < pMin.Length; jj++)
                {
                    p = 0;
                    for (int i = 0; i < person; i++)
                    {
                        if (p >= pMin.Length)
                            break;
                        try
                        {
                            pMin[p] = pAdress[i, jj];
                            p++;
                        }
                        catch (Exception ee)
                        {
                            break;
                        }
                    }
                    int index =0;
                    int min = pMin[0];
                    for (int h = 0; h < pMin.Length; h++)
                    {
                        for (int hh = 0; hh < pMin.Length; hh++)
                        {
                            if (pMin[hh] < min)
                            {
                                min = pMin[hh];
                                index = hh;
                            }
                        }
                    }
                    if (jj == listAdress.Length)
                        break;
                    pers[index].Append(null, null, null, null, null, null, null, listAdress[jj],false);
                }
            }
            if ((pPhone != null) && (countPhone!=0))
            {
                int[] pMin = new int[person];
                int p = 0;
                for (int jj = 0; jj < pMin.Length; jj++)
                {
                    p = 0;
                    for (int i = 0; i < person; i++)
                    {
                        if (p >= pMin.Length)
                            break;
                        try
                        {
                            pMin[p] = pPhone[i, jj];
                            p++;
                        }
                        catch (Exception ee)
                        {
                            break;
                        }
                    }
                    int index = 0;
                    int min = pMin[0];
                    for (int h = 0; h < pMin.Length; h++)
                    {
                        for (int hh = 0; hh < pMin.Length; hh++)
                        {
                            if (pMin[hh] < min)
                            {
                                min = pMin[hh];
                                index = hh;
                            }
                        }
                    }
                    if (jj == listPhone.Length)
                        break;
                    pers[index].Append(null, null, null, null, listPhone[jj], null, null, null, false);
                }
            }
            ////переосмыслить привязки телефонов и адресов к персонам
            // 
            // int countTel = 0;
            // int colAdress = 0;
            // Regex reg = new Regex(@"жив(\w*)");
            // for (int k = 0; k < words.Length; k++)
            // {
            //     if (reg.IsMatch(words[k]))
            //     {
            //         colAdress++;
            //     }
            //     if (words[k]=="телефон")
            //     {
            //         countTel++;
            //     }
            // }

            // string[] pAdress = new string[colAdress];
            // string[] pTel = new string[countTel];
            //countTel = 0;
            //colAdress = 0;
            // int[] pos = null;
            // int[] posadrees = null;
            // if (person > 1)
            // {
            //    pos  = new int[person];
            //     posadrees = new int[person];
            // }
            // int id = 0;
            // for (int i = 0; i < person; i++)
            // {
            //     string[] data = pers[i].Get();
            //     id = i;
            //     int pp = 0;
            //     foreach (var e in res.Entities)
            //     {

            //         if (e.InstanceOf.Caption == "Адрес")
            //         {
            //             if (pp == id)
            //             {
            //                 addres = e.ToString();

            //                 break;
            //             }
            //             pp++;

            //         }

            //     }
            //     for (int k = 0; k < words.Length; k++)
            //     {
            //         if (words[k] == "телефон")
            //         {

            //             double tel = 0;
            //             try
            //             {
            //                 tel = Convert.ToDouble(words[k + 1]);
            //                 phone = tel.ToString();
            //                 pTel[countTel] = phone;
            //                 countTel++;
            //                 pos[i] = InformationInPerson(words[k], words, data,person);
            //                 if (pos[i] == 0)
            //                 {
            //                     pers[i].Append(null, null, null, null, phone, null, null, null,false);
            //                     pos = null;
            //                     break;
            //                 }
            //             }
            //             catch (Exception e)
            //             {
            //                 phone = null;
            //                 pos = null;
            //             }                   
            //         }

            //         if (reg.IsMatch(words[k]))
            //         {
            //             posadrees[i] = InformationInPerson(words[k], words, data, person);
            //             pAdress[colAdress] = words[k];
            //             colAdress++;
            //             if (posadrees[i]==0)
            //             {
            //                 pers[i].Append(null, null, null, null, null, null, null, addres, false);
            //                 posadrees = null;
            //                 break;
            //             }
            //         }


            //     }
            //}
            //if (pos!=null)
            //{
            //    int min = pos[0];
            //    int index = 0;

            //    for (int i=0;i<pos.Length;i++)
            //        for(int p=0;p<pos.Length;p++)
            //    {
            //            if (pos[p] < min)
            //            {
            //                min = pos[p];
            //                index = p;
            //            }
            //    }

            //       // pers[index].Append(null, null, null, null, , null, null, null, false);





            //}
            //if (posadrees != null)
            //{
            //    int min = posadrees[0];
            //    int index = 0;

            //    for (int i = 0; i < posadrees.Length; i++)
            //        for (int p = 0; p < posadrees.Length; p++)
            //        {
            //            if ((posadrees[p] < min) && (posadrees[p]!=0))
            //            {
            //                min = posadrees[p];
            //                index = p;
            //            }
            //        }

            //    pers[index].Append(null, null, null, null, null, null, null, addres, false);
            //}


            return pers;
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
                ExportResult.ProcessXml(textControl1.Text, sfd.FileName, ref pr);
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(Person[]));
            if (!File.Exists("people.xml"))
            {
                using (FileStream fs = new FileStream("people.xml", FileMode.CreateNew))
                {
                    formatter.Serialize(fs, persondata);
                }

            }
            else
            {
                Person[] import = null;
                using (FileStream fs = new FileStream("people.xml", FileMode.Open))
                {
                    import = (Person[])formatter.Deserialize(fs);
                }
                int length = persondata.Length;
                bool find = false;
                int pp = 0;
                for (int j = 0; j < length; j++)
                {
                    find = false;
                    for (int i = 0; i < length; i++)
                    {
                        find = false;
                        if ((import[i].surname == persondata[j].surname) && ((persondata[j].name.StartsWith(import[i].name, StringComparison.CurrentCultureIgnoreCase)) || (persondata[j].secname.StartsWith(import[i].secname))))
                        {
                            find = true;
                            if ((persondata[j].name.Length > import[i].name.Length) && (persondata[j].name != "Нет данных"))
                            {
                                import[i].name = persondata[j].name;
                            }
                            if ((persondata[j].secname.Length > import[i].secname.Length) && (persondata[j].secname != "Нет данных"))
                            {
                                import[i].secname = persondata[j].secname;
                            }

                            if ((persondata[j].gender != import[i].gender) && (persondata[j].gender != "Нет данных"))
                            {
                                import[j].gender = persondata[i].gender;
                            }

                            if ((persondata[j].birthday != import[i].birthday) && (persondata[j].birthday != "Нет данных"))
                            {
                                import[i].birthday = persondata[j].birthday;
                            }
                            if ((persondata[j].phone != import[i].phone) && (persondata[j].phone != "Нет данных"))
                            {
                                import[i].phone = persondata[j].phone;
                            }
                            if ((persondata[j].status != import[i].status) && (persondata[j].status != "Нет данных"))
                            {
                                import[i].status = persondata[j].status;
                            }
                            if ((persondata[j].addres != import[i].addres) && (persondata[j].addres != "Нет данных"))
                            {
                                import[i].addres = persondata[j].addres;
                            }
                            if (persondata[j].crytical != import[i].crytical)
                            {
                                import[i].crytical = persondata[j].crytical;
                            }
                            break;
                        }
                        if ((i + 1 == length) && (!find))
                            pp = j; 
                    }
                        if (!find)
                        {
                            Person[] p = new Person[1];
                            p[0] = persondata[pp];
                            import = Person.Summa(import, p);
                        }
                    
                }

                try
                {
                    File.Delete("people.xml");
                }
                catch (IOException ee)
                {
                    for (;;)
                    {
                        MessageBox.Show("Закройте файл people.xml", "Ошибка экспорта", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        try
                        { 
                             File.Delete("people.xml");
                            break;
                        }
                        catch (Exception eee)
                        {
                            continue;
                        }
                    }
                }
                catch (Exception ee)
                {

                }
                finally
                {
                    using (FileStream fs = new FileStream("people.xml", FileMode.CreateNew))
                    {
                        formatter.Serialize(fs, import);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
