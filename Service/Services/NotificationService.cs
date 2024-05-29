using Entities;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Service.Services.DomainServices;
using Entities.Search;
using Newtonsoft.Json;
using Entities.DomainEntities;
using Request.RequestCreate;
using Microsoft.AspNetCore.Mvc;

namespace Service.Services
{
    public class NotificationService : DomainService<tbl_Notification, NotificationSearch>, INotificationService
    {
        public NotificationService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "Get_Notification";
        }

        public async Task<NotificationResponse> GetNotificationToSend(Guid? uid)
        {
            var result = new NotificationResponse();
            result.total = await this.unitOfWork.Repository<tbl_Notification>().GetQueryable().CountAsync(x=>x.userId == uid && x.isSeen == false);
            result.notifications = await this.unitOfWork.Repository<tbl_Notification>().GetQueryable()
                .Where(x => x.userId == uid).OrderBy(d => d.isSeen).ThenByDescending(s => s.created).Take(10).ToListAsync();
            return result;
        }

        public async Task<NotificationResponse> GetNotificationIsNotSeen(Guid? uid, int count)
        {
            var result = new NotificationResponse();
            result.total = await this.unitOfWork.Repository<tbl_Notification>().GetQueryable().CountAsync(x => x.userId == uid && x.isSeen == false);
            result.notifications = await this.unitOfWork.Repository<tbl_Notification>().GetQueryable()
                .Where(x => x.userId == uid).OrderBy(d => d.isSeen).ThenByDescending(s => s.created).Take(count).ToListAsync();
            return result;
        }

        public async Task<bool> UpdateNotificationToSeenOfUser(Guid? uid)
        {
            try
            {
                var notificationList = await this.unitOfWork.Repository<tbl_Notification>().GetQueryable().Where(x => x.userId == uid && x.isSeen == false).ToListAsync();
                foreach (var notification in notificationList)
                {
                    notification.isSeen = true;
                    this.unitOfWork.Repository<tbl_Notification>().Update(notification);
                }
                await this.unitOfWork.SaveChangesAsync(true);
                return true;
            }catch 
            {
                return false;
            }
        }

    }

}
