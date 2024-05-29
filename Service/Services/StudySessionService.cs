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
using System.Threading;
using Request.RequestCreate;
using System.Configuration;

namespace Service.Services
{
    public class StudySessionService : DomainService<tbl_StudySession, StudySessionSearch>, IStudySessionService
    {
        public StudySessionService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "Get_StudySession";
        }

        public override async Task Validate(tbl_StudySession model)
        {
            if (model.branchId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Branch>().Validate(model.branchId.Value) ?? throw new AppException(MessageContants.nf_branch);
            }


            //time validation
            var studySession = await this.unitOfWork.Repository<tbl_StudySession>().Validate(model.id) ?? throw new AppException(MessageContants.nf_studySession);
            var sTime = model.sTime ?? studySession.sTime;
            var eTime = model.eTime ?? studySession.eTime;

            if (sTime >= eTime)
                throw new AppException(MessageContants.cp_date);

            var existedTime = await this.unitOfWork.Repository<tbl_StudySession>().GetQueryable()
                .AnyAsync(x => x.deleted == false && x.id != model.id
                && ((x.sTime < sTime && sTime < x.eTime) || (x.sTime < eTime && eTime < x.eTime)));
            if (existedTime)
                throw new AppException(MessageContants.exs_studySession);
        }
    }
}
