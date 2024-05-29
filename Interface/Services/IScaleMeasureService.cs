using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.RequestCreate;
using Request.RequestUpdate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Interface.Services
{
    public interface IScaleMeasureService : IDomainService<tbl_ScaleMeasure, ScaleMeasureSearch>
    {
        Task SendNotification(ScaleMeasureNotificationRequest request);
    }
}
