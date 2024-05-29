using Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Interface.Services;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System.IO;
using OfficeOpenXml.Style;

namespace Extensions
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger<ControllerBase> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ControllerBase> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _logger = logger;
            this._serviceScopeFactory = serviceScopeFactory;
        }
        private int GetLanguageFromRequest(Microsoft.AspNetCore.Http.HttpContext context)
        {
            //var debug = context.Items["User"];
            //return debug == null ? 1 : (debug as UserLoginModel).language;
            return 2;
        }

        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var multipleMessageService = scope.ServiceProvider.GetRequiredService<IMultipleMessageService>();
                var logService = scope.ServiceProvider.GetRequiredService<ILogService>();
                try
                {
                    await _next(context);
                }
                catch (Exception error)
                {
                    var response = context.Response;
                    response.ContentType = "application/json";
                    string resultMessage = error?.Message;
                    switch (error)
                    {
                        case AggregateException e:
                            response.StatusCode = (int)HttpStatusCode.Locked;
                            break;
                        case AppException e:
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                            break;
                        case UnauthorizedAccessException e:
                            response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            break;
                        case InvalidCastException e:
                            response.StatusCode = (int)HttpStatusCode.Forbidden;
                            break;
                        case EntryPointNotFoundException e:
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                            break;
                        case KeyNotFoundException e:
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                            break;
                        case TimeoutException e:
                            response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                            break;
                        default:
                            {
                                var RouteData = context.Request.Path.Value.Split("/");
                                string apiName = string.Empty;
                                string actionName = string.Empty;

                                if (RouteData.Count() >= 2)
                                    apiName = RouteData[1];
                                if (RouteData.Count() >= 3)
                                    actionName = RouteData[2];

                                _logger.LogError(string.Format("{0} {1}: {2}", apiName
                                    , actionName, error?.Message));
                                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                            }
                            break;
                    }
                    string userName = "Chưa login";
                    if(LoginContext.Instance.CurrentUser != null)
                        userName = LoginContext.Instance.CurrentUser.userName;
                    
                    Writelog(response.StatusCode, "userName: " + userName + " ** Erro:"+ error.Message);
                    
                    //get current language
                    int language = GetLanguageFromRequest(context);
                    
                    //get string message convert by language
                    resultMessage = await multipleMessageService.GetMessage(resultMessage, language);
                    
                    //add dblog
                    await logService.InsertLog(error);

                    var result = JsonSerializer.Serialize(new AppDomainResult()
                    {
                        resultCode = response.StatusCode,
                        resultMessage = resultMessage,
                        success = false
                    });
                    await response.WriteAsync(result);
                }
            }
        }
        public static void Writelog(int httpCode, string contenterror)
        {
            string name = DateTime.Now.ToString("dd-MM-yyyy");
            string content = "HTTPCode: " + httpCode + "    MessageError: " + contenterror + "    " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "" + Environment.NewLine;
            string path = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\net6.0\\", "");
            string filePath = path + "\\FileLog\\log" + name + ".txt";
            File.AppendAllText(filePath, content);
        }
    }
}
