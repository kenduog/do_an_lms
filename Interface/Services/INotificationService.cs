using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services
{
    public interface INotificationService : IDomainService<tbl_Notification, NotificationSearch>
    {
        Task<NotificationResponse> GetNotificationToSend(Guid? uid);
        Task<NotificationResponse> GetNotificationIsNotSeen(Guid? uid, int count);
        Task<bool> UpdateNotificationToSeenOfUser(Guid? uid);
    }
}
