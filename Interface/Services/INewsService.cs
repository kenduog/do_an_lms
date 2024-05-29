using Entities;
using Entities.AuthEntities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.RequestUpdate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface INewsService : IDomainService<tbl_News, NewsSearch>
    {
        Task Pin(Guid newId);
        Task UnPin(Guid newId);
        Task PinPositionUpdate(PinPositionUpdate itemModel);
    }
}
