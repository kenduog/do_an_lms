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
using Azure.Core;
using Request.RequestCreate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Request.RequestUpdate;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Service.Services
{
    public class GroupNewsService : DomainService<tbl_GroupNews, BaseSearch>, IGroupNewsService
    {
        private readonly IUserService userService;
        private readonly ISendNotificationService sendNotificationService;

        public GroupNewsService(IServiceProvider serviceProvider, IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.sendNotificationService = serviceProvider.GetRequiredService<ISendNotificationService>();
        }
        protected override string GetStoreProcName()
        {
            return "Get_GroupNews";
        }
        public override async Task Validate(tbl_GroupNews model)
        {
            if (model == null)
                throw new ArgumentNullException(MessageContants.nf_item);
            var userLog = LoginContext.Instance.CurrentUser ?? throw new AppException(MessageContants.auth_expiried);
            if (model.id != Guid.Empty)
            {
                var item = await this.unitOfWork.Repository<tbl_GroupNews>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.id)
                    ?? throw new AppException(MessageContants.nf_groupNews);
                if (!userLog.isSuperUser)
                {
                    var role = await this.unitOfWork.Repository<tbl_UserJoinGroupNews>().GetQueryable().FirstOrDefaultAsync(x => x.groupNewsId == item.id && x.userId == userLog.userId)
                        ?? throw new AppException(MessageContants.nf_userInGroupNews);
                    if (role.userType != 1 && role.userType != 3)
                        throw new AppException(MessageContants.unauthorized);
                }
            }
        }
        public override async Task<tbl_GroupNews> GetByIdAsync(Guid id)
        {
            return await this.unitOfWork.Repository<tbl_GroupNews>().GetSingleRecordAsync("Get_GroupNewById", new SqlParameter[] { new SqlParameter("id", id) });
        }
        public override async Task<PagedList<tbl_GroupNews>> GetPagedListData(BaseSearch baseSearch)
        {
            var userLog = LoginContext.Instance.CurrentUser ?? throw new AppException(MessageContants.auth_expiried);
            GroupNewsSearch search = new GroupNewsSearch
            {
                pageIndex = baseSearch.pageIndex,
                pageSize = baseSearch.pageSize,
                searchContent = baseSearch.searchContent,
                orderBy = baseSearch.orderBy,
                userId = userLog.isSuperUser ? null : userLog.userId,
            };
            PagedList<tbl_GroupNews> pagedList = new PagedList<tbl_GroupNews>();
            SqlParameter[] parameters = GetSqlParameters(search);
            pagedList = await this.unitOfWork.Repository<tbl_GroupNews>().ExcuteQueryPagingAsync(this.GetStoreProcName(), parameters);
            pagedList.pageIndex = search.pageIndex;
            pagedList.pageSize = search.pageSize;
            return pagedList;
        }
        public async Task<PagedList<tbl_Users>> GetUserGroupNews(UserJoinGroupNewsSearch baseSearch)
        {
            PagedList<tbl_Users> pagedList = new PagedList<tbl_Users>();
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            pagedList = await this.unitOfWork.Repository<tbl_Users>().ExcuteQueryPagingAsync("Get_UserInOrOutGroupNews", parameters);
            pagedList.pageIndex = baseSearch.pageIndex;
            pagedList.pageSize = baseSearch.pageSize;
            return pagedList;
        }
        public override async Task UpdateItem(tbl_GroupNews model)
        {
            await this.Validate(model);
            model.countMember = this.GetByIdAsync(model.id).Result.countMember;
            await this.UpdateAsync(model);
            await this.unitOfWork.SaveAsync();
        }
        public override async Task DeleteItem(Guid id)
        {
            await this.Validate(this.GetByIdAsync(id).Result);
            await DeleteAsync(id);
        }
        public async Task<bool> UserJoinGroupForClass(GroupNewsCreate request, tbl_GroupNews group)
        {
            bool success = true;
            await Task.Run(async () =>
            {
                List<tbl_UserJoinGroupNews> userJoinGroup = new List<tbl_UserJoinGroupNews>();
                // danh sách userId phụ huynh + giáo viên phụt trách lớp
                var users = this.unitOfWork.Repository<tbl_Users>().ExcuteStoreAsync("Get_UserParentInClass", GetSqlParameters(new
                {
                    classIds = request.classIds
                })).Result.ToList();
                // người tạo
                var userCreate = await this.userService.GetByIdAsync(group.createdBy ?? Guid.Empty);
                if (userCreate != null)
                    users.Add(userCreate);
                if (users.Any())
                {
                    userJoinGroup = users.Select(x => new tbl_UserJoinGroupNews
                    {
                        groupNewsId = group.id,
                        userId = x.id,
                        userType = x.id == group.createdBy ? 1 : 2,
                        isHide = false
                    }).ToList();
                    await this.unitOfWork.Repository<tbl_UserJoinGroupNews>().CreateAsync(userJoinGroup);
                    users.Remove(userCreate);
                    List<IDictionary<string, string>> notiParamList = new List<IDictionary<string, string>>();
                    foreach (var receiver in users)
                    {
                        IDictionary<string, string> notiParam = new Dictionary<string, string>();
                        notiParam.Add("[GroupNewsName]", group.name);
                        notiParamList.Add(notiParam);
                    }
                    string linkQuery = "group=" + sendNotificationService.EncodingParam(group.id.ToString());
                    string subLink = string.Empty;
                    sendNotificationService.SendNotification(Guid.Parse("ded2f0e3-ba3e-4969-6fe3-08dc17fc7766"), users, notiParamList, null, linkQuery, null, LookupConstant.ScreenCode_GroupNews, subLink);
                }
                await this.unitOfWork.SaveAsync();
            });
            return success;
        }
        public async Task<bool> GrantPermissions(GrantPermissionsGroupNewsCreate model)
        {
            List<tbl_UserJoinGroupNews> userJoinGroupNews = new List<tbl_UserJoinGroupNews>();
            var getUserInGroup = await unitOfWork.Repository<tbl_UserJoinGroupNews>().GetQueryable()
                .Where(x => x.groupNewsId == model.groupNewsId && x.deleted == false)
                .ToListAsync();

            var user = model.userIds.Split(",").ToList();
            if (user.Any())
            {
                foreach (var item in user)
                {
                    var parent = await this.unitOfWork.Repository<tbl_Parent>().GetQueryable().AnyAsync(x => x.userId == Guid.Parse(item));
                    var userInGroup = getUserInGroup.FirstOrDefault(x => x.userId == Guid.Parse(item));
                    if (!getUserInGroup.Any(x => x.userId == Guid.Parse(item)))
                        throw new AppException(MessageContants.nf_userInGroupNews);
                    if (userInGroup.userType == 1)
                        throw new AppException(MessageContants.can_not_update_admin_for_owner_group_news);
                    if (parent)
                        throw new AppException(MessageContants.can_not_update_admin_for_parent_group_news);
                    userInGroup.userType = 3;
                    userJoinGroupNews.Add(userInGroup);
                }
                this.unitOfWork.Repository<tbl_UserJoinGroupNews>().UpdateRange(userJoinGroupNews);
            }
            await this.unitOfWork.SaveAsync();
            return true;
        }
        public async Task<bool> ChangeOwner(ChangeOwnerGroupNewsCreate model)
        {
            var userLog = LoginContext.Instance.CurrentUser;
            var getUserInGroup = await unitOfWork.Repository<tbl_UserJoinGroupNews>().GetQueryable()
            .Where(x => x.groupNewsId == model.groupNewsId && x.deleted == false)
            .ToListAsync();
            if (getUserInGroup == null)
                throw new AppException(MessageContants.nf_item);

            var currentInGroup = getUserInGroup.FirstOrDefault(x => x.userId == userLog.userId);
            var userAppointment = getUserInGroup.FirstOrDefault(x => x.userId == model.userId);
            if (userAppointment == null)
                throw new AppException(MessageContants.nf_item);
            if (userAppointment.userType == 1)
                throw new AppException(MessageContants.user_is_owner_group_news);
            if (!userLog.isSuperUser && currentInGroup.userType != 1)
                throw new AppException(MessageContants.unauthorized);

            List<tbl_UserJoinGroupNews> userJoinGroupNews = new List<tbl_UserJoinGroupNews>();
            if (!getUserInGroup.Any(x => x.userId == model.userId))
                throw new AppException(MessageContants.nf_userInGroupNews);
            if (await this.unitOfWork.Repository<tbl_Parent>().GetQueryable().AnyAsync(x => x.userId == userAppointment.userId))
                throw new AppException(MessageContants.can_not_update_admin_for_parent_group_news);
            userAppointment.userType = 1;
            userJoinGroupNews.Add(userAppointment);
            var getAdminGroup = await unitOfWork.Repository<tbl_UserJoinGroupNews>().GetQueryable()
            .FirstOrDefaultAsync(x => x.groupNewsId == model.groupNewsId && x.deleted == false && x.userType == 1);
            if (getAdminGroup == null)
                throw new AppException(MessageContants.nf_item);
            getAdminGroup.userType = 2;
            userJoinGroupNews.Add(getAdminGroup);
            this.unitOfWork.Repository<tbl_UserJoinGroupNews>().UpdateRange(userJoinGroupNews);
            await this.unitOfWork.SaveAsync();
            return true;
        }
        public async Task<bool> RevokePermissions(ChangeOwnerGroupNewsCreate model)
        {
            var userLog = LoginContext.Instance.CurrentUser;
            var groupNews = await this.unitOfWork.Repository<tbl_GroupNews>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.groupNewsId && x.deleted == false);
            await Validate(groupNews);

            var getUserInGroup = await unitOfWork.Repository<tbl_UserJoinGroupNews>().GetQueryable()
            .Where(x => x.groupNewsId == model.groupNewsId && x.deleted == false)
            .ToListAsync();
            if (getUserInGroup == null)
                throw new AppException(MessageContants.nf_item);
            var userRevoke = getUserInGroup.FirstOrDefault(x => x.userId == model.userId);
            if (userRevoke == null)
                throw new AppException(MessageContants.nf_userInGroupNews);
            if (userRevoke.userType == 1)
                throw new AppException(MessageContants.can_not_update_admin_for_owner_group_news);
            if (userRevoke.userType == 3)
                userRevoke.userType = 2;
            this.unitOfWork.Repository<tbl_UserJoinGroupNews>().Update(userRevoke);
            await this.unitOfWork.SaveAsync();
            return true;
        }
        public async Task<bool> CheckAdmin(CheckAdminGroupNewsCreate model)
        {
            var userInGroup = await this.unitOfWork.Repository<tbl_UserJoinGroupNews>().GetQueryable().FirstOrDefaultAsync(x => x.groupNewsId == model.groupNewsId && x.userId == model.userId && x.deleted == false);
            if (userInGroup == null)
                return false;
            if (userInGroup.userType != 1 && userInGroup.userType != 3)
                return false;
            return true;
        }
    }
}
