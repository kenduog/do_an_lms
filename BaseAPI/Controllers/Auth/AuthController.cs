using Extensions;
using Interface.Services;
using Interface.Services.Auth;
using Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Request.Auth;
using Models;
using static Utilities.CoreContants;
using System.ComponentModel;
using Request.RequestUpdate;
using Entities.DomainEntities;
using Request.RequestCreate;
using Entities.AuthEntities;
using Service.Services;

namespace BaseAPI.Controllers.Auth
{
    [ApiController]
    public abstract class AuthController : ControllerBase
    {
        protected readonly ILogger<AuthController> logger;
        protected IUserService userService;
        protected IConfiguration configuration;
        protected IMapper mapper;
        private readonly ITokenManagerService tokenManagerService;
        private readonly ICitiesService citiesService;
        private readonly IDistrictsService districtsService;
        private readonly IWardsService wardsService;
        public AuthController(IServiceProvider serviceProvider
            , IConfiguration configuration
            , IMapper mapper, ILogger<AuthController> logger
            )
        {
            this.logger = logger;
            this.configuration = configuration;
            this.mapper = mapper;

            userService = serviceProvider.GetRequiredService<IUserService>();
            tokenManagerService = serviceProvider.GetRequiredService<ITokenManagerService>();
            this.citiesService = serviceProvider.GetRequiredService<ICitiesService>();
            this.districtsService = serviceProvider.GetRequiredService<IDistrictsService>();
            this.wardsService = serviceProvider.GetRequiredService<IWardsService>();
        }
        [HttpGet("user-info")]
        //[AppAuthorize]
        [Description("Lấy thông tin")]
        public virtual async Task<AppDomainResult> GetUserInfo()
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            var user = LoginContext.Instance.CurrentUser;
            var item = await this.userService.GetByIdAsync(user?.userId ?? Guid.Empty);
            if (item != null)
            {
                var city = await citiesService.GetByIdAsync(item.cityId ?? Guid.Empty);
                var district = await districtsService.GetByIdAsync(item.districtId ?? Guid.Empty);
                var ward = await wardsService.GetByIdAsync(item.wardId ?? Guid.Empty);
                var data = new
                {
                    active = item.active,
                    address = item.address,
                    birthday = item.birthday,
                    cityId = item.cityId,
                    code = item.code,
                    districtId = item.districtId,
                    deleted = item.deleted,
                    created = item.created,
                    createdBy = item.createdBy,
                    email = item.email,
                    firstName = item.firstName,
                    fullName = item.fullName,
                    gender = item.gender,
                    genderName = item.genderName,
                    id = item.id,
                    lastName = item.lastName,
                    phone = item.phone,
                    thumbnail = item.thumbnail,
                    thumbnailResize = item.thumbnailResize,
                    updated = item.updated,
                    updatedBy = item.updatedBy,
                    username = item.username,
                    wardId = item.wardId,
                    cityName = city?.name,
                    districtName = district?.name,
                    wardName = ward?.name,
                    status = item.status,
                    statusName = item.statusName,
                    groupIds = Task.Run(() => userService.GetGroupByUserId(item.id)).Result.Item1,
                    groups = Task.Run(() => userService.GetGroupByUserId(item.id)).Result.Item2
                };
                appDomainResult = new AppDomainResult()
                {
                    success = true,
                    data = data,
                    resultCode = (int)HttpStatusCode.OK
                };
            }
            else
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            return appDomainResult;
        }

