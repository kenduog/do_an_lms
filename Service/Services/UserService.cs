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
using static Utilities.CoreContants;
using Microsoft.Extensions.Configuration;
using Request.DomainRequests;
using Request.Auth;
using Entities.DomainEntities;
using Request.RequestUpdate;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Internal;
using OfficeOpenXml.ConditionalFormatting;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Entities.AuthEntities;
using System.Runtime.InteropServices;
using Entities;
using Microsoft.Identity.Client;
using Entities.DataTransferObject;
using AppDbContext;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Azure.Core;

namespace Service.Services
{
    public class UserService : DomainService<tbl_Users, UserSearch>, IUserService
    {
        private int totalSecond = 120;
        protected IAppDbContext coreDbContext;
        private IRoleService roleService;
        private INecessaryService necessaryService;
        private IAutoGenCodeConfigService autoGenCodeConfigService;
        private IConfiguration configuration;
        public UserService(IAppUnitOfWork unitOfWork,
            IMapper mapper,
            IAppDbContext coreDbContext,
            IRoleService roleService,
            INecessaryService necessaryService,
            IConfiguration configuration,
            IAutoGenCodeConfigService autoGenCodeConfigService) : base(unitOfWork, mapper)
        {
            this.autoGenCodeConfigService = autoGenCodeConfigService;
            this.necessaryService = necessaryService;
            this.coreDbContext = coreDbContext;
            this.roleService = roleService;
            this.configuration = configuration;
        }
        protected override string GetStoreProcName()
        {
            return "Get_Users";
        }
        /// <summary>
        /// Cập nhật password mới cho user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserPassword(Guid userId, string newPassword)
        {
            bool result = false;

            var existUserInfo = await unitOfWork.Repository<tbl_Users>().GetQueryable().Where(e => e.id == userId).FirstOrDefaultAsync();
            if (existUserInfo != null)
            {
                existUserInfo.password = newPassword;
                existUserInfo.updated = Timestamp.Now();
                Expression<Func<tbl_Users, object>>[] includeProperties = new Expression<Func<tbl_Users, object>>[]
                {
                    e => e.password,
                    e => e.updated
                };
                await unitOfWork.Repository<tbl_Users>().UpdateFieldsSaveAsync(existUserInfo, includeProperties);
                await unitOfWork.SaveAsync();
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Kiểm tra user đăng nhập
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> Verify(string userName, string password)
        {

            var user = await Queryable
                .Where(e => e.deleted == false
                && e.username == userName
                //|| e.phone == userName
                //|| e.email == userName
                )
                .FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.active == false)
                {
                    throw new AppException("Tài khoản chưa được kích hoạt");
                }
                if (user.status != ((int)UserStatus.Active))
                {
                    throw new AppException("Tài khoản không hoạt động");
                }
                if (user.password == SecurityUtilities.HashSHA1(password))
                {
                    return true;
                }
                else
                    return false;

            }
            else
                return false;
        }

