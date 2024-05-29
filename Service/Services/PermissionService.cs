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
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace Service.Services
{
    public class PermissionService : DomainService<tbl_Permission, PermissionSearch>, IPermissionService
    {
        public PermissionService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "Get_PermissionList";
        }

        public async Task<List<tbl_Permission>> GetAction(string code, Guid? userId, bool isSuperUser, string selectedGroupCode)
        {
            SqlParameter[] sqlParameters = new SqlParameter[] {
                new SqlParameter("code", code),
                new SqlParameter("userId", userId),
                new SqlParameter("isSuperUser", isSuperUser),
                new SqlParameter("selectedGroup", selectedGroupCode),
            };
            string spName = "Get_Action";
            var datas = await this.unitOfWork.Repository<tbl_Permission>().ExcuteStoreAsync(spName, sqlParameters);
            return datas.ToList();
        }

        public async Task<List<tbl_Permission>> GetPermission(Guid? userId, string controller, string action)
        {
            SqlParameter[] sqlParameters = new SqlParameter[] {
                new SqlParameter("controller", controller),
                new SqlParameter("action", action),
                new SqlParameter("userId", userId),
            };
            string spName = "Get_Permission";
            var datas = await this.unitOfWork.Repository<tbl_Permission>().ExcuteStoreAsync(spName, sqlParameters);
            return datas.ToList();
        }
    }
}
