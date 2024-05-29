using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Entities.DomainEntities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
namespace Extensions
{
    public sealed class LoginContext
    {
        private static LoginContext instance = null;

        private LoginContext()
        {
        }

        public static LoginContext Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LoginContext();
                }
                return instance;
            }
        }

        public UserLoginModel CurrentUser
        {
            get
            {
                var user = Extensions.HttpContext.Current == null ? null : (UserLoginModel)Extensions.HttpContext.Current.Items["User"];
                if (user != null)
                    return user;
                return null;
            }
        }

        public void Clear()
        {
            instance = null;
        }

        public UserLoginModel GetCurrentUser(IHttpContextAccessor httpContext)
        {
            if (httpContext != null && httpContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var claim = httpContext.HttpContext.User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.UserData);
                if (claim != null)
                    return JsonConvert.DeserializeObject<UserLoginModel>(claim.Value);
            }
            return null;
        }
    }

    public class UserLoginModel
    {
        public Guid userId { get; set; }
        public string userName { get; set; }
        public string fullName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string thumbnail { get; set; }
        public bool isSuperUser{ get; set; }
        /// <summary>
        /// Ngôn ngữ
        /// 1 - English
        /// 2 - Tiếng Việt
        /// </summary>
        public int language { get; set; } = 2;
        public string groupIds { get; set; }
        public List<GroupOption> groups { get; set; }
        public List<string> branchIds { get; set; }
        public Guid? branchId { get; set; }
        public bool isMobile { get; set; } = false;
    }
}
