using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
//using tc = TuleapHelper.TuleapClasses;

namespace TuleapHelper
{
    public static class CommonVariables
    {
        public static string tuleapHost = "https://tuleap.nat.kz/api/";
        public static TuleapClasses.UserInfo globalUserInfo = new TuleapClasses.UserInfo();// { user_id = "171", token = "207f8dd40f8732fd0623a4b567341400" } ;
        public static string user = "";
        public static string pass = "";
        public static TuleapClasses.Project[] globalProjects;
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
