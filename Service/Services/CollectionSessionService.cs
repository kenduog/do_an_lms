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
using System.Security.Permissions;
using Request.RequestCreate;
using Request.RequestUpdate;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Azure.Core;
using Microsoft.CodeAnalysis.Operations;
using Request.DomainRequests;
using static Utilities.CoreContants;

namespace Service.Services
{
    public class CollectionSessionService : DomainService<tbl_CollectionSession, CollectionSessionSearch>, ICollectionSessionService
    {
        private readonly ICollectionSessionHeaderService collectionSessionHeaderService;
        private readonly IParentService parentService;
        private readonly ISendNotificationService sendNotificationService;
        public CollectionSessionService(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            ICollectionSessionHeaderService collectionSessionHeaderService,
            IParentService parentService,
            ISendNotificationService sendNotificationService
            ) : base(unitOfWork, mapper)
        {
            this.collectionSessionHeaderService = collectionSessionHeaderService;
            this.sendNotificationService = sendNotificationService;
            this.parentService = parentService;
        }
        protected override string GetStoreProcName()
        {
            return "Get_CollectionSession";
        }

        public override async Task Validate(tbl_CollectionSession model)
        {
            if (model.collectionPlanId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_CollectionPlan>().Validate(model.collectionPlanId.Value) ?? throw new AppException(MessageContants.nf_collectionPlan);
            }
        }

        public override async Task DeleteItem(Guid id)
        {
            var anyPaid = await this.unitOfWork.Repository<tbl_CollectionSessionHeader>().GetQueryable()
                .AnyAsync(x => x.deleted == false && x.collectionSessionId == id && x.isPaid == true);
            if (anyPaid)
                throw new AppException(MessageContants.can_not_delete_collection_session_because_any_parent_paid);

            await this.DeleteAsync(id);
            await this.unitOfWork.SaveAsync();
        }

        public async Task CustomAddItem(CollectionSessionCreate model)
        {
            #region validate
            tbl_CollectionPlan collectionPlan = await this.unitOfWork.Repository<tbl_CollectionPlan>().Validate(model.collectionPlanId.Value)
                ?? throw new AppException(MessageContants.nf_collectionPlan);

            //existed 
            var item = await this.unitOfWork.Repository<tbl_CollectionSession>().GetQueryable()
                .FirstOrDefaultAsync(x => x.deleted == false && x.year == model.year && model.month == x.month && x.collectionPlanId == model.collectionPlanId);
            if (item != null)
                throw new AppException(MessageContants.exs_collectionSession);

            if(collectionPlan.status == (int)CollectionPlanStatus.DaDong)
                throw new AppException(MessageContants.can_not_create_collection_status_lose);
            if (collectionPlan.status == (int)CollectionPlanStatus.SapMo)
            {
                collectionPlan.status = (int)CollectionPlanStatus.DangMo;
                collectionPlan.statusName = GetCollectionPlanName((int)CollectionPlanStatus.DangMo);
                this.unitOfWork.Repository<tbl_CollectionPlan>().Update(collectionPlan);
            }
            //validate studentIds and feeIds
            var fees = model.fees.Distinct().ToList();
            var feeIds = fees.Select(x => x.feeId).Distinct().ToList();
            var feeCount = await this.unitOfWork.Repository<tbl_Fee>().GetQueryable().CountAsync(x => x.deleted == false && feeIds.Contains(x.id));
            if (feeCount != feeIds.Count)
                throw new AppException(MessageContants.nf_fee);

            var studentIds = model.studentIds.Distinct().ToList();
            var studentCount = await this.unitOfWork.Repository<tbl_Student>().GetQueryable().CountAsync(x => x.deleted == false && studentIds.Contains(x.id));
            if (studentCount != studentIds.Count)
                throw new AppException(MessageContants.nf_student);
            #endregion

            tbl_CollectionSession collectionSession = mapper.Map<tbl_CollectionSession>(model);
            await this.unitOfWork.Repository<tbl_CollectionSession>().CreateAsync(collectionSession);

            //map ra danh sách collectionSessionHeader và collectionSessionLine theo danh sách học sinh và khoản thu được chọn
            var amt = fees.Sum(x => (x.price ?? 0));
            List<tbl_CollectionSessionHeader> collectionSessionHeaders = studentIds
                .Select(x => new tbl_CollectionSessionHeader(collectionPlan, x.Value, collectionSession.id, amt))
                .ToList();
            await this.unitOfWork.Repository<tbl_CollectionSessionHeader>().CreateAsync(collectionSessionHeaders);


            List<tbl_CollectionSessionLine> collectionSessionLines = collectionSessionHeaders
               .SelectMany(d => fees.Select(x => new tbl_CollectionSessionLine
               {
                   collectionSessionHeaderId = d.id,
                   collectionSessionId = collectionSession.id,
                   collectionPlanId = collectionPlan.id,
                   feeId = x.feeId,
                   collectionType = x.collectionType,
                   description = x.description,
                   price = x.price,
               })).ToList();

            await this.unitOfWork.Repository<tbl_CollectionSessionLine>().CreateAsync(collectionSessionLines);
            await this.unitOfWork.SaveAsync();
        }

