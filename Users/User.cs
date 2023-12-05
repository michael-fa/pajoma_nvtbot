using DSharpPlus.Entities;
using pajoma_nvtbot.Discord;
using pajoma_nvtbot.Web;
using pajoma_nvtbot.Web.Routines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace pajoma_nvtbot.Users
{
    public class User
    {
        //public int m_Id { get; set; }
        //public string m_Password { get; set; } = null!;
        //public string m_DCID = null!;
        public DiscordMessage m_lastmsg = null!;
        private Thread m_thread = null!;
        public Driver m_Driver = null!;
        public Properties m_IniFile = null!;
        private bool m_threadRun;
        private int m_MsgCount = 0;
        private bool m_Mute = false;
        public string m_JournalStr = null!;



        //Create a user
        public User(int ID, string Password, string dcID)
        {
            //m_DiscordHandle = dcuser;

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Users/" + ID.ToString() + ".ini"))
            {
                m_IniFile = new Properties(AppDomain.CurrentDomain.BaseDirectory + "/Users/" + ID.ToString() + ".ini");

                m_IniFile.set("ID", ID.ToString());
                m_IniFile.set("Password", Password);
                m_IniFile.set("DiscordUserID", dcID);
                m_IniFile.Save();
            }

            //Bot settings to default?

            RunBotThread();


        }

        //Load a user
        public User(int ID)
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Users/" + ID.ToString() + ".ini"))
            {
                m_IniFile = new Properties(AppDomain.CurrentDomain.BaseDirectory + "/Users/" + ID.ToString() + ".ini");

                //Find every message send by us and delete it, clean up a lil

                if (MainBot.Client == null) return;
                if (MainBot.Client.GetChannelAsync(Convert.ToUInt64(m_IniFile.get("DiscordUserID"))).Result.GetMessagesAsync().Result != null!)
                {
                    if (MainBot.Client.GetChannelAsync(Convert.ToUInt64(m_IniFile.get("DiscordUserID"))).Result.GetMessagesAsync().Result.Count > 4)
                        Console.WriteLine("[INFO] Deleting old messages in chat with user " + m_IniFile.get("DiscordUserID") + " will take about " + MainBot.Client.GetChannelAsync(Convert.ToUInt64(m_IniFile.get("DiscordUserID"))).Result.GetMessagesAsync().Result.Count() + " seconds, freezing actions for this user.");

                    int fast = 0;
                    foreach (DiscordMessage x in MainBot.Client.GetChannelAsync(Convert.ToUInt64(m_IniFile.get("DiscordUserID"))).Result.GetMessagesAsync().Result)
                    {
                        if (!x.Content.Contains("NVT"))
                        {
                            fast--;
                            if (fast < -3) break;
                            continue;
                        }
                        if (x.Author.Id == MainBot.Client.CurrentUser.Id)
                            x.DeleteAsync();
                        Thread.Sleep(500);
                        if (fast == 6)
                        {
                            Thread.Sleep(4400);
                            fast = 0;
                        }
                        fast++;
                    }
                }




                //Load bot settings

                Console.WriteLine("[INFO] Loaded User " + m_IniFile.get("ID") + " from save file.");
                RunBotThread();
            }
        }

        private void RunBotThread()
        {
            //Start thread and such
            m_threadRun = true;
            m_thread = new Thread(Loop);
            m_thread.Start();
        }

        async void Loop()
        {




            //Fetch user options
            bool _Done = false;

            while (m_threadRun)
            {
#if !DEBUG
                if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                {
                    Thread.Sleep((60 * 60000));
                    continue;
                }
                //Console.WriteLine("Tick");

                if (DateTime.Now.Hour == 9 || DateTime.Now.Hour == 10 || DateTime.Now.Hour == 11 || DateTime.Now.Hour == 12 || DateTime.Now.Hour == 13 || DateTime.Now.Hour == 14 || DateTime.Now.Hour == 15 || DateTime.Now.Hour == 16 || DateTime.Now.Hour == 17 || DateTime.Now.Hour == 18)
#endif
                {
                    try
                    {
                        m_Driver = new Driver(this);

                        string[] tmp = Journal.GetFormatedSummary(this);

                        if (tmp.Length != 10 || tmp[2].Contains("T00"))
                        {
                            /*foreach (Process prs in Process.GetProcesses())
                            {
                                if (prs.Id == m_Driver.m_Proc)
                                {
                                    prs.Kill();
                                    Console.WriteLine("Firefox proccess " + m_Driver.m_Proc + " killed.");
                                    break;
                                }
                            }*/
                            m_Driver.m_Driver.Close();
                            m_Driver.m_Driver.Dispose();
                            m_Driver.m_Driver = null!;
                            m_MsgCount = 0;
                            Thread.Sleep(1000);
                            continue;
                        }


                        if (tmp[2].Trim().Contains('F') || tmp[2].Trim().Contains('F'))
                        {
                            Thread.Sleep((60 * 60000));
                            _Done = true;

                        }
                        else
                        {
                            try
                            {
                                DiscordDmChannel ch = null!;
                                if (MainBot.Client != null)
                                {
                                    ch = (DiscordDmChannel)MainBot.Client.GetChannelAsync(Convert.ToUInt64(m_IniFile.get("DiscordUserID"))).Result;
                                }

#if DEBUG
                                    Console.WriteLine("Float pause " + Math.Abs(Convert.ToDouble(tmp[4].Trim())));
#endif

                                if (m_MsgCount >= 24)
                                {
                                    if (tmp.Length < 10 || tmp[2].Contains("T00"))
                                    {
                                        m_Mute = false;
                                        //"Gleiche scheiße, neuer Tag beginnt"
                                        m_MsgCount = 0;
                                        Thread.Sleep(5 * 60000);
                                        continue;
                                    }
                                    if (m_Mute) continue;
                                    if (m_lastmsg! != null!) await ch.DeleteMessageAsync(m_lastmsg);
                                    m_lastmsg = null!;
                                    DiscordMessage x = await ch.SendMessageAsync("[PJ-NVT] Du bist nun zwei Stunden abwesend, bis morgen!");
                                    Thread.Sleep(25 * 60000);
                                    m_Mute = true;
                                    continue;
                                }

                                if (tmp[4].StartsWith('1') || tmp[4].StartsWith('2') || tmp[4].StartsWith('3') || tmp[4].StartsWith('4'))
                                {
                                    if (m_lastmsg! != null!) await ch.DeleteMessageAsync(m_lastmsg);
                                    DiscordMessage x = await ch.SendMessageAsync("[PJ-NVT] Deine heutige Arbeitszeit beträgt aktuell " + tmp[2].Trim() + ", Pausenzeit " + tmp[4].Trim() + ", Tagessaldo " + tmp[6].Trim() + ", Gesamtsaldo " + tmp[7].Trim() + " Stunden. Rauch mal weniger!");
                                    m_lastmsg = x;
                                }
                                else
                                {
                                    if (m_lastmsg! != null!) await ch.DeleteMessageAsync(m_lastmsg);
                                    DiscordMessage x = await ch.SendMessageAsync("[PJ-NVT] Deine heutige Arbeitszeit beträgt aktuell " + tmp[2].Trim() + ", Pausenzeit " + tmp[4].Trim() + ", Tagessaldo " + tmp[6].Trim() + ", Gesamtsaldo " + tmp[7].Trim() + " Stunden.");
                                    m_lastmsg = x;
                                }
                                m_MsgCount++;
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine(ex.Message + "\n" + ex.InnerException + "\n" + ex.Source);
                            }

                            _Done = true;
                        }

                        if (_Done && m_Driver.m_Driver != null)
                        {
                            m_Driver.m_Driver.Close();
                            m_Driver.m_Driver.Dispose();
                            m_Driver.m_Driver = null!;
                            _Done = false;
                        }

#if DEBUG

                        Thread.Sleep(2*60000);
#else
                        Thread.Sleep(5 * 60000);
#endif
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine("Error in routine:  \n" + ex.Message + "\n" + ex.InnerException + "\n--------------------------------\nException occurs at:\n" + ex.Source);
                    }
                }

                //Urlaub?
                //Wenn user aktiviert hat: Alle "neuen" einträge tracen

                Thread.Sleep(1000);
            }
            Console.WriteLine("User " + m_IniFile.get("ID") + " has been removed from loop.");
        }
        public void StopBotThread()
        {
            m_threadRun = false;
            //stop thread and all
        }
    }
}
