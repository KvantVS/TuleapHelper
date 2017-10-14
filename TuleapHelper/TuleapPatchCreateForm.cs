using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuleapHelper
{
    public partial class TuleapPatchCreateForm : Form
    {
        private ListViewColumnSorter lvwColumnSorter;
        private TuleapClasses.Project P;

        TuleapClasses.Artifact Art;
        public int Naznacheno = 0;
        public string Changes = "";
        public string BuildFile = "";
        public string Version = "";
        public int TrackerId;
        public bool ForT1 = false;
        public string messageOut = "";

        public TuleapPatchCreateForm(bool forT1, TuleapClasses.Artifact art=null)
        {
            InitializeComponent();
            Art = art;
            ForT1 = forT1;

            // Create an instance of a ListView column sorter and assign it // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            this.lv_libs.ListViewItemSorter = lvwColumnSorter;
        }

        /// <summary>
        /// Обновляем листбоксы Назначено: и Уведомлять: в зависимости от выбранного Проекта
        /// </summary>
        public void RefreshListboxes()
        {
            lb_Notificate.Items.Clear();
            lb_Naznacheno.Items.Clear();

            P = CommonVariables.globalProjects.Where(gp => ((ComboboxItem)cbProjects.SelectedItem).Value == gp.id).SingleOrDefault();

            // --- Done подгрузить в листбоксы людей из соответствующих полей (FValues?)
            TuleapClasses.Tracker3 t = P.trackers.Where(tr => tr.label.ToLower() == "patches").SingleOrDefault();
            var fieldAssignedTo = t.fields.Where(f => f.label == "Назначено" || f.name == "assigned_to").FirstOrDefault();
            foreach (var fv in fieldAssignedTo.FValues)
            {
                lb_Naznacheno.Items.Add(fv.label + " - " + fv.id);
            }

            var fieldNotificateTo = t.fields.Where(f => f.label == "Уведомлять участников обновления" || f.name == "field_4").FirstOrDefault();
            foreach (var fv in fieldNotificateTo.FValues)
            {
                lb_Notificate.Items.Add(new ComboboxItem { Text = fv.label, Value = fv.id });
                // ---Выбираем сразу кого надо в этих листбоксах
                if (fv.label.Contains("sergey_lebedev")
                    || fv.label.Contains("vadim_sidorchuk")
                    || fv.label.Contains("valeriy_suhorukov")
                    || fv.label.Contains("victor_vernigora")
                    ) lb_Notificate.SelectedIndex = lb_Notificate.Items.Count - 1;
            }
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
            P = CommonVariables.globalProjects.Where(gp => Art.project.id == gp.id).SingleOrDefault();

            // --- Подгружаем проекты в комбобокс на форме2
            foreach (var pr in CommonVariables.globalProjects)
            {
                cbProjects.Items.Add(new ComboboxItem { Text = pr.label, Value = pr.id });
                if (pr.id == Art.project.id) cbProjects.SelectedIndex = cbProjects.Items.Count - 1;
            }
            
            //foreach (var pr in cbProjects.Items)
            //{
            //    if ((pr as ComboboxItem).Value == Art.project.id)
            //    {
            //        cbProjects.SelectedIndex = cbProjects.Items.IndexOf(pr);
            //        break;
            //    }
            //}

            RefreshListboxes();

            // --- загружаем библиотеки в список ListView
            try
            {
                string dbg = "debug";
                string rls = "Release";
                string obj = "obj";
                string[] libFolders = Directory.GetDirectories(CommonVariables.projectFolder + @"\ESEDO_Sources").Where(d => !d.Contains("TestProjects")).ToArray(); // todo: sEsedoProjectFolder сохранять в настройки в файле и загружать из него

                for (int i = 0; i < libFolders.Count(); i++)
                {
                    string libName = libFolders[i].Substring((CommonVariables.projectFolder + @"\ESEDO_Sources").Length + 1);  // только имя либы (без пути и без .dll)

                    // --- выясняем, какую библиотеку брать: из Debug или Release конфигурации (чья дата обновления позже, та и нужна)
                    string[] extensions = { ".dll", ".pdb" };
                    string dllFileNameDebug = Path.Combine(libFolders[i], obj, dbg, libName + extensions[0]);
                    string dllFileNameRelease = Path.Combine(libFolders[i], obj, rls, libName + extensions[0]);

                    DateTime dtChangedDebug = File.GetLastWriteTime(dllFileNameDebug);  //todo: try except (NotSupportedException)
                    DateTime dtChangedRelease = File.GetLastWriteTime(dllFileNameRelease);
                    string neededFolder = (dtChangedDebug > dtChangedRelease) ? dbg : rls;
                    DateTime dtChanged = (dtChangedDebug > dtChangedRelease) ? dtChangedDebug : dtChangedRelease;

                    neededFolder = Path.Combine(libFolders[i], obj, neededFolder);

                    string dllFullFileName = Path.Combine(neededFolder, libName + extensions[0]);
                    string pdbFullFileName = Path.Combine(neededFolder, libName + extensions[1]);
                    
                    ListViewItem li = new ListViewItem();
                    li.Text = libName;
                    li.SubItems.Add(dtChanged.ToLongDateString() + " " + dtChanged.ToLongTimeString()); //.Tag = dtChanged; // done можно убрать Tag для сабитема, и при сортировке дату брать из ((EsedoLib)li.Tag).ChangeDateTime
                    li.Tag = new EsedoLib
                    {
                        Name = libName,
                        FullPathFileNameDLL = dllFullFileName,
                        FullPathFileNamePDB = pdbFullFileName,
                        ChangeDateTime = dtChanged
                    };
                    lv_libs.Items.Add(li);
                }

                lvwColumnSorter.SortColumn = 1;
                lvwColumnSorter.Order = SortOrder.Descending;
                lv_libs.Sort();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lv_libs_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.lv_libs.Sort();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            List<int> naznachenoList = new List<int>();
            List<int> notificateList = new List<int>();
            List<string> libsToPatch = new List<string>();
            List<EsedoLib> esedoLibsToPatch = new List<EsedoLib>();
            string version = tbVersion.Text;
            string localPatchFullFilename = "";
            string patchFilename = "";

            // --- инициализация комбобоксов "назначено" и "уведомлять"
            if (chb_takeFromBug.Checked && Art != null)
                naznachenoList.Add(Art.submitted_by);
            else
                foreach (var si in lb_Naznacheno.SelectedItems) naznachenoList.Add((si as ComboboxItem).Value);

            foreach (var si in lb_Notificate.SelectedItems) notificateList.Add((si as ComboboxItem).Value);
            #region oldcode
            //Naznacheno = naznacheno;
            //Changes = textBox1.Text;
            //BuildFile = tbFilename.Text;
            //this.DialogResult = DialogResult.OK;
            #endregion

            // --- запоминаем выбранные для патча библиотеки
            for (int i = 0; i < lv_libs.Items.Count; i++)
            {
                if (lv_libs.Items[i].Checked) libsToPatch.Add(lv_libs.Items[i].Text);
                if (lv_libs.Items[i].Checked) esedoLibsToPatch.Add((EsedoLib)lv_libs.Items[i].Tag);
            }

            // ------------------------
            // Стряпаем имя файла патча
            // ------------------------

            // done Conditions for T1 - T2
            var patchFilenameTemplate = "";
            if (ForT1)
                patchFilenameTemplate = ""; //todo хз, какое имя для патча на Т1
            else
                patchFilenameTemplate = CommonVariables.patchFilenameTemplate_hotfix;
            
            // --- для Hotfix (Т2)
            if (! ForT1)
            {
                //DONTUSE-ITS-A-TEST_update_{version}_{sdate}_{patchNumber} of {username} ({slibs}).zip
                var patchNumber = 1;
                var dummy_patchFilename = "";
                dummy_patchFilename = patchFilenameTemplate.Replace("{version}", version);
                dummy_patchFilename = dummy_patchFilename.Replace("{sdate}", DateTime.Now.Date.ToString("yyyy.MM.dd"));
                dummy_patchFilename = dummy_patchFilename.Replace("{username}", "Vadim.Sidorchuk"); //todo user
                
                string slibs = string.Join("_",
                    //(CommonConstants.libsShortnames.Where(kvp => libsToPatch.Contains(kvp.Key)).Select(kvp => kvp.Value)));
                    (CommonConstants.libsShortnames.Where(kvp => esedoLibsToPatch.Select(l => l.Name).Contains(kvp.Key)).Select(kvp => kvp.Value)));
                dummy_patchFilename = dummy_patchFilename.Replace("{slibs}", slibs);
                
                // --- работаем с номером патча (если такой патч есть, то прибавляем +1) (todo: галочку "заменить существующий патч?")
                do {
                    patchFilename = dummy_patchFilename.Replace("{patchNumber}", patchNumber.ToString());
                    localPatchFullFilename = Path.Combine(CommonVariables.localPatchFolder, patchFilename);
                    patchNumber++;
                } while (File.Exists(localPatchFullFilename));
            }
            //todo: галочку "использовать существующий патч на эту дату, или создать новый?"
            
            // --- Заранее готовим список, куда копировать патч
            List<string> pathsToCopy = new List<string>();
            pathsToCopy.Clear();

            // --- Если стоит галка "копировать на buildkrg", То добавляем путь в список. Путь - в Общих Переменных
            if (cb_copyPatchToRemote.Checked)
                pathsToCopy.Add(CommonVariables.remotePatchFolder_hotfix);
            // Т1 здесь не обрабатываем пока (мы в блоке if (! ForT1))
            //string mess;
            var b = FileSystem.CreatePhysicalPatch(out string mess, esedoLibsToPatch, patchFilename, false, pathsToCopy);

            // --- Создаем патч-артефакт в тулипе
            if (b)
            {
                mess = "";
                // (закоментировано, чтобы не портить ESEDO-проекты в тулипе (если есть возможность удалить артефакт - можно тестить)
                // var s = TuleapLevel.CreatePatchInTuleap(out mess, false, libsToPatch, naznachenoList, notificateList, TrackerId, null, tbFilename.Text, "", tbVersion.Text, patchFilename, Art);
            }

            //todo обновлять сам баг тоже

            // --- Обновляем баг (добавляем ссылку на тулиповский патч, (ставим статус на тестировании, если надо))

            //Dictionary<string, object> fieldValues = new Dictionary<string, object>();
            //var LinkFieldId = Art.tracker.fields.Where(f => f.label == "Artifact links" || f.name == "artifact_links" || f.type == "art_link").FirstOrDefault().field_id;
            //fieldValues.Add(new Dictionary<string, object>
            //{
            //    { "field_id", AssignedToFieldId },                    //22306 - для 544, 48968 - для 2016 ГП
            //    //{ "bind_value_ids", new string[] {n} }
            //    { "bind_value_ids", list_assignedTo.ToArray() }
            //});

            //TuleapLevel.UpdateArtifact(Art, null);

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void chb_takeFromBug_CheckedChanged(object sender, EventArgs e)
        {
            lb_Naznacheno.Enabled = lb_Naznacheno.Visible = !((sender as CheckBox).Checked);
        }

        private void cbProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshListboxes();
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
