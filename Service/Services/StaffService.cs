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
using System.Buffers.Text;
using Entities.AuthEntities;
using Entities.DataTransferObject;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.IO;
using Microsoft.Extensions.Hosting;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Hosting;
using static QRCoder.PayloadGenerator;
using System.Configuration;
using RestSharp.Extensions;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office.CustomUI;
using DropDown = Entities.DataTransferObject.DropDown;
using static Service.Services.DateTimeConfigService;

namespace Service.Services
{
    public class StaffService : DomainService<tbl_Staff, StaffSearch>, IStaffService
    {

        private readonly IUserService userService;
        protected readonly IExcelExportService excelExportService;
        private readonly IConfiguration configuration;
        private readonly IDepartmentService departmentService;
        private readonly IGroupService groupService;
        private readonly IDateTimeConfigService dateTimeConfigService;
        public StaffService(IAppUnitOfWork unitOfWork, IExcelExportService excelExportService,
            IMapper mapper, IConfiguration configuration, IDepartmentService departmentService,
            IGroupService groupService,
        IUserService userService, IDateTimeConfigService dateTimeConfigService) : base(unitOfWork, mapper)
        {
            this.userService = userService;
            this.excelExportService = excelExportService;
            this.configuration = configuration;
            this.departmentService = departmentService;
            this.groupService = groupService;
            this.dateTimeConfigService = dateTimeConfigService;
        }
        protected override string GetStoreProcName()
        {
            return "Get_Staff";
        }
        public async Task<Guid> GetDepartmentIdByName(string name)
        {
            var list = await departmentService.GetAllAsync();
            foreach (var i in list)
            {
                if (i.name.ToLower().Equals(name.ToLower()))
                {
                    return i.id;
                }
            }

            throw new ArgumentException("Don't Find Department Id.");
        }

        public async Task<Guid> GetGroupIdByName(string name)
        {
            var list = await groupService.GetAllAsync();
            foreach (var i in list)
            {

                if (i.name.ToLower().Equals(name.ToLower()))
                {
                    return i.id;
                }
            }

            throw new ArgumentException("Don't Find Group Id.");
        }

