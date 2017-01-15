using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EP.Semantix;
using EP;

namespace text_mining
{
    /// <summary>
    ///  онтрол дл€ отображени€ текста с возможностью подсветки позиций
    /// </summary>
    partial class TextControl : UserControl
    {
        public TextControl()
        {
            InitializeComponent();
            textControl1.BackColor = Color.FromKnownColor(KnownColor.Window);
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            internalText.Font = Font;
        }

        /// <summary>
        /// “екст можно только редактировать
        /// </summary>
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return textControl1.ReadOnly; }
            set { textControl1.ReadOnly = value; }
        }

        /// <summary>
        /// “екст документа
        /// </summary>
        [DefaultValue("")]
        public override string Text
        {
            get { return textControl1.Text; }
            set
            {
                m_GreenHighlights.Clear();
                m_GreenHighlightingCorrected = false;

                internalText.Text = value;
                ViewText(null);
            }
        }

        /// <summary>
        /// —обытие изменение текста пользователем
        /// </summary>
        public event EventHandler TextChanged;

        int m_TextChangedIgnore = 0;
        private void txtContent_TextChanged(object sender, EventArgs e)
        {
            if (m_TextChangedIgnore == 0)
            {
                internalText.Text = textControl1.Text;
                if (TextChanged != null)
                    TextChanged(this, EventArgs.Empty);
            }
        }

        class HiPeace
        {
            public int Pos;
            public int Len;
        }

        List<HiPeace> m_GreenHighlights = new List<HiPeace>();
        public void AddGreenHighlighting(IEnumerable<TextAnnotation> occurence)
        {
            foreach(var oc in occurence)
                m_GreenHighlights.Add(new HiPeace() { Pos = oc.BeginChar, Len = oc.EndChar - oc.BeginChar + 1 });
        }

        bool m_GreenHighlightingCorrected;
        void CorrectGreenHighlighting()
        {
            if (m_GreenHighlightingCorrected) return;
            m_GreenHighlightingCorrected = true;
            m_GreenHighlights.Sort(new HiComparer());
            for(int i=0; i<m_GreenHighlights.Count - 1; i++)
                if (m_GreenHighlights[i].Pos + m_GreenHighlights[i].Len > m_GreenHighlights[i + 1].Pos)
                {
                    m_GreenHighlights.RemoveAt(i); i--;
                }
        }
        class HiComparer : IComparer<HiPeace>
        {
            public int Compare(HiPeace x, HiPeace y)
            {
                if (x.Pos < y.Pos) return -1;
                if (x.Pos > y.Pos) return 1;
                return 0;
            }
        }

        /// <summary>
        /// ѕодсветить фрагменты текста
        /// </summary>
        /// <param name="locs"></param>
        public void RedHighlight(IEnumerable<TextAnnotation> locs)
        {
            List<HiPeace> hi = new List<HiPeace>();
            if(locs != null)
                foreach(var oc in locs)
                    hi.Add(new HiPeace() { Pos = oc.BeginChar, Len = oc.EndChar - oc.BeginChar + 1 });
            ViewText(hi);
        }

        RichTextBox internalText = new RichTextBox();
        Font editNormalFont = null;

        void ViewText(List<HiPeace> redPeaces)
        {
            CorrectGreenHighlighting();
            if (editNormalFont == null)
                editNormalFont = internalText.Font;
            textControl1.BackColor = Color.FromKnownColor(KnownColor.Window);

            // начинаем подсветку ...
            internalText.SuspendLayout();
            //memoEdit1.HideSelection = true;
            string txt = internalText.Text;
            Color textColor = Color.FromKnownColor(KnownColor.WindowText);
            Color selColor = textColor.ToArgb() == Color.Red.ToArgb() ? Color.Black : Color.Red;
            Color selColor0 = textColor.ToArgb() == Color.Green.ToArgb() ? Color.Black : Color.Green;

            Font normalFont = internalText.Font;
            Font boldFont = new Font(internalText.Font, FontStyle.Bold);

            internalText.SelectionStart = 0;
            internalText.SelectionLength = txt.Length;
            internalText.SelectionColor = textColor;
            internalText.SelectionFont = normalFont;

            foreach (var p in m_GreenHighlights)
            {
                internalText.SelectionStart = p.Pos;
                internalText.SelectionLength = p.Len;
                internalText.SelectionFont = boldFont;
                internalText.SelectionColor = selColor0;
            }
            if (redPeaces != null)
                foreach (var p in redPeaces)
                {
                    internalText.SelectionStart = p.Pos;
                    internalText.SelectionLength = p.Len;
                    internalText.SelectionFont = boldFont;
                    internalText.SelectionColor = selColor;
                }


            internalText.Font = normalFont;
            internalText.ResumeLayout();

            m_TextChangedIgnore++;
            // к сожалению, приходитс€ извращатьс€ через внутренний компонент,
            // поскольку txtContent слишком мигает во врем€ прорисовки ...
            textControl1.Rtf = internalText.Rtf;

            m_TextChangedIgnore--;

            if (redPeaces != null && redPeaces.Count > 0)
            {
                // делаем так, чтобы первый красный фрагмент попал в область видимости
                int pos = redPeaces[0].Pos;
                int pos1 = pos + 200; if (pos1 >= textControl1.Text.Length) pos1 = textControl1.Text.Length - 1;
                textControl1.SelectionStart = pos1;
                textControl1.SelectionLength = 0;
                textControl1.SelectionStart = pos;
                textControl1.SelectionLength = 0;
                textControl1.HideSelection = false;
            }
        }
    }
}
