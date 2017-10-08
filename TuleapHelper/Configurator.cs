using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuleapHelper
{
    public static class Configurator
    {
        public static void SaveConfig()
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Settings.txt")))
            {
                sw.WriteLine($"{CommonConstants.settingProjectFolder}={CommonVariables.projectFolder}");
                sw.WriteLine($"tuleapHost={CommonVariables.tuleapHost}");
                sw.WriteLine($"apiHost={CommonVariables.apiHost}");
                sw.WriteLine($"user_id={CommonVariables.globalUserInfo.user_id}");
                sw.WriteLine($"token={CommonVariables.globalUserInfo.token}");
                sw.WriteLine($"user={CommonVariables.user}");
                sw.WriteLine($"pass={CommonVariables.pass}");
            }
        }

        /// <summary>
        /// Сохраняем один параметр в конфиг-файле
        /// </summary>
        /// <param name="key">Имя параметра</param>
        /// <param name="value">Значение параметра</param>
        /// <returns></returns>
        public static bool SaveParam(string key, string value)
        {
            try
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Settings.txt")))
                {
                    sw.WriteLine($"{key}={value}");
                }
                return true;
            }
            catch
            {
                throw;
                return false;
            }
        }

        /// <summary>
        /// Загружаем из конфига один параметр. Возвращает значение параметра типа string
        /// </summary>
        /// <param name="paramName">Имя параметра</param>
        /// <returns></returns>
        public static string LoadParam(string paramName)
        {
            //bool bNotFinded = true;
            //string sRet = "";
            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Settings.txt")))
                {
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        int k = s.IndexOf('=');
                        string sName = s.Substring(0, k);
                        string sValue = s.Substring(k+1, s.Length-k-1);
                        if (sName == paramName)//CommonConstants.settingProjectFolder
                        {
                            return sValue;
                            //sRet = sValue;
                            //bFinded = true;
                        }
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                if (ex is System.IO.FileNotFoundException)
                    System.Windows.Forms.MessageBox.Show("Файл настроек не найден (" + System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Settings.txt"));
                else
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                return "";
            }
        }
    }
}
