using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pajoma_nvtbot.Web.Routines
{
    public static class Journal
    {
        private static void GetJournal(Users.User user)
        {
            user.m_Driver.m_Driver.SwitchTo().DefaultContent();
            user.m_Driver.m_Driver.SwitchTo().Frame("F2");
            Thread.Sleep(2000);

            IWebElement x =  user.m_Driver.m_Driver.FindElement(By.Name("Btn0100"));
            x.Click();

            Thread.Sleep(2000);

            user.m_Driver.m_Driver.SwitchTo().DefaultContent();
            user.m_Driver.m_Driver.SwitchTo().Frame("F4");
            Thread.Sleep(2500);

            IWebElement tableElement = user.m_Driver.m_Driver.FindElement(By.CssSelector(".art3"));
            IList<IWebElement> tableRow = tableElement.FindElements(By.TagName("tr"));
            IList<IWebElement> rowTD;
            int idx = 0;
            foreach (IWebElement row in tableRow)
            {
                rowTD = row.FindElements(By.TagName("td"));

                if (rowTD.Count > 9)
                {
                    if (rowTD[8].Text.Equals("Suspended") && rowTD[10].Text.Equals("Publishing Failed"))
                        Console.Write("Failed.");
                    if (row.Text.Equals(" ") || row.Text.Length == 0)
                        continue;
                    else
                    {
                        //Console.WriteLine(row.Text);
#if !DEBUG
                        if (idx == (tableRow.Count-2))
                        {
                            user.m_JournalStr = row.Text;
                            string.Format(user.m_JournalStr, string.Join(", ", rowTD));
                        }
#else

                        string[] spl = row.Text.Trim().Split('\n');
                        if (row.Text.Contains("6,02") || row.Text.Contains("700") &&  row.Text.Contains("-38,27"))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            
                            user.m_JournalStr = row.Text;
                            Console.WriteLine("DEBUG INFORMATION: " + user.m_JournalStr);
                            Console.ForegroundColor = ConsoleColor.White;
                            string.Format(user.m_JournalStr, string.Join(", ", rowTD));
                            break;
                        }
#endif

                    }
                    //test failed
                }
                idx++;
            }

            return;
        }
        public static string[] GetFormatedSummary(Users.User user)
        {
            GetJournal(user); //stored in user.m_JournalStr

            if (user.m_JournalStr != null)
            {
                string[] tmp = user.m_JournalStr.Split('\n');
                return tmp;
            }
            else return null!;
        }
    }
}