        public async Task<AppDomainResult> ValidateUsername(string username)
        {
            bool succes = await AnyAsync(x => x.username.ToUpper() == username.ToUpper() && x.deleted == false);
            return new AppDomainResult
            {
                success = succes,
                resultCode = succes ? (int)HttpStatusCode.OK : (int)HttpStatusCode.NotImplemented
            };
        }
        public async Task ValidatePassword(Guid userId, string password)
        {
            password = SecurityUtilities.HashSHA1(password);
            var validate = await GetSingleAsync(x => x.id == userId && x.password.ToUpper() == password.ToUpper());
            if (validate == null)
                throw new AppException("Mật khẩu không chính xác");
        }
        public async Task ForgotPassword(ForgotPasswordModel model)
        {
            string domainUrl = $"{configuration.GetSection("MySettings:DomainFE").Value}forgot-password?key=";
            string projectName = configuration.GetSection("MySettings:ProjectName").Value.ToString();
            var user = await GetSingleAsync(x => x.username.ToUpper() == model.username.ToUpper() && x.deleted == false);
            if (user == null)
                throw new AppException("Không tìm thấy tài khoản");
            string keyForgotPassword = Guid.NewGuid().ToString();
            user.keyForgotPassword = keyForgotPassword;
            user.createdDateKeyForgot = Timestamp.Now();
            StringBuilder content = new StringBuilder();
            string title = "Yêu cầu thay đổi mật khẩu";
            content.Append($"<p style=\"color:blue;font-size:30px;\">Đặt lại mật khẩu</p>");
            content.Append($"<p>Chào {user.fullName}</p>");
            content.Append($"<p>Để thực hiện thay đổi mật khẩu bạn vui lòng truy cập <a href=\"{domainUrl}{keyForgotPassword}\">tại đây</a></p>");
            content.Append($"<p>Thông báo từ {projectName}</p>");
            var sendMail = new Thread(async () =>
            {
                await necessaryService.SendMail(new SendMailModel
                {
                    to = user.email,
                    title = title,
                    content = content.ToString()
                });
            });
            sendMail.Start();
            await UpdateAsync(user);
        }
        public async Task ResetPassword(ResetPasswordModel model)
        {
            var user = await GetSingleAsync(x => x.keyForgotPassword == model.key);
            if (user == null)
                throw new AppException("Mã xác nhận không chính xác");
            ///mã tồn tại trong thời gian 30 phút
            var checkTime = Timestamp.TimestampDateTime(DateTime.UtcNow.AddHours(-0.5));
            if (user.createdDateKeyForgot < checkTime)
                throw new AppException("Mã xác nhận đã hết hạn sử dụng");
            user.password = SecurityUtilities.HashSHA1(model.newPassword);
            user.keyForgotPassword = "";
            user.createdDateKeyForgot = 0;
            await UpdateAsync(user);
        }
        public async Task ValidateUser(tbl_Users model)
        {
            if (!string.IsNullOrEmpty(model.code))
            {
                var validateCode = await
                    AnyAsync(x => x.code.ToUpper() == model.code.ToUpper() && x.deleted == false && x.id != model.id);
                if (validateCode)
                    throw new AppException("Mã người dùng đã tồn tại");
            }
            if (!string.IsNullOrEmpty(model.username))
            {
                if (!UserNameFormat(model.username))
                    throw new AppException("Tài khoản đăng nhập không hợp lệ");
                var validateUserName = await
                    AnyAsync(x => x.username.ToUpper() == model.username.ToUpper() && x.deleted == false && x.id != model.id);
                if (validateUserName)
                    throw new AppException("Tên đăng nhập đã tồn tại");
            }
        }
        public override async Task Validate(tbl_Users model)
        {
            await this.ValidateUser(model);
            if (model.cityId.HasValue)
            {
                var city = await this.unitOfWork.Repository<tbl_Cities>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.cityId && x.deleted == false)
                    ?? throw new AppException(MessageContants.nf_city);
            }
            if (model.districtId.HasValue)
            {
                var district = await this.unitOfWork.Repository<tbl_Districts>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.districtId && x.deleted == false)
                    ?? throw new AppException(MessageContants.nf_district);
            }
            if (model.wardId.HasValue)
            {
                var ward = await this.unitOfWork.Repository<tbl_Wards>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.wardId && x.deleted == false)
                    ?? throw new AppException(MessageContants.nf_ward);
            }
        }
        public bool UserNameFormat(string value)
        {
            string[] arr = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
            "í","ì","ỉ","ĩ","ị",
            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
            "ý","ỳ","ỷ","ỹ","ỵ"," ",};
            foreach (var item in arr)
            {
                if (value.Contains(item))
                    return false;
            }
            return true;
        }
        public async Task<List<AccountModel>> GetAccount()
        {
            var data = await coreDbContext.Set<tbl_Users>().Where(x => x.deleted == false)
                .Select(x => new AccountModel
                {
                    id = x.id,
                    fullName = x.fullName,
                    isSuperUser = x.isSuperUser
                }).ToListAsync();
            return data;
        }

        public async Task<bool> UpdateOnesignalPlayerId(OneSignalUpdate model)
        {
            bool result = false;

            var existUserInfo = await unitOfWork.Repository<tbl_Users>().GetQueryable()
                .FirstOrDefaultAsync(e => e.id == model.id);
            if (existUserInfo != null)
            {
                existUserInfo.oneSignalId = model.playerId;
                existUserInfo.updated = Timestamp.Now();
                Expression<Func<tbl_Users, object>>[] includeProperties = new Expression<Func<tbl_Users, object>>[]
                {
                    e => e.oneSignalId,
                    e => e.updated
                };
                await unitOfWork.Repository<tbl_Users>().UpdateFieldsSaveAsync(existUserInfo, includeProperties);
                await unitOfWork.SaveAsync();
                result = true;
            }
            return result;
        }

        public override async Task AddItem(tbl_Users model)
        {
            await Validate(model);
            model.code = await autoGenCodeConfigService.AutoGenCode(nameof(tbl_Users));
            await this.CreateAsync(model);
        }
        public async Task<bool> IsAdmin(Guid userId)
        {
            var user = await GetByIdAsync(userId);
            if (user.isSuperUser == true)
                return true;
            var adminGroup = await unitOfWork.Repository<tbl_Group>()
                .GetQueryable().FirstOrDefaultAsync(x => x.code == "QTV");
            if (adminGroup == null)
                return false;
            var hasPermisson = await unitOfWork.Repository<tbl_UserGroup>()
                .GetQueryable().AnyAsync(x => x.userId == userId && x.groupId == adminGroup.id && x.deleted == false);
            return hasPermisson;
        }

        public async Task<IList<tbl_Users>> GetByBrandId(string brandId)
        {
            return await unitOfWork.Repository<tbl_Users>().GetQueryable().Where(x => x.branchIds.Contains(brandId)).ToListAsync();
        }

        public async Task<(int, List<AdminDTO>)> GetAdmin(BaseSearch baseSearch)
        {
            return await Task.Run(() =>
            {
                var result = (0, new List<AdminDTO>());
                DataTable dataTable = new DataTable();
                Microsoft.Data.SqlClient.SqlConnection connection = null;
                Microsoft.Data.SqlClient.SqlCommand command = null;
                try
                {
                    connection = (Microsoft.Data.SqlClient.SqlConnection)coreDbContext.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = $"Get_Admin";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pageIndex", baseSearch.pageIndex);
                    command.Parameters.AddWithValue("@pageSize", baseSearch.pageSize);
                    command.Parameters.AddWithValue("@searchContent", $"N{baseSearch.searchContent}");
                    command.Parameters.AddWithValue("@orderBy", baseSearch.orderBy);
                    Microsoft.Data.SqlClient.SqlDataAdapter sqlDataAdapter = new Microsoft.Data.SqlClient.SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    var admins = ConvertToList(dataTable);
                    int totalItem = 0;
                    if (admins.Any())
                        totalItem = admins[0].totalItem;
                    result = (totalItem, admins);
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
                return result;
            });
        }

        public static List<AdminDTO> ConvertToList(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
            var properties = typeof(AdminDTO).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<AdminDTO>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name.ToLower()))
                    {
                        try
                        {
                            pro.SetValue(objT, row[pro.Name]);
                        }
                        catch
                        {
                        }
                    }
                }
                return objT;
            }).ToList();
        }

        public async Task<IList<tbl_Users>> GetUserForSendNoti(string branchId, Guid? groupNewsId)
        {
            return await unitOfWork.Repository<tbl_Users>().ExcuteStoreAsync("Get_UsersForSendNoti", GetSqlParameters(new { branchId = branchId, groupNewsId = groupNewsId }));
        }

        public async Task<(string, List<GroupOption>)> GetGroupByUserId(Guid userId)
        {
            var option = new List<GroupOption>();
            var user = await unitOfWork.Repository<tbl_Users>().GetQueryable()
                             .FirstOrDefaultAsync(x => x.id == userId);

            if (user == null)
            {
                return (null, option);
            }

            if (user.isSuperUser.HasValue && user.isSuperUser.Value)
            {
                option = await unitOfWork.Repository<tbl_Group>()
                                .GetQueryable()
                                .Where(x => x.deleted == false && x.code != "PH")
                                .Select(x => new GroupOption
                                {
                                    id = x.id,
                                    code = x.code,
                                    name = x.name
                                })
                                .ToListAsync();
                if (user.isDev.HasValue && user.isDev.Value)
                    option.Add(new GroupOption { id = Guid.NewGuid(), code = CoreContants.Group.Dev.ToString(), name = "Developer" });
                return (null, option);
            }

            var listGroupId = await unitOfWork.Repository<tbl_UserGroup>()
                                .GetQueryable()
                                .Where(x => x.userId == userId && x.deleted == false)
                                .Select(x => x.groupId)
                                .ToListAsync();

            if (listGroupId.Count == 0)
            {
                return (null, option);
            }

            var groupIds = string.Join(",", listGroupId);

            option = await unitOfWork.Repository<tbl_Group>()
                        .GetQueryable()
                        .Where(x => listGroupId.Contains(x.id) && x.deleted == false)
                        .Select(x => new GroupOption
                        {
                            id = x.id,
                            code = x.code,
                            name = x.name
                        })
                        .ToListAsync();

            return (groupIds, option);
        }
        /// <summary>
        /// Check role Phụ huynh
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<bool> CheckParent(Guid userId)
        {
            bool result = false;

            var existUserGroup = await unitOfWork.Repository<tbl_UserGroup>().GetQueryable().FirstOrDefaultAsync(e => e.userId == userId && e.deleted == false);
            if (existUserGroup != null)
            {
                var existGroup = await unitOfWork.Repository<tbl_Group>().GetQueryable().AnyAsync(e => e.id == existUserGroup.groupId && e.deleted == false && e.code.Contains("PH"));
                if (existGroup)
                    result = true;
            }
            return result;
        }
        #region mobile function

        public async Task<int> MobileForgotPassword(MobileForgotPasswordModel model)
        {
            string domainUrl = $"{configuration.GetSection("MySettings:DomainFE").Value}/forgot-password?key=";
            string projectName = configuration.GetSection("MySettings:ProjectName").Value.ToString();
            var user = await GetSingleAsync(x => x.username.ToUpper() == model.username.ToUpper() && x.deleted == false);
            if (user == null)
                throw new AppException("Không tìm thấy tài khoản");

            Random random = new Random();
            int code = random.Next(100000, 999999);
            string keyForgotPassword = code.ToString();
            user.keyForgotPassword = keyForgotPassword;
            user.createdDateKeyForgot = Timestamp.Now();

            string emailContent = "<!DOCTYPE html><html lang=\"en\"><head> <meta charset=\"UTF-8\"> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"> <title>Password Change Verification</title> <style> body { font-family: 'Arial', sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; } .container { max-width: 600px; margin: 0 auto; padding: 20px; background-color: #ffffff; border-radius: 5px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); } h2 { color: #333; } p { color: #666; } .verification-code { font-size: 24px; font-weight: bold; color: #007bff; } .expiration-time { color: #ff6347; font-style: italic; } .note { color: #777; } .footer { margin-top: 20px; color: #888; } </style></head><body> <div class=\"container\"> <h2>Xác minh thay đổi mật khẩu</h2> <p>Chào {fullName},</p> <p>Mã xác nhận thay đổi mật khẩu của bạn là:</p> <p class=\"verification-code\">{code}</p> <p class=\"expiration-time\">Mã sẽ hết hạn trong {totalSecond}.</p> <p class=\"note\">Lưu ý: Vui lòng không chia sẻ mã này với bất kỳ ai vì lý do bảo mật.</p> <div class=\"footer\"> <p>Best regards!</p> </div> </div></body></html>";

            string title = "Xác minh thay đổi mật khẩu";
            emailContent = emailContent.Replace("{fullName}", user.fullName);
            emailContent = emailContent.Replace("{code}", keyForgotPassword);
            emailContent = emailContent.Replace("{totalSecond}", NumberToText.ConvertSecondsToText(totalSecond));
            var sendMail = new Thread(async () =>
            {
                await necessaryService.SendMail(new SendMailModel
                {
                    to = user.email,
                    title = title,
                    content = emailContent
                });
            });
            sendMail.Start();
            await UpdateAsync(user);
            return totalSecond;
        }

        public async Task MobileVerifyChangePasswordCode(MobileVerifyChangePasswordModel model)
        {
            var user = await GetSingleAsync(x => x.keyForgotPassword == model.key && x.username == model.username);
            if (user == null)
                throw new AppException("Mã xác nhận không chính xác");

            var checkTime = Timestamp.AddSeconds(user.createdDateKeyForgot ?? 0, totalSecond);
            if (Timestamp.Now() > checkTime)
                throw new AppException("Mã xác nhận đã hết hạn sử dụng");

            //pass
        }

        public async Task MobileChangePassword(ChangePasswordModel model)
        {
            var user = await GetSingleAsync(x => x.username == model.username);
            if (user == null)
                throw new AppException("Không tìm thấy tài khoản");

            var checkTime = Timestamp.AddSeconds(user.createdDateKeyForgot ?? 0, totalSecond);
            if (Timestamp.Now() > checkTime)
                throw new AppException("Mã xác nhận đã hết hạn sử dụng");

            user.password = SecurityUtilities.HashSHA1(model.newPassword);
            user.keyForgotPassword = "";
            user.createdDateKeyForgot = 0;
            await UpdateAsync(user);
        }
        #endregion
    }
}