        public async Task UpdateFeeByStudent(CollectionSessionLineUpdate itemModel)
        {
            //update 
            var item = await this.unitOfWork.Repository<tbl_CollectionSessionLine>().GetQueryable()
                .FirstOrDefaultAsync(x => x.deleted == false && x.id == itemModel.id)
                ?? throw new AppException(MessageContants.nf_collectionSessionLine);

            item.price = itemModel.value;

            //save 
            this.unitOfWork.Repository<tbl_CollectionSessionLine>().Update(item);
            await this.unitOfWork.SaveAsync();
        }

        public async Task UpdateOtherFee(CollectionSessionHeaderUpdate model)
        {
            //validate
            var collectionSessionHeader = await this.unitOfWork.Repository<tbl_CollectionSessionHeader>().Validate(model.id)
                ?? throw new AppException(MessageContants.nf_collectionSessionHeader);

            if (model.feeReductionId.HasValue && collectionSessionHeader.allowFeeReduction == true)
            {
                var feeReduction = await this.unitOfWork.Repository<tbl_FeeReduction>().Validate(model.feeReductionId.Value) ?? throw new AppException(MessageContants.nf_feeReduction);
                var feeReductionConfigs = await this.unitOfWork.Repository<tbl_FeeReductionConfig>().GetQueryable()
                    .Where(x => x.feeReductionId == feeReduction.id).ToListAsync();

                //Xóa tiền giảm giá hiện tại 
                var existedItems = await this.unitOfWork.Repository<tbl_CollectionSessionFee>().GetQueryable()
                    .Where(x => x.deleted == false && x.collectionSessionHeaderId == collectionSessionHeader.id)
                    .ToListAsync();
                existedItems.ForEach(x => x.deleted = true);
                this.unitOfWork.Repository<tbl_CollectionSessionFee>().UpdateRange(existedItems);

                //Lấy những khoảng giảm giá của chính sách miễn giảm được chọn
                var collectionSessionLines = await this.unitOfWork.Repository<tbl_CollectionSessionLine>().GetQueryable()
                    .Where(x => x.deleted == false && x.collectionSessionHeaderId == collectionSessionHeader.id)
                    .ToListAsync();

                var existedFees = feeReductionConfigs
                    .Where(x => collectionSessionLines.Select(x => x.feeId).Contains(x.feeId))
                    .ToList();

                //mapping fee reduction content
                if (existedFees.Any())
                {
                    model.feeReductionId = feeReduction.id;
                    model.feeReductionName = feeReduction.name;

                    var feeDict = await this.unitOfWork.Repository<tbl_Fee>().GetQueryable()
                        .Where(x => x.deleted == false)
                        .ToDictionaryAsync(x => x.id, x => x.name);

                    //thêm những khoảng giảm giá mới
                    List<tbl_CollectionSessionFee> collectionSessionFees = existedFees.Select(x =>
                    {
                        var item = new tbl_CollectionSessionFee();
                        item.collectionSessionId = collectionSessionHeader.collectionSessionId;
                        item.collectionSessionHeaderId = collectionSessionHeader.id;
                        item.feeId = x.feeId;
                        item.type = x.type;
                        if (feeDict.TryGetValue(x.feeId ?? Guid.Empty, out string feeName))
                        {
                            item.name = feeName;
                        }

                        //tính số tiền được giảm
                        if (x.type == 2) //giảm tiền
                        {
                            item.value = x.value;
                        }
                        else //giảm phần trăm
                        {
                            var basePrice = collectionSessionLines.FirstOrDefault(d => d.feeId == x.feeId);
                            if (basePrice != null)
                            {
                                item.value = basePrice.price * x.value / 100;
                            }
                        }
                        return item;
                    }).ToList();
                    await this.unitOfWork.Repository<tbl_CollectionSessionFee>().CreateAsync(collectionSessionFees);
                }
            }

            //update 
            var item = mapper.Map<tbl_CollectionSessionHeader>(model);

            //save 
            await this.collectionSessionHeaderService.UpdateAsync(item);
            await this.unitOfWork.SaveAsync();
        }

