using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.RequestUpdate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Interface.Services
{
    public interface IReceiveOrderHeaderService : IDomainService<tbl_ReceiveOrderHeader, ReceiveOrderHeaderSearch>
    {
        Task ChangeStatus(ReceiveOrderHeaderStatusUpdate request);
    }
}
