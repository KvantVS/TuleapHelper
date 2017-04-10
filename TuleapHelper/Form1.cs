using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Web.Services.Protocols;
using System.Web.Services;
using System.Net;
using System.Web;
using System.IO;
using Newtonsoft.Json;

using System.Configuration;
using System.Collections.Specialized;
using System.Runtime.InteropServices.ComTypes;
using System.Web.UI.WebControls;
using static TuleapHelper.RequestSender;
using CheckBox = System.Windows.Forms.CheckBox;
using System.Reflection;

namespace TuleapHelper
{
    public partial class Form1 : Form
    {
        public const string SBindValueIds = "bind_value_ids";
        public const string SValue = "value";
        public const string SLinks = "links";
        private const string SFieldId = "field_id";
        private const string STemplateUserSelfId = "%UserSelfId%";
        private const string STemplateTaskDetailInfo = "%TaskDetailInfo%";
        private const string STemplateTaskPlanHours = "%TaskPlanHours%";
        private const string STemplateTaskFactHours = "%TaskFactHours%";
        private const string STemplateTaskStatus = "%TaskStatus%";
        private const string STemplateBugId = "%BugId%";
        private const string STemplateTaskStartDate = "%TaskStartDate%";
        private const string STemplateTaskEndDate = "%TaskEndDate%";

        //public static UserInfo globalUserInfo = new UserInfo();// { user_id = "171", token = "207f8dd40f8732fd0623a4b567341400" } ;
        //public static string tuleapHost = "https://tuleap.nat.kz/api/";
        
        //public byte callCounter = 0;

        public string sZipFileNameTemplate = "";
        public static string sWorkingFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.ProductName);
        public static string sZipTempFolder = Path.Combine(sWorkingFolder, "zippatch");
        public string sAttr = "";
        public string sEsedoProjectFolder = @"C:\NAT_ESEDO\src\CopiedManually\git-esedo\ESEDO_Sources";
        public string remotePatchFolder = @"\\buildkrg\GE-ESEDO\Сборка на тестирование\1.1.0";

        public bool patchButton = false;
        //public TuleapClasses.Artifact currentBug;

        List<string> libsToPatch = new List<string>();
        Dictionary<string, string> libsShortnames = new Dictionary<string, string>
        {
            {"JS.EDO.AccountingIntegration", "AI"},
            {"JS.EDO.Administration", "ADM" },
            {"JS.EDO.Assignments", "ASM" },
            {"JS.EDO.Assignments.Interaction", "ASMInt" },
            {"JS.EDO.Blanks", "BL" },
            {"JS.EDO.CertifiedExpertOrganizations", "CEO" },
            {"JS.EDO.CommonInfo", "CI" },
            {"JS.EDO.Controls", "CON" },
            {"JS.EDO.Dictionaries", "DIC" },
            {"JS.EDO.DictionariesLogic", "DICLogic" },
            {"JS.EDO.DictionariesSync", "DICSync" },
            {"JS.EDO.DigitalSignature", "DS" },
            {"JS.EDO.Equipments", "EQU" },
            {"JS.EDO.Host", "HOST" },
            {"JS.EDO.ImportExcelData", "IED" },
            {"JS.EDO.Indicators", "IND" },
            {"JS.EDO.Interaction", "INT" },
            {"JS.EDO.Interaction.AccreditedOrgs", "IntAO" },
            {"JS.EDO.InvolvementExperts", "InvExp" },
            {"JS.EDO.Jobs", "JOB" },
            {"JS.EDO.Notification", "NOT" },
            {"JS.EDO.Planning", "PLN" },
            {"JS.EDO.Reports", "REPS" },
            {"JS.EDO.Search", "SRC" },
            {"JS.EDO.SearchMaterialsPlugin", "SMP" },
            {"JS.EDO.SearchMaterialWordAddinWeb", "SMWAW" },
            {"JS.EDO.Service", "SVC" },
            {"JS.EDO.Templates", "TMP" },
            {"JS.EDO.WordEditor", "WRD" },
            {"TestProjects", "TEST" }
        };

        public struct TrackerField
        {
            /// <summary>
            /// id поля в системе Тулип.
            /// </summary>
            public int FieldId;

            /// <summary>
            /// Имя параметра, представляющее значение ("bind_value_ids" / "value" / "links").
            /// </summary>
            public string ValueParamName;

            /// <summary>
            /// Значение по умолчанию. Может содержать шаблоны для подстановки в виде %param%
            /// </summary>
            public object ValueByDefault;

            /// <summary>
            /// Описание поля. Взят текст с HTML-страницы. Пока не используется
            /// </summary>
            public string Description;
        }

        
        /// <summary>
        /// ключ - id трекера, значение - список необходимых полей (пока не решил, имя или id) вообще лучше id
        /// </summary>
        public Dictionary<int, List<TrackerField>> DictRequiredFields = new Dictionary<int, List<TrackerField>>
        {
            { 548, new List<TrackerField>  // Tasks_2016
                {
                    new TrackerField {FieldId = 22456, ValueParamName = SBindValueIds, ValueByDefault = STemplateUserSelfId, Description = "Назначена" },
                    new TrackerField {FieldId = 22458, ValueParamName = SBindValueIds, ValueByDefault = "16297, 12345", Description = "Тип"}, // "16297 Стабилизация"
                    new TrackerField {FieldId = 22459, ValueParamName = SBindValueIds, ValueByDefault = "16312", Description = "Приоритет"}, //16312 = 5 - Medium
                    new TrackerField {FieldId = 22457, ValueParamName = SValue, ValueByDefault = $"Устранение bugs #{STemplateBugId}", Description = "Краткое описание задачи"},
                    new TrackerField {FieldId = 22448, ValueParamName = SValue, ValueByDefault = STemplateTaskDetailInfo, Description = "Детальное описание задачи"},
                    new TrackerField {FieldId = 22461, ValueParamName = SValue, ValueByDefault = STemplateTaskPlanHours, Description = "План, чел*час"},
                    new TrackerField {FieldId = 22462, ValueParamName = SValue, ValueByDefault = STemplateTaskFactHours, Description = "Факт, чел*час"},
                    new TrackerField {FieldId = 22464, ValueParamName = SValue, ValueByDefault = STemplateTaskStatus, Description = "Статус"}, // 16315 - в Работе, 16316-Приост., 16317 - Завершена
                    new TrackerField {FieldId = 22465, ValueParamName = SValue, ValueByDefault = STemplateTaskStartDate, Description = "Дата начала"},  //DateTime.Now.ToString("yyyy-MM-dd") или 2016-01-15T00:00:00+06:00
                    new TrackerField {FieldId = 22466, ValueParamName = SValue, ValueByDefault = STemplateTaskEndDate, Description = "Дата завершения"},
                    new TrackerField {FieldId = 22468, ValueParamName = SLinks, ValueByDefault = STemplateBugId, Description = "Artifact Link"},
                }
            },
            { 557, null }
        };
        //540 - баги 2016

