namespace GJ2022.DmiIconConversionUtility
{
    partial class Form1
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
            this.ConvertFileButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label4Byte = new System.Windows.Forms.Label();
            this.label2Byte = new System.Windows.Forms.Label();
            this.labelChar = new System.Windows.Forms.Label();
            this.buttonReadBmp = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ConvertFileButton
            // 
            this.ConvertFileButton.Location = new System.Drawing.Point(12, 12);
            this.ConvertFileButton.Name = "ConvertFileButton";
            this.ConvertFileButton.Size = new System.Drawing.Size(200, 23);
            this.ConvertFileButton.TabIndex = 0;
            this.ConvertFileButton.Text = "Convert File";
            this.ConvertFileButton.UseVisualStyleBackColor = true;
            this.ConvertFileButton.Click += new System.EventHandler(this.ConvertFileButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 61);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(234, 211);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(95, 275);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Loaded DMI";
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(263, 25);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(484, 277);
            this.listBox1.TabIndex = 5;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // label4Byte
            // 
            this.label4Byte.AutoSize = true;
            this.label4Byte.Location = new System.Drawing.Point(274, 9);
            this.label4Byte.Name = "label4Byte";
            this.label4Byte.Size = new System.Drawing.Size(74, 13);
            this.label4Byte.TabIndex = 6;
            this.label4Byte.Text = "4 byte number";
            // 
            // label2Byte
            // 
            this.label2Byte.AutoSize = true;
            this.label2Byte.Location = new System.Drawing.Point(407, 9);
            this.label2Byte.Name = "label2Byte";
            this.label2Byte.Size = new System.Drawing.Size(74, 13);
            this.label2Byte.TabIndex = 7;
            this.label2Byte.Text = "2 byte number";
            // 
            // labelChar
            // 
            this.labelChar.AutoSize = true;
            this.labelChar.Location = new System.Drawing.Point(528, 9);
            this.labelChar.Name = "labelChar";
            this.labelChar.Size = new System.Drawing.Size(29, 13);
            this.labelChar.TabIndex = 8;
            this.labelChar.Text = "Char";
            // 
            // buttonReadBmp
            // 
            this.buttonReadBmp.Location = new System.Drawing.Point(12, 41);
            this.buttonReadBmp.Name = "buttonReadBmp";
            this.buttonReadBmp.Size = new System.Drawing.Size(200, 23);
            this.buttonReadBmp.TabIndex = 9;
            this.buttonReadBmp.Text = "Read Bmp Data";
            this.buttonReadBmp.UseVisualStyleBackColor = true;
            this.buttonReadBmp.Click += new System.EventHandler(this.buttonReadBmp_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 307);
            this.Controls.Add(this.buttonReadBmp);
            this.Controls.Add(this.labelChar);
            this.Controls.Add(this.label2Byte);
            this.Controls.Add(this.label4Byte);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ConvertFileButton);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ConvertFileButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label4Byte;
        private System.Windows.Forms.Label label2Byte;
        private System.Windows.Forms.Label labelChar;
        private System.Windows.Forms.Button buttonReadBmp;
    }
}

