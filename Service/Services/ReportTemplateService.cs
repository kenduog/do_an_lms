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
using System.Threading;
using Request.RequestCreate;
using System.Configuration;

namespace Service.Services
{
    public class ReportTemplateService : DomainService<tbl_ReportTemplate, BaseSearch>, IReportTemplateService
    {
        public ReportTemplateService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "Get_ReportTemplate";
        }

        public async Task<tbl_ReportTemplate> GetByCode(string code)
        {
            var item = await this.unitOfWork.Repository<tbl_ReportTemplate>().GetQueryable()
                .FirstOrDefaultAsync(x => x.code == code && x.deleted == false) ?? throw new AppException(MessageContants.nf_reportTemplate);
            return item;
        }
    }
}
