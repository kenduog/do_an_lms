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
using System.Net.Mail;
using Request.DomainRequests;
using Microsoft.Extensions.Configuration;
using static Utilities.CoreContants;
using System.Net;
using Request.RequestCreate;
using System.Net.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using Row = DocumentFormat.OpenXml.Spreadsheet.Row;
using Microsoft.AspNetCore.Http;
using Comment = Entities.Comment;
using Entities.DataTransferObject;
using OfficeOpenXml;
using ClosedXML.Excel;
using static Service.Services.ScaleMeasureDetailService;
using ExcelDataReader;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using static Service.Services.DateTimeConfigService;

namespace Service.Services
{
    public class StudentService : DomainService<tbl_Student, StudentSearch>, IStudentService
    {
        private readonly IParentService parentService;
        private readonly IBranchService branchService;
        private readonly IStudentInClassService studentInClassService;
        protected readonly IExcelExportService excelExportService;

        public StudentService(IServiceProvider serviceProvider, IAppUnitOfWork unitOfWork, IExcelExportService excelExportService, IMapper mapper) : base(unitOfWork, mapper)
        {
            this.parentService = serviceProvider.GetRequiredService<IParentService>();
            this.studentInClassService = serviceProvider.GetRequiredService<IStudentInClassService>();
            this.excelExportService = excelExportService;
            this.branchService = serviceProvider.GetRequiredService<IBranchService>();

        }

        //public override async Task<tbl_Student> GetByIdAsync(Guid id)
        //{
        //    var sqlParameters = new SqlParameter[]
        //    {
        //        new SqlParameter("@id", id)
        //    };
        //    var student = await this.unitOfWork.Repository<tbl_Student>()
        //        .ExcuteStoreAsync("Get_StudentInfo", sqlParameters);

        //    return student.SingleOrDefault();
        //}

        protected override string GetStoreProcName()
        {
            return "Get_Student";
        }
        public async Task<AppDomainResult> GetStudentsForArrange(ArrangeNewClassSearch request)
        {
            var result = new AppDomainResult { success = true, resultCode = (int)HttpStatusCode.OK };
            var students = this.unitOfWork.Repository<tbl_StudentInClass>().GetQueryable().Where(x => x.deleted == false);

            if (request.schoolYearId.HasValue)
                students = students.Where(x => x.schoolYearId == request.schoolYearId);

            if (request.gradeId.HasValue)
                students = students.Where(x => x.gradeId == request.gradeId);

            if (request.classId.HasValue)
                students = students.Where(x => x.classId == request.classId);

            result.data = await students.ToListAsync();
            return result;
        }

        public async Task<PagedList<tbl_Student>> AvailableStudent(AvailableStudentRequest request)
        {
            var result = new PagedList<tbl_Student>();

            var grade = await this.unitOfWork.Repository<tbl_Grade>().GetQueryable()
                               .FirstOrDefaultAsync(x => x.id == request.gradeId && x.deleted == false)
                               ?? throw new AppException(MessageContants.nf_grade);
            request.minYearOld = grade.studentYearOld;
            var lastGrade = await this.unitOfWork.Repository<tbl_Grade>().GetQueryable()
                               .FirstOrDefaultAsync(x => x.studentYearOld == grade.studentYearOld - 1 && x.deleted == false);
            if (lastGrade != null)
                request.gradeId = lastGrade.id;
            else
                request.gradeId = Guid.Empty;

            var schoolYear = await this.unitOfWork.Repository<tbl_SchoolYear>().GetQueryable()
                .FirstOrDefaultAsync(x => x.deleted == false && x.id == request.schoolYearId.Value)
                ?? throw new AppException(MessageContants.nf_schoolYear);

            var lastSchoolYear = await this.unitOfWork.Repository<tbl_SchoolYear>().GetQueryable()
                .FirstOrDefaultAsync(x => x.deleted == false
                ///&& x.startYear == schoolYear.startYear - 1 - khi nào code tới thì sửa nhe Đặng
                );
            if (lastSchoolYear != null)
                request.schoolYearId = lastSchoolYear.id;
            else
                request.schoolYearId = Guid.Empty;

            var sqlParameters = GetSqlParameters(request);
            result = await this.unitOfWork.Repository<tbl_Student>()
                .ExcuteQueryPagingAsync("Get_StudentAvailable", sqlParameters);
            result.pageSize = request.pageSize;
            result.pageIndex = request.pageIndex;
            return result;
        }

