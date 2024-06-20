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
using Microsoft.AspNetCore.Hosting;
using System.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages.Internal;
using System.Diagnostics.CodeAnalysis;
using Entities.DataTransferObject;
using Microsoft.Net.Http.Headers;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Entities.AuthEntities;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace Service.Services
{
    public class ScaleMeasureDetailService : DomainService<tbl_ScaleMeasureDetail, ScaleMeasureDetailSearch>, IScaleMeasureDetailService
    {
        private readonly IExcelExportService excelExportService;
        private readonly IDateTimeConfigService dateTimeConfigService;
        public ScaleMeasureDetailService(IServiceProvider serviceProvider, IAppUnitOfWork unitOfWork, IMapper mapper,
            IExcelExportService excelExportService, IDateTimeConfigService dateTimeConfigService) : base(unitOfWork, mapper)
        {
            this.excelExportService = excelExportService;
            this.dateTimeConfigService = dateTimeConfigService;
        }
        protected override string GetStoreProcName()
        {
            return "Get_ScaleMeasureDetail";
        }

        public override async Task Validate(tbl_ScaleMeasureDetail model)
        {
            if (model.scaleMeasureId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_ScaleMeasure>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.scaleMeasureId)
                    ?? throw new AppException(MessageContants.nf_scaleMeasure);
            }
            if (model.studentId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Student>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.studentId)
                    ?? throw new AppException(MessageContants.nf_student);
                var exsisted = await this.unitOfWork.Repository<tbl_ScaleMeasureDetail>().GetQueryable().AnyAsync(x => x.deleted == false && x.studentId == model.studentId
                && x.scaleMeasureId == model.scaleMeasureId);
                if (exsisted)
                    throw new AppException(MessageContants.exs_studentInScaleMeasure);
            }
        }
        public static DateTime UnixTimeStampToDateTime(double time)
        {
            // Chuyển đổi timestamp sang đối tượng DateTime
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
            return epoch.AddMilliseconds(time);
        }

        public async Task<string> ExportStudentList(Guid scaleMeasureId)
        {
            string url = "";
            string templateName = ExcelConstant.Export_ScaleMeasure;
            string folder = ExcelConstant.Export;
            var item = await this.unitOfWork.Repository<tbl_ScaleMeasure>().GetQueryable().FirstOrDefaultAsync(x => x.id == scaleMeasureId && x.deleted == false)
                ?? throw new AppException(MessageContants.nf_scaleMeasure);
            var pars = GetSqlParameters(new { scaleMeasureId = scaleMeasureId });
            var data = await this.unitOfWork.Repository<tbl_ScaleMeasureDetail>().GetDataExport("Get_ScaleMeasureDetailExport", pars);
            var dataToExportModels = mapper.Map<List<ScaleMeasureDetailExport>>(data);
            ExcelPayload<ScaleMeasureDetailExport> payload = new ExcelPayload<ScaleMeasureDetailExport>()
            {
                items = dataToExportModels,
                templateName = templateName,
                folderToSave = folder,
                fromRow = 4,
            };
            payload.keyValues = new Dictionary<ExcelIndex, string>
            {
                { new ExcelIndex(1,1), $"DANH SÁCH HỌC SINH - {item.name.ToUpper()}"},
                { new ExcelIndex(2,2), $"Ngày cân đo: {Timestamp.ToString(item.date, "dd/MM/yyyy")}"},
                { new ExcelIndex(2,5), $"Phạm vi: {CoreContants.GetScaleMeasureTypeName(item.type)}"},
            };
            url = excelExportService.Export(payload);
            return url;
        }

        public override async Task UpdateItem(tbl_ScaleMeasureDetail model)
        {
            await this.Validate(model);
            //model.bmi = BmiCalculator(model.height / 100, model.weight);
            //model.bmiResult = BmiEvalute(model.bmi);
            //model.weightMustHave = WeightMustHave(model.height / 100);
            await this.UpdateAsync(model);
            await this.unitOfWork.SaveAsync();
        }
        public override async Task<tbl_ScaleMeasureDetail> UpdateItemWithResponse(tbl_ScaleMeasureDetail model)
        {
            await this.Validate(model);
            await this.UpdateItem(model);
            await this.unitOfWork.SaveAsync();

            //custom response
            var data = await this.unitOfWork.Repository<tbl_ScaleMeasureDetail>().GetSingleRecordAsync("Get_ScaleMeasureDetailById", new SqlParameter[] { new SqlParameter("id", model.id) });
            return data;
        }

        public async Task<List<ScaleMeasureDetailExportWithClient>> Import(IFormFile excelFile)
        {

            var tokens = this.excelExportService.InitTokenHeader(new List<string>
            {
                "studentCode",
                "studentFullName",
                "studentFirstName",
                "studentLastName",
                "studentBirthDay",
                "monthOfAge",
                "studentGenderName",
                "gradeName",
                "className",
                "height",
                "weight",
                "evaluation",
            }, 1);
            var datas = this.excelExportService.FetchDataToModel<ScaleMeasureDetailExport>(excelFile, tokens, 3);
            var output = new List<ScaleMeasureDetailExportWithClient>();
            foreach (var i in datas)
            {
                output.Add(new ScaleMeasureDetailExportWithClient
                {
                    studentCode = i.studentCode,
                    studentFullName = i.studentFirstName,
                    studentFirstName = i.studentLastName,
                    studentLastName = i.studentFullName,
                    studentBirthDay = await dateTimeConfigService.StringToDouble(i.studentBirthDay.Split(" ")[0], "d/M/yyyy") ?? 0,
                    monthOfAge = i.monthOfAge,
                    studentGenderName = i.studentGenderName,
                    gradeName = i.gradeName,
                    className = i.className,
                    height = i.height,
                    weight = i.weight,
                    evaluation = i.evaluation,
                });
            }
            return output;
        }

        public async Task SaveList(List<ScaleMeasureDetailExport> datas, Guid? scaleMeasureId)
        {
            var item = await this.unitOfWork.Repository<tbl_ScaleMeasure>().GetQueryable().FirstOrDefaultAsync(x => x.id == scaleMeasureId && x.deleted == false)
               ?? throw new AppException(MessageContants.nf_scaleMeasure);
            List<tbl_ScaleMeasureDetail> oldDatas = new List<tbl_ScaleMeasureDetail>();

            if (datas.Any())
            {
                //init data
                oldDatas = await this.unitOfWork.Repository<tbl_ScaleMeasureDetail>().GetQueryable()
                    .Where(x => x.deleted == false && x.scaleMeasureId == scaleMeasureId && x.studentId.HasValue)
                    .ToListAsync();

                for (int i = 0; i < oldDatas.Count; i++)
                {
                    oldDatas[i].studentCode = datas[i].studentCode;
                    oldDatas[i].studentFullName = datas[i].studentFullName;
                    oldDatas[i].studentLastName = datas[i].studentLastName;
                    oldDatas[i].studentFirstName = datas[i].studentFirstName;
                    oldDatas[i].studentGender = ConvertGenderToNumber(datas[i].studentGenderName);
                    oldDatas[i].className = datas[i].className;
                    oldDatas[i].gradeName = datas[i].gradeName;
                    oldDatas[i].evaluation = datas[i].evaluation;
                }
                if (oldDatas.Any())
                {
                    var students = await this.unitOfWork.Repository<tbl_Student>().GetQueryable()
                        .Where(x => x.deleted == false && datas.Select(d => d.studentCode).Contains(x.code))
                        .ToDictionaryAsync(x => x.id, x => x.code);

                    var dict = datas.GroupBy(x => x.studentCode).ToDictionary(x => x.Key, x => new HeightAndWeight { height = x.FirstOrDefault().height, weight = x.FirstOrDefault().weight, evaluation = x.FirstOrDefault().evaluation });

                    //cập nhật lại chiều cao và cân nặng theo mã học viên
                    foreach (var oldData in oldDatas)
                    {
                        if (students.TryGetValue(oldData.studentId.Value, out string studentCode))
                        {
                            if (dict.TryGetValue(studentCode, out HeightAndWeight outValue))
                            {
                                oldData.height = outValue.height;
                                oldData.weight = outValue.weight;
                                oldData.evaluation = outValue.evaluation;

                                //oldData.bmi = BmiCalculator(outValue.height / 100, outValue.weight);
                                //oldData.bmiResult = BmiEvalute(oldData.bmi);
                                //oldData.weightMustHave = WeightMustHave(outValue.height / 100);
                            }
                        }
                        this.unitOfWork.Repository<tbl_ScaleMeasureDetail>().Update(oldData);
                    }
                }
            }
            //this.unitOfWork.Repository<tbl_ScaleMeasureDetail>().UpdateRange(oldDatas);
            await this.unitOfWork.SaveAsync();
        }






        private double? BmiCalculator(double? height, double? weight)
        {
            if (!height.HasValue || !weight.HasValue)
                return null;
            //kg / m^2
            return Math.Round(weight.Value / (height.Value * height.Value), 2);
        }

        private string BmiEvalute(double? bmiResult)
        {
            if (!bmiResult.HasValue)
                return null;

            switch (bmiResult)
            {
                case < 16:
                    return "Gầy độ 3";
                case < 17:
                    return "Gầy độ 2";
                case < 18.5:
                    return "Gầy độ 1";
                case < 25:
                    return "Bình thường";
                case < 30:
                    return "Thừa cân";
                case < 35:
                    return "Béo phì độ 1";
                case < 40:
                    return "Béo phì độ 2";
                default:
                    return "Béo phì độ 3";
            }
        }

        private double? WeightMustHave(double? height)
        {
            if (!height.HasValue)
                return null;
            //21.75 = w/h^2 => w = 21.75*h^2
            return Math.Round((double)(21.75 * height * height), 0);
        }

        public class HeightAndWeight
        {
            public double? height { get; set; }
            public double? weight { get; set; }
            public string evaluation { get; set; }
        }
        public async Task<List<ScaleMeasureDetailModel>> GetScaleMeasureDetailByStudent(Guid studentId)
        {
            List<ScaleMeasureDetailModel> result = new List<ScaleMeasureDetailModel>();
            var scaleMeasureDetail = this.unitOfWork.Repository<tbl_ScaleMeasureDetail>().ExcuteStoreAsync("Get_ScaleMeasureDetailByStudent", GetSqlParameters(new
            {
                studentId = studentId,
            })).Result.ToList();
            if (scaleMeasureDetail.Any())
                result = scaleMeasureDetail.Select(x => new ScaleMeasureDetailModel
                {
                    scaleMeasureId = x.scaleMeasureId,
                    scaleMeasureName = x.scaleMeasureName,
                    scaleMeasureDate = x.scaleMeasureDate,
                    height = x.height,
                    weight = x.weight,
                    bmi = x.bmi,
                    bmiResult = x.bmiResult,
                    weightMustHave = x.weightMustHave
                }).ToList();
            return result;
        }
        public async Task<string> ExportTemplate(ScaleMeasureDetailExportSearch baseSearch)
        {
            string url = "";
            string templateName = ExcelConstant.Export_ScaleMeasure;
            string folder = ExcelConstant.Export;

            ExcelPayload<ScaleMeasureDetailExport> payload = new ExcelPayload<ScaleMeasureDetailExport>()
            {
                templateName = templateName,
                folderToSave = folder,
                fromRow = 4,
            };

            var pars = GetSqlParameters(baseSearch);
            var data = await this.unitOfWork.Repository<tbl_ScaleMeasureDetail>().GetDataExport("Get_ScaleMeasureDetailExportDataWithClass", pars);
            var dataToExportModels = mapper.Map<List<ScaleMeasureDetailExport>>(data);
            payload.items = dataToExportModels;

            url = excelExportService.Export(payload);
            return url;
        }

        public async Task<string> Export(ScaleMeasureDetailExportSearch baseSearch)
        {
            string url = "";
            string templateName = ExcelConstant.Export_ScaleMeasure;
            string folder = ExcelConstant.Export;

            ExcelPayload<ScaleMeasureDetailExport> payload = new ExcelPayload<ScaleMeasureDetailExport>()
            {
                templateName = templateName,
                folderToSave = folder,
                fromRow = 4,
            };

            var pars = GetSqlParameters(baseSearch);
            var data = await this.unitOfWork.Repository<tbl_ScaleMeasureDetail>().GetDataExport("Get_ScaleMeasureDetailExportDataInLayout", pars);
            var dataToExportModels = mapper.Map<List<ScaleMeasureDetailExport>>(data);
            payload.items = dataToExportModels;

            url = excelExportService.Export(payload);
            return url;
        }
        private static int ConvertGenderToNumber(string gender)
        {
            switch (gender.ToLower()) // Chuyển đổi chuỗi đầu vào thành chữ thường và thực hiện kiểm tra
            {
                case "nam":
                    return 1;
                case "nữ":
                    return 2;
                default:
                    return 3;
            }
        }


    }
}