        public Form1()
        {
            InitializeComponent();
        }
        
        private void AppExit(object sender, EventArgs e)
        {
            try
            {
                System.Net.HttpStatusCode rc;// = HttpStatusCode.Accepted;
                var r = HttpLevel.HttpDeleteJSON($"tokens/{CommonVariables.globalUserInfo.token}", "", true, out rc);
                Log($"Удаляем токен ... {rc}");
            }
            catch
            { }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.ApplicationExit += new EventHandler(AppExit);

            // --- загрузка настроек
            Log("Загружаем настройки...");

            try
            {
                CommonVariables.globalUserInfo.user_id = ConfigurationManager.AppSettings.Get("user_id");
                CommonVariables.globalUserInfo.token = ConfigurationManager.AppSettings.Get("authToken");
                CommonVariables.tuleapHost = tb_Host.Text + "/api/";
            }
            catch (Exception ex)
            {
                Log("Ошибка: " + ex);
            }
            
            // --- Загружаем библиотеки для чекбоксов
            string[] libFolders = Directory.GetDirectories(sEsedoProjectFolder); // todo: sEsedoProjectFolder сохранять в настройки в файле и загружать из него
            var i = 0;
            foreach(Control c in panel_Libs.Controls)
            {
                if (c is CheckBox)
                {
                    (c as CheckBox).Text = libFolders[i].Substring(sEsedoProjectFolder.Length + 1);
                    i++;
                    if (i >= libFolders.Count())
                        break;
                }
            }

            // --- Создаем рабочие директории, если их не существует
            //if (!Directory.Exists(sWorkingFolder))
            //    Directory.CreateDirectory(sWorkingFolder);

            if (!Directory.Exists(sZipTempFolder))
                Directory.CreateDirectory(sZipTempFolder);
            
            // --- Для сертификата https
            //System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (
            //    object s,
            //    System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            //    System.Security.Cryptography.X509Certificates.X509Chain chain,
            //    System.Net.Security.SslPolicyErrors sslPolicyErrors)
            //{ return true; };

            Log("OK");
        }


        

        //static string HttpGetJSON(string url, string json, bool useToken)
        //{
        //    string query = "query=" + System.Web.HttpUtility.UrlEncode(json);
        //    string resultUrl = tuleapHost + url + "?" + query;
        //    //resultUrl = System.Web.HttpUtility.UrlEncode(query);
        //    HttpWebRequest req = WebRequest.Create(resultUrl) as HttpWebRequest;
        //    if (useToken)
        //    {
        //        req.Headers.Add("X-Auth-Token: " + globalUserInfo.token);
        //        req.Headers.Add("X-Auth-UserId: " + globalUserInfo.user_id);
        //    }
        //    string result = null;
        //    using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
        //    {
        //        StreamReader reader = new StreamReader(resp.GetResponseStream());
        //        result = reader.ReadToEnd();
        //    }
        //    return result;
        //}

        //public static string HttpDeleteJSON(string url, string json, bool useToken, out System.Net.HttpStatusCode responseCode)
        //{
        //    HttpWebRequest req = WebRequest.Create(new Uri(tuleapHost + url)) as HttpWebRequest;
        //    req.Method = "DELETE";
        //    req.ContentType = "application/json";  //req.ContentType = "application/x-www-form-urlencoded";

        //    if (useToken)
        //    {
        //        req.Headers.Add("X-Auth-Token: " + globalUserInfo.token);
        //        req.Headers.Add("X-Auth-UserId: " + globalUserInfo.user_id);
        //    }

        //    using (StreamWriter sw = new StreamWriter(req.GetRequestStream()))
        //    {
        //        sw.Write(json);
        //    }

        //    // Pick up the response:
        //    string result = null;
        //    using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
        //    {
        //        responseCode = resp.StatusCode;
        //        StreamReader reader = new StreamReader(resp.GetResponseStream());
        //        result = reader.ReadToEnd();
        //    }

        //    return result;
        //}

        //public string JsonCreate(Dictionary<string, object> jsonNodes, StringBuilder sb)
        //{
        //    using (StringWriter sw = new StringWriter(sb))
        //    using (JsonTextWriter jw = new JsonTextWriter(sw))
        //    {
        //        JsonCreate(jsonNodes, jw);
        //    }
        //    return sb.ToString();
        //}

