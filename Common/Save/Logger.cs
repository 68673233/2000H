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

        public string FilePath=null;

        private  string fileName;

        public Action<string> OnLoggerI=null;

        public void init(string filePath,Action<string> _onLoggerI) {
            this.FilePath = filePath;
            OnLoggerI = _onLoggerI;
        }

        /// <summary>
        /// 显示及保存
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        public void i(string tag,string val) {
            string content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "   " + tag + ":" + val;
            if (FilePath != null) {
                fileName = DateTime.Now.ToString("yyyy-MM-dd");
                string path = FilePath + "\\"+fileName;
                try
                {
                    File.AppendAllText(path, content + Environment.NewLine);
                    //File.AppendAllText(path, Environment.NewLine);
                }
                catch (Exception e) {

                }
                OnLoggerI?.Invoke(content);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        public void infoToFile(string tag, string val) {
            string content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "   " + tag + ":" + val;
            if (FilePath != null)
            {
                fileName = DateTime.Now.ToString("yyyy-MM-dd");
                string path = FilePath + "\\" + fileName;
                File.AppendAllText(path, content);
                File.AppendAllText(path, Environment.NewLine);
            }
        }
        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        public void infoToShow(string tag, string val) {
            string content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "   " + tag + ":" + val;
            OnLoggerI?.Invoke(content);
        }
    }
}
