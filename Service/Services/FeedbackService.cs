using Entities;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Utilities;
using AutoMapper;
using Service.Services.DomainServices;
using Entities.Search;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static Utilities.CoreContants;
using System.Linq.Expressions;

namespace Service.Services
{
    public class FeedbackService : DomainService<tbl_Feedback, FeedbackSearch>, IFeedbackService
    {
        protected readonly IAppDbContext Context;
        public FeedbackService(IAppDbContext appDbContext,IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            this.Context = appDbContext;
        }


        protected override string GetStoreProcName()
        {
            return "Get_Feedback";
        }

        public override async Task Validate(tbl_Feedback model)
        {
            if (model.feedbackGroupId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_FeedbackGroup>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.feedbackGroupId)
                    ?? throw new AppException(MessageContants.req_feedbackGroupId);
            }

            if (model.id == Guid.Empty) //validate người tạo
            {
                var userLogin = LoginContext.Instance.CurrentUser ?? throw new AppException(MessageContants.auth_expiried);
            }
            
        }
        public async Task<FeedbackPagedList> GetPagedStatus(FeedbackSearch baseSearch)
        {
            FeedbackPagedList  pagedList = new FeedbackPagedList();
            var userLog = LoginContext.Instance.CurrentUser ?? throw new AppException(MessageContants.auth_expiried);
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            pagedList = await ExcuteQueryWithStatusPagingAsync(this.GetStoreProcName(), parameters);
            pagedList.pageIndex = baseSearch.pageIndex;
            pagedList.pageSize = baseSearch.pageSize;
            return pagedList;
        }

        public async Task<List<tbl_Feedback>> GetFeedbackReport()
        {
            var data = await this.unitOfWork.Repository<tbl_Feedback>().ExcuteStoreAsync("Get_FeedbackReport", new SqlParameter[] {});
            return data.ToList();
        }

        public Task<FeedbackPagedList> ExcuteQueryWithStatusPagingAsync(string commandText, SqlParameter[] sqlParameters)
        {
            return Task.Run(() =>
            {
                FeedbackPagedList pagedList = new FeedbackPagedList();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = commandText;
                    command.Parameters.AddRange(sqlParameters);
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    pagedList.items = MappingDataTable.ConvertToList<tbl_Feedback>(dataTable);
                    if (pagedList.items != null && pagedList.items.Any())
                    {
                        var item = pagedList.items.FirstOrDefault();
                        pagedList.totalItem = item.totalItem;
                        pagedList.allState = item.allState;
                        pagedList.sent = item.sent;
                        pagedList.processing = item.processing;
                        pagedList.done = item.done;
                        if (item.id == Guid.Empty)
                            pagedList.items = new List<tbl_Feedback>();
                    }
                    return pagedList;
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
            });
        }

        public override async Task<tbl_Feedback> GetByIdAsync(Guid id)
        {
            var feedback = await this.unitOfWork.Repository<tbl_Feedback>()
                .GetSingleRecordAsync("Get_FeedbackById", GetSqlParameters(new {id = id})) 
                ?? throw new AppException(MessageContants.nf_feedback);

            var pars = GetSqlParameters(new { feedbackId = feedback.id });
            var data = await this.unitOfWork.Repository<tbl_FeedbackReply>().GetDataExport("Get_FeedbackReply", pars);

            var userLog = LoginContext.Instance.CurrentUser;
            feedback.replies = data.Select(item => { item.owner = item.createdBy == userLog.userId; return item; }).ToList(); ;
            return feedback;
        }

        public async Task Done (Guid id)
        {
            var feedback = await this.GetByIdAsync(id) ?? throw new AppException(MessageContants.nf_feedback);

            feedback.status = (int)feedbackStatus.da_xong;

            this.unitOfWork.Repository<tbl_Feedback>().Update(feedback);

            await this.unitOfWork.SaveAsync();
        }
    }
}
