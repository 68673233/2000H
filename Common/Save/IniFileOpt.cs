using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Common
{
    public class IniFileOpt
    {
        #region "声明变量"

        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="section">节点名称[如[TypeName]]</param>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键</param>
        /// <param name="def">值</param>
        /// <param name="retval">stringbulider对象</param>
        /// <param name="size">字节大小</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);

        //private string strFilePath = Application.StartupPath + "\\FileConfig.ini";//获取INI文件路径
        //private string strSec = ""; //INI文件名



        #endregion

        #region 属性

        private string strFilePath;
        private string strSec = "";

        #endregion

        #region  key
        public const string IP = "IP";
        public const string Port = "Port";
        #endregion

        public IniFileOpt(string filePath) {
            this.strFilePath = filePath;
        }

        public bool write(string key,string val) {
            WritePrivateProfileString(strSec, key, val, strFilePath);
            return true;
        }

        public string read(string key) {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(strSec, key, "", temp, 1024, strFilePath);
            return temp.ToString();
        }

    }
}
