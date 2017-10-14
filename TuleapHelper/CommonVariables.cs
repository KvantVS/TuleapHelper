using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Configuration;
using System.Windows.Forms;
//using tc = TuleapHelper.TuleapClasses;

namespace TuleapHelper
{
    public static class CommonVariables
    {
        public static string tuleapHost = "https://tuleap.nat.kz/api/";
        internal static string apiHost = "";
        public static TuleapClasses.UserInfo globalUserInfo = new TuleapClasses.UserInfo();// { user_id = "171", token = "207f8dd40f8732fd0623a4b567341400" } ;
        public static string user = "";
        public static string pass = "";
        public static TuleapClasses.Project[] globalProjects;
        public static string projectFolder = "";// ConfigurationManager.AppSettings.Get("projectFolder");
        public static string srcFolder = "";
        public static string settingsFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Settings.txt");
        public static string sWorkingFolder = Application.StartupPath;
        //public static string sWorkingFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.ProductName);
        public static string sZipTempFolder = Path.Combine(sWorkingFolder, "zippatch");
        public static string remotePatchFolder_hotfix = @"\\buildkrg\GE-ESEDO\Сборка на тестирование\1.1.0";
        public static string remotePatchFolder_develop = @"\\buildkrg\GE-ESEDO\Builds";
        //$@"\\buildkrg\GE-ESEDO\Builds\JS.ESEDO.Develop.v{version}.{pathToPatchFile.Substring(1, pathToPatchFile.Length-1)}.msi";
        public static string localPatchFolder = $"{Path.Combine(Application.StartupPath, "Patches")}";
        public static string patchFilenameTemplate_hotfix = @"DONTUSE-ITS-A-TEST_update_{version}_{sdate}_{patchNumber} of {username} ({slibs}).zip";
    }

    public static class CommonConstants
    {
        public const string settingProjectFolder = "projectFolder";

        //todo: автозагрузку из папки проекта, настройку сокращенных имен. Автонахождение новых проектов (библиотек)
        public static readonly Dictionary<string, string> libsShortnames = new Dictionary<string, string>
        {
            {"JS.EDO.AccountingIntegration", "AI"},
            {"JS.EDO.Administration", "ADM" },
            {"JS.EDO.Assignments", "ASM" },
            {"JS.EDO.Assigments.Logic", "ASM.Logic" },
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
    }

    public static class CommonFunctions
    {
        //public static void Log(string message, bool newLine = true)
        //{
        //    //var ending = newLine ? Environment.NewLine : string.Empty;
        //    //textBox1.Text += $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}\t{message}{ending}";
             
        //    Form1.textBox1.AppendText($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}\t{message}\r\n");
        //}

        public static string JsonCreate(Dictionary<string, object> jsonNodes, StringBuilder sb)
        {
            using (StringWriter sw = new StringWriter(sb))
            using (JsonTextWriter jw = new JsonTextWriter(sw))
            {
                JsonCreate(jsonNodes, jw);
            }
            return sb.ToString();
        }

        public static void JsonCreate(Dictionary<string, object> jsonNodes, JsonTextWriter jw)
        {
            //todo: проверку на пустой dictionary;
            //string json = "{'tracker': {'id':'557'}, 'values': [{'field_id':22456,'value':'asfasdf'}, {} ]}";
            //StringBuilder sb = new StringBuilder();
            //using (StringWriter sw = new StringWriter(sb))
            //using (JsonTextWriter jw = new JsonTextWriter(sw))
            {
                jw.WriteStartObject();
                foreach (var j in jsonNodes)
                {
                    jw.WritePropertyName(j.Key, true);
                    if (!(j.Value is string) && !(j.Value is int))
                    {
                        if (j.Value is Array)
                        {
                            Type t2 = j.Value.GetType().GetElementType();
                            jw.WriteStartArray();
                            foreach (var el in (Array)j.Value)
                            {
                                jw.WriteValue(el);
                            }
                            jw.WriteEndArray();
                        }

                        //else if (j.Value is IEnumerable)
                        //{ }

                        else
                        {
                            Type valueType = j.Value.GetType();
                            if (valueType.IsGenericType)
                            {
                                var baseType = valueType.GetGenericTypeDefinition();
                                //{Name = "Dictionary`2" FullName = "System.Collections.Generic.Dictionary`2"}	System.Type {System.RuntimeType}

                                if (baseType==typeof(Dictionary<,>))
                                {
                                    JsonCreate((Dictionary<string,object>)j.Value, jw);
                                }

                                if (baseType == typeof(KeyValuePair<,>))
                                {
                                    //jw.WriteStartObject();

                                    //Type t1 = j.Value.GetType().GetProperty("Key").PropertyType;
                                    //Type t2 = j.Value.GetType().GetProperty("Value").PropertyType;
                                    Type t1 = j.Value.GetType().GenericTypeArguments[0];
                                    Type t2 = j.Value.GetType().GenericTypeArguments[1];
                                    var kvpPropertyType = typeof(KeyValuePair<,>);
                                    var myKvpPropertyType = kvpPropertyType.MakeGenericType(t1, t2);
                                    dynamic v1 = myKvpPropertyType.GetProperty("Key").GetValue(j.Value, null);
                                    dynamic v2 = myKvpPropertyType.GetProperty("Value").GetValue(j.Value, null);

                                    var dict = new Dictionary<string, object>();
                                    dict.Add(v1.ToString(), v2);
                                    JsonCreate(dict, jw);

                                    //jw.WritePropertyName(v1, true);
                                    //jw.WriteValue(v2);
                                    //jw.WriteEndObject();
                                }

                                else if (baseType == typeof(List<>))
                                {
                                    Type t = j.Value.GetType().GenericTypeArguments[0];
                                    
                                    //dynamic val3 = props[2].GetValue(j.Value);

                                    if (t == typeof(int) || t== typeof(String))
                                    {
                                        var listPropertyType = typeof(List<>);
                                        var myListPropertyType = listPropertyType.MakeGenericType(t);
                                        var props = myListPropertyType.GetProperties();
                                        dynamic count = props[1].GetValue(j.Value);

                                        jw.WriteStartArray();
                                        foreach (var val in (j.Value as List<object>))
                                        {
                                            jw.WriteValue(val);
                                        }
                                        jw.WriteEndArray();
                                    }

                                    if (t.Name == "FieldValues")
                                    {
                                        var listPropertyType = typeof(List<>);
                                        var myListPropertyType = listPropertyType.MakeGenericType(t);
                                        var props = myListPropertyType.GetProperties();
                                        dynamic count = props[1].GetValue(j.Value);

                                        jw.WriteStartArray();
                                        foreach (var fieldvalue in (j.Value as List<TuleapClasses.FieldValues>))
                                        {
                                            jw.WriteValue(fieldvalue.id.ToString());
                                        }
                                        jw.WriteEndArray();
                                    }
                                    else
                                    {
                                        var listPropertyType = typeof(List<>);
                                        var myListPropertyType = listPropertyType.MakeGenericType(t);
                                        var props = myListPropertyType.GetProperties();
                                        dynamic count = props[1].GetValue(j.Value);

                                        jw.WriteStartArray();
                                        foreach (var dict in (j.Value as List<object>))
                                        {
                                            JsonCreate((Dictionary<string, object>)dict, jw);
                                        }
                                        jw.WriteEndArray();
                                    }

                                    

                                }
                            }
                        }
                    }
                    else
                        jw.WriteValue(j.Value);
                }
                jw.WriteEndObject();
            }
            //return sb.ToString();            
        }
    }

}