        /// <summary>
        /// Đăng nhập hệ thống
        /// </summary> 
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public virtual async Task<AppDomainResult> LoginAsync([FromForm] Login loginModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            success = await this.userService.Verify(loginModel.username, loginModel.password);
            if (!success)
                throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không chính xác");
            var userInfos = await this.userService.GetSingleAsync(e => e.deleted == false
            && (e.username == loginModel.username));
            if (userInfos == null)
                throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không chính xác");
            if (userInfos.status == ((int)UserStatus.Locked))
                throw new UnauthorizedAccessException("Tài khoản của bạn đã bị khoá");
            if (await userService.CheckParent(userInfos.id))
                throw new UnauthorizedAccessException("Tài khoản phụ huynh không có quyền truy cập");
            var token = await GenerateJwtToken(userInfos);
            Guid refreshToken = Guid.NewGuid();
            double refreshTokenExprires = Timestamp.TimestampDateTime(DateTime.UtcNow.AddDays(3));
            if (Timestamp.Now() >= userInfos.refreshTokenExprires)
            {
                userInfos.refreshToken = Guid.NewGuid();
            }
            userInfos.refreshTokenExprires = refreshTokenExprires;
            await userService.UpdateFieldAsync(userInfos, new Expression<Func<tbl_Users, object>>[]
                {
                                e => e.refreshToken,
                                e => e.refreshTokenExprires
                });

            //danh sách nhóm của user
            List<GroupOption> groups = new List<GroupOption>();
            if (!userInfos.isSuperUser.HasValue || !userInfos.isSuperUser.Value)
            {
                groups = this.userService.GetGroupByUserId(userInfos.id).Result.Item2;
            }
            appDomainResult = new AppDomainResult()
            {
                success = true,
                data = new
                {
                    token = token,
                    refreshToken = userInfos.refreshToken,
                    refreshTokenExprires = refreshTokenExprires,
                },
                resultCode = (int)HttpStatusCode.OK
            };
            return appDomainResult;
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public virtual async Task<AppDomainResult> RefreshToken([FromForm] RefreshTokenRequest itemModel)
        {

            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var userInfos = await userService.GetSingleAsync(x => x.refreshToken == itemModel.refreshToken);
            if (userInfos == null)
                throw new UnauthorizedAccessException("Hết hạn đăng nhập");
            if (userInfos.refreshTokenExprires < Timestamp.Now())
                throw new UnauthorizedAccessException("Hết hạn đăng nhập");
            var token = await GenerateJwtToken(userInfos);
            double refreshTokenExprires = Timestamp.TimestampDateTime(DateTime.UtcNow.AddDays(3));

            if (Timestamp.Now() >= userInfos.refreshTokenExprires)
            {
                userInfos.refreshToken = Guid.NewGuid();
            }
            userInfos.refreshTokenExprires = refreshTokenExprires;

            await userService.UpdateFieldAsync(userInfos, new Expression<Func<tbl_Users, object>>[]
            {
                e => e.refreshToken,
                e => e.refreshTokenExprires
            });

            return new AppDomainResult
            {
                resultCode = ((int)HttpStatusCode.OK),
                resultMessage = "Thành công",
                success = true,
                data = new
                {
                    token = token,
                    refreshToken = userInfos.refreshToken,
                    refreshTokenExprires = refreshTokenExprires
                },
            };
        }
        /// <summary>
        /// đăng nhập cho dev
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="AppException"></exception>
        [AllowAnonymous]
        [HttpPost("login-dev")]
        public virtual async Task<AppDomainResult> LoginDevAsync([FromForm] LoginDev loginModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                success = await this.userService.AnyAsync(x => x.id == loginModel.id && loginModel.key == "m0n4medi4");
                if (success)
                {
                    var userInfos = await this.userService.GetSingleAsync(e => e.deleted == false
                    && e.id == loginModel.id);
                    if (userInfos != null)
                    {
                        if (userInfos.status == ((int)UserStatus.Locked))
                            throw new UnauthorizedAccessException("Tài khoản của bạn đã bị khoá");
                        var token = await GenerateJwtToken(userInfos);
                        Guid refreshToken = Guid.NewGuid();
                        double refreshTokenExprires = Timestamp.TimestampDateTime(DateTime.UtcNow.AddDays(3));
                        userInfos.refreshToken = refreshToken;
                        userInfos.refreshTokenExprires = refreshTokenExprires;
                        await userService.UpdateFieldAsync(userInfos, new Expression<Func<tbl_Users, object>>[]
                            {
                                e => e.refreshToken,
                                e => e.refreshTokenExprires
                            });

                        appDomainResult = new AppDomainResult()
                        {
                            success = true,
                            data = new
                            {
                                token = token,
                                refreshToken = userInfos.refreshToken,
                                refreshTokenExprires = refreshTokenExprires
                            },
                            resultCode = (int)HttpStatusCode.OK
                        };

                    }
                }
                else
                    throw new UnauthorizedAccessException("Thông tin không chính xác");
            }
            else
                throw new AppException(ModelState.GetErrorMessage());
            return appDomainResult;
        }
        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        //[AllowAnonymous]
        //[HttpPost("register")]
        //public virtual async Task<AppDomainResult> Register([FromBody] Register register)
        //{
        //    AppDomainResult appDomainResult = new AppDomainResult();
        //    if (ModelState.IsValid)
        //    {
        //        var user = new tbl_Users()
        //        {
        //            fullName = register.fullName,
        //            username = register.username,
        //            password = register.password,
        //            created = Timestamp.Now(),
        //            updated = Timestamp.Now(),
        //            roleCode = Role.Student.ToString(),
        //            roleName = GetRoleName(Role.Student.ToString()),
        //            active = true,
        //            deleted = false,
        //            phone = register.phone,
        //            email = register.email,
        //        };
        //        // Kiểm tra item có tồn tại chưa?
        //        await this.userService.ValidateUser(user);
        //        user.password = SecurityUtilities.HashSHA1(register.password);
        //        appDomainResult.success = await userService.CreateAsync(user);
        //        appDomainResult.resultCode = (int)HttpStatusCode.OK;
        //    }
        //    else
        //    {
        //        var resultMessage = ModelState.GetErrorMessage();
        //        throw new AppException(resultMessage);
        //    }
        //    return appDomainResult;
        //}

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="changePasswordModel"></param>
        /// <returns></returns>
        [HttpPut("change-password")]
        [Authorize]
        public virtual async Task<AppDomainResult> ChangePassword([FromBody] ChangePassword changePasswordModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var user = LoginContext.Instance.CurrentUser;
            if (user == null)
                throw new AppException("Không phải người dùng hiện tại");
            await userService.ValidatePassword(user.userId, changePasswordModel.oldPassword);

            var userInfo = await this.userService.GetByIdAsync(user.userId);
            string newPassword = SecurityUtilities.HashSHA1(changePasswordModel.newPassword);
            await userService.UpdateUserPassword(user.userId, newPassword);
            return new AppDomainResult();
        }
        /// <summary>
        /// Quên mật khẩu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public virtual async Task<AppDomainResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            AppDomainResult appDomainResult = new AppDomainResult();
            await userService.ForgotPassword(model);
            return new AppDomainResult()
            {
                success = true,
                resultCode = (int)HttpStatusCode.OK
            };
        }
        /// <summary>
        /// Tạo mật khẩu mới
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut("reset-password")]
        public virtual async Task<AppDomainResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            AppDomainResult appDomainResult = new AppDomainResult();
            await userService.ResetPassword(model);
            return new AppDomainResult()
            {
                success = true,
                resultCode = (int)HttpStatusCode.OK
            };
        }
        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("logout")]
        public virtual async Task<AppDomainResult> Logout()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            await this.tokenManagerService.DeactivateCurrentAsync();
            appDomainResult = new AppDomainResult()
            {
                success = true,
                resultCode = (int)HttpStatusCode.OK
            };
            return appDomainResult;
        }
        [HttpPut("my-information")]
        //[AppAuthorize]
        public virtual async Task<AppDomainResult> UpdateMyInformation([FromBody] UserUpdate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            UserLoginModel user = LoginContext.Instance.CurrentUser;
            itemModel.id = user.userId;
            if (ModelState.IsValid)
            {
                var item = mapper.Map<tbl_Users>(itemModel);
                await this.userService.ValidateUser(item);
                if (item != null)
                {
                    success = await this.userService.UpdateAsync(item);
                    if (success)
                    {
                        appDomainResult.resultCode = (int)HttpStatusCode.OK;
                        appDomainResult.resultMessage = "Thành công";
                    }
                    else
                        throw new Exception("Lỗi trong quá trình xử lý");
                    appDomainResult.success = success;
                }
                else
                    throw new KeyNotFoundException("Item không tồn tại");
            }
            else
                throw new AppException(ModelState.GetErrorMessage());
            return appDomainResult;
        }
        [AllowAnonymous]
        [HttpGet("account")]
        public virtual async Task<AppDomainResult> GetAccount()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var data = await userService.GetAccount();
            return new AppDomainResult()
            {
                data = data,
                success = true,
                resultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Cập nhật id one signal cho user
        /// </summary>
        /// <param name="oneSignalId"></param>
        /// <returns></returns>
        [HttpPut("onesignal-id")]
        [AppAuthorize]
        [Description("Cập nhật id onesignal")]
        public virtual async Task<AppDomainResult> UpdateOneSignalId([FromBody] string oneSignalId)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            if (string.IsNullOrEmpty(oneSignalId))
                throw new AppException("Vui lòng nhập mã one signal");
            var userLogin = LoginContext.Instance.CurrentUser;
            if (userLogin == null)
                throw new AppException("Vui lòng đăng nhập");
            var user = await userService.GetByIdAsync(userLogin.userId);
            if (user == null)
                throw new AppException("Tài khoản không tồn tại");
            user.oneSignalId = oneSignalId;
            bool success = await userService.UpdateFieldAsync(user, x => x.oneSignalId);
            return new AppDomainResult()
            {
                resultCode = (int)HttpStatusCode.OK,
                resultMessage = "Cập nhật thành công!",
                success = success
            };
        }
        #region Private methods

        /// <summary>
        /// Tạo token từ thông tin user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [NonAction]
        protected async Task<string> GenerateJwtToken(tbl_Users user)
        {
            return await Task.Run(async () =>
            {
                // generate token that is valid for 7 days
                var tokenHandler = new JwtSecurityTokenHandler();
                var appSettingsSection = configuration.GetSection("AppSettings");
                // configure jwt authentication
                var appSettings = appSettingsSection.Get<AppSettings>();
                var key = Encoding.ASCII.GetBytes(appSettings.secret);

                var userLoginModel = new UserLoginModel()
                {
                    userId = user.id,
                    userName = user.username,
                    fullName = user.fullName,
                    email = user.email,
                    phone = user.phone,
                    thumbnail = user.thumbnail,
                    isSuperUser = user.isSuperUser.Value,
                    groupIds = (await userService.GetGroupByUserId(user.id)).Item1,
                    groups = (await userService.GetGroupByUserId(user.id)).Item2,
                    branchIds = string.IsNullOrEmpty(user.branchIds) ? null : user.branchIds.Split(',').ToList()
                };
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                                {
                                new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(userLoginModel))
                                }),
                    Expires = DateTime.UtcNow.AddDays(2),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            });
        }
        #endregion
    }
}
