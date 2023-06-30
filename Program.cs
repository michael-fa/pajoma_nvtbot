using OpenQA.Selenium;
using pajoma_nvtbot.Discord;
using pajoma_nvtbot.Web;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;



namespace pajoma_nvtbot
{
    internal class Program
    {
        internal static bool g_run = true;

        [DllImport("Kernel32")]
        static public extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
        public delegate bool EventHandler(CtrlType sig);
        public static EventHandler? m_Handler;
        public static string m_NvtLink = null!;

        public enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        public static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    StopSafely();
                    return false;
                default:
                    return false;
            }
        }

        public static void StopSafely()
        {
            foreach (Users.User u in m_UserList)
            {
                u.StopBotThread();

            }

            g_run = false;
            return;
        }



        public static List<Users.User> m_UserList = new List<Users.User>();

        static void Main(string[] args)
        {
#if DEBUG
             Console.WriteLine("------ DEBUGMODE!!!!");
#endif

            Console.OutputEncoding = Encoding.Unicode;
            m_Handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(m_Handler, true);

            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Users\\"))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\Users\\");



            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\config.ini"))
                File.Create(AppDomain.CurrentDomain.BaseDirectory + "\\config.ini");

            IniFile basecfg = new IniFile(AppDomain.CurrentDomain.BaseDirectory + "\\config.ini");
            if (!basecfg.KeyExists("auth-token", "Discord Bot")) basecfg.Write("auth-token", "changeme", "Discord Bot");

            if (!basecfg.KeyExists("url", "Novatime WEB Driver")) basecfg.Write("url", "changeme", "Novatime WEB Driver");

            if (basecfg.Read("auth-token", "Discord Bot").Equals("changeme"))
            {
                Console.WriteLine("Discord Bot Token not set! Please change it to a valid bot token in settings.cfg");
                Console.ReadLine();
                Program.StopSafely();
                return;
            }

            if (basecfg.Read("url", "Novatime WEB Driver").Equals("changeme"))
            {
                Console.WriteLine("Novatime WEB Url not set! Please change it to a valid bot token in settings.cfg");
                Console.ReadLine();
                Program.StopSafely();
                return;
            }
            else m_NvtLink = basecfg.Read("url", "Novatime WEB Driver");





            MainBot.Init(basecfg.Read("auth-token", "Discord Bot"));
            MainBot.Start();

            foreach (string file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Users\\"))
            {
                Console.WriteLine("Found user " + file);
                string fl = file.Replace(".ini", "").PadRight(1);
                string[] xfl = fl.Split('\\');


                m_UserList.Add(new Users.User(int.Parse(xfl[xfl.Length - 1])));
            }

            while(g_run)
            {
                Thread.Sleep(60000);
            }

            while (true)
            {

            }

        }
    }
}