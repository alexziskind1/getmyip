using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Mail;
using System.Configuration;


namespace GetMyIP
{
    class Program
    {
        static string autoUrl = string.Empty;
        static string smtpServer = string.Empty;
        static string smtpPort = string.Empty;
        static string emailFrom = string.Empty;
        static string emailPass = string.Empty;
        static string emailTo = string.Empty;

        static void Main(string[] args)
        {
            try
            {
                Console.SetWindowSize(1, 1);
                autoUrl = ConfigurationManager.AppSettings["AutomationUrl"];
                smtpServer = ConfigurationManager.AppSettings["SMTPServer"];
                smtpPort = ConfigurationManager.AppSettings["SMTPPort"];
                emailFrom = ConfigurationManager.AppSettings["EmailFrom"];
                emailPass = ConfigurationManager.AppSettings["EmailPass"];
                emailTo = ConfigurationManager.AppSettings["EmailTo"];

                string ip = GetIP();

                SendIPNotification(ip);
            }
            catch(Exception ex)
            {}
        }

        private static void SendIPNotification(string ip)
        {
            //Create Mail Message Object with content that you want to send with mail.
            

            MailMessage MyMailMessage = new MailMessage(emailFrom, emailTo, ip, ip);
            
            MyMailMessage.IsBodyHtml = false;

            //Proper Authentication Details need to be passed when sending email from gmail
            NetworkCredential mailAuthentication = new NetworkCredential(emailFrom, emailPass);

            //Smtp Mail server of Gmail is "smpt.gmail.com" and it uses port no. 587
            //For different server like yahoo this details changes and you can
            //get it from respective server.
            SmtpClient mailClient = new SmtpClient(smtpServer, Convert.ToInt32(smtpPort));
            mailClient.UseDefaultCredentials = false;
            mailClient.Credentials = mailAuthentication;
            
            mailClient.Send(MyMailMessage);

        }

        private static string GetIP()
        {
            String direction = "";
            WebRequest request = WebRequest.Create(autoUrl);
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                direction = stream.ReadToEnd();
            }

            //Search for the ip in the html
            int first = direction.IndexOf("Address: ") + 9;
            int last = direction.LastIndexOf("</body>");
            direction = direction.Substring(first, last - first);

            return direction;
        }
    }
}
