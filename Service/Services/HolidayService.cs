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
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Extensions.DependencyInjection;
using Azure.Core;

namespace Service.Services
{
    public class HolidayService : DomainService<tbl_Holiday, BaseSearch>, IHolidayService
    {
        public HolidayService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "Get_Holiday";
        }
        public async Task<bool> CheckHoliday(double date)
        {
            return await this.unitOfWork.Repository<tbl_Holiday>().GetQueryable().
            AnyAsync(x =>
            (x.sTime <= date && x.eTime >= date)
                && x.deleted == false);
        }
    }
}
