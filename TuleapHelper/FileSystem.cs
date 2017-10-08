using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuleapHelper
{
    class FileSystem
    {
        /// <summary>
        /// Создаем физически патч в локальной папке патчей. Возвращает true, если всё прошло успешно.
        /// </summary>
        /// <param name="message">если функция возвратила false, в message находится описана причина неудачи. Если возвр=true, то message пустая строка</param>
        /// <param name="esedoLibs">Библиотеки, которые будут в патче</param>
        /// <param name="libs">Библиотеки, которые будут в патче (удалить этот параметр)</param>
        /// <param name="patchFilename">Имя файла патча (без пути)</param>
        /// <param name="forT1">Патч для Т1?</param>
        /// <param name="PathsWhereToCopy">список путей, куда дополнительно копировать готовый патч</param>
        /// <returns></returns>
        public static bool CreatePhysicalPatch(out string message, List<EsedoLib> esedoLibs, string patchFilename, bool forT1, List<string> PathsWhereToCopy)
        {
            // todo Переделать, добавить возвращаемый параметр стрковоый, где будет обозначаться причина неудачи
            message = "";

            // --- очистить zip-директорию
            if (!Directory.Exists(CommonVariables.sZipTempFolder))
                Directory.CreateDirectory(CommonVariables.sZipTempFolder);
            else
            {
                var dirs = Directory.GetDirectories(CommonVariables.sZipTempFolder);
                foreach (var dir in dirs)
                    Directory.Delete(dir, true);

                var files = Directory.GetFiles(CommonVariables.sZipTempFolder);
                foreach (var f in files)
                    File.Delete(f);
            }
            // --- 2. Работаем с патчем
            //Directory.SetCurrentDirectory(CommonVariables.sWorkingFolder);// Application.StartupPath
            //Directory.SetCurrentDirectory(Application.StartupPath); 
            //string slibs = string.Join("_", libsShortnames.Values);

            #region excessCode
            // --- 2.1 копируем либы из проекта во временную директорию будущего патча
            // todo переделываем на EsedoLibs
            // ----------- попытка 1
            //foreach (var lib in esedoLibs)
            //{
            //    var srcFolder = Path.GetFullPath(lib.FullPathFileNameDLL);  // полный путь без имени файла (откуда будем копировать либу)
            //    var destFolder = Path.Combine(CommonVariables.sZipTempFolder, "App", "bin");  // путь куда будем копировать либу

            //    if (!Directory.Exists(destFolder))
            //        Directory.CreateDirectory(destFolder);
            //    if (!Directory.Exists(srcFolder))
            //    {
            //        MessageBox.Show($"Нету директории! {srcFolder}");
            //        //todo выход из процедуры с кодом и сообщением
            //    }

            //    var fileDLL = Path.GetFileNameWithoutExtension(lib.FullPathFileNameDLL) + ".dll";
            //    var filePDB = Path.GetFileNameWithoutExtension(lib.FullPathFileNameDLL) + ".pdb";  // что возвращает? путь с именем, или только имя?
            //    var destDllFileName = Path.Combine(destFolder, fileDLL);
            //    var destPdbFileName = Path.Combine(destFolder, filePDB);

            //    if (File.Exists(fileDLL))
            //    {
            //        File.Copy(lib.FullPathFileNameDLL, destDllFileName, true);
            //        File.Copy(Path.Combine(srcFolder, filePDB), destPdbFileName, true);
            //        // 'System.IO.FileNotFoundException' 
            //        return true;
            //    }
            //    else
            //    {
            //        MessageBox.Show($"Не найдена бибилиотека {fileDLL} в каталоге {srcFolder}");
            //        return false;
            //    }
            //}
            #endregion


            // ------------------- Попытка 2
            var destFolder = Path.Combine(CommonVariables.sZipTempFolder, "App", "bin");  // путь куда будем копировать либу
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            foreach (var lib in esedoLibs)
            {
                var dllDestFile = Path.Combine(destFolder, lib.Name + ".dll");
                var pdbDestFile = Path.Combine(destFolder, lib.Name + ".pdb");
                
                if (File.Exists(lib.FullPathFileNameDLL))
                {
                    File.Copy(lib.FullPathFileNameDLL, dllDestFile, true);  // копируем .dll
                    File.Copy(lib.FullPathFileNamePDB, pdbDestFile, true);  // копируем .pdb
                    // может вылезти 'System.IO.FileNotFoundException' 
                }
                else
                {
                    MessageBox.Show($"Не найдена бибилиотека {lib.FullPathFileNameDLL}");
                    return false;
                }
            }
            
            #region oldCode
            //foreach (var libName in libs)
            //{
            //    // todo debug или Release (брать из списка libs)
            //    var srcFolder = Path.Combine(CommonVariables.projectFolder, libName, "obj", "Debug");
            //    var destFolder = Path.Combine(CommonVariables.sZipTempFolder, "App", "bin");

            //    if (!Directory.Exists(destFolder))
            //        Directory.CreateDirectory(destFolder);
            //    if (!Directory.Exists(srcFolder))
            //    {
            //        MessageBox.Show($"Нету директории! {srcFolder}");
            //    }

            //    var fileDLL = libName + ".dll";
            //    var filePDB = libName + ".pdb";

            //    if (File.Exists(Path.Combine(srcFolder, fileDLL)))
            //    {
            //        File.Copy(Path.Combine(srcFolder, fileDLL), Path.Combine(destFolder, fileDLL), true);
            //        File.Copy(Path.Combine(srcFolder, filePDB), Path.Combine(destFolder, filePDB), true);
            //        // 'System.IO.FileNotFoundException' 
            //        return true;
            //    }
            //    else
            //    {
            //        MessageBox.Show($"Не найдена бибилиотека {fileDLL} в каталоге {srcFolder}");
            //        return false;
            //    }
            //}
            #endregion

            // --- 2.2 создаем zip-файл из временной директории
            try
            {
                System.IO.Compression.ZipFile.CreateFromDirectory(
                    CommonVariables.sZipTempFolder, Path.Combine(CommonVariables.localPatchFolder, patchFilename));
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }

            // --- поспим секунду...
            //System.Threading.Thread.Sleep(1000);

            // --- 2.3 копируем zip в папки, задаваемые в параметре PathsWhereToCopy (в BuildKRG + в локальную какую-то папку)
            if (PathsWhereToCopy.Count > 0)
            {
                string patchFullFileName = Path.Combine(CommonVariables.localPatchFolder, patchFilename);
                var c = 0;
                if (!File.Exists(patchFullFileName))
                {
                    System.Threading.Thread.Sleep(1000);
                    c++;
                    if (c > 10)
                    {
                        MessageBox.Show($"Файл {patchFullFileName} не найден.");
                        return false;
                    }
                }
                else
                {
                    foreach (var p in PathsWhereToCopy)
                    {
                        File.Copy(patchFullFileName, Path.Combine(p, patchFilename), true);
                    }
                }
            }

            return true;
        }
    }
}
