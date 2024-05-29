using Entities.AuthEntities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services
{
    public interface ISendNotificationService
    {
        string EncodingParam(string toEncode);
        void SendAllNotification(string notificationContent, string notficationTitle, bool isSendEmail, string link, string deeplink);
        void SendNotification(Guid notificationConfigId, List<tbl_Users> receiverList, List<IDictionary<string, string>> notiParams, List<IDictionary<string, string>> emailParams, string linkQuery, IDictionary<string, string> deepLinkQueryDic, string screen, string subLink);
        void SendNotification_v2(
            string notificationConfigCode,
            string title,
            string content,
            List<tbl_Users> receiverList,
            List<IDictionary<string, string>> notiParams,
            List<IDictionary<string, string>> emailParams,
            string linkQuery,
            IDictionary<string, string> deepLinkQueryDic,
            string screen,
            IDictionary<string, string> param);
    }
}
