using Entities.Search;
using Entities;
using Interface.Services.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services
{
   public interface INotificationConfigService : IDomainService<tbl_NotificationConfig, NotificationConfigSearch>
    {

    }
}
