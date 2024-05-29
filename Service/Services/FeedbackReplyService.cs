using Entities;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using AutoMapper;
using Service.Services.DomainServices;
using Entities.Search;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Extensions;
using Utilities;
using static Utilities.CoreContants;
using System.Net.WebSockets;
using System.Linq;
using Request.RequestCreate;
using System.Data;
using System;
using Azure.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using Entities.AuthEntities;
using Microsoft.Extensions.DependencyInjection;

namespace Service.Services
{
    public class FeedbackReplyService : DomainService<tbl_FeedbackReply, FeedbackReplySearch>, IFeedbackReplyService
    {
        private readonly ISendNotificationService sendNotificationService;
        public FeedbackReplyService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext appDbContext,
            IServiceProvider serviceProvider) : base(unitOfWork, mapper)
        {
            this.sendNotificationService = serviceProvider.GetRequiredService<ISendNotificationService>();
        }
        protected override string GetStoreProcName()
        {
            return "Get_FeedbackReply";
        }

        public override async Task Validate(tbl_FeedbackReply model)
        {
            if (model.feedbackId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Feedback>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == model.feedbackId) ?? throw new AppException(MessageContants.nf_feedback);
                if (item.status == (int)CoreContants.feedbackStatus.da_xong)
                    throw new AppException(MessageContants.feedback_is_done);
            }
        }

        public override async Task<tbl_FeedbackReply> AddItemWithResponse(tbl_FeedbackReply model)
        {
            var feedback = await this.unitOfWork.Repository<tbl_Feedback>().GetQueryable()
                   .FirstOrDefaultAsync(x => x.id == model.feedbackId) ?? throw new AppException(MessageContants.nf_feedback);

            if (feedback.status == (int)feedbackStatus.da_xong)
                throw new AppException(MessageContants.feedback_is_done);

            if (feedback.status == (int)feedbackStatus.moi_gui)
            {
                feedback.status = (int)feedbackStatus.dang_xu_ly;
            }

            #region push notification
            //gửi thông báo cho phụ huynh nếu người trả lời không phải là phụ huynh
            var userLog = LoginContext.Instance.CurrentUser ?? throw new AppException(MessageContants.auth_expiried);

            if (userLog.groups != null && userLog.groups.Count > 0 && feedback.createdBy.HasValue)
            {
                var groupCodes = userLog.groups.Select(x => x.code).ToList();
                if (groupCodes.Contains("PH"))
                {
                    await SendNotification(feedback.createdBy.Value, feedback);
                }
            }

            #endregion

            await this.CreateAsync(model);
            this.unitOfWork.Repository<tbl_Feedback>().Update(feedback);
            await this.unitOfWork.SaveAsync();

            //custom response 
            var item = await this.unitOfWork.Repository<tbl_FeedbackReply>().GetSingleRecordAsync("Get_FeedbackReplyById", new SqlParameter[] { new SqlParameter("id", model.id) });
            if (item != null)
                item.owner = true;
            return item;
        }

        public override async Task<tbl_FeedbackReply> UpdateItemWithResponse(tbl_FeedbackReply model)
        {
            await this.Validate(model);
            await this.UpdateAsync(model);
            await this.unitOfWork.SaveAsync();

            //custom response 
            var item = await this.unitOfWork.Repository<tbl_FeedbackReply>().GetSingleRecordAsync("Get_FeedbackReplyById", new SqlParameter[] { new SqlParameter("id", model.id) });
            if (item != null)
                item.owner = true;
            return item;
        }

        public async Task Vote(FeedbackVote request)
        {
            var feedback = await this.unitOfWork.Repository<tbl_Feedback>().GetQueryable()
                .FirstOrDefaultAsync(x => x.id == request.id) ?? throw new AppException(MessageContants.nf_feedback);
            var userLogin = LoginContext.Instance.CurrentUser;

            if (userLogin.groups.Any() && userLogin.groups.Any(x => x.code == "PH"))
                throw new AppException(MessageContants.unauthorized);

            if (feedback.createdBy != userLogin.userId)
                throw new AppException(MessageContants.only_creator_can_vote_feedback);

            feedback.numberOfStars = request.numberOfStars;
            this.unitOfWork.Repository<tbl_Feedback>().Update(feedback);
            await this.unitOfWork.SaveAsync();
        }

        public async Task SendNotification(Guid users, tbl_Feedback feedback)
        {
            var userReceiver = await this.unitOfWork.Repository<tbl_Users>().Validate(users)
                ?? throw new AppException(MessageContants.nf_parent);

            //send notification
            List<tbl_Users> receiver = new List<tbl_Users>();
            receiver.Add(userReceiver);
            List<IDictionary<string, string>> notiParamList = new List<IDictionary<string, string>>();
            IDictionary<string, string> notiParam = new Dictionary<string, string>();
            notiParam.Add("[FeedbackName]", feedback.title);
            notiParamList.Add(notiParam);
            string linkQuery = "id=" + feedback.id.ToString();/*sendNotificationService.EncodingParam(feedback.id.ToString());*/
            string subLink = "/feedback/detail/";
            sendNotificationService.SendNotification(Guid.Parse("b4c8d27b-5b1e-47f0-ad70-08dc361299fe"), receiver, notiParamList, null, linkQuery, null, LookupConstant.ScreenCode_Feedback, subLink);
        }
    }
}
