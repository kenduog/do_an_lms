using Entities;
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
using Entities.DomainEntities;
using Request.RequestUpdate;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Azure.Core;
using Request.RequestCreate;
using System.Diagnostics.Metrics;


namespace Service.Services
{
    public class NewsService : DomainService<tbl_News, NewsSearch>, INewsService
    {
        IAppDbContext appDbContext;
        
        public NewsService(IAppUnitOfWork unitOfWork, IMapper mapper,IAppDbContext appDbContext) : base(unitOfWork, mapper)
        {
            this.appDbContext = appDbContext;
        }
        protected override string GetStoreProcName()
        {
            return "Get_News_V2";
        }

        public override async Task<tbl_News> GetByIdAsync(Guid id)
        {
            var userLog = LoginContext.Instance.CurrentUser ?? throw new AppException(MessageContants.auth_expiried);
            var prs = new SqlParameter[] { new SqlParameter("id", id), new SqlParameter("userId", userLog.userId) };
            //custom response 
            var item = await this.unitOfWork.Repository<tbl_News>().GetSingleRecordAsync("Get_NewById", prs);
            return item;
        }
        public async Task Pin(Guid newId)
        {
            var item = await this.unitOfWork.Repository<tbl_News>().GetQueryable()
                 .FirstOrDefaultAsync(x => x.id == newId && x.deleted == false) ?? throw new AppException(MessageContants.nf_news);

            //last position
            var lastItem = await this.unitOfWork.Repository<tbl_News>().GetQueryable().Where(x => x.deleted == false && x.pinned == true && x.groupNewsId == item.groupNewsId && x.pinnedPosition.HasValue)
                .OrderByDescending(x => x.pinnedPosition).FirstOrDefaultAsync();
            int lastPos = 1;
            if(lastItem != null)
                lastPos = lastItem.pinnedPosition.HasValue ? lastItem.pinnedPosition.Value + 1: 1;

            item.pinned = true;
            item.pinnedPosition = lastPos;
            this.unitOfWork.Repository<tbl_News>().Update(item);
            await this.unitOfWork.SaveAsync();
        }

        public async Task UnPin(Guid newId)
        {
            var item = await this.unitOfWork.Repository<tbl_News>().GetQueryable()
                 .FirstOrDefaultAsync(x => x.id == newId && x.deleted == false) ?? throw new AppException(MessageContants.nf_news);

            //update pos
            if (item.pinnedPosition.HasValue)
                await UpdatePos(item, -1, item.pinnedPosition.Value);

            item.pinned = false;
            item.pinnedPosition = null;

            this.unitOfWork.Repository<tbl_News>().Update(item);
            await this.unitOfWork.SaveAsync();
        }
        public async Task PinPositionUpdate(PinPositionUpdate itemModel)
        {
            using(var tran = await appDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if(!itemModel.items.Any())
                        throw new AppException(MessageContants.nf_others);
                    foreach(var item in itemModel.items)
                    {
                        var news = await unitOfWork.Repository<tbl_News>()
                                        .GetQueryable().SingleOrDefaultAsync(x=>x.id == item.id);
                        if(news == null)
                            throw new AppException(MessageContants.nf_news);
                        news.pinnedPosition = item.pinnedPosition;
                        unitOfWork.Repository<tbl_News>().Update(news);
                    }
                    await unitOfWork.SaveAsync();
                    await tran.CommitAsync();
                }
                catch(AppException e)
                {
                    await tran.RollbackAsync();
                    throw e;
                }
            }
        }
        // public async Task PinPositionUpdate(PinPositionUpdate request)
        // {
        //     var item = await this.unitOfWork.Repository<tbl_News>().GetQueryable()
        //         .FirstOrDefaultAsync(x=>x.id == request.id && x.deleted == false) ?? throw new AppException(MessageContants.nf_news);

        //     //last position
        //     var lastItem = await this.unitOfWork.Repository<tbl_News>().GetQueryable().Where(x => x.deleted == false && x.pinned == true).OrderByDescending(x => x.pinnedPosition).FirstOrDefaultAsync();
        //     if(lastItem != null && request.pos > lastItem.pinnedPosition)
        //         request.pos = lastItem.pinnedPosition;

        //     int? from = 0;
        //     int step = 0;
        //     int? to = 0;

        //     //ghim mới
        //     if (item.pinned != true)
        //     {
        //         item.pinned = true;
        //         to = null;
        //         step = 1;
        //         from = request.pos;
        //         if (lastItem != null)
        //             item.pinnedPosition = lastItem.pinnedPosition.HasValue ? lastItem.pinnedPosition.Value : 1;
        //     }
        //     else
        //     {
        //         from = Math.Min(request.pos ?? 0, item.pinnedPosition ?? 0);
        //         to = Math.Max(request.pos ?? 0, item.pinnedPosition ?? 0);
        //         step = (request.pos.Value > item.pinnedPosition.Value) ? -1 : 1;
        //     }

        //     //update any pos of pinned item
        //     await UpdatePos(item, step, from, to);

        //     item.pinnedPosition = request.pos;
        //     this.unitOfWork.Repository<tbl_News>().Update(item);
        //     await this.unitOfWork.SaveAsync();
        // }

        public async Task UpdatePos(tbl_News item,  int step, int? from = null, int? to = null)
        {
            //Cập nhật những thằng khác với item
            var query = this.unitOfWork.Repository<tbl_News>().GetQueryable()
                .Where(x => 
                x.deleted == false 
                && x.groupNewsId == item.groupNewsId
                && x.id != item.id
                );
            if (to.HasValue)
                query = query.Where(x => x.pinnedPosition <= to.Value);
            if (from.HasValue)
                query = query.Where(x => from.Value <= x.pinnedPosition);

            var items = await query.ToListAsync();

            if (items.Any())
            {
                foreach(var i in items)
                {
                    i.pinnedPosition += step;
                }
                this.unitOfWork.Repository<tbl_News>().UpdateRange(items);
            }
            await this.unitOfWork.SaveAsync();
        }
    }
}
