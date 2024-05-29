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
using Entities.DomainEntities;
using BaseAPI.Controllers;
using Service.Services;
using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;
using System.Reflection.Metadata;
using Interface.DbContext;
using System.Data.Entity;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Màn hình mẫu hồ sơ")]
    [Authorize]
    public class ProfileTemplateController : BaseController<tbl_ProfileTemplate, ProfileTemplateCreate, ProfileTemplateUpdate, BaseSearch>
    {
        private readonly IAppDbContext appDbContext;
        public ProfileTemplateController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_ProfileTemplate, ProfileTemplateCreate, ProfileTemplateUpdate, BaseSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<IProfileTemplateService>();
            this.appDbContext = serviceProvider.GetRequiredService<IAppDbContext>();
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] ProfileTemplateCreate itemModel)
        {
            //vị trí cuối cùng trong danh sách
            var count = await this.domainService.CountAsync(x => x.deleted == false);
            itemModel.index = count + 1;

            return await base.AddItem(itemModel);
        }

        ///// <summary>
        ///// Cập nhật thông tin item
        ///// </summary>
        ///// <param name="itemModel"></param>
        ///// <returns></returns>
        //[HttpPut("position")]
        //[AppAuthorize]
        //[Description("Dời vị trí")]
        //public async Task<AppDomainResult> UpdatePosition([FromBody] ProfileTemplatePositionUpdate itemModel)
        //{
        //    if (!ModelState.IsValid)
        //        throw new AppException(ModelState.GetErrorMessage());
        //    var item = await this.domainService.GetByIdAsync(itemModel.id);
        //    if (item == null)
        //        throw new KeyNotFoundException(MessageContants.nf_profileTemplate);

        //    //Nếu isUp = true thì cập nhật thằng trên xuống và ngược lại
        //    int index = (int)(itemModel.isUp ? item.index - 1 : item.index + 1);
        //    var template = await this.domainService.GetSingleAsync(x => x.index == index);
        //    if (item == null)
        //        throw new KeyNotFoundException(MessageContants.nf_profileTemplate);
        //    template.index = item.index;
        //    item.index = index;

        //    bool success = await this.domainService.UpdateAsync(item) && await this.domainService.UpdateAsync(template);
        //    if (!success)
        //        throw new AppException(MessageContants.err);

        //    return new AppDomainResult
        //    {
        //        success = success,
        //        resultCode = (int)HttpStatusCode.OK,
        //        resultMessage = MessageContants.success 
        //    };
        //}
        /// <summary>
        /// Sắp xếp
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("change-index")]
        [AppAuthorize]
        [Description("Sắp xếp")]
        public async Task<AppDomainResult> ChangeIndex([FromBody] ChangeIndexModel itemModel)
        {
            using (var tran = await appDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (!itemModel.items.Any())
                        throw new AppException("Không tìm thấy dữ liệu");
                    foreach (var item in itemModel.items)
                    {
                        var entity = await domainService.GetByIdAsync(item.id);
                        if (entity == null)
                            throw new AppException("Không tìm thấy dữ liệu");
                        entity.index = item.index;
                        await domainService.UpdateAsync(entity);
                    }
                    await tran.CommitAsync();
                    return new AppDomainResult
                    {
                        success = true,
                        resultCode = (int)HttpStatusCode.OK,
                        resultMessage = MessageContants.success
                    };
                }
                catch (AppException e)
                {
                    await tran.RollbackAsync();
                    throw e;
                }
            }
        }

    }
}
