using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuleapHelper
{
    public class EsedoLib
    {
        /// <summary>
        /// Имя библиотеки (без пути и без .dll. Только имя, типа JS.EDO.Administration)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Полный путь к .dll-файлу библиотеки (причем корректной конфигурации: Debug/Release)
        /// </summary>
        public string FullPathFileNameDLL { get; set; }

        /// <summary>
        /// Полный путь к .pdb-файлу библиотеки (причем корректной конфигурации: Debug/Release)
        /// </summary>
        public string FullPathFileNamePDB { get; set; }

        /// <summary>
        /// Дата-время изменения .dll-файла
        /// </summary>
        public DateTime ChangeDateTime { get; set; }
        
        // todo: public string ShortName { get; set; }
    }
}
