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
using System.Net;
using Request.RequestCreate;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Service.Services
{
    public class ClassShiftService : DomainService<tbl_ClassShift, BaseSearch>, IClassShiftService
    {
        private readonly IAppDbContext appDbContext;
        private readonly IAutoGenCodeConfigService autoGenCodeConfigService;
        public ClassShiftService(IAppUnitOfWork unitOfWork, 
            IMapper mapper, 
            IAutoGenCodeConfigService autoGenCodeConfigService,
            IAppDbContext appDbContext) : base(unitOfWork, mapper)
        {
            this.appDbContext = appDbContext;
            this.autoGenCodeConfigService = autoGenCodeConfigService;
        }
        protected override string GetStoreProcName()
        {
            return "Get_ClassShift";
        }
    }
}
