using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fileDelete
{
    static class FileDelete
    {
        public static void fileDelete(string FilePath, string Style)
        {
            try {
                string path = Environment.CurrentDirectory;
                string[] strFileName = Directory.GetFiles(FilePath, Style);
                foreach (var item in strFileName)
                {
                    File.Delete(item);
                }
            }
            catch (System.IO.DirectoryNotFoundException) {// 文件路径不对的时候才会触发这个错误
            }
        }
    }
}
