namespace FormeleMethodenPracticum
{
    partial class Window
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            outputTextBox = new System.Windows.Forms.RichTextBox();
            inputTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // outputTextBox
            // 
            outputTextBox.Enabled = false;
            outputTextBox.Location = new System.Drawing.Point(13, 13);
            outputTextBox.Name = "outputTextBox";
            outputTextBox.Size = new System.Drawing.Size(259, 193);
            outputTextBox.TabIndex = 0;
            outputTextBox.Text = "";
            // 
            // inputTextBox
            // 
            inputTextBox.Location = new System.Drawing.Point(13, 212);
            inputTextBox.Name = "inputTextBox";
            inputTextBox.Size = new System.Drawing.Size(259, 37);
            inputTextBox.TabIndex = 1;
            inputTextBox.Text = "";
            inputTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.inputTextBox_KeyUp);
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(inputTextBox);
            this.Controls.Add(outputTextBox);
            this.Name = "Window";
            this.Text = "Window";
            this.ResumeLayout(false);

        }

        #endregion

        private static System.Windows.Forms.RichTextBox outputTextBox;
        private static System.Windows.Forms.RichTextBox inputTextBox;


    }
}

