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
using System.Net.Mail;
using Request.DomainRequests;
using Microsoft.Extensions.Configuration;
using static Utilities.CoreContants;
using System.Net;
using Request.RequestCreate;
using System.Buffers.Text;
using Entities.AuthEntities;
using OneSignal.RestAPIv3.Client.Resources.Notifications;

namespace Service.Services
{
    public class ParentService : DomainService<tbl_Parent, ParentSearch>, IParentService
    {
        private readonly IUserService userService;
        public ParentService(IAppUnitOfWork unitOfWork,
            IMapper mapper,
            IUserService userService) : base(unitOfWork, mapper)
        {
            this.userService = userService;
        }
        protected override string GetStoreProcName()
        {
            return "Get_Parent";
        }
        public async Task<List<StudentByParent>> GetStudentByParent(Guid parentId)
        {
            return await unitOfWork.Repository<tbl_Student>()
                .GetQueryable().Where(x => x.deleted == false
                    && (x.fatherId == parentId || x.motherId == parentId || x.guardianId == parentId))
                .Select(x => new StudentByParent
                {
                    id = x.id,
                    fullName = x.fullName,
                    code = x.code,
                    thumbnail = x.thumbnail
                    
                }).ToListAsync();
        }
        public async Task<ParentModel> GetParent(Guid parentId)
        {
            var parent = await unitOfWork.Repository<tbl_Parent>().GetQueryable().FirstOrDefaultAsync(x => x.id == parentId && x.deleted == false);
            if (parent == null)
                return null;
            var user = await unitOfWork.Repository<tbl_Users>().GetQueryable().FirstOrDefaultAsync(x => x.id == parent.userId && x.deleted == false);
            if (user == null)
                return null;
            return new ParentModel
            {
                id = user.id,
                fullName = user.fullName,
                bod = user.birthday,
                job = parent.job,
                phone = user.phone,
                thumbnail = user.thumbnail,
                type = parent.type,
                typeName = parent.typeName
            };
        }

        public async Task<List<tbl_Users>> GetParentUserByStudentId(List<Guid> studentIds)
        {
            List<tbl_Users> result = new List<tbl_Users>();

            //parent ids
            var parentIds = await unitOfWork.Repository<tbl_Student>()
                .GetQueryable()
                .Where(x => x.deleted == false && studentIds.Contains(x.id) &&
                        (x.motherId.HasValue || x.fatherId.HasValue || x.guardianId.HasValue))
                .Select(x => x.motherId)
                .Union(unitOfWork.Repository<tbl_Student>()
                    .GetQueryable()
                    .Where(x => x.deleted == false && studentIds.Contains(x.id) && x.fatherId.HasValue)
                    .Select(x => x.fatherId))
                .Union(unitOfWork.Repository<tbl_Student>()
                    .GetQueryable()
                    .Where(x => x.deleted == false && studentIds.Contains(x.id) && x.guardianId.HasValue)
                    .Select(x => x.guardianId))
                .Where(id => id.HasValue)
                .Distinct()
                .ToListAsync();

            //user ids 
            var userIds = await this.unitOfWork.Repository<tbl_Parent>().GetQueryable()
                .Where(x => x.deleted == false && parentIds.Contains(x.id) && x.userId.HasValue)
                .Select(x => x.userId.Value)
                .Distinct()
                .ToListAsync();

            result = await this.unitOfWork.Repository<tbl_Users>().GetQueryable()
                .Where(x => x.deleted == false && userIds.Contains(x.id))
                .ToListAsync();

            return result;
        }
        public async Task<List<tbl_Users>> GetParentUserByBranchId(Guid branchId)
        {
            List<tbl_Users> result = new List<tbl_Users>();

            //parent 
            var parentIds = await unitOfWork.Repository<tbl_Parent>()
                .GetQueryable()
                .Where(x => x.deleted == false && x.userId != null)
                .Select(x => x.userId).ToListAsync();

            //user ids 
            foreach (var userId in parentIds)
            {
                var user = await this.unitOfWork.Repository<tbl_Users>().GetQueryable()
                .FirstOrDefaultAsync(x => x.deleted == false
                && x.id == userId
                && x.deleted == false
                && x.branchIds.Contains(branchId.ToString()));
                if (user != null)
                    result.Add(user);
            }
            return result.Distinct().ToList();
        }
    }
}
