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
using Entities.AuthEntities;

namespace Service.Services
{
    public class UserJoinGroupNewsService : DomainService<tbl_UserJoinGroupNews, UserJoinGroupNewsSearch>, IUserJoinGroupNewsService
    {
        public UserJoinGroupNewsService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        public async Task<int> GetTypeUser(Guid groupId, Guid userId)
        {
            int type = 0;
            var userInGroup = unitOfWork.Repository<tbl_UserJoinGroupNews>()
                    .GetQueryable().FirstOrDefault(x => x.groupNewsId == groupId && x.userId == userId && x.deleted == false);
            if (userInGroup != null)
                type = userInGroup.userType ?? 0;
            return type;
        }
        public async Task<string> GetTypeUserName(Guid groupId, Guid userId)
        {
            string result = string.Empty;
            int type = 0;
            var userInGroup = unitOfWork.Repository<tbl_UserJoinGroupNews>()
                   .GetQueryable().FirstOrDefault(x => x.groupNewsId == groupId && x.userId == userId && x.deleted == false);
            if (userInGroup != null)
            {
                type = userInGroup.userType ?? 0;
                if (type == 1)
                    result = "Chủ nhóm";
                else if (type == 3)
                    result = "Quản trị viên";
                else
                    result = "Thành viên";
            }
            return result;
        }
        public async Task<string> GetGroupName(Guid userId)
        {
            var userGroup = await this.unitOfWork.Repository<tbl_UserGroup>().GetQueryable().FirstOrDefaultAsync(x => x.userId == userId && x.deleted == false);
            if(userGroup == null)
                return "";
            var group = await this.unitOfWork.Repository<tbl_Group>().GetQueryable().FirstOrDefaultAsync(x => x.id == userGroup.groupId && x.deleted == false);
            if (group == null)
                return "";
            return group.name;
        }
        public async Task<bool> Update(tbl_UserJoinGroupNews item)
        {
            bool success = true;
            this.unitOfWork.Repository<tbl_UserJoinGroupNews>().Update(item);
            await this.unitOfWork.SaveAsync();
            return success;
        }
        public async Task Validate(Guid groupNewsId)
        {
            var acLog = LoginContext.Instance.CurrentUser;
            var userInGroupNews = unitOfWork.Repository<tbl_UserJoinGroupNews>()
                   .GetQueryable().FirstOrDefault(x => x.userId == acLog.userId && x.groupNewsId == groupNewsId);
            if (acLog.isSuperUser != true)
                if (userInGroupNews != null)
                {
                    if (userInGroupNews.userType == 2)
                        throw new AppException(MessageContants.unauthorized);
                }
                else
                    throw new AppException(MessageContants.unauthorized);
        }
    }
}
