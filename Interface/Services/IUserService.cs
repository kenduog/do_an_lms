using Entities;
using Entities.AuthEntities;
using Entities.DataTransferObject;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.Auth;
using Request.RequestCreate;
using Request.RequestUpdate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface IUserService: IDomainService<tbl_Users, UserSearch>
    {
        Task<bool> Verify(string userName, string password);
        Task<bool> UpdateUserPassword(Guid userId, string newPassword);
        Task ValidatePassword(Guid userId, string password);
        Task ForgotPassword(ForgotPasswordModel model);
        Task ResetPassword(ResetPasswordModel model);
        Task<List<AccountModel>> GetAccount();
        Task ValidateUser(tbl_Users model);
        Task<bool> UpdateOnesignalPlayerId(OneSignalUpdate model);
        Task<AppDomainResult> ValidateUsername(string username);
        Task<bool> IsAdmin(Guid userId);
        Task<(int, List<AdminDTO>)> GetAdmin(BaseSearch baseSearch);
        Task<(string, List<GroupOption>)> GetGroupByUserId(Guid userId);

        Task<IList<tbl_Users>> GetUserForSendNoti(string branchIds, Guid? groupNewsId);
        Task<bool> CheckParent(Guid userId);
    }
}
