using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuleapHelper
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)  //btnSave
        {
            CommonVariables.projectFolder = tb_projectFolder.Text;

            //using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Settings.txt")))
            //{
            //    sw.WriteLine($"{CommonConstants.settingProjectFolder}={tb_projectFolder.Text}");
            //}

            Configurator.SaveConfig();


            
            //Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //ConfigurationSection sec = cfg.GetSection("appSettings");
            //cfg.AppSettings.Settings.Add(CommonConstants.settingProjectFolder, tb_projectFolder.Text);
            //cfg.Save();

            label4.Visible = true;
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            label4.Visible = false;
            tb_projectFolder.Text = CommonVariables.projectFolder;

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