        //public void JsonCreate(Dictionary<string, object> jsonNodes, JsonTextWriter jw)
        //{
        //    //todo: проверку на пустой dictionary;
        //    //string json = "{'tracker': {'id':'557'}, 'values': [{'field_id':22456,'value':'asfasdf'}, {} ]}";
        //    //StringBuilder sb = new StringBuilder();
        //    //using (StringWriter sw = new StringWriter(sb))
        //    //using (JsonTextWriter jw = new JsonTextWriter(sw))
        //    {
        //        jw.WriteStartObject();
        //        foreach (var j in jsonNodes)
        //        {
        //            jw.WritePropertyName(j.Key, true);
        //            if (!(j.Value is string) && !(j.Value is int))
        //            {
        //                if (j.Value is Array)
        //                {
        //                    Type t2 = j.Value.GetType().GetElementType();
        //                    jw.WriteStartArray();
        //                    foreach (var el in (Array) j.Value)
        //                    {
        //                        jw.WriteValue(el);
        //                    }
        //                    jw.WriteEndArray();
        //                }

        //                //else if (j.Value is IEnumerable)
        //                //{ }
                        
        //                else
        //                {
        //                    Type valueType = j.Value.GetType();
        //                    if (valueType.IsGenericType)
        //                    {
        //                        var baseType = valueType.GetGenericTypeDefinition();

        //                        if (baseType == typeof (KeyValuePair<,>))
        //                        {
        //                            //jw.WriteStartObject();

        //                            //Type t1 = j.Value.GetType().GetProperty("Key").PropertyType;
        //                            //Type t2 = j.Value.GetType().GetProperty("Value").PropertyType;
        //                            Type t1 = j.Value.GetType().GenericTypeArguments[0];
        //                            Type t2 = j.Value.GetType().GenericTypeArguments[1];
        //                            var kvpPropertyType = typeof(KeyValuePair<,>);
        //                            var myKvpPropertyType = kvpPropertyType.MakeGenericType(t1, t2);
        //                            dynamic v1 = myKvpPropertyType.GetProperty("Key").GetValue(j.Value, null);
        //                            dynamic v2 = myKvpPropertyType.GetProperty("Value").GetValue(j.Value, null);

        //                            var dict = new Dictionary<string, object>();
        //                            dict.Add(v1.ToString(), v2);
        //                            JsonCreate(dict, jw);

        //                            //jw.WritePropertyName(v1, true);
        //                            //jw.WriteValue(v2);
        //                            //jw.WriteEndObject();
        //                        }
        //                        else if (baseType == typeof (List<>))
        //                        {
        //                            Type t = j.Value.GetType().GenericTypeArguments[0];
        //                            var listPropertyType = typeof (List<>);
        //                            var myListPropertyType = listPropertyType.MakeGenericType(t);
        //                            var props = myListPropertyType.GetProperties();
        //                            dynamic count = props[1].GetValue(j.Value);
        //                            //dynamic val3 = props[2].GetValue(j.Value);

                                    

        //                            jw.WriteStartArray();
        //                            foreach (var dict in (j.Value as List<object>))
        //                            {
        //                                JsonCreate((Dictionary<string,object>)dict, jw);
        //                            }
        //                            jw.WriteEndArray();

        //                        }
        //                    }
        //                }
        //            }
        //            else
        //                jw.WriteValue(j.Value);
        //        }
        //        jw.WriteEndObject();
        //    }
        //    //return sb.ToString();            
        //}
        
        public void TranscodeJson(object json)
        {
            Log("JSON object: " + json);
        }

        public void Log(string message, bool newLine=true)
        {
            //var ending = newLine ? Environment.NewLine : string.Empty;
            //textBox1.Text += $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}\t{message}{ending}";
            textBox1.AppendText($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}\t{message}\r\n");
        }
                                          
