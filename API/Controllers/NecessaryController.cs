using Entities;
using Extensions;
using Interface.Services;
using Interface.Services.Auth;
using Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Models;
using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Request;
using Entities.Search;
using Request.DomainRequests;
using AutoMapper;
using Request.RequestCreate;
using Request.RequestUpdate;
using System.Reflection;
using Newtonsoft.Json;
using static Utilities.CoreContants;
using Microsoft.AspNetCore.Http;
namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Description("Màn hình cấu hình")]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class NecessaryController : ControllerBase
    {
        protected INecessaryService necessaryService;
        protected IWebHostEnvironment env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public NecessaryController(INecessaryService necessaryService, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.necessaryService = necessaryService;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("upload-image")]
        [AppAuthorize]
        [RequestSizeLimit(2147483647)]
        [Description("Tải hình ảnh")]
        public virtual async Task<AppDomainResult> UploadImage(IFormFile file)
        {
            string result = await UploadFile(file, "Image");
            var appDomainResult = new AppDomainResult
            {
                success = true,
                data = result,
                resultCode = (int)HttpStatusCode.OK
            };
            return appDomainResult;
        }

        /// <summary>
        /// Upload Multiple File
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("upload-multiple-files")]
        [AppAuthorize]
        [RequestSizeLimit(2147483647)]
        [Description("Upload Multiple File lên hệ thống")]
        public virtual async Task<AppDomainResult> UploadFiles(List<IFormFile> files)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            await Task.Run(() =>
            {
                if (files != null && files.Any())
                {
                    List<string> fileNames = new List<string>();
                    string host = string.Empty;
                    if (Extensions.HttpContext.Current.Request.IsHttps)
                        host = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host.Value;
                    else
                        host = _httpContextAccessor.HttpContext.Request.Scheme + "s://" + _httpContextAccessor.HttpContext.Request.Host.Value;
                    foreach (var file in files)
                    {
                        string fileName = string.Format("{0}-{1}", Guid.NewGuid().ToString(), file.FileName);
                        string fileUploadPath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME);
                        string path = Path.Combine(fileUploadPath, fileName);
                        FileUtilities.CreateDirectory(fileUploadPath);
                        var fileByte = FileUtilities.StreamToByte(file.OpenReadStream());
                        FileUtilities.SaveToPath(path, fileByte);
                        string fileImgPath = host + "/" + fileName;
                        fileNames.Add(fileImgPath);
                    }
                    appDomainResult = new AppDomainResult()
                    {
                        success = true,
                        data = fileNames,
                        resultMessage = "Upload thành công!",
                        resultCode = (int)HttpStatusCode.OK
                    };
                    //List<FileModel> fileModels = new List<FileModel>();
                    //string host = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host.Value;
                    //foreach (var file in files)
                    //{
                    //    string fileName = string.Format("{0}-{1}", Guid.NewGuid().ToString(), file.FileName);
                    //    string fileUploadPath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME);
                    //    string path = Path.Combine(fileUploadPath, fileName);
                    //    FileUtilities.CreateDirectory(fileUploadPath);
                    //    var fileByte = FileUtilities.StreamToByte(file.OpenReadStream());
                    //    FileUtilities.SaveToPath(path, fileByte);
                    //    string fileImgPath = host + "/" + fileName;
                    //    fileModels.Add(new FileModel() { url = fileImgPath, name = fileName, typeFile = Path.GetExtension(fileName) });
                    //}
                    //appDomainResult = new AppDomainResult()
                    //{
                    //    success = true,
                    //    data = fileModels,
                    //    resultMessage = "Upload thành công!",
                    //    resultCode = (int)HttpStatusCode.OK
                    //};
                }
                else
                {
                    throw new AppException("Không tìm thấy tệp");
                }
            });
            return appDomainResult;
        }


        [NonAction]
        public async Task<string> UploadFile(IFormFile file, string folder)
        {
            var httpContextHost = HttpContext.Request.Host;
            string result = "";
            await Task.Run(() =>
            {
                if (file != null && file.Length > 0)
                {
                    string fileName = string.Format("{0}-{1}", Guid.NewGuid().ToString(), file.FileName);
                    string fileUploadPath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME, folder);
                    string path = Path.Combine(fileUploadPath, fileName);
                    FileUtilities.CreateDirectory(fileUploadPath);
                    var fileByte = FileUtilities.StreamToByte(file.OpenReadStream());
                    FileUtilities.SaveToPath(path, fileByte);
                    result = $"https://{httpContextHost}/{folder}/{fileName}";
                }
                else
                {
                    throw new AppException("Không tìm thấy tệp");
                }
            });
            return result;
        }
    }
}
