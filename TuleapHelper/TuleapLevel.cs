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
        }

        public static string CreatePatchInTuleap(
            out string mess,
            bool isBuildForT1,
            List<string> libs,
            List<int> list_assignedTo,
            List<int> list_notificateTo,
            int trackerId, 
            Dictionary<string, object> dictParams, 
            string pathToPatchFile, 
            string changes, 
            string version,
            string patchFilename,
            TuleapClasses.Artifact art=null)
        {
            mess = "";
            List<object> fieldValues = new List<object>();

            if (art == null)
            {
                mess = "Не передан артефакт";
                return mess;
            }

            var p = CommonVariables.globalProjects.Where(gp => gp.id == art.project.id).SingleOrDefault();
            //var1) TuleapClasses.Tracker3 t = art.project.trackers.Where(tr => tr.label.ToLower() == "patches").SingleOrDefault();
            TuleapClasses.Tracker3 t = p.trackers.Where(tr => tr.label.ToLower() == "patches").SingleOrDefault();

            // -------------------------------------------
            // Получаем ID'шники необходимых полей трекера
            // -------------------------------------------
            // --- Нужные поля ---
            // Назначено, 
            // Кго уведомлять, 
            // номер патча
            // Ссылка на патч
            // изменения
            // ветка
            // зависимый функционал
            // статус
            // линки

            var AssignedToFieldId = t.fields.Where(f => f.label == "Назначено" || f.name == "assigned_to").FirstOrDefault().field_id;  //48968 - 2016-GP, 22306 - 2016-R, 48072 - 2017-R1, 22708 - 2015-GP
            var NotificateFieldId = t.fields.Where(f => f.label == "Уведомлять участников обновления" || f.name == "field_4").FirstOrDefault().field_id;
            var NomerPatchaFieldId = t.fields.Where(f => f.label == "Номер патча" || f.name == "summary").FirstOrDefault().field_id;
            var PathToPatchFieldId = t.fields.Where(f => f.label == "Ссылка на патч" || f.name == "__11").FirstOrDefault().field_id;
            var ChangesFieldId = t.fields.Where(f => f.label == "Изменения" || f.name == "details").FirstOrDefault().field_id;
                    
            // Ветка
            TuleapClasses.Field fVetka = t.fields.Where(f => f.label == "Ветка" || f.name == "field_1").FirstOrDefault();
            var BranchFieldId = fVetka.field_id;
            int BranchValueId = 0;
            string wanted = (isBuildForT1 ? "develop" : "hotfix"); 
            BranchValueId = fVetka.FValues.SingleOrDefault(fv => fv.label.ToLower() == wanted).id;

            var ZavisFunctionalFieldId = t.fields.Where(f => f.label == "Зависимый функционал" || f.name == "_").FirstOrDefault().field_id;

            // Статус
            TuleapClasses.Field fStatus = t.fields.Where(f => f.label == "Статус" || f.name == "status_id").FirstOrDefault();
            var StatusFieldId = fStatus.field_id;
            // todo проверку на существование параметра           
            var StatusValueId = fStatus.FValues.Where(v => v.label == "Передан на тестирование").Select(v => v.id);
            
            var LinkFieldId = t.fields.Where(f => f.label == "Artifact links" || f.name == "artifact_links" || f.type == "art_link").FirstOrDefault().field_id;

            #region oldcode
            //foreach (var trackField in DictRequiredFields[trackerId])
            //{
            //    fieldValues.Add(new Dictionary<string, object>
            //    {
            //        { SFieldId, trackField.FieldId },
            //        { trackField.ValueParamName, GetArtifactValueByType(trackField.ValueByDefault, trackField.ValueParamName, bugLinkId, dictParams, true) }
            //    });
            //}
            #endregion

            // ----------------------------------------------
            // Получаем значения для изменяемых полей
            // ----------------------------------------------

            // --- назначена
            //string n = naznachena.ToString();                        //"113"; // 136 - Катя, 113 - Витя, 150 - Лена Зуева, 151 - Л.Купченко, 149 - Наташа // todo брать консультанта с бага
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", AssignedToFieldId },                    //22306 - для 544, 48968 - для 2016 ГП
                //{ "bind_value_ids", new string[] {n} }
                { "bind_value_ids", list_assignedTo.ToArray() }
            });
            //todo: по умолчанию стоит галочка "взять из бага", если убрать, то дать возможность выбрать из пользователей тулипа

            // --- кого уведомлять:                                
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", NotificateFieldId },                              //48969 , 22305
                { "bind_value_ids", list_notificateTo.ToArray() } 
                //{ "bind_value_ids", new string[] {"171", "113", "165", "114"} } // 113 - Виктор 165 (серг), 171 -я, 114-сухоруков, 113-Витя 
            });

            // --- nomer patcha (просто имя файла)
            var patchFullFilePath = "";

            if (isBuildForT1)
            {
                if (pathToPatchFile[0] == '#')  // todo: для сборки на Т1
                    patchFullFilePath = $@"\\buildkrg\GE-ESEDO\Builds\JS.ESEDO.Develop.v{version}.{pathToPatchFile.Substring(1, pathToPatchFile.Length-1)}.msi";
                else                     
                    patchFullFilePath = pathToPatchFile; // todo: для патча на Т1
            }
            else
            {
                // --- ДЛЯ Т2
                //\\buildkrg\GE-ESEDO\Сборка на тестирование\1.1.0
                //$"DONTUSE-ITS-A-TEST_update_{version}_{sdate}_{patchNumber} of {username} ({slibs}).zip";
                //patchFileFullPath = $@"\\buildkrg\GE-ESEDO\Сборка на тестирование\1.1.0\DONTUSE-ITS-A-TEST_update_{version}_{sdate}_{patchNumber} of {username} ({slibs}).zip";
                patchFullFilePath = System.IO.Path.Combine(CommonVariables.remotePatchFolder_hotfix, patchFilename);
            }

            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", NomerPatchaFieldId },                 //48955, 22291
                { "value", patchFilename }
            });

            // путь к патчу
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", PathToPatchFieldId},                      //48956, 22292
                { "value", patchFullFilePath }
            });

            // Изменения
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", ChangesFieldId},                      //48957, 22293
                { "value", changes }
            });

            // ветка
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", BranchFieldId}, //48970  
                { "bind_value_ids", new string[] { BranchValueId.ToString() } } //16184 - develop(544), 16187 - HotFix(544) //36214 - HotFix (1189)
            });

            // Зависимый функционал
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", ZavisFunctionalFieldId},  //48960
                { "value", "Нет" }  
            });

            // Статус
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", StatusFieldId}, //48972
                { "bind_value_ids", new string[] { StatusValueId.ToString() } }  //36218, 16179 - передан на тестирование
            });

            // Links
            List<object> links = new List<object>();
            links.Add(new Dictionary<string, object> { { "id", art.id } });  // из-за особенностей json-структуры в тулипе, приходится делать так. Т.к. там идет объект массива объектов {[{"id" : "9607"}]} (или типа того, не помню))
            fieldValues.Add(new Dictionary<string, object>
            {
                { "field_id", LinkFieldId}, //48965  22300
                { "links", links }  
            });
            // ---------------------------------------------------

            // --- облачаем всё в values и в основной json-объект
            var jsonNodes = new Dictionary<string, object>
            {
                {"tracker", new KeyValuePair<string, int>("id", trackerId) },
                {"values", fieldValues }
            };

            // --- Создаём JSON-объект из дикшионари, и отправляем запрос в виде этого JSON`а 
            var json = CommonFunctions.JsonCreate(jsonNodes, new StringBuilder());
            return json.ToString();
            HttpStatusCode statusCode;
            return "testOK";
            //////////////var r = HttpLevel.HttpPostJSON("artifacts", json, true, out statusCode);
            //////////////return ($"Status Code: {statusCode}, response: {r}");
            //Log($"Status Code: {statusCode}, response: {r}");

            
        }


        public static void UpdateArtifact(TuleapClasses.Artifact art, Dictionary<string, object> fieldValues)
        {

        }
    }

}
