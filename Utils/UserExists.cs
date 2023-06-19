using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pajoma_nvtbot.Utils
{
    public static class UserTools
    {
        public static bool UserExists(int ID)
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Users\\"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\Users\\");
            }

            return File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Users\\" + ID.ToString() + ".ini");
        }
    }
}
