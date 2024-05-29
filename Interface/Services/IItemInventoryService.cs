using Entities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.RequestCreate;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.DataTransferObject;
using Request.RequestUpdate;
using System;
using Entities.DomainEntities;

namespace Interface.Services
{
    public interface IItemInventoryService : IDomainService<tbl_ItemInventory, ItemInventorySearch>
    {
        Task<string> Export(ItemInventorySearch baseSearch);
        Task<List<InventoryDetailBySKU>> DetailInventory(Guid id);
    }
}
