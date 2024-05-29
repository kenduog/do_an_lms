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
using AppDbContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Internal;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Service.Services
{
    public class TuitionConfigService : DomainService<tbl_TuitionConfig, TuitionConfigSearch>, ITuitionConfigService
    {
        private IAppDbContext appDbContext;
        private INecessaryService necessaryService;
        private IConfiguration configuration;
        public TuitionConfigService(IAppUnitOfWork unitOfWork
            , IMapper mapper, IAppDbContext appDbContext
            , IConfiguration configuration
            , INecessaryService necessaryService) : base(unitOfWork, mapper)
        {
            this.appDbContext = appDbContext;
            this.necessaryService = necessaryService;
            this.configuration = configuration;
        }
        protected override string GetStoreProcName()
        {
            return "Get_TuitionConfig";
        }
    }
}
