namespace text_mining
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.addFiles = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.addFolder = new System.Windows.Forms.Button();
            this.deleteOfList = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.analizeDocument = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // addFiles
            // 
            this.addFiles.Location = new System.Drawing.Point(60, 35);
            this.addFiles.Name = "addFiles";
            this.addFiles.Size = new System.Drawing.Size(101, 23);
            this.addFiles.TabIndex = 0;
            this.addFiles.Text = "Добавить файл";
            this.addFiles.UseVisualStyleBackColor = true;
            this.addFiles.Click += new System.EventHandler(this.addFiles_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(38, 83);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(288, 212);
            this.listBox1.TabIndex = 1;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // addFolder
            // 
            this.addFolder.Location = new System.Drawing.Point(192, 35);
            this.addFolder.Name = "addFolder";
            this.addFolder.Size = new System.Drawing.Size(101, 23);
            this.addFolder.TabIndex = 2;
            this.addFolder.Text = "Добавить папку";
            this.addFolder.UseVisualStyleBackColor = true;
            this.addFolder.Click += new System.EventHandler(this.addFolder_Click);
            // 
            // deleteOfList
            // 
            this.deleteOfList.Location = new System.Drawing.Point(374, 110);
            this.deleteOfList.Name = "deleteOfList";
            this.deleteOfList.Size = new System.Drawing.Size(125, 23);
            this.deleteOfList.TabIndex = 3;
            this.deleteOfList.Text = "Удалить из списка";
            this.deleteOfList.UseVisualStyleBackColor = true;
            this.deleteOfList.Click += new System.EventHandler(this.deleteOfList_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(374, 155);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(125, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Очистить список";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // analizeDocument
            // 
            this.analizeDocument.Location = new System.Drawing.Point(374, 200);
            this.analizeDocument.Name = "analizeDocument";
            this.analizeDocument.Size = new System.Drawing.Size(125, 23);
            this.analizeDocument.TabIndex = 5;
            this.analizeDocument.Text = "Анализ документа";
            this.analizeDocument.UseVisualStyleBackColor = true;
            this.analizeDocument.Click += new System.EventHandler(this.analizeDocument_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripComboBox1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(694, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(160, 22);
            this.toolStripLabel1.Text = "Тип анализируемого текста";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(230, 25);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 443);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.analizeDocument);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.deleteOfList);
            this.Controls.Add(this.addFolder);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.addFiles);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Графематический анализ текста ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button addFiles;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button addFolder;
        private System.Windows.Forms.Button deleteOfList;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button analizeDocument;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
    }
}

