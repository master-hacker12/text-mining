namespace text_mining
{
    partial class TextControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextControl));
            this.textControl1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // textControl1
            // 
            resources.ApplyResources(this.textControl1, "textControl1");
            this.textControl1.Name = "textControl1";
            this.textControl1.TextChanged += new System.EventHandler(this.txtContent_TextChanged);
            // 
            // TextControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.textControl1);
            resources.ApplyResources(this, "$this");
            this.Name = "TextControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox textControl1;

    }
}
