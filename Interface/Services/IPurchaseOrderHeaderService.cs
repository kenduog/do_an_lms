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
    public interface IPurchaseOrderHeaderService : IDomainService<tbl_PurchaseOrderHeader, PurchaseOrderHeaderSearch>
    {
        Task<tbl_PurchaseOrderHeader> GetByIdAsync(Guid id);
        Task<tbl_PurchaseOrderHeader> ChangeStatus(ChangeStatusPurchase item);
    }
}
