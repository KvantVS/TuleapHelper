namespace TuleapHelper
{
    partial class TuleapPatchCreateForm
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.chb_takeFromBug = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tbFilename = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.cbProjects = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(221, 26);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(313, 121);
            this.textBox1.TabIndex = 0;
            // 
            // chb_takeFromBug
            // 
            this.chb_takeFromBug.AutoSize = true;
            this.chb_takeFromBug.Checked = true;
            this.chb_takeFromBug.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb_takeFromBug.Location = new System.Drawing.Point(83, 8);
            this.chb_takeFromBug.Name = "chb_takeFromBug";
            this.chb_takeFromBug.Size = new System.Drawing.Size(97, 17);
            this.chb_takeFromBug.TabIndex = 1;
            this.chb_takeFromBug.Text = "Брать из бага";
            this.chb_takeFromBug.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(221, 253);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(151, 31);
            this.button1.TabIndex = 2;
            this.button1.Text = "Создать";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(218, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Изменения:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Назначено:";
            // 
            // listBox1
            // 
            this.listBox1.Enabled = false;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(15, 26);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(200, 121);
            this.listBox1.TabIndex = 6;
            this.listBox1.Visible = false;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(383, 253);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(151, 31);
            this.button2.TabIndex = 7;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 156);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Файл сборки:";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.CheckFileExists = false;
            this.openFileDialog1.CheckPathExists = false;
            this.openFileDialog1.DefaultExt = "msi";
            this.openFileDialog1.Filter = "Файлы Microsoft Installer (.msi)|*.msi|Все файлы|*.*";
            this.openFileDialog1.InitialDirectory = "\\\\buildkrg\\GE-ESEDO\\Builds";
            // 
            // tbFilename
            // 
            this.tbFilename.Location = new System.Drawing.Point(96, 153);
            this.tbFilename.Name = "tbFilename";
            this.tbFilename.Size = new System.Drawing.Size(414, 20);
            this.tbFilename.TabIndex = 9;
            // 
            // button3
            // 
            this.button3.Image = global::TuleapHelper.Properties.Resources.Open;
            this.button3.Location = new System.Drawing.Point(511, 151);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(23, 23);
            this.button3.TabIndex = 10;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // cbProjects
            // 
            this.cbProjects.FormattingEnabled = true;
            this.cbProjects.Items.AddRange(new object[] {
            "фыафы",
            "йцукйцу",
            "чсмячсм"});
            this.cbProjects.Location = new System.Drawing.Point(397, 182);
            this.cbProjects.Name = "cbProjects";
            this.cbProjects.Size = new System.Drawing.Size(121, 21);
            this.cbProjects.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(344, 185);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Проект:";
            // 
            // TuleapPatchCreateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 296);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbProjects);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.tbFilename);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chb_takeFromBug);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "TuleapPatchCreateForm";
            this.Text = "Создание патча в Tuleap";
            this.Load += new System.EventHandler(this.TuleapPatchCreateForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox chb_takeFromBug;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox tbFilename;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.ComboBox cbProjects;
    }
}