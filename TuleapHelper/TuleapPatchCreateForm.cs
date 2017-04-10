using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuleapHelper
{
    public partial class TuleapPatchCreateForm : Form
    {
        TuleapClasses.Artifact Art;
        public int Naznacheno = 0;
        public string Changes = "";
        public string BuildFile = "";

        public TuleapPatchCreateForm(TuleapClasses.Artifact art=null)
        {
            InitializeComponent();
            Art = art;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var naznacheno = 0;
            if( chb_takeFromBug.Checked && Art != null)
            {
                // done Взять Назначено из бага                
                naznacheno = Art.submitted_by;
            }
            Naznacheno = naznacheno;
            Changes = textBox1.Text;
            BuildFile = tbFilename.Text;

            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var r = openFileDialog1.ShowDialog();
            if (r != DialogResult.Cancel)
            {
                tbFilename.Text = openFileDialog1.FileName;
                //todo: Смотреть в BuildKrg каким-то макаром.... есть REST API: https://confluence.jetbrains.com/display/TCD10/REST+API
            }
        }

        private void TuleapPatchCreateForm_Load(object sender, EventArgs e)
        {
            // --- Подгружаем проекты в комбобокс на форме2
            foreach(var p in CommonVariables.globalProjects)
            {
                cbProjects.Items.Add(new ComboboxItem { Text = p.label, Value = p.id });
            }


            var seekingProject = new ComboboxItem { Text = Art.project.label, Value = Art.project.id };
            var i = cbProjects.Items.IndexOf(seekingProject);
            cbProjects.SelectedIndex = (i == -1) ? 0 : i;
            
        }
    }

    public class ComboboxItem
    {
        public string Text { get; set; }
        public int Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
