using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.DevTools;
using pajoma_nvtbot.Web.Routines;

namespace pajoma_nvtbot.Web
{
    public class Driver
    {
        public IWebDriver m_Driver = null!;
        public FirefoxDriverService m_DriverService = FirefoxDriverService.CreateDefaultService();
        public IJavaScriptExecutor m_JavaScript = null!;
        public int m_Proc;

        private Users.User m_User;
        public Driver(Users.User user) 
        {
            
            m_User = user;

            try
            {
                FirefoxOptions options = new FirefoxOptions();
                options.AddArguments("--headless");
                m_Driver = new FirefoxDriver(m_DriverService, options);
                m_Proc = m_DriverService.ProcessId;
            }
            catch
            {

                m_Driver = new FirefoxDriver(m_DriverService);
            }
            
            //Log.Debug("PID = " + m_DriverService.ProcessId);
            m_Driver.Manage().Window.Size = new System.Drawing.Size(1100, 950);
            m_Driver.Manage().Window.Position = new System.Drawing.Point(850, 1);
            m_JavaScript = (IJavaScriptExecutor)m_Driver;
            m_Driver.Url = Program.m_NvtLink;
            Console.WriteLine("[INFO] Web Driver for User " + user.m_Id + " initiated.");



            m_Driver.SwitchTo().Frame("F3");

            Thread.Sleep(2000);

            IWebElement x = m_Driver.FindElement(By.Name("Ident1"));

            //Enter data

            x.SendKeys(user.m_Id.ToString());
            Thread.Sleep(1000);

            x = m_Driver.FindElement(By.Name("Ident2"));
            x.SendKeys(user.m_Password.ToString());
            Thread.Sleep(1000);

            x = m_Driver.FindElement(By.Name("Btn0001"));
            x.Click();


            Thread.Sleep(3000);

            Console.WriteLine("[INFO] [NOVA-TIME] Loggedin as User " + m_User.m_Id);

            
        } 
    }
}