        public async Task<bool> GetConditionValueByGroupName(string s)
        {

            var list = await groupService.GetAllAsync();
            foreach (var i in list)
            {
                if (i.name.ToLower().Equals(s.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }


        public async Task<List<SubjectStaff>> GetSubjectByStaffId(Guid staffId)
        {
            var teachingAssignment = await unitOfWork.Repository<tbl_TeachingAssignment>()
           .GetQueryable()
           .Where(x => x.teacherId == staffId && x.deleted == false)
           .Select(x => x.subjectId.ToString()).ToListAsync();
            if (teachingAssignment.Any())
            {
                var listSubject = String.Join(",", teachingAssignment);

                var subject = await unitOfWork.Repository<tbl_Subject>()
                .GetQueryable()
                .Where(x => listSubject.Contains(x.id.ToString()) && x.deleted == false)
                .Select(x => new SubjectStaff()
                {
                    id = x.id,
                    name = x.name,
                }).ToListAsync();
                return subject;
            }
            else
                return new List<SubjectStaff>();
        }
        public async Task<string> Export(StaffSearch baseSearch)
        {
            string url = "";
            string templateName = ExcelConstant.Export_Staff;
            string folder = ExcelConstant.Export;
            var pars = GetSqlParameters(baseSearch);
            var data = await this.unitOfWork.Repository<tbl_Staff>().GetDataExport("Get_Staff_Export", pars);
            data.ForEach(x => x.itemBOD = dateTimeConfigService.DoubleToStringExcel(x.itemBOD, "dd/MM/yyyy").Result);
            var dataToExportModels = mapper.Map<List<ItemStaffExport>>(data);
            ExcelPayload<ItemStaffExport> payload = new ExcelPayload<ItemStaffExport>()
            {
                items = dataToExportModels,
                templateName = templateName,
                folderToSave = folder,
                fromRow = 3,
            };

            url = excelExportService.Export(payload);
            return url;
        }

        public async Task<string> ExportTeacher(StaffSearch baseSearch)
        {
            string url = "";
            string templateName = ExcelConstant.Export_Teacher;
            string folder = ExcelConstant.Export;
            var pars = GetSqlParameters(baseSearch);
            var data = await this.unitOfWork.Repository<tbl_Staff>().GetDataExport("Get_Staff_Export", pars);
            data.ForEach(x => x.itemBOD = dateTimeConfigService.DoubleToStringExcel(x.itemBOD, "dd/MM/yyyy").Result);

            var dataToExportModels = mapper.Map<List<ItemTeacherExport>>(data);
            ExcelPayload<ItemTeacherExport> payload = new ExcelPayload<ItemTeacherExport>()
            {
                items = dataToExportModels,
                templateName = templateName,
                folderToSave = folder,
                fromRow = 3,
            };

            url = excelExportService.Export(payload);
            return url;
        }

        public async Task<List<ItemTeacherImport>> ImportTecher(IFormFile file)
        {
            List<ItemTeacherImport> items = new List<ItemTeacherImport>();
            using (var stream = file.OpenReadStream())
            {
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Giả sử dữ liệu ở sheet đầu tiên

                    int rowCount = worksheet.Dimension.Rows; // Số hàng có dữ liệu

                    // Lặp qua từng hàng để đọc dữ liệu
                    for (int row = 3; row <= rowCount; row++) // Bắt đầu từ hàng thứ 2 vì hàng đầu tiên thường là tiêu đề
                    {
                        if (worksheet.Cells[row, 2].Value?.ToString() == null)
                        {
                            break;
                        }
                        var bod = StringToDoubleExcel(worksheet.Cells[row, 7].Value?.ToString(), "dd/MM/yyyy");
                        ItemTeacherImport item = new ItemTeacherImport
                        {
                            //STT	Mã nhân viên	Tên nhân viên	Tên tài khoản 	Số điện thoại	Email 	Ngày sinh	Quê quán 	Phòng ban 	Chức vụ 
                            // Đọc dữ liệu từ các cột theo tên cột hoặc chỉ số cột
                            itemCode = worksheet.Cells[row, 2].Value?.ToString(),// Cột B
                            itemName = worksheet.Cells[row, 3].Value?.ToString(),// Cột C
                            itemUserName = worksheet.Cells[row, 4].Value?.ToString(),// Cột D
                            itemPhone = worksheet.Cells[row, 5].Value?.ToString(),// Column E
                            itemEmail = worksheet.Cells[row, 6].Value?.ToString(),// Column F
                            itemBOD = bod,// Column G ---
                            itemCity = worksheet.Cells[row, 8].Value?.ToString(),// Column H ---
                            itemDepartmentName = worksheet.Cells[row, 9].Value?.ToString(),// Column I
                            itemSubject = worksheet.Cells[row, 10].Value?.ToString(),// Column J
                            itemGroup = "Giáo Viên",//worksheet.Cells[row, 11].Value?.ToString(),
                            itemStatus = worksheet.Cells[row, 12].Value?.ToString(),
                            itemMajor = worksheet.Cells[row, 13].Value?.ToString(),
                            itemCertificate = worksheet.Cells[row, 14].Value?.ToString(),
                            itemBranchName = worksheet.Cells[row, 15].Value?.ToString(),
                            itemGroupUser = worksheet.Cells[row, 16].Value?.ToString()


                        };

                        items.Add(item);


                    }
                }
            }
            return items;
        }

        public async Task<List<ItemStaffImport>> ImportStaff(IFormFile file)
        {
            List<ItemStaffImport> items = new List<ItemStaffImport>();
            using (var stream = file.OpenReadStream())
            {
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Giả sử dữ liệu ở sheet đầu tiên

                    int rowCount = worksheet.Dimension.Rows; // Số hàng có dữ liệu

                    // Lặp qua từng hàng để đọc dữ liệu
                    for (int row = 3; row <= rowCount; row++) // Bắt đầu từ hàng thứ 2 vì hàng đầu tiên thường là tiêu đề
                    {
                        if (worksheet.Cells[row, 2].Value?.ToString() == null)
                        {
                            break;
                        }
                        var bod = StringToDoubleExcel(worksheet.Cells[row, 7].Value?.ToString(), "dd/MM/yyyy");
                        ItemStaffImport item = new ItemStaffImport
                        {
                            itemCode = worksheet.Cells[row, 2].Value?.ToString(),// Cột B
                            itemName = worksheet.Cells[row, 3].Value?.ToString(),// Cột C
                            itemUserName = worksheet.Cells[row, 4].Value?.ToString(),// Cột D
                            itemPhone = worksheet.Cells[row, 5].Value?.ToString(),// Column E
                            itemEmail = worksheet.Cells[row, 6].Value?.ToString(),// Column F
                            itemBOD = bod,// Column G ---
                            itemCity = worksheet.Cells[row, 8].Value?.ToString(),// Column H ---
                            itemDepartmentName = worksheet.Cells[row, 9].Value?.ToString(),// Column I
                            //itemGroup = worksheet.Cells[row, 10].Value?.ToString(),// Column J
                            itemStatus = "Hoạt động",
                            itemMajor = worksheet.Cells[row, 10].Value?.ToString(),
                            itemCertificate = worksheet.Cells[row, 11].Value?.ToString(),
                            itemBranchName = worksheet.Cells[row, 12].Value?.ToString(),
                            itemGroupUser = worksheet.Cells[row, 13].Value?.ToString()
                        };

                        items.Add(item);


                    }
                }
            }
            return items;

        }
        public async Task<string> ExportStaffTemplate()
        {
            string url = "";
            string templateName = ExcelConstant.Import_Staff;
            string folder = ExcelConstant.Export;
            List<ItemStaffExportTemplete> dataToImportModels = new List<ItemStaffExportTemplete>();
            dataToImportModels.Add(new ItemStaffExportTemplete()
            {
                itemCode = "NV-0001",
                itemName = "Nguyễn Văn A",
                itemUserName = "nguyenvana01",
                itemPhone = "0900000000",
                itemEmail = "nguyenvana@gmail.com",
                itemBOD = "31/12/1990",
                itemCity = "Hồ Chí Minh",
                itemDepartmentName = "Phòng Hành Chính",
                itemMajor = "Kỹ thuật phần mềm",
                itemCertificate = "Đại học",
                itemBranchName = "Mần Non Đức Trí",
                itemGroupUser = "Nhân viên văn phòng",
            });
            ExcelPayload<ItemStaffExportTemplete> payload = new ExcelPayload<ItemStaffExportTemplete>()
            {
                items = dataToImportModels,
                templateName = templateName,
                folderToSave = folder,
                fromRow = 3,
            };
            // phòng ban
            var department = await departmentService.GetAllAsync();
            var dataDepartment = department.Where(x => x.deleted == false).Select(x => new DropDown()
            {
                id = x.id,
                name = x.name,
            }).ToList();
            // chức vụ
            var group = await groupService.GetAllAsync();
            var data = group.Where(x => x.deleted == false && x.userCanEdit == true).Select(x => new DropDown()
            {
                id = x.id,
                name = x.name,
            }).ToList();
            List<ListDropDown> dropDowns = new List<ListDropDown>();
            dropDowns.Add(new ListDropDown()
            {
                columnName = "M",
                dataDropDown = data,
            });
            dropDowns.Add(new ListDropDown()
            {
                columnName = "I",
                dataDropDown = dataDepartment,
            });
            url = excelExportService.ExportTemplate(payload, dropDowns);
            return url;
        }

        public async Task<string> ExportTeacherTemplate()
        {
            string url = "";
            string templateName = ExcelConstant.Import_Teacher;
            string folder = ExcelConstant.Export;
            List<ItemTeacherExportTemplete> dataToImportModels = new List<ItemTeacherExportTemplete>();
            dataToImportModels.Add(new ItemTeacherExportTemplete()
            {
                itemCode = "GV-0001",
                itemName = "Nguyễn Văn A",
                itemUserName = "nguyenvana01",
                itemPhone = "0900000000",
                itemEmail = "nguyenvana@gmail.com",
                itemBOD = "31/12/1990",
                itemCity = "Hồ Chí Minh",
                itemDepartmentName = "Phòng Hành Chính",
                itemMajor = "Kỹ thuật phần mềm",
                itemCertificate = "Đại học",
                itemBranchName = "Mần Non Đức Trí",
                itemGroupUser = "Nhân viên văn phòng",
                itemGroup = "",
                itemStatus = "Hoạt Động",
                itemSubject = "Toán",
                

            }); 
            ExcelPayload<ItemTeacherExportTemplete> payload = new ExcelPayload<ItemTeacherExportTemplete>()
            {
                items = dataToImportModels,
                templateName = templateName,
                folderToSave = folder,
                fromRow = 3,
            };
            // phòng ban
            var department = await departmentService.GetAllAsync();
            var dataDepartment = department.Where(x => x.deleted == false).Select(x => new DropDown()
            {
                id = x.id,
                name = x.name,
            }).ToList();
            List<ListDropDown> dropDowns = new List<ListDropDown>();
            dropDowns.Add(new ListDropDown()
            {
                columnName = "I",
                dataDropDown = dataDepartment,
            });
            url = excelExportService.ExportTemplate(payload, dropDowns);
            return url;
        }
    }
}

