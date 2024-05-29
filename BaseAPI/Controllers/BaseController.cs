using Entities.DomainEntities;
using Extensions;
using Interface;
using Interface.Services;
using Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Interface.Services.DomainServices;
using Request.DomainRequests;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Interface.UnitOfWork;
using AppDbContext;
using Interface.DbContext;
using OfficeOpenXml.ConditionalFormatting;

namespace BaseAPI.Controllers
{
    [ApiController]
    public abstract class BaseController<E, C, U, F> : ControllerBase
        where E : Entities.DomainEntities.DomainEntities
        where C : DomainCreate
        where U : DomainUpdate
        where F : BaseSearch, new()
    {
        protected readonly ILogger<BaseController<E, C, U, F>> logger;
        protected readonly IServiceProvider serviceProvider;
        protected readonly IMapper mapper;
        protected IDomainService<E, F> domainService;
        protected IWebHostEnvironment env;
        private readonly IUserService userService;
        private readonly IAppDbContext appDbContext;

        public BaseController(IServiceProvider serviceProvider, ILogger<BaseController<E, C, U, F>> logger, IWebHostEnvironment env)
        {
            this.env = env;
            this.logger = logger;
            this.mapper = serviceProvider.GetService<IMapper>();
            this.serviceProvider = serviceProvider;
            userService = serviceProvider.GetRequiredService<IUserService>();
            this.appDbContext = serviceProvider.GetRequiredService<IAppDbContext>();
        }

        /// <summary>
        /// Lấy thông tin theo id v1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AppAuthorize]
        [Description("Lấy thông tin")]
        public virtual async Task<AppDomainResult> GetById(Guid id)
        {
            var item = await this.domainService.GetByIdAsync(id) ?? throw new KeyNotFoundException(MessageContants.nf_item);
            return new AppDomainResult(item);
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public virtual async Task<AppDomainResult> AddItem([FromBody] C itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<E>(itemModel);
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            var newData = await this.domainService.AddItemWithResponse(item);
            return new AppDomainResult(newData);
        }

        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut]
        [AppAuthorize]
        [Description("Chỉnh sửa")]
        public virtual async Task<AppDomainResult> UpdateItem([FromBody] U itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<E>(itemModel);

            if (item == null)
                throw new KeyNotFoundException(MessageContants.nf_item);

            var newData = await this.domainService.UpdateItemWithResponse(item);
            return new AppDomainResult(newData);
        }

        /// <summary>
        /// Xóa item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AppAuthorize]
        [Description("Xoá")]
        public virtual async Task<AppDomainResult> DeleteItem(Guid id)
        {
            await this.domainService.DeleteItem(id);
            return new AppDomainResult();
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public virtual async Task<AppDomainResult> Get([FromQuery] F baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            PagedList<E> pagedData = await this.domainService.GetPagedListData(baseSearch);
            return new AppDomainResult(pagedData);
        }

        [NonAction]
        public virtual async Task Validate(E model)
        {
            await this.domainService.Validate(model);
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
                    throw new AppException(MessageContants.nf_file);
                }
            });
            return result;
        }
    }
}
