using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;


namespace Interface.Services
{
    public interface IFeeService : IDomainService<tbl_Fee, FeeSearch>
    {
        Task<PagedList<tbl_Fee>> GetFeeByCollectionPlan(GetFeeByCollectionPlanRequest request);
    }
}