        public async Task<List<StudentSelection>> GetByParent(Guid parentId)
        {
            List<StudentSelection> result = new List<StudentSelection>();
            var parent = await this.unitOfWork.Repository<tbl_Parent>().GetQueryable().FirstOrDefaultAsync(x => x.deleted == false && x.id == parentId)
                ?? throw new AppException(MessageContants.nf_parent);

            var students = await this.unitOfWork.Repository<tbl_Student>().ExcuteStoreAsync("Get_StudentByParent", new SqlParameter[] { new SqlParameter("parentId", parentId) });

            if (students.Any())
            {
                result = students.Select(x => new StudentSelection
                {
                    id = x.id,
                    fullName = x.fullName,
                    thumbnail = x.thumbnail,
                    className = x.className,
                    classId = x.classId,
                    schoolYearId = x.schoolYearId,
                    branchId = x.branchId
                }).ToList();
            }
            return result;
        }
        public override async Task<tbl_Student> GetByIdAsync(Guid id)
        {
            var prs = new SqlParameter[] { new SqlParameter("id", id) };
            //custom response 
            var item = await this.unitOfWork.Repository<tbl_Student>().GetSingleRecordAsync("Get_StudentById", prs);
            return item;
        }
        public async Task<PagedList<tbl_Student>> GetStudentByGrade(GetStudentByGradeRequest request)
        {
            var result = new PagedList<tbl_Student>();

            var grade = await this.unitOfWork.Repository<tbl_Grade>().GetQueryable()
                               .FirstOrDefaultAsync(x => x.id == request.gradeId && x.deleted == false)
                               ?? throw new AppException(MessageContants.nf_grade);

            var schoolYear = await this.unitOfWork.Repository<tbl_SchoolYear>().GetQueryable()
                .FirstOrDefaultAsync(x => x.deleted == false && x.id == request.schoolYearId.Value)
                ?? throw new AppException(MessageContants.nf_schoolYear);

            var sqlParameters = GetSqlParameters(request);

            result = await this.unitOfWork.Repository<tbl_Student>()
                .ExcuteQueryPagingAsync("Get_StudentByGrade", sqlParameters);
            result.pageSize = request.pageSize;
            result.pageIndex = request.pageIndex;
            return result;
        }
        public async Task<SchoolReportCardModel> SchoolReportCard(Guid studentId)
        {
            SchoolReportCardModel result = new SchoolReportCardModel();
            // student
            var student = await this.unitOfWork.Repository<tbl_Student>().GetQueryable()
                               .FirstOrDefaultAsync(x => x.id == studentId && x.deleted == false)
                               ?? throw new AppException(MessageContants.nf_student);
            result.studentId = studentId;
            result.studentName = student.fullName;
            result.studentThumbnail = student.thumbnail;
            // class 
            var studentInClass = await this.unitOfWork.Repository<tbl_StudentInClass>().GetQueryable()
                   .OrderByDescending(x => x.created).FirstOrDefaultAsync(x => x.studentId == studentId && x.deleted == false)
                   ?? throw new AppException(MessageContants.nf_studentInClass);
            var classes = await this.unitOfWork.Repository<tbl_Class>().GetQueryable()
                               .FirstOrDefaultAsync(x => x.id == studentInClass.classId && x.deleted == false)
                               ?? throw new AppException(MessageContants.nf_class);
            result.classId = classes.id;
            result.className = classes.name;
            result.status = studentInClass.status;
            result.statusName = GetStudentInClassStatusName(studentInClass.status ?? 0);
            // khối
            var grade = await this.unitOfWork.Repository<tbl_Grade>().GetQueryable()
                   .FirstOrDefaultAsync(x => x.id == studentInClass.gradeId && x.deleted == false)
                   ?? throw new AppException(MessageContants.nf_grade);
            result.gradeId = grade.id;
            result.gradeName = grade.name;
            // điểm danh
            var attendance = await this.unitOfWork.Repository<tbl_Attendance>().GetQueryable()
               .Where(x => x.classId == classes.id && x.deleted == false && x.studentId == student.id).ToListAsync();
            result.attendance = new Attendance
            {
                total = attendance.Count,
                attendanced = attendance.Count(x => x.status == (int)AttendanceStatus.CoMat),
                dayOff = attendance.Count(x => x.status != (int)AttendanceStatus.CoMat),
                dayOffAllow = attendance.Count(x => x.status == (int)AttendanceStatus.VangPhep),
                dayOffNotAllow = attendance.Count(x => x.status == (int)AttendanceStatus.VangKhongPhep)
            };
            // nhận xét 7 ngày gần nhất
            var comment = await this.unitOfWork.Repository<tbl_Comment>().GetQueryable()
                .Where(x => x.classId == classes.id && x.deleted == false && x.studentId == student.id).OrderByDescending(x => x.date).Take(7).ToListAsync();
            result.comments = comment.Select(x => new Comment
            {
                id = x.id,
                afternoon = x.afternoon,
                afternoonSnack = x.afternoonSnack,
                date = x.date,
                dayComment = x.dayComment,
                hygiene = x.hygiene,
                lunch = x.lunch,
                sleep = x.sleep
            }).ToList();
            // phiếu bé ngoan
            var goodBehaviorCertificate = await this.unitOfWork.Repository<tbl_GoodBehaviorCertificate>().GetQueryable()
                .Where(x => x.classId == classes.id && x.deleted == false && x.studentId == student.id).ToListAsync();
            result.goodBehaviorCertificate = new GoodBehaviorCertificate
            {
                total = goodBehaviorCertificate.Count,
                receive = goodBehaviorCertificate.Count(x => x.status == true)
            };
            // đánh giá
            var childAssessmentTopics = await this.unitOfWork.Repository<tbl_ChildAssessmentTopic>().GetQueryable()
                .Where(x => x.gradeId == grade.id && x.deleted == false).ToListAsync();
            result.childAssessment = new List<ChildAssessmentTopic>();
            foreach (var topic in childAssessmentTopics)
            {
                ChildAssessmentTopic childAssessmentTopic = new ChildAssessmentTopic();
                childAssessmentTopic.id = topic.id;
                childAssessmentTopic.name = topic.name;
                var childAssessmentDetail = await this.unitOfWork.Repository<tbl_ChildAssessmentDetail>().GetQueryable()
                    .Where(x => x.childAssessmentId == topic.id && x.deleted == false).ToListAsync();
                childAssessmentTopic.details = new List<ChildAssessmentDetail>();
                foreach (var detail in childAssessmentDetail)
                {
                    tbl_StudentInAssessment studentInAssessment = await this.unitOfWork.Repository<tbl_StudentInAssessment>().GetQueryable()
                        .FirstOrDefaultAsync(x => x.studentId == student.id
                        && x.assessmentTopicId == topic.id
                        && x.assessmentDetailId == detail.id
                        && x.deleted == false);
                    childAssessmentTopic.details.Add(new ChildAssessmentDetail
                    {
                        id = detail.id,
                        name = detail.name,
                        status = studentInAssessment?.status,
                        statusName = GetStudentInAssessmentStatusName(studentInAssessment?.status)
                    });
                }
                result.childAssessment.Add(childAssessmentTopic);
            }
            return result;
        }
        public async Task<tbl_Student> UpdateStatus(Guid? studentId, int status)
        {
            var student = await this.unitOfWork.Repository<tbl_Student>().GetQueryable()
                        .FirstOrDefaultAsync(x => x.id == studentId);
            if (student != null)
            {
                student.status = status;
                this.unitOfWork.Repository<tbl_Student>().Update(student);
            }
            await this.unitOfWork.SaveAsync();
            return student;
        }
        private static bool IsExcelFile(IFormFile file)
        {
            string[] allowedExtensions = { ".xlsx", ".xls" };
            string extension = Path.GetExtension(file.FileName);
            return allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }


