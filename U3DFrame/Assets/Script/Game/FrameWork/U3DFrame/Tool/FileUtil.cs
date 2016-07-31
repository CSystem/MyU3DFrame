using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace U3DFrame.Tool
{
    public class FileUtil
    {
        static public void CopyDirectory(string source, string dest)
        {
            string[] files = Directory.GetFiles(source);

            for (int i = 0; i < files.Length; i++)
            {
                string str = Path.GetFileName(files[i]);

                string path = dest + "/" + str;
                string dir = Path.GetDirectoryName(path);
                Directory.CreateDirectory(dir);
                File.Copy(files[i], path, true);
            }

            string[] directories = Directory.GetDirectories(source);
            for(int j = 0; j < directories.Length; j++)
            {
                string directName = Path.GetFileName(directories[j]);
                string path = dest + "/" + directName;
                CopyDirectory(directories[j], path);
            }
        }
    }
}

