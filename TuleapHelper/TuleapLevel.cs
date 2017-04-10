using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;

namespace TuleapHelper
{
    public static class TuleapLevel
    {
        public static HttpStatusCode Authorize(string host, string user, string pass)
        {
            HttpStatusCode statusCode;
            Dictionary<string, object> jsonNodes = new Dictionary<string, object>
            {
                {"username", user},
                {"password", pass}
            };
            string json = CommonFunctions.JsonCreate(jsonNodes, new StringBuilder());

            // --- Авторизуемся
            var response = HttpLevel.HttpPostJSON("tokens", json, false, out statusCode);

            if (statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Created)
            {
                CommonVariables.globalUserInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<TuleapClasses.UserInfo>(response);
            }
            else
            {
                //Log($"Ответ: {statusCode}");
            }
            return statusCode;

            // TODO: сделать сохранение токена в файл, или реестр
            //Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //cfg.AppSettings.Settings.Add("user_id", globalUserInfo.user_id);
            //cfg.AppSettings.Settings.Add("authToken", globalUserInfo.token);
            //cfg.Save();
        }


        public static string CreatePatchInTuleap(bool isBuildForT1, int trackerId, Dictionary<string, object> dictParams, string pathToPatchFile, string changes, int bugLinkId = 0, TuleapClasses.Artifact art=null, int naznachena=0)
        {
            List<object> fieldValues = new List<object>();

            //foreach (var trackField in DictRequiredFields[trackerId])
            //{
            //    fieldValues.Add(new Dictionary<string, object>
            //    {
            //        { SFieldId, trackField.FieldId },
            //        { trackField.ValueParamName, GetArtifactValueByType(trackField.ValueByDefault, trackField.ValueParamName, bugLinkId, dictParams, true) }
            //    });
            //}

            // --- назначена
            string n = naznachena.ToString(); //"113"; // 136 - Катя, 113 - Витя, 150 - Лена Зуева, 151 - Л.Купченко, 149 - Наташа // todo брать консультанта с бага
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", 22306 }, //22306 - для 544, 48968 - для 2016 ГП
                { "bind_value_ids", new string[] {n} }
            });
            //todo: по умолчанию стоит галочка "взять из бага", если убрать, то дать возможность выбрать из пользователей тулипа

            // кого уведомлять: 165 (серг), 171 -я, 114-сухоруков, 113-Витя 
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", 22305 }, //48969 
                { "bind_value_ids", new string[] {"171", "113", "165", "114"} } // 113 - Виктор
            });

            // nomer patcha
            var patchFileFullPath = "";
            if (isBuildForT1)
            {
                if (pathToPatchFile[0] == '#')  // todo: для сборки на Т1
                    patchFileFullPath = $@"\\buildkrg\GE-ESEDO\Builds\JS.ESEDO.Develop.v1.12.{pathToPatchFile.Substring(1, pathToPatchFile.Length-1)}.msi";
                else                     
                    patchFileFullPath = pathToPatchFile; // todo: для патча на Т1
            }
            var patchName = System.IO.Path.GetFileName(patchFileFullPath);

            //Log($"patchFileFullPath = {patchFileFullPath}");
            //Log($"patchName = {patchName}");

            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", 22291 },  //48955
                { "value", patchName }  //update_1.12.4_2016.12.22_1 of Vadim.Sidorchuk (ASM).zip
            });

            // путь к патчу
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", 22292},//48956
                { "value", patchFileFullPath }
            });

            //string changes = "При сохранении изменений в пунктах \"Настройка номеров входящих писем\", \"Настройка номеров исходящих писем\", \"Настройка номеров договора\", \"Настройка номеров распоряжении о назначении экспертов\", \"Настройка номеров заключения\", \"Настройка номеров бланков исходящих писем\" и \"Настройка номеров бланка распоряжения\" если было изменено значение в поле \"Текущий учетный год\", больше не выходит ошибка";
            // Изменения
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", 22293}, //48957
                { "value", changes }
            });

            // ветка
            string branch;
            if (isBuildForT1)
                branch = "16184";
            else
                branch = "16187";
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", 22303}, //48970  
                { "bind_value_ids", new string[] { branch } } //16184 - develop(544), 16187 - HotFix(544) //36214 - HotFix (1189)
            });

            // Зависимый функционал
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", 22296},  //48960
                { "value", "Нет" }  
            });

            // Статус
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", 22297}, //48972
                { "bind_value_ids", new string[] {"16179" } }  //36218, 16179 - передан на тестирование
            });

            // Links
            //string bugId = "9607";
            List<object> links = new List<object>();
            links.Add(new Dictionary<string, object> { { "id", bugLinkId } });  // из-за особенностей json-структуры в тулипе, приходится делать так. Т.к. там идет объект массива объектов {[{"id" : "9607"}]} (или типа того, не помню))
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", 22300}, //48965
                { "links", links }  
            });



            var jsonNodes = new Dictionary<string, object>
            {
                {"tracker", new KeyValuePair<string, int>("id", trackerId) },
                {"values", fieldValues }
            };

            
            var json = CommonFunctions.JsonCreate(jsonNodes, new StringBuilder());
            HttpStatusCode statusCode;
            return "test";
            //////////////var r = HttpLevel.HttpPostJSON("artifacts", json, true, out statusCode);
            //////////////return ($"Status Code: {statusCode}, response: {r}");
            //Log($"Status Code: {statusCode}, response: {r}");

            //todo обновлять баг тоже


        }
    }
}