        public async Task<string> Export(StudentSearchExport baseSearch)
        {
            string url = "";
            string templateName = ExcelConstant.Export_Student;
            string folder = ExcelConstant.Export;

            var pars = GetSqlParameters(baseSearch);
            var data = await this.unitOfWork.Repository<tbl_Student>().GetDataExport("Get_Student_notPageIndex", pars);

            var dataToExportModels = mapper.Map<List<ItemStudentExport>>(data);
            List<ItemStudentExportWithClient> dataToExportWithClientModels = new List<ItemStudentExportWithClient>();
            foreach (var i in dataToExportModels)
            {
                string firstName = i.firstName;
                string lastName = i.lastName;
                string fullName = i.fullName;
                string nickname = i.nickname;
                string type = ConvertNumberToType(i.type);
                string gender = ConvertNumberToGender(i.gender);
                string ethnicity = i.ethnicity;
                string birthday = i.birthday.ToString(); // Chuyển đổi double thành chuỗi
                string placeOfBirth = i.placeOfBirth;
                string address = i.address;
                string hometown = i.hometown;
                string method = ConvertNumberToMethod(i.method);
                string enrollmentDate = i.enrollmentDate.ToString(); // Chuyển đổi double thành chuỗi
                string note = i.note;
                string startLearningDate = i.enrollmentDate.ToString();
                dataToExportWithClientModels.Add(ConvertItemStudentImportToExportWithClient(i));
            }
            ExcelPayload<ItemStudentExportWithClient> payload = new ExcelPayload<ItemStudentExportWithClient>()
            {
                items = dataToExportWithClientModels,
                templateName = templateName,
                folderToSave = folder,
                fromRow = 3,
            };

            url = excelExportService.Export(payload);
            return url;
        }


