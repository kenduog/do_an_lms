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

namespace Service.Services
{
    public class RoomService : DomainService<tbl_Room, RoomSearch>, IRoomService
    {
        public RoomService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        public override async Task<PagedList<tbl_Room>> GetPagedListData(RoomSearch baseSearch)
        {
            PagedList<tbl_Room> pagedList = new PagedList<tbl_Room>();
            var ids = await unitOfWork.Repository<tbl_Room>().GetQueryable()
                .Where(x => x.deleted == false
                && x.branchId == baseSearch.branchId 
                && (String.IsNullOrEmpty(baseSearch.searchContent) || x.name.Contains(baseSearch.searchContent)))
                .OrderByDescending(x => x.created)
                .Select(x => x.id).ToListAsync();

            if (ids.Any())
            {
                pagedList.totalItem = ids.Count();
            }

            var data = ids.Skip((baseSearch.pageIndex - 1) * baseSearch.pageSize).Take(baseSearch.pageSize).ToList();
            pagedList.items = (from i in data
                               select Task.Run(() => this.GetByIdAsync(i)).Result).ToList();
            pagedList.pageIndex = baseSearch.pageIndex;
            pagedList.pageSize = baseSearch.pageSize;
            return pagedList;
        }
    }
}
