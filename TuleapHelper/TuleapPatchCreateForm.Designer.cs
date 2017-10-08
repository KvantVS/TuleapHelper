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
            this.btnCreate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lb_Naznacheno = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tbFilename = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.cbProjects = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbVersion = new System.Windows.Forms.TextBox();
            this.lv_libs = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lb_Notificate = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cb_copyPatchToRemote = new System.Windows.Forms.CheckBox();
            this.cb_copyBuildToRemote = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(221, 53);
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
            this.chb_takeFromBug.Location = new System.Drawing.Point(83, 35);
            this.chb_takeFromBug.Name = "chb_takeFromBug";
            this.chb_takeFromBug.Size = new System.Drawing.Size(97, 17);
            this.chb_takeFromBug.TabIndex = 1;
            this.chb_takeFromBug.Text = "Брать из бага";
            this.chb_takeFromBug.UseVisualStyleBackColor = true;
            this.chb_takeFromBug.CheckedChanged += new System.EventHandler(this.chb_takeFromBug_CheckedChanged);
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreate.Location = new System.Drawing.Point(661, 539);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(151, 31);
            this.btnCreate.TabIndex = 2;
            this.btnCreate.Text = "Создать";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(218, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Изменения:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Назначено:";
            // 
            // lb_Naznacheno
            // 
            this.lb_Naznacheno.FormattingEnabled = true;
            this.lb_Naznacheno.Location = new System.Drawing.Point(15, 53);
            this.lb_Naznacheno.Name = "lb_Naznacheno";
            this.lb_Naznacheno.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lb_Naznacheno.Size = new System.Drawing.Size(200, 121);
            this.lb_Naznacheno.TabIndex = 6;
            this.lb_Naznacheno.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(823, 539);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(151, 31);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 183);
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
            this.tbFilename.Location = new System.Drawing.Point(96, 180);
            this.tbFilename.Name = "tbFilename";
            this.tbFilename.Size = new System.Drawing.Size(414, 20);
            this.tbFilename.TabIndex = 9;
            // 
            // button3
            // 
            this.button3.Image = global::TuleapHelper.Properties.Resources.Open;
            this.button3.Location = new System.Drawing.Point(511, 178);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(23, 23);
            this.button3.TabIndex = 10;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // cbProjects
            // 
            this.cbProjects.FormattingEnabled = true;
            this.cbProjects.Location = new System.Drawing.Point(65, 6);
            this.cbProjects.Name = "cbProjects";
            this.cbProjects.Size = new System.Drawing.Size(150, 21);
            this.cbProjects.TabIndex = 11;
            this.cbProjects.SelectedIndexChanged += new System.EventHandler(this.cbProjects_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Проект:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(43, 209);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Версия:";
            // 
            // tbVersion
            // 
            this.tbVersion.Location = new System.Drawing.Point(96, 206);
            this.tbVersion.Name = "tbVersion";
            this.tbVersion.Size = new System.Drawing.Size(100, 20);
            this.tbVersion.TabIndex = 14;
            this.tbVersion.Text = "1.15";
            // 
            // lv_libs
            // 
            this.lv_libs.CheckBoxes = true;
            this.lv_libs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lv_libs.GridLines = true;
            this.lv_libs.Location = new System.Drawing.Point(15, 231);
            this.lv_libs.Name = "lv_libs";
            this.lv_libs.Size = new System.Drawing.Size(931, 287);
            this.lv_libs.TabIndex = 15;
            this.lv_libs.UseCompatibleStateImageBehavior = false;
            this.lv_libs.View = System.Windows.Forms.View.Details;
            this.lv_libs.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lv_libs_ColumnClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Библиотека";
            this.columnHeader1.Width = 180;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Дата изменения";
            this.columnHeader2.Width = 145;
            // 
            // lb_Notificate
            // 
            this.lb_Notificate.FormattingEnabled = true;
            this.lb_Notificate.Location = new System.Drawing.Point(540, 53);
            this.lb_Notificate.Name = "lb_Notificate";
            this.lb_Notificate.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lb_Notificate.Size = new System.Drawing.Size(200, 121);
            this.lb_Notificate.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(537, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Уведомлять:";
            // 
            // cb_copyPatchToRemote
            // 
            this.cb_copyPatchToRemote.AutoSize = true;
            this.cb_copyPatchToRemote.Checked = true;
            this.cb_copyPatchToRemote.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_copyPatchToRemote.Location = new System.Drawing.Point(540, 182);
            this.cb_copyPatchToRemote.Name = "cb_copyPatchToRemote";
            this.cb_copyPatchToRemote.Size = new System.Drawing.Size(511, 17);
            this.cb_copyPatchToRemote.TabIndex = 18;
            this.cb_copyPatchToRemote.Text = "Копировать патч в удаленную папку патчей (buildkrg\\GE-ESEDO\\Сборка на тестировани" +
    "е\\1.1.0)";
            this.cb_copyPatchToRemote.UseVisualStyleBackColor = true;
            // 
            // cb_copyBuildToRemote
            // 
            this.cb_copyBuildToRemote.AutoSize = true;
            this.cb_copyBuildToRemote.Location = new System.Drawing.Point(540, 205);
            this.cb_copyBuildToRemote.Name = "cb_copyBuildToRemote";
            this.cb_copyBuildToRemote.Size = new System.Drawing.Size(266, 17);
            this.cb_copyBuildToRemote.TabIndex = 19;
            this.cb_copyBuildToRemote.Text = "Копировать сборку в buildkrg\\GE-ESEDO\\Builds";
            this.cb_copyBuildToRemote.UseVisualStyleBackColor = true;
            this.cb_copyBuildToRemote.Visible = false;
            // 
            // TuleapPatchCreateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(986, 582);
            this.Controls.Add(this.cb_copyBuildToRemote);
            this.Controls.Add(this.cb_copyPatchToRemote);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lb_Notificate);
            this.Controls.Add(this.lv_libs);
            this.Controls.Add(this.tbVersion);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbProjects);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.tbFilename);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lb_Naznacheno);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCreate);
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
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lb_Naznacheno;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox tbFilename;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.ComboBox cbProjects;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbVersion;
        private System.Windows.Forms.ListView lv_libs;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListBox lb_Notificate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox cb_copyPatchToRemote;
        private System.Windows.Forms.CheckBox cb_copyBuildToRemote;
    }
}