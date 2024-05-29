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
using Service.Services;
using Entities.AuthEntities;
using System.Net.WebSockets;
using Microsoft.AspNetCore.StaticFiles;

namespace Service.Auth
{
    public class ContentTypeService : DomainService<tbl_ContentType, ContentTypeSearch>, IContentTypeService
    {
        public ContentTypeService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "Get_ContentType";
        }

        public async Task<List<tbl_ContentType>> GetData(ContentTypeSearch baseSearch)
        {
            List<tbl_ContentType> allMenu = new List<tbl_ContentType>();
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            allMenu = this.unitOfWork.Repository<tbl_ContentType>().ExcuteStoreAsync(this.GetStoreProcName(), parameters).Result.ToList();
            return allMenu;
        }

        public async Task<List<tbl_ContentType>> GetAllMenu()
         => this.unitOfWork.Repository<tbl_ContentType>().ExcuteStoreAsync("Get_AllMenu", new SqlParameter[] { }).Result.ToList();

      
        public async Task<List<tbl_ContentType>> GetMenu(Guid? userId, bool isSuperUser, string groupCode)
        {
            SqlParameter sqlParameter = new SqlParameter("@userId", userId ?? null);
            SqlParameter sqlParameter2 = new SqlParameter("@isSuperUser", isSuperUser);
            SqlParameter sqlParameter3 = new SqlParameter("@groupCode", groupCode);
            string spName = "Get_Menu";
            var datas = await unitOfWork.Repository<tbl_ContentType>().ExcuteStoreAsync(spName, new SqlParameter[] { sqlParameter, sqlParameter2, sqlParameter3 });
            return datas.ToList();
        }

        public async Task<List<tbl_ContentType>> GetMenuByGroup(Guid? groupId)
        {
            SqlParameter sqlParameter = new SqlParameter("groupId", groupId ?? null);
            string spName = "Get_MenuByGroup";
            var datas = await unitOfWork.Repository<tbl_ContentType>().ExcuteStoreAsync(spName, new SqlParameter[] { sqlParameter });

            if (datas == null || datas.Count == 0)
                throw new AppException(MessageContants.err);

            var parent = datas.Where(x => x.allowed == true && x.parentId.HasValue).Select(x=>x.parentId.Value).ToList();
            datas = EnableParent(parent, datas.ToList());

            var contentTypes = datas
                .Where(x => x.isRoot == true)
                .Select(x => new tbl_ContentType { id = x.id, hasChild = x.hasChild, allowed = x.allowed, code = x.code, name = x.name, route = x.route, isRoot = x.isRoot })
                .Distinct()
                .ToList();

            var result = GetChild(contentTypes, datas.ToList());

            foreach (var contentType in result)
            {
                if (contentType.allowed == true && contentType.childs != null && contentType.childs.All(x => x.allowed == false) )
                    contentType.allowed = false;
            }
            return result.ToList();
        }
        private List<tbl_ContentType> EnableParent(List<Guid> parents, List<tbl_ContentType> source)
        {
            if (parents == null || parents.Count == 0)
                return source;
            
            //allowed for parent
            foreach(var parent in parents)
            {
                var item = source.FirstOrDefault(x => x.id == parent);
                if (item != null)
                    item.allowed = true;
            }

            //get parent of parent to đệ qui
            var parentOfParent = source.Where(x=>parents.Contains(x.id) && x.parentId.HasValue).Select(x=>x.parentId.Value).ToList();
            source = EnableParent(parentOfParent, source);

            return source;
        }

        private List<tbl_ContentType> GetChild(List<tbl_ContentType> parents, List<tbl_ContentType> source)
        {
            if (parents == null || parents.Count == 0)
                return null;
            foreach (var parent in parents)
            {
                var details = source.Where(x => x.parentId == parent.id).ToList();
                parent.childs = GetChild(details, source);
            }
            return parents;
        }
    }
}