        public async Task<string> ExportTemplate()
        {
            string url = "";
            string templateName = ExcelConstant.Import_Student;
            string folder = ExcelConstant.Export;
            List<ItemStudentExportWithClient> dataToExportModels = new List<ItemStudentExportWithClient>();
            var data = GetGenderNameList().Select(x => new DropDown()
            {
                name = x.value
            }).ToList();
            List<ListDropDown> dropDowns = new List<ListDropDown>();
            dropDowns.Add(new ListDropDown()
            {
                columnName = "G",
                dataDropDown = data,
            });
            var dataType = GetTypeStudent().Select(x => new DropDown()
            {
                name = x.value
            }).ToList();
            dropDowns.Add(new ListDropDown()
            {
                columnName = "F",
                dataDropDown = dataType,
            });
            var dataMethod = GetMethodStudent().Select(x=> new DropDown()
            {
                name= x.value
            }).ToList();
            dropDowns.Add(new ListDropDown()
            {
                columnName = "M",
                dataDropDown = dataMethod,
            });
            ExcelPayload<ItemStudentExportWithClient> payload = new ExcelPayload<ItemStudentExportWithClient>()
            {
                items = dataToExportModels,
                templateName = templateName,
                folderToSave = folder,
                fromRow = 3,
            };
            url = excelExportService.ExportTemplate(payload, dropDowns);
            return url;
        }

        public async Task<List<ItemStudentImport>> Import(IFormFile excelFile)
        {
            return ReadExcelFile(excelFile);
        }

