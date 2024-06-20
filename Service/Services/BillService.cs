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
using Entities.AuthEntities;
using Request.RequestUpdate;
using DocumentFormat.OpenXml.Wordprocessing;
using Azure.Core;
using System.Reflection;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using static Utilities.CoreContants;
using static Service.Services.DateTimeConfigService;
using Entities.DataTransferObject;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace Service.Services
{
    public class BillService : DomainService<tbl_Bill, BillSearch>, IBillService
    {
        private readonly IAppDbContext appDbContext;
        private readonly IAutoGenCodeConfigService autoGenCodeConfigService;
        private readonly IExcelExportService excelExportService;
        public BillService(IServiceProvider serviceProvider,
            IAppUnitOfWork unitOfWork, 
            IMapper mapper, 
            IAppDbContext appDbContext,
            IAutoGenCodeConfigService autoGenCodeConfigService
            ) : base(unitOfWork, mapper)
        {
            this.excelExportService = serviceProvider.GetRequiredService<IExcelExportService>();
            this.appDbContext = appDbContext;
            this.autoGenCodeConfigService = autoGenCodeConfigService;
        }
        protected override string GetStoreProcName()
        {
            return "Get_Bill";
        }

        public override async Task<PagedList<tbl_Bill>> GetPagedListData(BillSearch request)
        {
            var result = new PagedList<tbl_Bill>();

            var sqlParameters = GetSqlParameters(request);

            result = await this.unitOfWork.Repository<tbl_Bill>()
                .ExcuteQueryPagingAsync("Get_Bill", sqlParameters);
            result.pageSize = request.pageSize;
            result.pageIndex = request.pageIndex;
            return result;
        }

        public async Task<List<ClassByGradeModel>> GetClassByGrade(ClassInBillSearch request)
        {
            var result = new List<ClassByGradeModel>();

            var gradeIdList = request.gradeIds.Split(',').Select(Guid.Parse).ToList();

            var listClass = await this.unitOfWork.Repository<tbl_Class>().GetQueryable().Where(x => x.deleted == false && gradeIdList.Contains(x.gradeId.Value) && x.schoolYearId == request.schoolYearId).ToListAsync();

            result = listClass.Select(x => new ClassByGradeModel
            {
                id = x.id,
                name = x.name

            }).ToList();

            return result;
        }

        public async Task<PagedList<StudentByClassModel>> GetStudentByClass(FilterInBillSearch request)
        {
            var result = new PagedList<StudentByClassModel>();
            var dataInSQL = new PagedList<tbl_StudentInClass>();

            var sqlParameters = GetSqlParameters(request);

            dataInSQL = await this.unitOfWork.Repository<tbl_StudentInClass>()
                .ExcuteQueryPagingAsync("Get_StudentByClass", sqlParameters);
            result.items = dataInSQL.items.Select(x => new StudentByClassModel
            {
                id = x.id,
                code = x.code,
                fullName = x.fullName,
                thumbnail = x.thumbnail,
                className = x.className,
                gradeName = x.gradeName
            }).ToList();
            result.totalItem = dataInSQL.totalItem;
            result.pageSize = request.pageSize;
            result.pageIndex = request.pageIndex;
            return result;
        }

        public async Task<PagedList<tbl_BillDetail>> GetDetail(BillDetailSearch request)
        {
            var result = new PagedList<tbl_BillDetail>();

            var sqlParameters = GetSqlParameters(request);

            result = await this.unitOfWork.Repository<tbl_BillDetail>()
                .ExcuteQueryPagingAsync("Get_BillDetail", sqlParameters);
            result.pageSize = request.pageSize;
            result.pageIndex = request.pageIndex;
            return result;
        }

        public async Task<PagedList<tbl_PaymentSession>> GetPaymentSession(PaymentSessionSearch request)
        {
            var result = new PagedList<tbl_PaymentSession>();

            var sqlParameters = GetSqlParameters(request);

            result = await this.unitOfWork.Repository<tbl_PaymentSession>()
                .ExcuteQueryPagingAsync("Get_PaymentSession", sqlParameters);
            result.pageSize = request.pageSize;
            result.pageIndex = request.pageIndex;
            return result;
        }

        public async Task Payment(PaymentsRequest itemModel)
        {
            using (var tran = await appDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = LoginContext.Instance.CurrentUser;
                    var bill = await unitOfWork.Repository<tbl_Bill>().GetQueryable()
                        .SingleOrDefaultAsync(x => x.id == itemModel.billId && x.deleted == false);
                    if (bill == null)
                        throw new AppException("Không tìm thấy công nợ");
                    if (bill.debt <= 0)
                        throw new AppException("Học viên đã thanh toán hết");
                    if(itemModel.value <= 0)
                        throw new AppException("Vui lòng nhập số tiền");
                    if (itemModel.value > bill.debt)
                        throw new AppException("Số tiền không hợp lệ");

                    //lưu lại phiên thanh toán
                    var paymentSession = new tbl_PaymentSession();
                    paymentSession.billId = itemModel.billId;
                    paymentSession.paymentDate = itemModel.paymentDate;
                    paymentSession.code = await autoGenCodeConfigService.AutoGenCode(nameof(tbl_PaymentSession));
                    paymentSession.studentId = bill.studentId;
                    paymentSession.value = itemModel.value;
                    paymentSession.type = itemModel.type;
                    paymentSession.typeName = itemModel.typeName;
                    paymentSession.note = itemModel.note;
                    paymentSession.branchId = bill.branchId;
                    paymentSession.active = true;
                    paymentSession.reason = $"Thanh toán công nợ [{bill.code}]";
                    paymentSession.paymentMethodId = itemModel.paymentMethodId;
                    paymentSession.status = 2;
                    paymentSession.statusName = tbl_PaymentSession.GetStatusName(2);
                    await unitOfWork.Repository<tbl_PaymentSession>().CreateAsync(paymentSession);
                    await unitOfWork.SaveAsync();
                    paymentSession.printContent = await GetPrintContent(paymentSession, bill);
                    unitOfWork.Repository<tbl_PaymentSession>().Update(paymentSession);
                    await unitOfWork.SaveAsync();

                    //cập nhật lại thông tin bill

                    bill.paid += itemModel.value;               
                    bill.debt -= itemModel.value;
                    //tiền nợ > 0 thì cập nhật bill này chưa thanh toán hết
                    if (bill.debt > 0)
                    {
                        bill.status = 3;
                        bill.statusName = tbl_Bill.GetStatusName(3);
                    }
                    //tiền nợ <= 0 thì cập nhật bill này đã thanh toán
                    else
                    {
                        bill.status = 4;
                        bill.statusName = tbl_Bill.GetStatusName(4);
                    }
                    bill.paymentDate = Timestamp.Now();
                    unitOfWork.Repository<tbl_Bill>().Update(bill);
                    await unitOfWork.SaveAsync();                  
                    await tran.CommitAsync();
                }
                catch (AppException e)
                {
                    await tran.RollbackAsync();
                    throw e;
                }
            }
        }
        public async Task<string> GetPrintContent(tbl_PaymentSession paymentSession, tbl_Bill bill)
        {
            string result = "";
            var template = await this.unitOfWork.Repository<tbl_Template>().GetQueryable().FirstOrDefaultAsync(x => x.type == paymentSession.type);
            if (template != null)
                result = template.content;          

            var fullName = "";          
            var student = await this.unitOfWork.Repository<tbl_Student>().GetQueryable().FirstOrDefaultAsync(x => x.id == paymentSession.studentId);
            if (student != null)
            {
                fullName = student.fullName;
            }

            var parent = await this.unitOfWork.Repository<tbl_Parent>().GetQueryable().FirstOrDefaultAsync(x => x.id == student.fatherId);
            var mobile = "";
            if (parent != null)
            {
                mobile = parent.phone;
            }

            var branchName = "";
            var branchLogo = "";
            var branch = await this.unitOfWork.Repository<tbl_Branch>().GetQueryable().FirstOrDefaultAsync(x => x.id == paymentSession.branchId);
            if (branch != null)
            {
                branchName = branch.name;
                branchLogo = branch.logoInBill;
            }

            result = result.Replace("{Logo}", branchLogo);
            result = result.Replace("{BranchName}", branchName);
            result = result.Replace("{ContractCode}", paymentSession.code);
            result = result.Replace("{PaymentDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            result = result.Replace("{FullName}", fullName);
            result = result.Replace("{Mobile}", mobile);
            result = result.Replace("{Day}", DateTime.Now.Day.ToString());
            result = result.Replace("{Month}", DateTime.Now.Month.ToString());
            result = result.Replace("{Year}", DateTime.Now.Year.ToString());

            //nếu phiếu này có thông tin bill
            if(bill != null)
            {
                result = result.Replace("{Note}", bill.note);
                result = result.Replace("{TotalFinalPrice}", String.Format("{0:0,0}", bill.totalFinalPrice));
                result = result.Replace("{Note}", bill.note);
                var totalPriceString = NumberToWords((long)bill.totalFinalPrice);
                result = result.Replace("{TotalPriceString}", totalPriceString);
                result = result.Replace("{Paid}", String.Format("{0:0,0}", paymentSession.value));
                result = result.Replace("{Surplus}", "0");

                var listBillDetail = await this.unitOfWork.Repository<tbl_BillDetail>().GetQueryable().Where(x => x.deleted == false && x.billId == bill.id).ToListAsync();
                var finalContent = "";
                var content = @"
                <tr style=""height: 20.7292px;"">
                    <td style=""width: 7.19194%; height: 20.7292px; text-align: center;""><span style=""font-size: 16pt;"">{STT}</span></td>
                    <td style=""width: 37.2063%; height: 20.7292px; text-align: left;""><span style=""font-size: 16pt;"">{TuitionConfig}</span></td>
                    <td style=""width: 10.9318%; height: 20.7292px; text-align: center;""><span style=""font-size: 16pt;"">{Unit}</span></td>
                    <td style=""width: 5.46588%; height: 20.7292px; text-align: center;""><span style=""font-size: 16pt;"">{Quantity}</span></td>
                    <td style=""width: 14.3359%; height: 20.7292px; text-align: center;""><span style=""font-size: 16pt;"">{Price}</span></td>
                    <td style=""width: 8.87007%; height: 20.7292px; text-align: center;""><span style=""font-size: 16pt;"">{Discount}</span></td>
                    <td style=""width: 16.062%; height: 20.7292px; text-align: center;""><span style=""font-size: 16pt;"">{TotalPrice}</span></td>
                </tr>";

                

                var stt = 1;
                foreach (var billDetail in listBillDetail)
                {
                    var itemContent = content;
                    var tuitionConfigName = "";
                    var unit = "";
                    var tuitionConfig = await this.unitOfWork.Repository<tbl_TuitionConfig>().GetQueryable().FirstOrDefaultAsync(x => x.id == billDetail.tuitionConfigId);
                    if (tuitionConfig != null)
                    {
                        tuitionConfigName = tuitionConfig.name;
                        unit = tuitionConfig.unit;
                    }
                    itemContent = itemContent.Replace("{STT}", stt.ToString());
                    itemContent = itemContent.Replace("{TuitionConfig}", tuitionConfigName);
                    itemContent = itemContent.Replace("{Unit}", unit);
                    itemContent = itemContent.Replace("{Quantity}", billDetail.quantity.ToString());
                    itemContent = itemContent.Replace("{Price}", String.Format("{0:0,0}", billDetail.price));
                    itemContent = itemContent.Replace("{Discount}", String.Format("{0:0,0}", billDetail.totalReduced));
                    itemContent = itemContent.Replace("{TotalPrice}", String.Format("{0:0,0}", billDetail.totalFinalPrice));
                    finalContent += itemContent;
                    stt++;
                }
                result = result.Replace("{BillDetail}", finalContent);
            }
            else
            {
                var reason = @"<span style=""font-size: 14pt;""><span style=""font-size: 14pt;"">Lý do thanh toán: {Reason}</span>";
                result = result.Replace("{Note}", paymentSession.note);
                result = result.Replace("{TotalFinalPrice}", String.Format("{0:0,0}", paymentSession.value));
                result = result.Replace("{Note}", paymentSession.note);
                var totalPriceString = NumberToWords((long)paymentSession.value);
                result = result.Replace("{TotalPriceString}", totalPriceString);
                result = result.Replace("{Paid}", String.Format("{0:0,0}", paymentSession.value));
                result = result.Replace("{Surplus}", "0");
                reason = reason.Replace("{Reason}", paymentSession.reason);
                //nếu không có bill => không có sản phẩm => xóa cái table đi
                string pattern = @"<table.*?</table>";
                //ở đây đang thực thiện xóa table bằng cách replace cái reason vào vị trí cái table đó
                result = Regex.Replace(result, pattern, reason, RegexOptions.Singleline);
            }
            return result;
        }
        #region hàm chuyển đổi tiền sang chữ

        private static readonly string[] Units = { "", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        private static readonly string[] PlaceValues = { "", "nghìn", "triệu", "tỷ" };

        public static string NumberToWords(long number)
        {
            if (number == 0)
                return "không đồng";

            string result = "";
            int placeValue = 0;

            do
            {
                int n = (int)(number % 1000);
                if (n != 0)
                {
                    string str = NumberToWordsLessThanOneThousand(n, placeValue > 0 && result.Length > 0);
                    result = str + " " + PlaceValues[placeValue] + " " + result;
                }
                placeValue++;
                number /= 1000;
            } while (number > 0);

            return result.Trim() + " đồng";
        }

        private static string NumberToWordsLessThanOneThousand(int number, bool addLe)
        {
            string result = "";

            int hundreds = number / 100;
            number %= 100;

            if (hundreds > 0)
            {
                result += Units[hundreds] + " trăm";
            }

            int tens = number / 10;
            int ones = number % 10;

            if (tens > 0)
            {
                if (tens == 1)
                {
                    result += " mười";
                }
                else
                {
                    result += " " + Units[tens] + " mươi";
                }

                if (ones > 0)
                {
                    if (ones == 1)
                    {
                        result += " mốt";
                    }
                    else if (ones == 5)
                    {
                        result += " lăm";
                    }
                    else
                    {
                        result += " " + Units[ones];
                    }
                }
            }
            else if (ones > 0)
            {
                if (hundreds > 0 || addLe)
                {
                    result += " lẻ " + Units[ones];
                }
                else
                {
                    result += Units[ones];
                }
            }

            return result.Trim();
        }
        #endregion
    }
    /*public async Task<string> ExportTemplate()
    {
        string url = "";
        string templateName = ExcelConstant.Import_Student_In_Bill;
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
        var dataMethod = GetMethodStudent().Select(x => new DropDown()
        {
            name = x.value
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

    //import học viên khi tạo đợt thu
    public async Task<List<ItemStudentImport>> PrepareData(IFormFile excelFile)
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

                    if (string.IsNullOrEmpty(item.fullName))
                        throw new AppException("Vui lòng nhập đầy đủ thông tin họ tên");
                    if (item.type == null || item.type == 0)
                        throw new AppException("Vui lòng nhập đầy đủ thông tin loại");
                    if (item.birthday == null || item.birthday == 0)
                        throw new AppException("Vui lòng kiểm tra định dạng ngày sinh");
                    items.Add(item);
                }
            }
        }
        return items;
    }*/

    /*/// <summary>
    /// cập nhật lại các khoản tiền của các bill
    /// </summary>
    /// <returns></returns>
    public async Task UpdateInfor()
    {
        var bills = await unitOfWork.Repository<tbl_Bill>().GetQueryable().Where(x => x.deleted == false).ToListAsync();
        if(bills.Count > 0)
        {
            foreach(var item in bills)
            {
                var billDetails = await unitOfWork.Repository<tbl_BillDetail>().GetQueryable().Where(x => x.deleted == false && x.billId == item.id).ToListAsync();
                if (billDetails.Count > 0)
                {
                    double totalPrice = 0;
                    double totalReduced = 0;
                    foreach (var detail in itemModel.billDetails)
                    {
                        var tuitionConfig = await tuitionConfigService.GetByIdAsync(detail.tuitionConfigId);
                        if (tuitionConfig == null)
                            throw new AppException("Không tìm thấy khoản thu");

                        //tổng tiền của khoản thu này bằng tiền khoản thu * số lượng
                        var totalPriceItem = (tuitionConfig.price ?? 0) * detail.quantity;
                        totalPrice += totalPriceItem;

                        //nếu giảm tiền mặt thì cộng thẳng vào
                        if (detail.discountId != null)
                        {
                            var discount = await discountService.GetByIdAsync(detail.discountId.Value);
                            if (discount == null)
                                throw new AppException("Không tìm thấy giảm giá");
                            if (discount.type == 1)
                            {
                                totalReduced += ((discount.value ?? 0) * detail.quantity);
                            }
                            //nếu giảm % thì kiểm tra xem số tiền giảm vượt qua max chưa nếu vượt qua thì gán bằng max
                            else
                            {
                                double reduced = (tuitionConfig.price ?? 0) * ((discount.value ?? 0) / 100);
                                if (reduced > discount.maximumValue)
                                {
                                    reduced = (discount.maximumValue ?? 0);
                                }
                                totalReduced += (reduced * detail.quantity);
                            }
                        }

                    }
                }
            }
        }

    }*/
}
