using Entities.Search;
using Entities;
using Interface.Services.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Model.Statistical;
using Utilities;

namespace Interface.Services
{
    public interface IStatisticalService
    {
        Task<List<StatisticalModel>> FinanceOverview(StatisticalSearch baseSearch);
        Task<List<Statistical12MonthModel>> Revenue12Month(StatisticalSearch baseSearch);
        Task<List<StatisticalModel>> RateTutionConfig(StatisticalSearch baseSearch);
        Task<PagedList<BillInDebtModel>> ReportBillInDebt(StatisticalSearch baseSearch);
    }
}
