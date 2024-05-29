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

namespace Service.Services
{
    public class DayOfWeekService : DomainService<tbl_DayOfWeek, BaseSearch>, IDayOfWeekService
    {
        public DayOfWeekService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "Get_DayOfWeek";
        }
        public async Task<tbl_DayOfWeek> GetByKeyAsync(int key)
        {
            return await this.unitOfWork.Repository<tbl_DayOfWeek>().GetQueryable().FirstOrDefaultAsync(x =>
            x.key == key
            && x.deleted == false);
        }
    }
}