        public async Task<tbl_CollectionSession> CustomGetByIdAsync(CollectionSessionByIdSearch request)
        {
            var result = new tbl_CollectionSession();

            //get base data
            result = await this.unitOfWork.Repository<tbl_CollectionSession>().Validate(request.id ?? Guid.Empty) ?? throw new AppException(MessageContants.nf_CollectionSession);

            //mapping item
            //lấy danh sách những học viên có phân trang
            CollectionSessionHeaderSearch collectionSessionHeaderSearch = new CollectionSessionHeaderSearch();
            collectionSessionHeaderSearch.pageSize = request.pageSize;
            collectionSessionHeaderSearch.pageIndex = request.pageIndex;
            collectionSessionHeaderSearch.searchContent = request.searchContent;
            collectionSessionHeaderSearch.orderBy = request.orderBy;
            collectionSessionHeaderSearch.collectionSessionId = result.id;
            var collectionSessionHeaders = await this.unitOfWork.Repository<tbl_CollectionSessionHeader>()
                .ExcuteQueryPagingAsync("Get_CollectionSessionHeader", GetSqlParameters(collectionSessionHeaderSearch));


            //mỗi học sinh là 1 row, ngoài ra phải map thêm reductionFees và các cột khoản thu khác
            List<CollectionSessionItem> collectionSessionItems = new List<CollectionSessionItem>();
            collectionSessionItems = mapper.Map<List<CollectionSessionItem>>(collectionSessionHeaders.items);

            var collectionSessionLines = await this.unitOfWork.Repository<tbl_CollectionSessionLine>().GetDataExport("Get_CollectionSessionLine", new SqlParameter[]
            {
                new SqlParameter("collectionSessionId", result.id)
            });

            var collectionFees = await this.unitOfWork.Repository<tbl_CollectionSessionFee>().GetQueryable()
                .Where(x => x.deleted == false && x.collectionSessionId == result.id)
                .ToListAsync();

            foreach (var collectionSessionItem in collectionSessionItems)
            {
                collectionSessionItem.fees = collectionSessionLines.Where(x => x.collectionSessionHeaderId == collectionSessionItem.id && x.feeId.HasValue)
                                                                    .ToDictionary(x => x.feeId.Value, x => new CollectionSessionSubItem { id = x.id, price = x.price, feeName = x.feeName });
                collectionSessionItem.reductionFees = collectionFees.Where(x => x.collectionSessionHeaderId == collectionSessionItem.id).ToList();
                var FeeReduction = this.unitOfWork.Repository<tbl_FeeReduction>().GetQueryable().FirstOrDefault(x => x.id == collectionSessionItem.feeReductionId);
                if (FeeReduction != null)
                    collectionSessionItem.feeReductionName = FeeReduction.name;
            }

            result.collectionSessionItems = new PagedList<CollectionSessionItem>
            {
                pageIndex = request.pageIndex,
                pageSize = request.pageSize,
                totalItem = collectionSessionHeaders.totalItem,
                items = collectionSessionItems
            };
            return result;
        }
        public async Task ConfirmPayment(ComfirmPayment model)
        {
            //validate
            var collectionSessionHeader = await this.unitOfWork.Repository<tbl_CollectionSessionHeader>().Validate(model.id)
                ?? throw new AppException(MessageContants.nf_collectionSessionHeader);

            //update status
            collectionSessionHeader.paymentStatus = 2;

            //save change
            this.unitOfWork.Repository<tbl_CollectionSessionHeader>().Update(collectionSessionHeader);
            await this.unitOfWork.SaveAsync();
        }

        public async Task SendNotification(CollectionSessionNotificationRequest request)
        {
            //validate
            var collectionSession = await this.unitOfWork.Repository<tbl_CollectionSession>()
                .Validate(request.collectionSessionId.Value) ?? throw new AppException(MessageContants.nf_CollectionSession);

            //get student ids
            var studentIds = await unitOfWork.Repository<tbl_CollectionSessionHeader>()
                            .GetQueryable()
                            .Where(x => x.deleted == false && x.collectionSessionId == collectionSession.id && x.studentId.HasValue)
                            .Select(x => x.studentId.Value)
                            .ToListAsync();

            //get data parent 
            var users = await this.parentService.GetParentUserByStudentId(studentIds);

            //send notification
            List<IDictionary<string, string>> notiParams = new List<IDictionary<string, string>>();
            List<IDictionary<string, string>> emailParams = new List<IDictionary<string, string>>();
            Dictionary<string, string> deepLinkQueryDic = new Dictionary<string, string>();
            Dictionary<string, string> param = new Dictionary<string, string>();
            param["id"] = collectionSession.id.ToString();


            sendNotificationService.SendNotification_v2(LookupConstant.NCC_CollectionSession,
                request.title,
                request.content,
                users,
                notiParams,
                emailParams,
                null,
                deepLinkQueryDic,
                LookupConstant.ScreenCode_DailyActivities,
                param);
        }
    }
}
