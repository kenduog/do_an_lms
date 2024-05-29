using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class SendMail
    {
        private static IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();
        public static void Send(string to, string title, string content)
        {
            try
            {
                string fromAddress = configuration.GetSection("MySettings:Email").Value.ToString();
                string mailPassword = configuration.GetSection("MySettings:PasswordMail").Value.ToString();
                SmtpClient client = new SmtpClient();
                client.Port = 587;//outgoing port for the mail.
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Timeout = 1000000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(fromAddress, mailPassword);

                // Fill the mail form.
                var send_mail = new MailMessage();
                send_mail.IsBodyHtml = true;
                //address from where mail will be sent.
                send_mail.From = new MailAddress(fromAddress);
                //address to which mail will be sent.           
                send_mail.To.Add(new MailAddress(to));
                //subject of the mail.
                send_mail.Subject = title;
                send_mail.Body = content;
                client.Send(send_mail);
            }
            catch
            {
                return;
            }
        }
    }
}