        private static List<ItemStudentImport> ReadExcelFile(IFormFile excelFile)
        {
            List<ItemStudentImport> items = new List<ItemStudentImport>();
            using (var stream = excelFile.OpenReadStream())
            {
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Giả sử dữ liệu ở sheet đầu tiên

                    int rowCount = worksheet.Dimension.Rows; // Số hàng có dữ liệu

                    // Lặp qua từng hàng để đọc dữ liệu
                    for (int row = 3; row <= rowCount; row++) // Bắt đầu từ hàng thứ 2 vì hàng đầu tiên thường là tiêu đề
                    {                      
                        ItemStudentImport item = new ItemStudentImport();
                        // Đọc dữ liệu từ các cột theo tên cột hoặc chỉ số cột                      
                        item.fullName = worksheet.Cells[row, 1].Value?.ToString(); //
                        item.firstName = ""; //
                        item.lastName = ""; //
                        if (!string.IsNullOrEmpty(item.fullName))
                        {
                            var names = item.fullName.Split(' ');
                            item.firstName = names[names.Length - 2];
                            item.lastName = item.fullName.Replace($" {item.firstName}", "");
                        }
                        item.nickname = worksheet.Cells[row, 2].Value?.ToString();
                        item.type = ConvertToTypeNumber(worksheet.Cells[row, 3].Value?.ToString()); // 
                        item.gender = ConvertGenderToNumber(worksheet.Cells[row, 4].Value?.ToString());
                        item.ethnicity = worksheet.Cells[row, 5].Value?.ToString();
                        item.birthday = StringToDoubleExcel(worksheet.Cells[row, 6].Value?.ToString(), "d/M/yyyy") ?? 0; //
                        item.placeOfBirth = worksheet.Cells[row, 7].Value?.ToString();
                        item.address = worksheet.Cells[row, 8].Value?.ToString();
                        item.hometown = worksheet.Cells[row, 9].Value?.ToString();
                        item.method = ConvertMethodToNumber(worksheet.Cells[row, 10].Value?.ToString());
                        item.enrollmentDate = StringToDoubleExcel(worksheet.Cells[row, 11].Value?.ToString(), "d/M/yyyy") ?? 0;
                        item.note = worksheet.Cells[row, 12].Value?.ToString();
                        item.startLearningDate = StringToDoubleExcel(worksheet.Cells[row, 13].Value?.ToString(), "d/M/yyyy") ?? 0;

                        if(string.IsNullOrEmpty(item.fullName))
                            throw new AppException("Vui lòng nhập đầy đủ thông tin họ tên");
                        if(item.type == null || item.type == 0)
                            throw new AppException("Vui lòng nhập đầy đủ thông tin loại");
                        if(item.birthday == null || item.birthday == 0)
                            throw new AppException("Vui lòng kiểm tra định dạng ngày sinh");
                        items.Add(item);

                    }
                }
            }
            return items;
        }       

        private ItemStudentExportWithClient ConvertItemStudentImportToExportWithClient(ItemStudentExport importItem)
        {
            var exportItem = new ItemStudentExportWithClient
            {
                firstName = importItem.firstName,
                lastName = importItem.lastName,
                fullName = importItem.fullName,
                nickname = importItem.nickname,
                type = ConvertNumberToType(importItem.type),
                gender = ConvertNumberToGender(importItem.gender),
                ethnicity = importItem.ethnicity,
                birthday = ConvertMillisecondsToDateString(importItem.birthday), // Chuyển đổi double thành chuỗi
                placeOfBirth = importItem.placeOfBirth,
                address = importItem.address,
                hometown = importItem.hometown,
                method = ConvertNumberToMethod(importItem.method),
                enrollmentDate = ConvertMillisecondsToDateString(importItem.enrollmentDate), // Chuyển đổi double thành chuỗi
                note = importItem.note,
                startLearningDate = ConvertMillisecondsToDateString(importItem.enrollmentDate)
            };
            return exportItem;
        }
        
    }
        
}


