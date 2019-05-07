using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Save
{
    public class Logger
    {
        public static Logger logger = new Logger();
        public static Logger Instance { get { return logger; } }

        private string FilePath=null;

        private  string fileName;

        private Action<string> OnLoggerI=null;

        public void init(string filePath,Action<string> _onLoggerI) {
            this.FilePath = filePath;
            OnLoggerI = _onLoggerI;
        }


        public void i(string tag,string val) {
            string content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "   " + tag + ":" + val;
            if (FilePath != null) {
                fileName = DateTime.Now.ToString("yyyy-MM-dd");
                string path = FilePath + "\\"+fileName;
                File.AppendAllText(path,  content);
                File.AppendAllText(path, Environment.NewLine);
                OnLoggerI?.Invoke(content);
            }
            
        }
    }
}
