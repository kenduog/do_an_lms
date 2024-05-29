﻿using Interface.Services;
using Interface.Services.Auth;
using Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Microsoft.CodeAnalysis;

namespace Extensions
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        private readonly ITokenManagerService tokenManagerService;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings, ITokenManagerService tokenManagerService)
        {
            _next = next;
            _appSettings = appSettings.Value;
            this.tokenManagerService = tokenManagerService;
        }
        private bool IsAllowAnonymous(string _Controller, string _Action)
        {
            bool result = false;
            System.AppDomain currentDomain = System.AppDomain.CurrentDomain;
            Assembly[] assems = currentDomain.GetAssemblies();

            foreach (Assembly assem in assems)
            {
                var controller = assem.GetTypes().Where(type =>
                typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract)
                  .Select(e => new
                  {
                      id = e.Name.Replace("Controller", string.Empty),
                      action = (from i in e.GetMembers().Where(x => (
                                 x.Module.Name == "API.dll" ||
                                 x.Module.Name == "BaseAPI.dll")
                                 && x.Name == _Action
                                 && x.GetCustomAttributes(typeof(NonActionAttribute)).Select(x => x.GetType().Name).FirstOrDefault() != typeof(NonActionAttribute).Name
                                 && x.GetCustomAttributes<AllowAnonymousAttribute>(true).Any()
                                 )
                                 select new
                                 {
                                     id = i.Name
                                 }).Any()
                  })
                  .Where(e => e.id == _Controller)
                  .FirstOrDefault();
                if (controller != null)
                        result = controller.action;
            }
            return result;
        }
        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext context, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var routeData = context.Request.HttpContext.GetRouteData().Values;
            string action = "";
            string controller = "";
            if (routeData.Values.FirstOrDefault() != null)
            {
                action = routeData.Values.FirstOrDefault().ToString();
                controller = routeData.Values.LastOrDefault().ToString();
            }

            bool isAllowAnonymous = IsAllowAnonymous(controller,action);
            if (await tokenManagerService.IsCurrentActiveToken())
            {
                if (token != null && !isAllowAnonymous)
                    attachUserToContext(context, userService, token);
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                var result = System.Text.Json.JsonSerializer.Serialize(new AppDomainResult()
                {
                    resultCode = context.Response.StatusCode,
                    success = false
                });
                await context.Response.WriteAsync(result);
            }
        }
        private void attachUserToContext(Microsoft.AspNetCore.Http.HttpContext context, IUserService userService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userModel = new UserLoginModel();
                var claim = jwtToken.Claims.First(x => x.Type == ClaimTypes.UserData);
                if (claim != null)
                {
                    userModel = JsonConvert.DeserializeObject<UserLoginModel>(claim.Value);
                }

                context.Items["User"] = userModel;
            }
            catch(Exception ex)
            {
                throw new UnauthorizedAccessException("Phiên đăng nhập hết hạn");
            }
        }
    }
}