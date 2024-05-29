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
    public class BranchService : DomainService<tbl_Branch, BranchSearch>, IBranchService
    {
        public BranchService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "Get_Branch";
        }

        public override async Task AddItem(tbl_Branch model)
        {
            await this.Validate(model);
            await this.CreateAsync(model);

            List<tbl_StudySession> studySessions = new List<tbl_StudySession>();
            for(int i = 1; i <=10; i++)
            {
                studySessions.Add(new tbl_StudySession { index = i, branchId = model.id }) ; 
            }
            await this.unitOfWork.Repository<tbl_StudySession>().CreateAsync(studySessions);
            await this.unitOfWork.SaveAsync();
        }
        //public override async Task<PagedList<tbl_Branch>> GetPagedListData(BranchSearch baseSearch)
        //{
        //    PagedList<tbl_Branch> pagedList = new PagedList<tbl_Branch>();
        //    var ids = await unitOfWork.Repository<tbl_Branch>().GetQueryable()
        //        .Where(x => x.deleted == false
        //        && (string.IsNullOrEmpty(baseSearch.searchContent) || x.code.Contains(baseSearch.searchContent) || x.name.Contains(baseSearch.searchContent))
        //        && x.cityId == (baseSearch.cityId.HasValue ? baseSearch.cityId : x.cityId))
        //        .OrderByDescending(x => x.created)
        //        .Select(x => x.id).ToListAsync();

        //    if (ids.Any())
        //    {
        //        pagedList.totalItem = ids.Count();
        //    }

        //    var data = ids.Skip((baseSearch.pageIndex - 1) * baseSearch.pageSize).Take(baseSearch.pageSize).ToList();
        //    pagedList.items = (from i in data
        //                       select Task.Run(() => this.GetByIdAsync(i)).Result).ToList();
        //    pagedList.pageIndex = baseSearch.pageIndex;
        //    pagedList.pageSize = baseSearch.pageSize;
        //    return pagedList;
        //}
        public override async Task<bool> DeleteAsync(Guid id)
        {
            var exists = Queryable
                .FirstOrDefault(e => e.id == id);
            if (exists != null)
            {
                exists.deleted = true;
                unitOfWork.Repository<tbl_Branch>().Update(exists);
                await unitOfWork.SaveAsync();
                Thread clearBranchInUser = new Thread(() =>
                BackgroundService.ClearBranchInUser(id));
                clearBranchInUser.Start();
                return true;
            }
            else
            {
                throw new AppException(id + " not exists");
            }
        }
        public async Task<List<DomainOption>> GetBranchByIds(string branchIds)
        {
            if (string.IsNullOrEmpty(branchIds))
                return new List<DomainOption>();
            var listBranchId = branchIds.Split(',').ToList();
            return await unitOfWork.Repository<tbl_Branch>()
                .GetQueryable().Where(x => listBranchId.Contains(x.id.ToString()))
                .Select(x=> new DomainOption
                { 
                    id = x.id,
                    name = x.name
                }).ToListAsync();
        }
    }
}