        public bool InitiateWorkWithTuleap()
        {
            //todo постраничная загрузка артифактов - проверить, чтобы загружались все артифакты
            
            // --- 1. Получим проекты и трекеры проектов
            var j = HttpLevel.HttpGet("projects", true); ;
            if (j == null)
            {
                Log("Не удалось получить проекты.");
                return false;
            }

            try
            {
                // --- подгружаем все проекты
                CommonVariables.globalProjects = JsonConvert.DeserializeObject<TuleapClasses.Project[]>(j);

                foreach (TuleapClasses.Project p in CommonVariables.globalProjects)
                {
                    Log($"Загружен новый проект: id: {p.id}, label: {p.label}, shortname: {p.shortname}");
                     
                    // --- получаем трекеры проекта
                    try
                    {
                        j = HttpLevel.HttpGet("projects/" + p.id.ToString() + "/trackers", true);
                    }
                    catch(Exception ex)
                    {
                        Log(ex.ToString());
                    }
                    
                    List<TuleapClasses.Tracker3> trackers = JsonConvert.DeserializeObject<List<TuleapClasses.Tracker3>>(j);
                    p.trackers = trackers;

                    foreach (TuleapClasses.Tracker3 t in p.trackers)
                    {
                        Log($"Загружен трекер: id: {t.id}, description: {t.description}, label: {t.label}, item_name: {t.item_name}");
                        //t.fields.ForEach(f => Log(f.field_id.ToString() + " - " + f.label));

                        foreach (var f in t.fields)
                        {
                            if (f.values != null)
                            {
                                if (f.values is Newtonsoft.Json.Linq.JArray)
                                {
                                    var vals = ((Newtonsoft.Json.Linq.JArray)f.values).ToObject<TuleapClasses.FieldValues[]>();
                                    f.FValues = vals;
                                }
                                //Log(f.values.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //JsonConvert.DeserializeObject(j);
                Log(ex.ToString());
                TranscodeJson(j);
            }
            
            return true;
        }
        
        private void button11_Click(object sender, EventArgs e)
        {
            var s = HttpLevel.HttpGet("artifacts/" + tbArtifact.Text, true);  // + "?values_format=collection"
            textBox1.Text += s;

            TranscodeJson(JsonConvert.DeserializeObject(s));
        }

        private void btn_GetAllBugs_Click(object sender, EventArgs e)
        {
            var d = new Dictionary<string, object>();
            string jsonq = "";

            // --- Вытаскиваем баги. ID проектов нам заранее известны (181 и 182), если что - добавить. У каждого свой трекер багов (540 и 549 соотв.)
            foreach (var p in CommonVariables.globalProjects)
                foreach (var t in p.trackers)
                {
                    if (t.label == "Bugs")
                    {
                        d.Clear();
                        jsonq = "";

                        var fieldNaznachenaId = t.fields.Where(f => f.label == "Назначена").FirstOrDefault().field_id.ToString();

                        var fieldResolution = t.fields.Where(f => f.label == "Резолюция").FirstOrDefault();
                        var fieldResolutionId = fieldResolution.field_id.ToString();
                        var fieldResolutionValues = fieldResolution.FValues.Where(f => f.label == "Принято в работу" || f.label == "Повторная").Select(f => f.id).ToArray();

                        var fieldCurStatus = t.fields.Where(f => f.label == "Текущий статус").FirstOrDefault();
                        var fieldCurStatusId = fieldCurStatus.field_id.ToString();
                        var fieldCurStatusValues = fieldCurStatus.FValues.Where(f => f.label == "Зарегистрировано" || f.label == "Доработка / исправление").Select(f=>f.id).ToArray();
                                                                                                                                
                        d.Add(fieldNaznachenaId, CommonVariables.globalUserInfo.user_id);
                        d.Add(fieldResolutionId, new Dictionary<string, object>
                        {
                            { "operator", "contains" },
                            { "value", fieldResolutionValues}
                        });
                        d.Add(fieldCurStatusId, new Dictionary<string, object>
                        {
                            { "operator", "contains" },
                            { "value", fieldCurStatusValues }
                        });

                        jsonq = CommonFunctions.JsonCreate(d, new StringBuilder());
                        var j = HttpLevel.HttpGetJSON($"trackers/{t.id}/artifacts", jsonq, true);
                        TuleapClasses.Artifact[] artifacts = JsonConvert.DeserializeObject<TuleapClasses.Artifact[]>(j);

                        foreach (var a in artifacts)
                        {
                            var jsonArtifact = HttpLevel.HttpGet(a.uri, true);
                            TuleapClasses.Artifact art = JsonConvert.DeserializeObject<TuleapClasses.Artifact>(jsonArtifact);

                            // values у артифакта - это набор полей, с их айдишником в теге field_id, и их значением в теге value
                            ListViewItem li = new ListViewItem(a.id.ToString() + " - " + art.values.Where(v => v.label == "Краткое описание").FirstOrDefault().value);
                            li.Tag = art;
                            li.SubItems.Add(p.label); //art.project.label);
                            li.SubItems.Add(a.submitted_on);
                            li.SubItems.Add(a.last_modified_date);
                            listView1.Items.Add(li);
                        }
                    }
                }

            
            /*
            return;    
            // --- Проект 181 ---
            // Запрос:
            // 22124 - поле Назначена. Значения: айдишник юзера или массив айдишников юзеров
            // 22120 - поле "Резолюция". Значения: 16032 - Принято в работу, 16033 - Повторная
            // 22121 - поле "Текущий статус". Значения: 16039 - "Зарегистрировано", 16040 - "Доработка\исправление"
            //string jsonq = "{\"22124\":" + CommonVariables.globalUserInfo.user_id + ",\"22120\":{\"operator\":\"contains\",\"value\":[\"16032\",\"16033\"]},\"22121\":{\"operator\":\"contains\",\"value\":[\"16039\",\"16040\"]}}";

            // 540 - айди трекера "Bugs" для проекта EISKVE-R-2016
            
            var j = HttpLevel.HttpGetJSON("trackers/540/artifacts", jsonq, true);

            TuleapClasses.Artifact[] artifacts = JsonConvert.DeserializeObject<TuleapClasses.Artifact[]>(j);

            foreach(var a in artifacts)
            {
                var artquery = HttpLevel.HttpGet(a.uri, true);
                TuleapClasses.Artifact art = JsonConvert.DeserializeObject<TuleapClasses.Artifact>(artquery);

                ListViewItem li = new ListViewItem(a.id.ToString() + " - " + art.values.Where(v => v.field_id == 22108).FirstOrDefault().value.ToString());
                li.Tag = art;
                li.SubItems.Add(art.project.label);
                listView1.Items.Add(li);
            }

            // --- Проект 182 ---
            j = "";
            jsonq = "{\"22527\":" + CommonVariables.globalUserInfo.user_id + ",\"22523\":{\"operator\":\"contains\",\"value\":[\"16334\",\"16335\"]},\"22524\":{\"operator\":\"contains\",\"value\":[\"16341\",\"16342\"]}}";

            j = HttpLevel.HttpGetJSON("trackers/549/artifacts", jsonq, true);
            //textBox1.Text += j.ToString();

            TuleapClasses.Artifact[] artifacts182 = JsonConvert.DeserializeObject<TuleapClasses.Artifact[]>(j);

            foreach (var a in artifacts182)
            {
                var artquery = HttpLevel.HttpGet(a.uri, true);
                //textBox1.Text += artquery;

                TuleapClasses.Artifact art = JsonConvert.DeserializeObject<TuleapClasses.Artifact>(artquery);
                ListViewItem li = new ListViewItem(a.id.ToString() + " - " + art.values.Where(v => v.field_id == 22511).FirstOrDefault().value.ToString());
                li.Tag = art;
                listView1.Items.Add(li);
            }
            */
        }


        private void button13_Click(object sender, EventArgs e)
        {
            HttpStatusCode respCode;
            respCode = TuleapLevel.Authorize(CommonVariables.tuleapHost, tb_User.Text, tb_Pass.Text);
            if (respCode == HttpStatusCode.OK || respCode == HttpStatusCode.Created)
                Log($"Ответ: {respCode}. Авторизован пользователь {tb_User.Text}. user_id: {CommonVariables.globalUserInfo.user_id}, token: {CommonVariables.globalUserInfo.token}");
            else
                Log($"Ответ: {respCode}");
        }

        /// <summary>
        /// Получаем значение для поля при создании артефакта. 
        /// </summary>
        /// <param name="artifactValue">Значение параметра, которое будет обработано</param>
        /// <param name="valueParamName">Тип параметра (bind_ids, value, links), в зависимости от которого будет опр.образом обработано результ.значение</param>
        /// <param name="bugLinkId"></param>
        /// <param name="dictParams"></param>
        /// <param name="isStart"></param>
        /// <returns></returns>
        public object GetArtifactValueByType(object artifactValue, string valueParamName, int bugLinkId, Dictionary<string, object> dictParams, bool isStart)
        {
            var value = ReplaceValueTemplate(valueParamName, bugLinkId, dictParams);

            switch (valueParamName)
            {
                case SLinks:
                   //return new List<Dictionary<string, object>> {new Dictionary<string, object> {{}, {}}};
                    break;

                case SBindValueIds:
                    return value.Split(',');

                case SValue:
                    return value;

                default:
                    break;
            }




            /*
            var stringValue = artifactValue as string;
            if (stringValue != null)
            {
                stringValue = stringValue.Replace("%UserSelfId%", globalUserInfo.user_id);
                stringValue = stringValue.Replace()
                // todo;
            }



            if (artifactValue is int)
                return (int) artifactValue;  // todo: неправильно
            */

            return "";
        }

        private string ReplaceValueTemplate(string value, int bugLinkId, Dictionary<string, object> dictParams)
        {
            //var s = value.Replace(STemplateUserSelfId, globalUserInfo.user_id);
            //s = s.Replace(STemplateBugId, bugLinkId.ToString());
            var s = value;
            foreach (var kvp in dictParams)
            {
                s = s.Replace(kvp.Key, (string)kvp.Value);
            }
            return s;
        }


        

        /// <summary>
        /// Создаем артефакт в тулипе (надо потом разделить на отдельные функции: создание патча, создание таска)
        /// </summary>
        /// <param name="trackerId">Id трэкера, в котором создаем артефакт. Определяет вид артефакта: таск, баг, патч.</param>
        /// <param name="bugLinkId">Свзяь с багом. Id бага, с которым следует связать создаваемый артефакт.</param>
        /// <param name="dictParams">Параметры (значения полей) артефакта</param>
        /// <param name="isStart">Если тип создаваемого артефакта == таск, то определяет, Начало ли это работы над багом</param>
        public void CreateArtifactInTuleap(int trackerId, Dictionary<string, object> dictParams, bool isStart, int bugLinkId=0)  // CREATETASK
        {
            List<object> fieldValues = new List<object>();

            // --- Смотрим обязательные поля для этого трекера, берем их имена, их значения, и преобразуем их в json, предварительно отформатировав значения
            foreach (var trackField in DictRequiredFields[trackerId])
            {
                fieldValues.Add(new Dictionary<string, object>
                {
                    { SFieldId, trackField.FieldId },
                    { trackField.ValueParamName, GetArtifactValueByType(trackField.ValueByDefault, trackField.ValueParamName, bugLinkId, dictParams, true) }
                });
            }

            var jsonNodes = new Dictionary<string, object>
            {
                {"tracker", new KeyValuePair<string, int>("id", trackerId) },
                {"values", fieldValues }
            };

            var json = CommonFunctions.JsonCreate(jsonNodes, new StringBuilder());
            Log(json);


            /*
            // -------------------------
            // --- FOR TEST - tuleap.net - tasks
            // -------------------------
            List<object> fieldValues = new List<object>();
            fieldValues.Add(new Dictionary<string, object> { { "field_id", "1358" }, { "value", "Vadim_Sidorchuk_Test" } });
            fieldValues.Add(new Dictionary<string, object> { { "field_id", "1359" }, { "value", "Description_Hello_all!" } });
            fieldValues.Add(new Dictionary<string, object> { { "field_id", "1371" }, { "bind_value_ids", new string[] {"105", "120"} } });  // можно int[], можно и string[]
            fieldValues.Add(new Dictionary<string, object> { { "field_id", "1373" }, { "bind_value_ids", new string[] {"1767"} } });

            // --- artifact Link
            List<object> v = new List<object>();
            v.Add(new Dictionary<string, object> { { "id", 9607 } });
            fieldValues.Add(new Dictionary<string, object> { { "field_id", "2906" }, { "links", v } });  // Its works!
            
            // --- finally form json query
            var jsonNodes = new Dictionary<string, object>
            {
                {"tracker", new KeyValuePair<string, int>("id", trackerId) },
                {"values", fieldValues }
            };

            var sb = new StringBuilder();
            var json = JsonCreate(jsonNodes, sb);
            //var json = "{\"tracker\":{\"id\":151},\"values\":[{\"field_id\":\"1358\",\"value\":\"Vadim_Sidorchuk_Test\"},{\"field_id\":\"1359\",\"value\":\"Description_Hello_all!\"},{\"field_id\":\"1371\",\"bind_value_ids\":[105,120]}]}";
            HttpStatusCode statusCode;
            var r = HttpPostJSON("artifacts", json, true, out statusCode);
            Log($"Status Code: {statusCode}, response: {r}");
            */

            /*
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------
            // --- FOR TEST - tuleap.net - Requests
            // ----------------------------------
            List<object> fieldValues = new List<object>();
            fieldValues.Add(new Dictionary<string, object> { { "field_id", 1008 }, { "value", "Vadim_Sidorchuk_Test" } });
            fieldValues.Add(new Dictionary<string, object> { { "field_id", 1009 }, { "value", "Description_Hello_all!" } });
            fieldValues.Add(new Dictionary<string, object> { { "field_id", 1011 }, { "bind_value_ids", new int[] { 961 } } });
            fieldValues.Add(new Dictionary<string, object> { { "field_id", 1013 }, { "bind_value_ids", new int[] { 1654 } } });
            //fieldValues.Add(new Dictionary<string, object> { { "field_id", "1730" }, { "value", "2016-04-17" } });  // дата
            //fieldValues.Add(new Dictionary<string, object> { { "field_id", "1730" }, { "value", "2016-01-15T18:18:08+06:00" } });  // дата, тоже работает
            fieldValues.Add(new Dictionary<string, object> { { "field_id", 1730 }, { "value", DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd") } });  // дата, тоже работает

            // --- artifact Link
            //List<object> v = new List<object>();
            //v.Add(new Dictionary<string, object> { { "id", 9607 } });
            //fieldValues.Add(new Dictionary<string, object> { { "field_id", "2906" }, { "links", v } });  // Its works!

            // --- finally form json query
            var jsonNodes = new Dictionary<string, object>
            {
                {"tracker", new KeyValuePair<string, int>("id", 140) },  // !!!!!!!!!!!!!!!!!!!!!!!!!!!
                {"values", fieldValues }
            };

            var sb = new StringBuilder();
            var json = JsonCreate(jsonNodes, sb);
            //var json = "{\"tracker\":{\"id\":151},\"values\":[{\"field_id\":\"1358\",\"value\":\"Vadim_Sidorchuk_Test\"},{\"field_id\":\"1359\",\"value\":\"Description_Hello_all!\"},{\"field_id\":\"1371\",\"bind_value_ids\":[105,120]}]}";
            HttpStatusCode statusCode;
            var r = HttpPostJSON("artifacts", json, true, out statusCode);
            Log($"Status Code: {statusCode}, response: {r}");
            */
        }

        public void CreatePhysicalPatch()
        {
            patchButton = false;

            // --- очистить zip-директорию
            var dirs = Directory.GetDirectories(sZipTempFolder);
            foreach (var dir in dirs)
                Directory.Delete(dir, true);

            var files = Directory.GetFiles(sZipTempFolder);
            foreach (var f in files)
                File.Delete(f);

            // --- 2. Работаем с патчем
            Directory.SetCurrentDirectory(sWorkingFolder);
            //string slibs = string.Join("_", libsShortnames.Values);

            // --- 2.1 копируем либы из проекта во временную директорию будущего патча
            foreach (var libName in libsToPatch)
            {
                var srcFolder = Path.Combine(sEsedoProjectFolder, libName, "obj", "Debug");
                var destFolder = Path.Combine(sZipTempFolder, "App", "bin");

                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);
                if (!Directory.Exists(srcFolder))
                {
                }

                var fileDLL = libName + ".dll";
                var filePDB = libName + ".pdb";

                if (File.Exists(Path.Combine(srcFolder, fileDLL)))
                {
                    File.Copy(Path.Combine(srcFolder, fileDLL), Path.Combine(destFolder, fileDLL), true);
                        // 'System.IO.FileNotFoundException' 
                    File.Copy(Path.Combine(srcFolder, filePDB), Path.Combine(destFolder, filePDB), true);
                }
                else
                {
                    // todo: убрать библиотеку из названия патча (надо или нет)
                    Log($"Не найдена бибилиотека {fileDLL} в каталоге {srcFolder}");
                }
            }

            // --- 2.2 создаем zip-файл из временной директории
            var version = "1.12.4";
            var sdate = DateTime.Now.Date.ToString("yyyy.MM.dd");
            var username = "Vadim.Sidorchuk";
                // todo: получение имени пользователя по id'шнику в тулипе (globalUserInfo.user_id)
            var patchNumber = 1;
            string slibs = string.Join("_",
                (libsShortnames.Where(kvp => libsToPatch.Contains(kvp.Key)).Select(kvp => kvp.Value)));

            sZipFileNameTemplate =
                $"DONTUSE-ITS-A-TEST_update_{version}_{sdate}_{patchNumber} of {username} ({slibs}).zip";
            //update_1.12.4_2016.10.05_1 of Vadim.Sidorchuk (ASM_SqlFunctions_CommonInfo).zip

            while (File.Exists(sZipFileNameTemplate))
            {
                patchNumber++;
                sZipFileNameTemplate = $"DONTUSE-ITS-A-TEST_update_{version}_{sdate}_{patchNumber} of {username} ({slibs}).zip";
                //todo: галочку "использовать существующий патч на эту дату, или создать новый?"
            }

            try
            {    
                System.IO.Compression.ZipFile.CreateFromDirectory(sZipTempFolder, sZipFileNameTemplate);
            }
            catch (System.IO.IOException ex)
            {
                Log(ex.ToString());
            }

            // todo: sleep
            // --- 2.3 копируем zip в BuildKRG + в локальную какую то папку
            if (File.Exists(sZipFileNameTemplate))
                File.Copy(sZipFileNameTemplate, Path.Combine(remotePatchFolder, sZipFileNameTemplate), true);
            else
            {
                Log($"Файл {sZipFileNameTemplate} не найден.");
            }
        }


        private void MenuBeginWorkOnBug_Click(object sender, EventArgs e)
        {
            // Начинаем работу над багом:
            // 1. Через проект бага получим соответствующие трекер задач и трекер патчей
            // 2. Создаем задачу
            // 3. Апдейтим баг (статус)

            // --- 1. Получим id нужных нам трекеров (Патчи и Задания)
            int projId = (listView1.SelectedItems[0].Tag as TuleapClasses.Artifact).project.id;
            int bugId = (listView1.SelectedItems[0].Tag as TuleapClasses.Artifact).id;
            TuleapClasses.Project p = CommonVariables.globalProjects.SingleOrDefault(pr => pr.id == projId);
            int patchTrackerId = p.trackers.SingleOrDefault(t => t.label == "Patches").id;
            int tasksTrackerId = p.trackers.SingleOrDefault(t => t.label == "Tasks").id;
            //Log($"patchTrackerId={patchTrackerId}, tasksTrackerId={tasksTrackerId}");

            // --- 2. Создаем задачу для бага
            // --- 2.1 Параметры для создания артефакта типа Task
            var d = new Dictionary<string, object>();
            d.Add(STemplateBugId, bugId);
            d.Add(STemplateTaskPlanHours, 4);
            d.Add(STemplateTaskFactHours, "");
            d.Add(STemplateTaskStatus, 16315);
            d.Add(STemplateTaskStartDate, DateTime.Now.ToString("yyyy-MM-dd"));
            d.Add(STemplateTaskEndDate, DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));

            CreateArtifactInTuleap(tasksTrackerId, d, true, bugId);
            
            // "{'tracker': {'id':'557'}, 'values': [{'field_id':22456,'value':'asfasdf'}, {} ]}";
            //List<object> fieldValues = new List<object>();
            //fieldValues.Add(new Dictionary<string, object>{ { "field_id", "22456"}, { "value","asfadhfjk"} });
            //fieldValues.Add(new Dictionary<string, object> { { "field_id", "22457" }, { "value", "12345" } });
            //fieldValues.Add(new Dictionary<string, object> { { "field_id", "22458" }, { "value", "_+--_" } });

            //var json = new Dictionary<string, object>
            //{
            //    {"tracker", new KeyValuePair<string, int>("id", tasksTrackerId) },
            //    {"values", fieldValues }
            //};

            //textBox1.Text += "\r\n\r\n" + JsonCreate(json, new StringBuilder());







            //var json = new Dictionary<string, object>
            //{
            //    {"tracker", new KeyValuePair<string, int>("id", tasksTrackerId) },
            //    {"values", new List<KeyValuePair<string, string>>
            //        {
            //            new KeyValuePair<string, string>("field_id", "22456"),

            //        }
            //    }
            //}

            //HttpPostJSON("artifacts", json, true);

            //textBox2.Text = (string)((Artifact)listView1.SelectedItems[0].Tag).values.Where(v => v.label == "Детальное описание").FirstOrDefault().value;
            //int trackerId = 0;

            // --- 3. Апдейтим баг

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                TuleapClasses.Artifact b = (TuleapClasses.Artifact)listView1.SelectedItems[0].Tag;
                textBox3.Text = (string)((TuleapClasses.Artifact)listView1.SelectedItems[0].Tag).values.Where(v => v.label == "Детальное описание").FirstOrDefault().value;

                var li = listView1.SelectedItems[0];
                int projId = (li.Tag as TuleapClasses.Artifact).project.id;
                int bugId = (li.Tag as TuleapClasses.Artifact).id;
                string projLabel = (li.Tag as TuleapClasses.Artifact).project.label;
                textBox1.Text += $"Выбран баг: {b.id}, проект: {projLabel} ({projId});/r/n";
            }
        }

        private void btn_InitializeWork_Click(object sender, EventArgs e)
        {
            InitiateWorkWithTuleap();
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(tbTracker.Text);
            var j = HttpLevel.HttpGet("trackers/" + tbTracker.Text, true);
            TranscodeJson(j);
        }

        private void MenuCreatePatchT1_Click(object sender, EventArgs e)
        {
            //panel_Libs.Left = 0;
            //panel_Libs.Top = 0;
            //panel_Libs.Visible = true;
            //patchButton = true;
            
            
            //var bugId = (listView1.SelectedItems[0].Tag as Artifact).id;
            //var projectId = (listView1.SelectedItems[0].Tag as Artifact).project.id;

            // 1
            // todo: CreateArtifactInTuleap("patch", bugId, projectId)

            // --- 2. Работаем с патчем (перенес в кнопку ОК на панели библиотек)
            //CreatePhysicalPatch();

            /*
            // --- 2.3 копируем zip в BuildKRG + в локуальную какую то папку
            if (File.Exists(sZipFileNameTemplate))
                File.Copy(sZipFileNameTemplate, Path.Combine(remotePatchFolder, sZipFileNameTemplate));
            */
            // --- 2.4 SQL
            // --- 2.5 Шаблоны


        }
        
        private void btn_libsPanelOk_Click(object sender, EventArgs e)
        {
            libsToPatch.Clear();

            foreach (Control c in panel_Libs.Controls)  // todo Controls.OfType<CheckBox>
            {
                if (c is CheckBox && (c as CheckBox).Checked)
                {
                    var s = (c as CheckBox).Text;
                    libsToPatch.Add(s);

                    //var s2 = libsShortnames[s];
                    //libsToPatch.Add(s2);
                }
            }

            if (patchButton)
                CreatePhysicalPatch();

            panel_Libs.Visible = false;
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            tb_Host.Text = @"https://tuleap.net";
            CommonVariables.tuleapHost = tb_Host.Text + "/api/";
            tb_User.Text = @"KvantVS";
            tb_Pass.Text = @"andr0medaT";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Net.HttpStatusCode rc;
            var r = HttpLevel.HttpDeleteJSON($"tokens/{CommonVariables.globalUserInfo.token}", "", true, out rc);
            Log("Delete token: " + rc);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CreateArtifactInTuleap(151, new Dictionary<string, object>(), true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel_Libs.Left = 9;
            panel_Libs.Top = 9;
            panel_Libs.Visible = true;
            patchButton = true;
        }

        private void btn_panelCancel_Click(object sender, EventArgs e)
        {
            patchButton = false;
            panel_Libs.Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var s = tb_Test.Text;
            var a = s.Split(',');
            Log(a.ToString());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tb_Host.Text = @"https://tuleap.nat.kz";
            CommonVariables.tuleapHost = tb_Host.Text + "/api/";
            tb_User.Text = @"Vadim.Sidorchuk";
            tb_Pass.Text = @"1qwer0Nat";
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Text = $"formX: {Width}; memoX: {textBox1.Width}; panel2_X: {splitContainer1.Panel2.Width}";
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {

        }

        private void создатьПатчВТулипеToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void MenuCreateTuleapPatch_Click(object sender, EventArgs e)
        {
            //var bugId = (listView1.SelectedItems[0].Tag as Artifact).id;
            //var projectId = (listView1.SelectedItems[0].Tag as Artifact).project.id;

            // 1
            // todo: CreateArtifactInTuleap("patch", bugId, projectId)

            // --- 2. Работаем с патчем (перенес в кнопку ОК на панели библиотек)
            //CreatePhysicalPatch();

            /*
            // --- 2.3 копируем zip в BuildKRG + в локуальную какую то папку
            if (File.Exists(sZipFileNameTemplate))
                File.Copy(sZipFileNameTemplate, Path.Combine(remotePatchFolder, sZipFileNameTemplate));
            */
            // --- 2.4 SQL
            // --- 2.5 Шаблоны
        }

        private void tb_Host_TextChanged(object sender, EventArgs e)
        {
            CommonVariables.tuleapHost = tb_Host.Text + "/api/";
        }

        private void MenuCreatePatchT2_Click(object sender, EventArgs e)
        {
            var bug = (listView1.SelectedItems[0].Tag as TuleapClasses.Artifact);
            int projId = (listView1.SelectedItems[0].Tag as TuleapClasses.Artifact).project.id;
            int bugId = (listView1.SelectedItems[0].Tag as TuleapClasses.Artifact).id;
            int trackerId;

            /* универсальное нахождение трекера патча */
            trackerId = bug.project.trackers.Where(t => t.item_name.ToLower() == "patches").FirstOrDefault().id;

            TuleapPatchCreateForm form2 = new TuleapPatchCreateForm(bug);
            var b = form2.ShowDialog();
            if (b == DialogResult.OK)
            {
                var nazn = form2.Naznacheno;
                var changes = form2.Changes;

                switch (bug.project.label)
                {
                    case "EISKVE-2016-R":
                    case "EISKVE-2016-GP":
                    case "EISKVE-2015-GP":
                        trackerId = 1189;
                        break;
                    case "EISKVE-2017-R-1":
                        trackerId = 1167;
                        break;
                    case "EISKVE-2017-R-2":
                        trackerId = 1178;
                        break;
                    default:
                        trackerId = 0;
                        break;
                }
                if (trackerId == 0)
                {
                    Log("Не найден корректный трекер патча");
                    //todo сделать определение трекера
                }
                else
                {
                    var s = TuleapLevel.CreatePatchInTuleap(true, trackerId, null, "", "", bugId, bug);  //1189 - patches in 2016GP
                    Log(s);
                }
            }
        }

        private void MenuCreateBuildT1_Click(object sender, EventArgs e)
        {
            var li = listView1.SelectedItems[0];
            
            TuleapClasses.Artifact art = li.Tag as TuleapClasses.Artifact;
            int projId = (li.Tag as TuleapClasses.Artifact).project.id;
            int bugId = (li.Tag as TuleapClasses.Artifact).id;

            TuleapPatchCreateForm form2 = new TuleapPatchCreateForm(art);
            var b = form2.ShowDialog();
            if (b == DialogResult.OK)
            {
                var nazn = form2.Naznacheno;
                var changes = form2.Changes;

                int trackerId;
                switch ((li.Tag as TuleapClasses.Artifact).project.label)
                {
                    case "EISKVE-2016-R":
                        trackerId = 544;
                        break;
                    case "EISKVE-2016-GP":
                    case "EISKVE-2015-GP":
                        trackerId = 1189;
                        break;
                    case "EISKVE-2017-R-1":
                        trackerId = 1167;
                        break;
                    case "EISKVE-2017-R-2":
                        trackerId = 1178;
                        break;
                    default:
                        trackerId = 0;
                        break;
                }
                if (trackerId == 0)
                {
                    Log("Не найден корректный трекер патча");
                    return;
                    //todo сделать определение трекера
                }
                else
                {
                    //todo переделать под полный путь файла
                    var buildFilePath = Path.GetFileNameWithoutExtension(form2.BuildFile);
                    Log(string.Format("naz:{0} file:{1} changes:{2}", nazn, buildFilePath, changes));
                    var s = TuleapLevel.CreatePatchInTuleap(true, trackerId, null, buildFilePath, changes, bugId, art, nazn); //#2107 //1189 - patches in 2016GP
                    //////////Log(s);
                }
            }
           

            /*
            var li = listView1.SelectedItems[0];
            int projId = (li.Tag as Artifact).project.id;
            int bugId = (li.Tag as Artifact).id;
            //Project p = globalProjects.SingleOrDefault(pr => pr.id == projId);

            
            */
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //var myObject = globalProjects[0].trackers.Where(t => t.label == "Bugs").FirstOrDefault().fields.Where(f => f.field_id == 22131).FirstOrDefault().values;

            //Type mytype = myObject.GetType();
            //IList<PropertyInfo> props = new List<PropertyInfo>(mytype.GetProperties());

            //foreach (PropertyInfo p in props)
            //{
            //    var myparams = p.GetIndexParameters();

            //    object propValue = p.GetValue(myObject, new object[] { myparams.Count() });
            //    Log(string.Format("propName={0}, propValue={1}", p.Name, propValue));
            //}

            var tracker = CommonVariables.globalProjects[0].trackers.Where(t => t.label == "Bugs").FirstOrDefault();
            foreach (var f in tracker.fields)
            {
                if (f.values != null)
                {
                    if (f.values is Newtonsoft.Json.Linq.JArray)
                    {
                        Log("is JArray");
                        var vals = ((Newtonsoft.Json.Linq.JArray)f.values).ToObject<TuleapClasses.FieldValues[]>();
                        
                    }
                    Log(f.values.ToString());
                }   
            }
            
        }
    }


    // --------------------------------------------------------------




}
