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
using System.Net;
using Request.RequestCreate;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Azure.Core;

namespace Service.Services
{
    public class TranscriptService : DomainService<tbl_Transcript, TranscriptSearch>, ITranscriptService
    {
        public TranscriptService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        public override async Task Validate(tbl_Transcript model)
        {
            if (model.score.HasValue)
                if (model.score < 0 || model.score > 10)
                    throw new AppException(MessageContants.err_score);
        }
        public async Task<PagedList<tbl_Transcript>> GetOrGenerateTranscript(TranscriptSearch request)
        {
            PagedList<tbl_Transcript> pagedList = new PagedList<tbl_Transcript>();
            List<tbl_Transcript> transcript = new List<tbl_Transcript>();
            List<tbl_Transcript> result = new List<tbl_Transcript>();
            //lấy dữ liệu điểm danh của theo request
            transcript = this.unitOfWork.Repository<tbl_Transcript>().ExcuteStoreAsync("Get_Transcript", GetSqlParameters(request)).Result.ToList();
            var studentInClasses = this.unitOfWork.Repository<tbl_StudentInClass>().ExcuteStoreAsync("Get_StudentInClassForAttendance", GetSqlParameters(new
            {
                classId = request.classId,
                status = 1
            })).Result.ToList();
            if (!studentInClasses.Any())
            {
                pagedList.totalItem = 0;
                pagedList.items = null;
                pagedList.pageIndex = request.pageIndex;
                pagedList.pageSize = request.pageSize;
                return pagedList;
            }
            //nếu không có dữ liệu thì thêm mới dữ liệu điểm danh các học viên trong lớp vào tbl_Attendance
            if (!transcript.Any())
            {
                var classes = this.unitOfWork.Repository<tbl_Class>().GetQueryable()
                 .FirstOrDefault(x => x.id == (request.classId ?? Guid.Empty)) ?? throw new AppException(MessageContants.nf_class);
                var subject = this.unitOfWork.Repository<tbl_Subject>().GetQueryable()
                .FirstOrDefault(x => x.id == (request.subjectId ?? Guid.Empty)) ?? throw new AppException(MessageContants.nf_subject);

                if (studentInClasses.Any())
                {
                    transcript = studentInClasses.Select(x => new tbl_Transcript
                    {
                        classId = request.classId,
                        studentId = x.studentId,
                        studentName = x.fullName,
                        studentCode = x.code,
                        className = classes.name,
                        studentThumbnail = x.thumbnail,
                        type = request.type,
                        subjectName = subject.name,
                        schoolYearId = request.schoolYearId,
                        semesterId = request.semesterId,
                        subjectId = request.subjectId,
                        typeName = CoreContants.GetTranscriptTypeName(request.type ?? 0),

                    }).ToList();
                    await this.unitOfWork.Repository<tbl_Transcript>().CreateAsync(transcript);
                    await this.unitOfWork.SaveAsync();
                    result = transcript;
                    pagedList.totalItem = transcript.Count;
                    pagedList.items = result.Skip((request.pageIndex - 1) * request.pageSize).Take(request.pageSize).ToList();
                    pagedList.pageIndex = request.pageIndex;
                    pagedList.pageSize = request.pageSize;
                    return pagedList;
                }
            }

            result = transcript;
            pagedList.totalItem = result[0].totalItem;
            pagedList.items = result;
            pagedList.pageIndex = request.pageIndex;
            pagedList.pageSize = request.pageSize;
            await this.unitOfWork.SaveAsync();
            return pagedList;
        }
    }
}
