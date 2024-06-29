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
using Entities.AuthEntities;
using AppDbContext;
using Microsoft.AspNetCore.Http;
using System.Drawing.Printing;
using Microsoft.CodeAnalysis.Operations;
using System.Xml.Linq;
using Interface.UnitOfWork;
using OfficeOpenXml;
using API.Model;
using Azure.Core;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Bảng tin")]
    [Authorize]
    public class NewsController : BaseController<tbl_News, NewsCreate, NewsUpdate, NewsSearch>
    {
        protected ILikeInNewsService likeInNewsService;
        protected INewsService newsService;
        protected ICommentInNewsService commentInNewsService;
        protected IDocumentNewsService documentNewsService;
        private readonly IBranchService branchService;
        private readonly IGroupNewsService groupNewsService;
        private readonly IUserService userService;
        private readonly IUserJoinGroupNewsService userJoinGroupNewsService;
        private readonly ISendNotificationService sendNotificationService;
        public NewsController(IServiceProvider serviceProvider
            , ILogger<BaseController<tbl_News, NewsCreate, NewsUpdate, NewsSearch>> logger
            , IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<INewsService>();
            this.likeInNewsService = serviceProvider.GetRequiredService<ILikeInNewsService>();
            this.commentInNewsService = serviceProvider.GetRequiredService<ICommentInNewsService>();
            this.documentNewsService = serviceProvider.GetRequiredService<IDocumentNewsService>();
            this.newsService = serviceProvider.GetRequiredService<INewsService>();
            this.branchService = serviceProvider.GetRequiredService<IBranchService>();
            this.groupNewsService = serviceProvider.GetRequiredService<IGroupNewsService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.userJoinGroupNewsService = serviceProvider.GetRequiredService<IUserJoinGroupNewsService>();
            this.sendNotificationService = serviceProvider.GetRequiredService<ISendNotificationService>();
        }
        /// <summary>
        /// Lấy thông tin theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AppAuthorize]
        [Description("Lấy thông tin")]
        public override async Task<AppDomainResult> GetById(Guid id)
        {
            var data = await GetSingleNewsById(id);
            return new AppDomainResult(data);
        }

        public override async Task<AppDomainResult> Get([FromQuery] NewsSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var userId = LoginContext.Instance.CurrentUser.userId;
            //bool isAdmin = await userService.IsAdmin(userId);
            var privateSearch = new PrivateNewSearch
            {
                isAdmin = false,
                orderBy = baseSearch.orderBy,
                pageIndex = baseSearch.pageIndex,
                pageSize = baseSearch.pageSize,
                searchContent = baseSearch.searchContent,
            };
            if (baseSearch.branchId.HasValue)
                privateSearch.myBranchIds = baseSearch.branchId.Value.ToString();
            //var user = await userService.GetByIdAsync(userId);
            //user.branchIds;
            var myGroupIds = (await userJoinGroupNewsService.GetAsync(x => x.userId == userId && x.deleted == false))
                .Select(x => x.groupNewsId).Distinct().ToList();
            if (myGroupIds.Any())
                privateSearch.myGroupIds = string.Join(",", myGroupIds);
            PagedList<tbl_News> pagedData = await this.domainService.GetPagedListData(privateSearch);
            if (pagedData.totalItem == 0)
                return new AppDomainResult(pagedData);
            var data = (from i in pagedData.items
                        select new NewsDTO
                        {
                            id = i.id,
                            active = i.active,
                            branchIds = i.branchIds,
                            content = i.content,
                            countComment = i.countComment,
                            countLike = i.countLike,
                            created = i.created,
                            createdBy = i.createdBy,
                            deleted = i.deleted,
                            fullName = i.fullName,
                            groupName = i.groupName,
                            pinned = i.pinned,
                            pinnedPosition = i.pinnedPosition,
                            title = i.title,
                            totalItem = i.totalItem,
                            updated = i.updated,
                            updatedBy = i.updatedBy,
                            userId = i.userId,
                            userThumbnail = i.userThumbnail,
                            liked = Task.Run(() => likeInNewsService.GetLiked(i.id, userId)).Result,
                            branch = Task.Run(() => branchService.GetAsync(x => x.deleted == false && i.branchIds.Contains(x.id.ToString()))).Result
                                       .Select(x => new BranchInNewsDTO
                                       {
                                           id = x.id,
                                           code = x.code,
                                           name = x.name
                                       }).ToList(),
                            documentNews = Task.Run(() => documentNewsService.GetAsync(x => x.deleted == false && x.newsId == i.id)).Result
                                           .Select(x => new DocumentNewsDTO
                                           {
                                               link = x.link,
                                               typeCode = x.typeCode
                                           }).ToList()
                        }).ToList();

            int totalPage = 0;
            decimal count = pagedData.totalItem;
            if (count > 0)
                totalPage = (int)Math.Ceiling(count / baseSearch.pageSize);
            return new AppDomainResult
            {
                resultCode = ((int)HttpStatusCode.OK),
                resultMessage = "Thành công",
                success = true,
                data = new
                {
                    items = data,
                    pageIndex = baseSearch.pageIndex,
                    pageSize = baseSearch.pageSize,
                    totalItem = count,
                    totalPage = totalPage
                }
            };
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] NewsCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            Guid userId = LoginContext.Instance.CurrentUser.userId;
            var branchIds = itemModel.branchIds.Split(',');
            if (branchIds.Any())
            {
                foreach (var branchId in branchIds)
                {
                    var hasBranch = await branchService.AnyAsync(x => x.id.ToString() == branchId);
                    if (!hasBranch)
                        throw new AppException(MessageContants.nf_branch);
                    
                }
            }
            var item = mapper.Map<tbl_News>(itemModel);
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            item.userId = userId;
            await this.domainService.CreateAsync(item);
            if (itemModel.uploads.Count > 0)
                foreach (var u in itemModel.uploads)
                {
                    var document = new tbl_DocumentNews()
                    {
                        newsId = item.id,
                        link = u,
                        typeCode = GetFileExtensionFromUrl(u),
                    };
                    await documentNewsService.CreateAsync(document);
                }
            List<tbl_Users> receiverList = new List<tbl_Users>();
            HashSet<Guid> checkDuplicateReceiver = new HashSet<Guid>();
            for (int i =0;i < receiverList.Count;)
            {
                // Không gửi thông báo đến người tạo
                if(receiverList[i].id == userId)
                {
                    receiverList.RemoveAt(i);
                }
                else if (!checkDuplicateReceiver.Add(receiverList[i].id))
                {
                    receiverList.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            List<IDictionary<string, string>> notiParamList = new List<IDictionary<string, string>>();
            foreach(var receiver in receiverList)
            {
                IDictionary<string, string> notiParam = new Dictionary<string, string>();
                notiParam.Add("[NewsName]", itemModel.title);
                notiParamList.Add(notiParam);
            }
            string linkQuery = string.Empty;
            string subLink = string.Empty;
            sendNotificationService.SendNotification(Guid.Parse("f239a311-6c8a-4d82-ffeb-08dc180db26d"), receiverList, notiParamList, null, linkQuery, null, LookupConstant.ScreenCode_News, subLink);
            return new AppDomainResult(item);
        }

        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut]
        [AppAuthorize]
        [Description("Chỉnh sửa")]
        public override async Task<AppDomainResult> UpdateItem([FromBody] NewsUpdate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var userId = LoginContext.Instance.CurrentUser.userId;
            var news = await domainService.GetByIdAsync(itemModel.id);
            if (news.userId != userId)
                throw new AppException(MessageContants.can_not_update_news_others);
            var item = mapper.Map<tbl_News>(itemModel);
            if (item == null)
                throw new KeyNotFoundException(MessageContants.nf_item);
            await domainService.UpdateAsync(item);
            var documents = await documentNewsService.GetAsync(x => x.newsId == item.id && x.commentId == null && x.deleted == false);
            if (documents.Any())
            {
                foreach (var document in documents)
                {
                    if (!itemModel.uploads.Contains(document.link))
                        document.deleted = true;
                }
                await documentNewsService.UpdateAsync(documents);
                if (itemModel.uploads.Any())
                {
                    foreach (var upload in itemModel.uploads)
                    {
                        if (!documents.Any(x => x.link == upload))
                        {
                            await documentNewsService.CreateAsync(new tbl_DocumentNews
                            {
                                active = true,
                                commentId = null,
                                created = Timestamp.Now(),
                                deleted = false,
                                link = upload,
                                newsId = item.id,
                                typeCode = GetFileExtensionFromUrl(upload),
                                updated = Timestamp.Now()
                            });
                        }
                    }
                }
            }
            else
            {
                if (itemModel.uploads.Any())
                {
                    foreach (var upload in itemModel.uploads)
                    {
                        await documentNewsService.CreateAsync(new tbl_DocumentNews
                        {
                            active = true,
                            commentId = null,
                            created = Timestamp.Now(),
                            deleted = false,
                            link = upload,
                            newsId = item.id,
                            typeCode = GetFileExtensionFromUrl(upload),
                            updated = Timestamp.Now()
                        });
                    }
                }
            }

            return new AppDomainResult();
        }
        #region ẩn
        //[NonAction]
        //private async Task<bool> createDocumentNews(List<string> uploads, IList<tbl_DocumentNews> document, Guid? newsId)
        //{
        //    var createDocument = new List<tbl_DocumentNews>();
        //    foreach (var u in uploads)
        //    {
        //        if (document.Any())
        //        {
        //            if (document.Count(x => x.link == u) == 0)
        //            {
        //                var create = new tbl_DocumentNews()
        //                {
        //                    newsId = newsId,
        //                    link = u,
        //                    typeCode = GetFileExtensionFromUrl(u),
        //                };
        //                createDocument.Add(create);
        //            }
        //        }
        //    }
        //    await documentNewsService.CreateAsync(createDocument);
        //    return true;
        //}
        //[NonAction]
        //private async Task<bool> updateDocumentNews(List<string> uploads, IList<tbl_DocumentNews> document)
        //{
        //    foreach (var d in document)
        //    {
        //        if (uploads.IndexOf(d.link) == -1)
        //            d.deleted = true;
        //    }
        //    await documentNewsService.UpdateAsync(document);
        //    return true;
        //}
        /// <summary>
        /// Số lượt like và bình luận
        /// </summary>
        /// <param name="newsid"></param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("TotalLikeAndCommentInNews")]
        //[AppAuthorize]
        //[Description("Số lượt like và bình luận")]
        //public async Task<AppDomainResult> TotalLikeAndCommentInNews([FromBody] Guid newsid, Guid? commentid)
        //{
        //    AppDomainResult appDomainResult = new AppDomainResult();
        //    if (!ModelState.IsValid)
        //        throw new AppException(ModelState.GetErrorMessage());
        //    try
        //    {
        //        TotalLikeAndCommentInNewsModel data = new TotalLikeAndCommentInNewsModel()
        //        {
        //            number_comment = await this.commentInNewsService.CountAsync(x => x.newsId == newsid && (commentid == null || x.replyCommentId == commentid)),
        //            number_like = await this.likeInNewsService.CountAsync(x => x.newsId == newsid && (commentid == null || x.commentId == commentid))
        //        };
        //        appDomainResult.success = true;
        //        appDomainResult.data = data;
        //        appDomainResult.resultCode = (int)HttpStatusCode.OK;
        //        appDomainResult.resultMessage = MessageContants.success;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new AppException(MessageContants.err);
        //    }
        //    return appDomainResult;
        //}
        #endregion

        [NonAction]
        private string GetFileExtensionFromUrl(string url)
        {
            try
            {
                Uri uri = new Uri(url);

                string fileName = Path.GetFileName(uri.LocalPath);

                string extension = Path.GetExtension(fileName);
                string result = extension.Split('.')[1];

                return result.ToLower();
            }
            catch (UriFormatException)
            {
                return null;
            }
        }

        ///// <summary>
        ///// Lấy danh sách item 
        ///// </summary>
        ///// <param name="baseSearch"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[AppAuthorize]
        //[Description("Lấy danh sách")]
        //public async Task<AppDomainResult> GetNews([FromQuery] NewsSearch baseSearch)
        //{
        //    if (!ModelState.IsValid)
        //        throw new AppException(ModelState.GetErrorMessage());
        //    var data = await this.newsService.getNews(baseSearch);
        //    return new AppDomainResult(data);
        //}

        //public class TotalLikeAndCommentInNewsModel
        //{
        //    public int number_like { get; set; }
        //    public int number_comment { get; set; }
        //}

        [HttpPut("pin")]
        [AppAuthorize]
        [Description("Ghim bài viết")]
        public async Task<AppDomainResult> Pin([FromBody] DomainUpdate request)
        {
            await this.newsService.Pin(request.id);
            var data = await GetSingleNewsById(request.id);
            return new AppDomainResult(data);
        }
        [HttpPut("unpin")]
        [AppAuthorize]
        [Description("Bỏ ghim bài viết")]
        public async Task<AppDomainResult> UnPin([FromBody] DomainUpdate request)
        {
            await this.newsService.UnPin(request.id); 
            var data = await GetSingleNewsById(request.id);
            return new AppDomainResult(data);
        }

        [HttpPut("pin/position")]
        [AppAuthorize]
        [Description("Cập nhật thứ tự bài ghim")]
        public async Task<AppDomainResult> PositionPin([FromBody] PinPositionUpdate itemModel)
        {
            await this.newsService.PinPositionUpdate(itemModel);
            return new AppDomainResult();
        }

        [NonAction]
        private async Task<NewsDTO> GetSingleNewsById(Guid id)
        {
            var item = await this.domainService.GetByIdAsync(id) ?? throw new KeyNotFoundException(MessageContants.nf_item);
            var userId = LoginContext.Instance.CurrentUser.userId;
            var data = new NewsDTO
            {
                id = item.id,
                active = item.active,
                branchIds = item.branchIds,
                content = item.content,
                countComment = item.countComment,
                countLike = item.countLike,
                created = item.created,
                createdBy = item.createdBy,
                deleted = item.deleted,
                fullName = item.fullName,
                groupName = item.groupName,
                pinned = item.pinned,
                pinnedPosition = item.pinnedPosition,
                title = item.title,
                totalItem = item.totalItem,
                updated = item.updated,
                updatedBy = item.updatedBy,
                userId = userId,
                userThumbnail = item.userThumbnail,
                liked = Task.Run(() => likeInNewsService.GetLiked(item.id, userId)).Result,
                branch = Task.Run(() => branchService.GetAsync(x => x.deleted == false && item.branchIds.Contains(x.id.ToString()))).Result
                            .Select(x => new BranchInNewsDTO
                            {
                                id = x.id,
                                code = x.code,
                                name = x.name
                            }).ToList(),
                documentNews = Task.Run(() => documentNewsService.GetAsync(x => x.deleted == false && x.newsId == item.id)).Result
                            .Select(x => new DocumentNewsDTO
                            {
                                link = x.link,
                                typeCode = x.typeCode
                            }).ToList()
            };
            return data;
        }
    }
}
