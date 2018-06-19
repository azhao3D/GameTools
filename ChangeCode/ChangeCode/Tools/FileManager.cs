using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tools
{
    public class FileManager
    {
        private static FileManager _instance;
        public const string SAVE_PATH = "savePath.txt";

        public static FileManager Instance
        {
            get { 
                if(_instance == null)
                {
                    _instance = new FileManager();
                }
                return FileManager._instance; 
            }

        }
        public FileManager()
        {
            System.Text.Encoding.GetEncoding("gb2312");
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public void SaveFile(string path,string content,bool needUtf8 = false)
        {
            CheckFileSavePath(path);
            if(needUtf8)
            {
                File.WriteAllText(path, content, Encoding.UTF8);
            }
            else
            {
                File.WriteAllText(path, content, Encoding.Default);
            }
             
        }

        public void SaveFileByCode(string path,string content,Encoding code)
        {
            CheckFileSavePath(path);
            File.WriteAllText(path, content, code);
        }

        /// <summary>
        /// 保存bytes
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        public void SaveBytes(string path,byte[] bytes)
        {
            CheckFileSavePath(path);
            File.WriteAllBytes(path, bytes);
        }

        public string ReadSavePath()
        {
            return ReadFileText(Application.StartupPath+"\\"+SAVE_PATH);
        }

        public void SaveSavePath(string str)
        {
            string path = Application.StartupPath + "\\" + SAVE_PATH;
            SaveFile(path, str);
        }

        public string ReadFileText(string path,bool needUtf8 = false)
        {
            if(!File.Exists(path))
            {
                return "";
            }
            string str;
            if(needUtf8)
            {
                str = File.ReadAllText(path, Encoding.UTF8);
            }
            else
            {
                str = File.ReadAllText(path, Encoding.Default);
            }
            
            return str;
        }

        public string ReadFileTextByCode(string path, Encoding code)
        {
            if (!File.Exists(path))
            {
                return "";
            }
            string str = File.ReadAllText(path,code);
            //if (needUtf8)
            //{
            //    str = File.ReadAllText(path, Encoding.UTF8);
            //}
            //else
            //{
            //    str = File.ReadAllText(path, Encoding.Default);
            //}

            return str;
        }

        public string GetMouldPath(string mouldName)
        {
            string mouldPath = Application.StartupPath + "\\Mould\\" + mouldName + ".txt";
            return mouldPath;
        }

        public List<string> GetAllFiles(string path, string exName)
        {
            List<string> names = new List<string>();
            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles();
            string ex;
            for (int i = 0; i < files.Length; i++)
            {
                ex = FilePathHelper.GetExName(files[i].FullName);
                if (ex != exName)
                {
                    continue;
                }
                names.Add(files[i].FullName);
            }
            DirectoryInfo[] dirs = root.GetDirectories();
            if (dirs.Length > 0)
            {
                for (int i = 0; i < dirs.Length; i++)
                {
                    List<string> subNames = GetAllFiles(dirs[i].FullName, exName);
                    if (subNames.Count > 0)
                    {
                        for (int j = 0; j < subNames.Count; j++)
                        {
                            names.Add(subNames[j]);
                        }
                    }
                }
            }

            return names;

        }

        public List<string> GetAllFilesExcept(string path, string exName)
        {
            List<string> names = new List<string>();
            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles();
            string ex;
            for (int i = 0; i < files.Length; i++)
            {
                ex = FilePathHelper.GetExName(files[i].FullName);
                if (ex == exName)
                {
                    continue;
                }
                names.Add(files[i].FullName);
            }
            DirectoryInfo[] dirs = root.GetDirectories();
            if (dirs.Length > 0)
            {
                for (int i = 0; i < dirs.Length; i++)
                {
                    List<string> subNames = GetAllFilesExcept(dirs[i].FullName, exName);
                    if (subNames.Count > 0)
                    {
                        for (int j = 0; j < subNames.Count; j++)
                        {
                            names.Add(subNames[j]);
                        }
                    }
                }
            }

            return names;

        }

        public List<string> GetSubFolders(string path)
        {

            DirectoryInfo root = new DirectoryInfo(path);

            DirectoryInfo[] dirs = root.GetDirectories();
            List<string> folders = new List<string>();
            if (dirs.Length > 0)
            {
                for (int i = 0; i < dirs.Length; i++)
                {
                    folders.Add(dirs[i].FullName);
                }
            }

            return folders;

        }

        public void CheckDirection(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public bool IsDirectoryExists(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsFileExists(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static void CopyFiles(string varFromDirectory, string varToDirectory)
        {//实现从一个目录下完整拷贝到另一个目录下。 
            Directory.CreateDirectory(varToDirectory);
            if (!Directory.Exists(varFromDirectory))
            {
                //m_eorrStr = "对不起，您要拷贝的目录不存在。 ";
                return;
            }

            string[] directories = Directory.GetDirectories(varFromDirectory);//取文件夹下所有文件夹名，放入数组； 
            if (directories.Length > 0)
            {
                foreach (string d in directories)
                {
                    int ind = d.LastIndexOf("\\");
                    if (ind >= 0)
                        CopyFiles(d, varToDirectory + d.Substring(ind));//递归拷贝文件和文件夹 
                }
            }

            string[] files = Directory.GetFiles(varFromDirectory);//取文件夹下所有文件名，放入数组； 
            if (files.Length > 0)
            {
                foreach (string s in files)
                {
                    int ind = s.LastIndexOf("\\");
                    if (ind >= 0)
                        File.Copy(s, varToDirectory + s.Substring(ind));
                }
            }
        }

        private void CheckFileSavePath(string path)
        {
            string realPath = path;
            int ind = path.LastIndexOf("\\");
            if(ind>=0)
            {
                realPath = path.Substring(0, ind);
            }
            CheckDirection(realPath);
        }

        /// <summary>
        /// 复制一个文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="tarPath"></param>
        public void CopyFile(string path, string tarPath)
        {
            if (!IsFileExists(path))
            {
                return;
            }
            CheckFileSavePath(tarPath);
            File.Copy(path, tarPath, true);
        }

    }
}
