using AutoMapper;
using Entities;
using Entities.AuthEntities;
using Entities.Search;
using Interface.Services;
using Interface.UnitOfWork;
using Service.Services.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class NotificationConfigService : DomainService<tbl_NotificationConfig, NotificationConfigSearch>, INotificationConfigService
    {
        public NotificationConfigService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
