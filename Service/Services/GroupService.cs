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
using Entities.DomainEntities;
using Entities.AuthEntities;

namespace Service.Services
{
    public class GroupService : DomainService<tbl_Group, BaseSearch>, IGroupService
    {
        public GroupService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "Get_Group";
        }
        public async Task<tbl_Group> GetByCode(string code)
        {
            return await unitOfWork.Repository<tbl_Group>().GetQueryable()
                .FirstOrDefaultAsync(x => x.code == code);
        }
    }
}
