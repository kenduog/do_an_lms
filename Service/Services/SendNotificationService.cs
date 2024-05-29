using Entities;
using Entities.AuthEntities;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.Services.Background;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OneSignal.RestAPIv3.Client;
using OneSignal.RestAPIv3.Client.Resources;
using OneSignal.RestAPIv3.Client.Resources.Notifications;
using Service.Services.Background;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Utilities;

namespace Service.Services
{
    public class SendNotificationService : ISendNotificationService
    {

        protected readonly IBackgroundTaskQueue taskQueue;
        protected readonly ILogService logService;

        public SendNotificationService(IBackgroundTaskQueue taskQueue, ILogService logService)
        {
            this.taskQueue = taskQueue;
            this.logService = logService;
        }


        private class DeepLink
        {
            public string link { get; set; }
            public IDictionary<string, string> linkParams { get; set; }
        }

        public string EncodingParam(string toEncode)
        {
            toEncode = HttpUtility.UrlEncode(toEncode);
            toEncode = Uri.EscapeUriString(toEncode);
            byte[] bytes = Encoding.GetEncoding(28591).GetBytes(toEncode);
            string toReturn = System.Convert.ToBase64String(bytes);
            return toReturn;
        }

        public void SendAllNotification(string notificationContent, string notficationTitle, bool isSendEmail, string link, string deeplink)
        {
            taskQueue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = RuntimeBackgroundService.ServiceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var _hubContext = scopedServices.GetRequiredService<IDomainHub>();
                    var _userService = scopedServices.GetRequiredService<IUserService>();
                    var _configuration = scopedServices.GetRequiredService<IConfiguration>();
                    var userList = _userService.GetAll();
                    if (userList != null)
                    {
                        foreach (var user in userList)
                        {
                            OneSignalPushNotification(user.oneSignalId, notficationTitle, notificationContent, deeplink, _configuration);
                            await _hubContext.SendNotification("send-to-user", user.id.ToString(), new { Title = notficationTitle, Content = notificationContent, Link = link });
                            if (isSendEmail)
                            {
                                SendEmailText(notficationTitle, user.email, notificationContent, null, _configuration);
                            }
                        }
                    }
                }
            });
        }
        public void SendNotification(Guid notificationConfigId, List<tbl_Users> receiverList, List<IDictionary<string, string>> notiParams, List<IDictionary<string, string>> emailParams, string linkQuery, IDictionary<string, string> deepLinkQueryDic, string screen, string subLink)
        {
            taskQueue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = RuntimeBackgroundService.ServiceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var _configuration = scopedServices.GetRequiredService<IConfiguration>();
                    var _userService = scopedServices.GetRequiredService<IUserService>();
                    var _hubContext = scopedServices.GetRequiredService<IDomainHub>();
                    var _notificationService = scopedServices.GetRequiredService<INotificationService>();
                    var _notificationConfigService = scopedServices.GetRequiredService<INotificationConfigService>();
                    var notificationConfig = await _notificationConfigService.GetByIdAsync(notificationConfigId);
                    if (notificationConfig != null)
                    {
                        notificationConfig.linkForAll += (subLink + "?" + linkQuery);
                        notificationConfig.linkForParent += (subLink + "?" + linkQuery);
                        notificationConfig.linkForTeacher += (subLink + "?" + linkQuery);

                        var deepLinkForAll = new DeepLink()
                        {
                            link = notificationConfig.deepLinkForAll,
                            linkParams = deepLinkQueryDic
                        };
                        var deepLinkForParent = new DeepLink()
                        {
                            link = notificationConfig.deepLinkForParent,
                            linkParams = deepLinkQueryDic
                        };

                        var deepLinkForTeacher = new DeepLink()
                        {
                            link = notificationConfig.deepLinkForTeacher,
                            linkParams = deepLinkQueryDic
                        };
                        string deepLinkForAllJson = JsonConvert.SerializeObject(deepLinkForAll);
                        string deepLinkForParentJson = JsonConvert.SerializeObject(deepLinkForParent);
                        string deepLinkForTeacherJson = JsonConvert.SerializeObject(deepLinkForTeacher);
                        if (notificationConfig.isSendNotiAll)
                        {
                            int index = 0;
                            foreach (var recipient in receiverList)
                            {

                                string notificationContent = notificationConfig.notiContentForAll;
                                foreach (var item in notiParams[index].Keys)
                                {
                                    notificationContent = notificationContent.Replace(item, notiParams[index][item]);
                                }
                                OneSignalPushNotification(recipient.oneSignalId, notificationConfig.notiTitle, notificationContent, notificationConfig.deepLinkForAll, _configuration);
                                await _hubContext.SendNotification("send-to-user", recipient.id.ToString(), new { Title = notificationConfig.notiTitle, Content = notificationContent, Link = notificationConfig.linkForAll, DeepLink = deepLinkForAll });
                                tbl_Notification notification = new tbl_Notification()
                                {
                                    userId = recipient.id,
                                    content = notificationContent,
                                    link = notificationConfig.linkForAll,
                                    deepLink = deepLinkForAllJson,
                                    title = notificationConfig.notiTitle,
                                };
                                await _notificationService.CreateAsync(notification);
                                Debug.WriteLine("[Notification] " + recipient.username);

                                index++;
                            }
                        }
                        else
                        {
                            if (notificationConfig.isSendNotiParent)
                            {
                                int index = 0;
                                foreach (var recipient in receiverList)
                                {
                                    var groupList = (await _userService.GetGroupByUserId(recipient.id)).Item2;
                                    bool isValid = false;
                                    foreach (var group in groupList)
                                    {
                                        if (group.code == "PH")
                                        {
                                            isValid = true;
                                            break;
                                        }
                                    }
                                    if (isValid)
                                    {
                                        string notificationContent = notificationConfig.notiContentForParent;
                                        foreach (var item in notiParams[index].Keys)
                                        {
                                            notificationContent = notificationContent.Replace(item, notiParams[index][item]);
                                        }
                                        OneSignalPushNotification(recipient.oneSignalId, notificationConfig.notiTitle, notificationContent, notificationConfig.deepLinkForParent, _configuration);
                                        await _hubContext.SendNotification("send-to-user", recipient.id.ToString(), new { Title = notificationConfig.notiTitle, Content = notificationContent, Link = notificationConfig.linkForParent, DeepLink = deepLinkForParent });
                                        tbl_Notification notification = new tbl_Notification()
                                        {
                                            userId = recipient.id,
                                            content = notificationContent,
                                            link = notificationConfig.linkForParent,
                                            deepLink = deepLinkForParentJson,
                                            title = notificationConfig.notiTitle,
                                            screen = screen
                                        };
                                        await _notificationService.CreateAsync(notification);
                                        Debug.WriteLine("[Notification] " + recipient.username);
                                    }
                                    index++;
                                }
                            }

                            if (notificationConfig.isSendNotiTeacher)
                            {

                                var index = 0;
                                foreach (var recipient in receiverList)
                                {
                                    var groupList = (await _userService.GetGroupByUserId(recipient.id)).Item2;
                                    bool isValid = false;
                                    foreach (var group in groupList)
                                    {
                                        if (group.code == "GV")
                                        {
                                            isValid = true;
                                            break;
                                        }
                                    }
                                    if (isValid)
                                    {
                                        string notificationContent = notificationConfig.notiContentForTeacher;
                                        foreach (var item in notiParams[index].Keys)
                                        {
                                            notificationContent = notificationContent.Replace(item, notiParams[index][item]);
                                        }
                                        OneSignalPushNotification(recipient.oneSignalId, notificationConfig.notiTitle, notificationContent, notificationConfig.deepLinkForTeacher, _configuration);
                                        await _hubContext.SendNotification("send-to-user", recipient.id.ToString(), new { Title = notificationConfig.notiTitle, Content = notificationContent, Link = notificationConfig.linkForTeacher, DeepLink = deepLinkForTeacher });
                                        tbl_Notification notification = new tbl_Notification()
                                        {
                                            userId = recipient.id,
                                            content = notificationContent,
                                            link = notificationConfig.linkForTeacher,
                                            deepLink = deepLinkForTeacherJson,
                                            title = notificationConfig.notiTitle,
                                        };
                                        await _notificationService.CreateAsync(notification);
                                        Debug.WriteLine("[Notification] " + recipient.username);
                                    }
                                    index++;
                                }
                            }
                        }
                        if (notificationConfig.isSendEmailAll)
                        {
                            int index = 0;
                            foreach (var recipient in receiverList)
                            {
                                emailParams[index].Add("[Link]", notificationConfig.linkForAll);
                                SendEmailFileTemplate(notificationConfig.notiTitle, recipient.email, notificationConfig.emailTemplateFileAll, emailParams[index], _configuration);
                            }
                            index++;
                        }
                        else
                        {
                            if (notificationConfig.isSendEmailParent)
                            {
                                int index = 0;
                                foreach (var recipient in receiverList)
                                {
                                    var groupList = (await _userService.GetGroupByUserId(recipient.id)).Item2;
                                    bool isValid = false;
                                    foreach (var group in groupList)
                                    {
                                        if (group.code == "PH")
                                        {
                                            isValid = true;
                                            break;
                                        }
                                    }
                                    if (isValid)
                                    {
                                        emailParams[index].Add("[Link]", notificationConfig.linkForParent);
                                        SendEmailFileTemplate(notificationConfig.notiTitle, recipient.email, notificationConfig.emailTemplateFileParent, emailParams[index], _configuration);
                                    }
                                }
                                index++;
                            }

                            if (notificationConfig.isSendEmailTeacher)
                            {
                                int index = 0;
                                foreach (var recipient in receiverList)
                                {
                                    var groupList = (await _userService.GetGroupByUserId(recipient.id)).Item2;
                                    bool isValid = false;
                                    foreach (var group in groupList)
                                    {
                                        if (group.code == "GV")
                                        {
                                            isValid = true;
                                            break;
                                        }
                                    }
                                    if (isValid)
                                    {
                                        emailParams[index].Add("[Link]", notificationConfig.linkForTeacher);
                                        SendEmailFileTemplate(notificationConfig.notiTitle, recipient.email, notificationConfig.emailTemplateFileTeacher, emailParams[index], _configuration);
                                    }
                                }
                                index++;
                            }
                        }
                    }
                }
            });
        }


        public void SendNotification_v2(
            string notificationConfigCode,
            string title,
            string content,
            List<tbl_Users> receiverList,
            List<IDictionary<string, string>> notiParams,
            List<IDictionary<string, string>> emailParams,
            string linkQuery,
            IDictionary<string, string> deepLinkQueryDic,
            string screen,
            IDictionary<string, string> param
            )
        {
            taskQueue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = RuntimeBackgroundService.ServiceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var _configuration = scopedServices.GetRequiredService<IConfiguration>();
                    var _userService = scopedServices.GetRequiredService<IUserService>();
                    var _hubContext = scopedServices.GetRequiredService<IDomainHub>();
                    var _notificationService = scopedServices.GetRequiredService<INotificationService>();
                    var _notificationConfigService = scopedServices.GetRequiredService<INotificationConfigService>();
                    var _appDbContext = scopedServices.GetRequiredService<IAppDbContext>();

                    var notificationConfig = await _notificationConfigService.GetSingleAsync(x => x.deleted == false && x.code == notificationConfigCode);

                    //return if config not found
                    if (notificationConfig == null)
                    {
                        sys_Log log = new sys_Log(LogLevel.Error, LogType.Notification,
                            LookupConstant.NotificationFailShortMessage, $"Notification Config Code: {notificationConfigCode}",
                            Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString(),
                            null, null, null);
                        //add log
                        //await this.logService.InsertLog(log);
                        log.created = Timestamp.Now();
                        await _appDbContext.Set<sys_Log>().AddAsync(log);
                        await _appDbContext.SaveChangesAsync();
                        return;
                    }

                    //continue 
                    notificationConfig.linkForAll += "?" + linkQuery;
                    notificationConfig.linkForParent += "?" + linkQuery;
                    notificationConfig.linkForTeacher += "?" + linkQuery;

                    var deepLinkForAll = new DeepLink()
                    {
                        link = notificationConfig.deepLinkForAll,
                        linkParams = deepLinkQueryDic
                    };
                    var deepLinkForParent = new DeepLink()
                    {
                        link = notificationConfig.deepLinkForParent,
                        linkParams = deepLinkQueryDic
                    };
                    var deepLinkForTeacher = new DeepLink()
                    {
                        link = notificationConfig.deepLinkForTeacher,
                        linkParams = deepLinkQueryDic
                    };

                    string deepLinkForAllJson = JsonConvert.SerializeObject(deepLinkForAll);
                    string deepLinkForParentJson = JsonConvert.SerializeObject(deepLinkForParent);
                    string deepLinkForTeacherJson = JsonConvert.SerializeObject(deepLinkForTeacher);
                    string paramString = JsonConvert.SerializeObject(param);

                    string notificationTitle = string.IsNullOrEmpty(title) ? notificationConfig.notiTitle : title;

                    //signal
                    if (notificationConfig.isSendNotiAll)
                    {
                        int index = 0;
                        foreach (var recipient in receiverList)
                        {

                            string notificationContent = string.IsNullOrEmpty(content) ? notificationConfig.notiContentForAll : content;
                            foreach (var item in notiParams[index].Keys)
                            {
                                notificationContent = notificationContent.Replace(item, notiParams[index][item]);
                            }
                            OneSignalPushNotification(recipient.oneSignalId, notificationConfig.notiTitle, notificationContent, notificationConfig.deepLinkForAll, _configuration);
                            await _hubContext.SendNotification("send-to-user", recipient.id.ToString(), new { Title = notificationConfig.notiTitle, Content = notificationContent, Link = notificationConfig.linkForAll, DeepLink = deepLinkForAll });
                            tbl_Notification notification = new tbl_Notification()
                            {
                                userId = recipient.id,
                                content = notificationContent,
                                link = notificationConfig.linkForAll,
                                deepLink = deepLinkForAllJson,
                                title = notificationTitle,
                                screen = screen,
                                param = paramString
                            };
                            await _notificationService.CreateAsync(notification);
                            Debug.WriteLine("[Notification] " + recipient.username);
                            index++;
                        }
                    }
                    else
                    {
                        if (notificationConfig.isSendNotiParent)
                        {
                            int index = 0;
                            foreach (var recipient in receiverList)
                            {
                                var groupList = (await _userService.GetGroupByUserId(recipient.id)).Item2;
                                bool isValid = false;
                                foreach (var group in groupList)
                                {
                                    if (group.code == "PH")
                                    {
                                        isValid = true;
                                        break;
                                    }
                                }
                                if (isValid)
                                {
                                    string notificationContent = string.IsNullOrEmpty(content) ? notificationConfig.notiContentForParent : content;
                                    if (notiParams != null && notiParams.Count > 0)
                                    {
                                        foreach (var item in notiParams[index].Keys)
                                        {
                                            notificationContent = notificationContent.Replace(item, notiParams[index][item]);
                                        }
                                    }
                                    OneSignalPushNotification(recipient.oneSignalId, notificationConfig.notiTitle, notificationContent, notificationConfig.deepLinkForParent, _configuration);
                                    await _hubContext.SendNotification("send-to-user", recipient.id.ToString(), new { Title = notificationConfig.notiTitle, Content = notificationContent, Link = notificationConfig.linkForParent, DeepLink = deepLinkForParent });
                                    tbl_Notification notification = new tbl_Notification()
                                    {
                                        userId = recipient.id,
                                        content = notificationContent,
                                        link = notificationConfig.linkForParent,
                                        deepLink = deepLinkForParentJson,
                                        title = notificationTitle,
                                        screen = screen,
                                        param = paramString
                                    };
                                    await _notificationService.CreateAsync(notification);
                                    Debug.WriteLine("[Notification] " + recipient.username);
                                }
                                index++;
                            }
                        }

                        if (notificationConfig.isSendNotiTeacher)
                        {

                            var index = 0;
                            foreach (var recipient in receiverList)
                            {
                                var groupList = (await _userService.GetGroupByUserId(recipient.id)).Item2;
                                bool isValid = false;
                                foreach (var group in groupList)
                                {
                                    if (group.code == "GV")
                                    {
                                        isValid = true;
                                        break;
                                    }
                                }
                                if (isValid)
                                {
                                    string notificationContent = string.IsNullOrEmpty(content) ? notificationConfig.notiContentForTeacher : content;
                                    foreach (var item in notiParams[index].Keys)
                                    {
                                        notificationContent = notificationContent.Replace(item, notiParams[index][item]);
                                    }
                                    OneSignalPushNotification(recipient.oneSignalId, notificationConfig.notiTitle, notificationContent, notificationConfig.deepLinkForTeacher, _configuration);
                                    await _hubContext.SendNotification("send-to-user", recipient.id.ToString(), new { Title = notificationConfig.notiTitle, Content = notificationContent, Link = notificationConfig.linkForTeacher, DeepLink = deepLinkForTeacher });
                                    tbl_Notification notification = new tbl_Notification()
                                    {
                                        userId = recipient.id,
                                        content = notificationContent,
                                        link = notificationConfig.linkForTeacher,
                                        deepLink = deepLinkForTeacherJson,
                                        title = notificationTitle,
                                        screen = screen,
                                        param = paramString
                                    };
                                    await _notificationService.CreateAsync(notification);
                                    Debug.WriteLine("[Notification] " + recipient.username);
                                }
                                index++;
                            }
                        }
                    }

                    //email
                    if (notificationConfig.isSendEmailAll)
                    {
                        int index = 0;
                        foreach (var recipient in receiverList)
                        {
                            emailParams[index].Add("[Link]", notificationConfig.linkForAll);
                            SendEmailFileTemplate(notificationConfig.notiTitle, recipient.email, notificationConfig.emailTemplateFileAll, emailParams[index], _configuration);
                        }
                        index++;
                    }
                    else
                    {
                        if (notificationConfig.isSendEmailParent)
                        {
                            int index = 0;
                            foreach (var recipient in receiverList)
                            {
                                var groupList = (await _userService.GetGroupByUserId(recipient.id)).Item2;
                                bool isValid = false;
                                foreach (var group in groupList)
                                {
                                    if (group.code == "PH")
                                    {
                                        isValid = true;
                                        break;
                                    }
                                }
                                if (isValid)
                                {
                                    emailParams[index].Add("[Link]", notificationConfig.linkForParent);
                                    SendEmailFileTemplate(notificationConfig.notiTitle, recipient.email, notificationConfig.emailTemplateFileParent, emailParams[index], _configuration);
                                }
                            }
                            index++;
                        }

                        if (notificationConfig.isSendEmailTeacher)
                        {
                            int index = 0;
                            foreach (var recipient in receiverList)
                            {
                                var groupList = (await _userService.GetGroupByUserId(recipient.id)).Item2;
                                bool isValid = false;
                                foreach (var group in groupList)
                                {
                                    if (group.code == "GV")
                                    {
                                        isValid = true;
                                        break;
                                    }
                                }
                                if (isValid)
                                {
                                    emailParams[index].Add("[Link]", notificationConfig.linkForTeacher);
                                    SendEmailFileTemplate(notificationConfig.notiTitle, recipient.email, notificationConfig.emailTemplateFileTeacher, emailParams[index], _configuration);
                                }
                            }
                            index++;
                        }
                    }
                }
            });
        }

        protected virtual string GetTemplateFilePath(string fileTemplateName)
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string path = System.IO.Path.Combine(currentDirectory, CoreContants.TEMPLATE_FOLDER_NAME, "EmailTemplate", fileTemplateName);
            if (!System.IO.File.Exists(path))
                throw new AppException("File template không tồn tại!");
            return path;
        }

        private void SendEmailFileTemplate(string subject, string to, string emailTemplateFile, IDictionary<string, string> emailParam, IConfiguration configuration)
        {
            string fromEmail = configuration.GetSection("MySettings").GetValue<string>("Email");
            string fromEmailPassword = configuration.GetSection("MySettings").GetValue<string>("PasswordMail");
            string emailDisplayname = configuration.GetSection("MySettings").GetValue<string>("ProjectName");

            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = subject;
            mailMessage.SubjectEncoding = Encoding.UTF8;
            mailMessage.To.Add(to);
            string emailFilePath = GetTemplateFilePath(emailTemplateFile);
            string emailBody = File.ReadAllText(emailFilePath);
            if (emailParam != null)
            {
                foreach (var item in emailParam.Keys)
                {
                    emailBody = emailBody.Replace(item, emailParam[item]);
                }
            }
            mailMessage.Body = emailBody;
            mailMessage.IsBodyHtml = true;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.Priority = MailPriority.Normal;

            mailMessage.From = new MailAddress(fromEmail, emailDisplayname, Encoding.UTF8);

            SmtpClient client = new SmtpClient
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, fromEmailPassword),
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true
            };

            try
            {
                client.Send(mailMessage);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                Console.WriteLine("SmtpFailedRecipientsExceptionMessage:" + ex.Message);
                Console.WriteLine("SmtpFailedRecipientsException:" + ex.StackTrace);
                throw new Exception("SmtpFailedRecipientsException:" + ex.StackTrace);
            }
            catch (Exception e)
            {
                Console.WriteLine("ExceptionMAILMessage:" + e.Message);
                Console.WriteLine("ExceptionMAIL:" + e.StackTrace);
                throw new Exception("ExceptionMAIL:" + e.StackTrace);
            }
            finally
            {
                mailMessage.Dispose();
            }
        }

        private void SendEmailText(string subject, string to, string emailBody, IDictionary<string, string> emailParam, IConfiguration configuration)
        {
            string fromEmail = configuration.GetSection("MySettings").GetValue<string>("Email");
            string fromEmailPassword = configuration.GetSection("MySettings").GetValue<string>("PasswordMail");
            string emailDisplayname = configuration.GetSection("MySettings").GetValue<string>("ProjectName");

            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = subject;
            mailMessage.SubjectEncoding = Encoding.UTF8;
            mailMessage.To.Add(to);

            if (emailParam != null)
            {
                foreach (var item in emailParam.Keys)
                {
                    emailBody = emailBody.Replace(item, emailParam[item]);
                }
            }
            mailMessage.Body = emailBody;
            mailMessage.IsBodyHtml = true;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.Priority = MailPriority.Normal;

            mailMessage.From = new MailAddress(fromEmail, emailDisplayname, Encoding.UTF8);

            SmtpClient client = new SmtpClient
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, fromEmailPassword),
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true
            };

            try
            {
                client.Send(mailMessage);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                Console.WriteLine("SmtpFailedRecipientsExceptionMessage:" + ex.Message);
                Console.WriteLine("SmtpFailedRecipientsException:" + ex.StackTrace);
                throw new Exception("SmtpFailedRecipientsException:" + ex.StackTrace);
            }
            catch (Exception e)
            {
                Console.WriteLine("ExceptionMAILMessage:" + e.Message);
                Console.WriteLine("ExceptionMAIL:" + e.StackTrace);
                throw new Exception("ExceptionMAIL:" + e.StackTrace);
            }
            finally
            {
                mailMessage.Dispose();
            }
        }

        private void OneSignalPushNotification(string playerId, string heading, string notificationContent, string deepLink, IConfiguration configuration)
        {
            Guid appId = configuration.GetSection("MySettings").GetValue<Guid>("OnesignalAppId");
            string restKey = configuration.GetSection("MySettings").GetValue<string>("OnesignalRestId");
            Thread oneSignalThread = new Thread(async () =>
            {
                OneSignalClient client = new OneSignalClient(restKey);
                List<string> playerIds = new List<string>() { playerId };
                var opt = new NotificationCreateOptions()
                {
                    AppId = appId,
                    IncludePlayerIds = playerIds,
                    SendAfter = DateTime.Now.AddSeconds(5)
                };
                opt.Headings.Add(LanguageCodes.Vietnamese, heading);
                opt.Contents.Add(LanguageCodes.Vietnamese, notificationContent);
                opt.Url = deepLink;
                try
                {
                    await client.Notifications.CreateAsync(opt);
                }
                catch (AppException)
                {
                    throw new AppException("Gửi thông báo One Signal thất bại");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[Lỗi playerID]");
                }
            });
            oneSignalThread.Start();
        }
    }
}
