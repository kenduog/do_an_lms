using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities
{
    public class sys_Log : DomainEntities.DomainEntities
    {
        public sys_Log() { }
        public sys_Log(LogLevel logLevel, LogType logType, string shortMessage, string fullMessage, string ipAddress, Guid? userId, string pageUrl, string referrerUrl) 
        {
            this.logLevel = logLevel;
            this.logType = logType;
            this.shortMessage = shortMessage;
            this.fullMessage = fullMessage;
            this.ipAddress = ipAddress;
            this.userId = userId;
            this.pageUrl = pageUrl;
            this.referrerUrl = referrerUrl;
        }

        public sys_Log(Exception exception) 
        {
            logLevel = LogLevel.Error;
            ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
            created = Timestamp.Now();
            shortMessage = exception.Message;
            fullMessage = exception.ToString();
            logType = LogType.Basic;
        }

        [Required]
        public LogLevel logLevel { get; set; }
        [Required]
        public LogType logType { get; set; }
        public string shortMessage { get; set; }
        public string fullMessage { get; set; }
        [MaxLength(100)]
        public string ipAddress { get; set; }
        public Guid? userId { get; set; }
        public string pageUrl { get; set; }
        public string referrerUrl { get; set; }

    }
    public enum LogLevel
    {
        Debug = 10,
        Information = 20,
        Warning = 30,
        Error = 40,
        Fatal = 50
    }
    public enum LogType
    {
        Basic = 1,
        Notification = 2,
    }

}
