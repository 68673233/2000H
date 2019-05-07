using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsServiceClient.Manage
{
    class PathManage
    {
       public static string ServiceFilePath = $"{Application.StartupPath}\\MyWindowsService.exe";
       public static string ServiceName = "MyService";

       public static string IniFilePath= $"{Application.StartupPath}\\config.ini";

        public static string LogDir= $"{Application.StartupPath}\\Log";
    }
}
