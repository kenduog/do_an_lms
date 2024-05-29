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
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using System.Net.WebSockets;
using Microsoft.Identity.Client;
using Request.RequestCreate;

namespace Service.Services
{
    public class StudentLeaveRequestService : DomainService<tbl_StudentLeaveRequest, StudentLeaveRequestSearch>, IStudentLeaveRequestService
    {
        public StudentLeaveRequestService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "Get_StudentLeaveRequest";
        }
        public override async Task Validate(tbl_StudentLeaveRequest model)
        {
            if (model.classId.HasValue)
            {
                var _class = await this.unitOfWork.Repository<tbl_Class>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.classId && x.deleted == false)
                    ?? throw new AppException(MessageContants.nf_class);
            }
        }

        public async Task AddRange(MultipleStudentLeaveRequest request) 
        {
            var _class = await this.unitOfWork.Repository<tbl_Class>().GetQueryable().FirstOrDefaultAsync(x => x.id == request.classId && x.deleted == false)
                    ?? throw new AppException(MessageContants.nf_class);

            if(request.requests == null && request.requests.Count == 0)
                throw new AppException(MessageContants.req_min1);

            var studentCount = await this.unitOfWork.Repository<tbl_Student>().GetQueryable().CountAsync(x => request.requests.Select(d => d.studentId).Contains(x.id));
            if (studentCount != request.requests.Count)
                throw new AppException(MessageContants.nf_student);

            //tạo nhiều phiếu nghỉ phép cho trẻ
            foreach(var item in request.requests)
            {
                //check xem có đơn xin phép trong thời  gian này chưa, nếu chưa thì tạo mới
                //đóng những đơn trong khoảng thời gian đó
                var childSchedules = await this.unitOfWork.Repository<tbl_StudentLeaveRequest>().GetQueryable()
                    .Where(x => x.deleted == false 
                    && x.studentId == item.studentId 
                    && item.fromDate<= x.fromDate && x.toDate <= item.toDate).ToListAsync();

                if (childSchedules != null && childSchedules.Count > 0)
                {
                    foreach (var childSchedule in childSchedules)
                    {
                        childSchedule.deleted = true;
                        this.unitOfWork.Repository<tbl_StudentLeaveRequest>().Update(childSchedule);
                    }
                }

                var sTimeDup = await this.unitOfWork.Repository<tbl_StudentLeaveRequest>().GetQueryable()
                                .FirstOrDefaultAsync(x => 
                                x.deleted == false 
                                && x.studentId == item.studentId
                                && x.fromDate <= item.toDate 
                                && item.toDate <= x.toDate);

                var eTimeDup = await this.unitOfWork.Repository<tbl_StudentLeaveRequest>().GetQueryable()
                                .FirstOrDefaultAsync(x => 
                                x.deleted == false 
                                && x.studentId == item.studentId
                                && x.fromDate <= item.fromDate && item.fromDate <= x.toDate);

                if (eTimeDup != null && sTimeDup != null)
                {
                    //bị trùng đầu và cuối
                    sTimeDup.fromDate = eTimeDup.fromDate;
                    sTimeDup.description = item.description;
                    eTimeDup.deleted = true;
                    this.unitOfWork.Repository<tbl_StudentLeaveRequest>().Update(sTimeDup);
                    this.unitOfWork.Repository<tbl_StudentLeaveRequest>().Update(eTimeDup);
                }
                else if (sTimeDup != null)
                {
                    //update sTimeDup
                    sTimeDup.fromDate = item.fromDate;
                    sTimeDup.description = item.description;
                    this.unitOfWork.Repository<tbl_StudentLeaveRequest>().Update(sTimeDup);
                }
                else if (eTimeDup != null)
                {
                    //update eTimeDup
                    eTimeDup.toDate = item.toDate;
                    eTimeDup.description = item.description;
                    this.unitOfWork.Repository<tbl_StudentLeaveRequest>().Update(eTimeDup);
                }
                else
                {
                    await this.unitOfWork.Repository<tbl_StudentLeaveRequest>().CreateAsync(new tbl_StudentLeaveRequest
                    {
                        studentId = item.studentId,
                        classId = _class.id,
                        fromDate = item.fromDate,
                        toDate = item.toDate,
                        description = item.description
                    });
                }
            }
           
            await this.unitOfWork.SaveAsync();
        }

    }
}
