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

namespace Service.Services
{
    public class SchoolYearService : DomainService<tbl_SchoolYear, BaseSearch>, ISchoolYearService
    {
        public SchoolYearService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "Get_SchoolYear";
        }
        public override async Task DeleteItem(Guid id)
        {
            await this.unitOfWork.SaveAsync();
            await DeleteAsync(id);
            Thread clearSchoolYear = new Thread(() =>
            BackgroundService.ClearSchoolYear(id));
            clearSchoolYear.Start();
        }
    }
}
