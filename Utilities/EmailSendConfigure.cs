using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Gửi mail
    /// </summary>
    public class EmailSendConfigure
    {
        public string smtpServer { set; get; }
        public IList<string> tos { get; set; }
        public string emailTo { get; set; }
        public IList<string> ccs { get; set; }
        public string from { get; set; }
        public string fromEmail { get; set; }
        public bool enableSsl { get; set; }
        public int port { get; set; }
        public string fromDisplayName { get; set; }
        public string subject { get; set; }
        public MailPriority priority { get; set; }
        public string clientCredentialUserName { get; set; }
        public string clientCredentialPassword { get; set; }
        public IList<string> bccs { get; set; }
        public EmailContent content{ get; set; }

        public EmailSendConfigure()
        {
            tos = new List<string>();
            ccs = new List<string>();
            bccs = new List<string>();
        }
    }
}
