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
using static Service.Services.StatisticalService;
using System.Reflection;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;

namespace Service.Services
{
    public class PaymentSessionService : DomainService<tbl_PaymentSession, PaymentSessionSearch>, IPaymentSessionService
    {
        private readonly IAppDbContext appDbContext;
        public PaymentSessionService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext appDbContext) : base(unitOfWork, mapper)
        {
            this.appDbContext = appDbContext;
        }
        protected override string GetStoreProcName()
        {
            return "Get_PaymentSession";
        }

        public TimeV2 GetTimeModelV2(int? type, double? sTime, double? eTime)
        {
            DateTime timeNow = DateTime.Now;
            TimeV2 time = new TimeV2();
            switch (type)
            {
                //- - - lọc theo ngày - - -
                case 1:
                    // Thời gian bắt đầu của ngày (00:00:00)
                    DateTime startOfDay = timeNow.Date;
                    // Thời gian kết thúc của ngày (23:59:59)
                    DateTime endOfDay = timeNow.Date.AddDays(1).AddTicks(-1);

                    long startOfDayTimestamp = new DateTimeOffset(startOfDay).ToUnixTimeMilliseconds();
                    long endOfDayTimestamp = new DateTimeOffset(endOfDay).ToUnixTimeMilliseconds();

                    time.sTime = startOfDayTimestamp;
                    time.eTime = endOfDayTimestamp;
                    break;
                //- - - lọc theo tuần - - -
                case 2:
                    DayOfWeek currentDay = timeNow.DayOfWeek;
                    // Lấy ngày bắt đầu tuần (thứ hai)
                    int daysToSubtract = (int)currentDay - (int)DayOfWeek.Monday;
                    if (daysToSubtract < 0)
                    {
                        daysToSubtract += 7;
                    }
                    DateTime startOfWeek = timeNow.AddDays(-daysToSubtract);

                    // Lấy ngày kết thúc tuần (chủ nhật)
                    DateTime endOfWeek = startOfWeek.AddDays(6);

                    // Chuyển đổi sang Timestamp
                    long startOfWeekTimestamp = new DateTimeOffset(startOfWeek).ToUnixTimeMilliseconds();
                    long endOfWeekTimestamp = new DateTimeOffset(endOfWeek).ToUnixTimeMilliseconds();

                    time.sTime = startOfWeekTimestamp;
                    time.eTime = endOfWeekTimestamp;
                    break;
                //- - - lọc theo tháng - - -
                case 3:
                    // Lấy ngày đầu tiên của tháng hiện tại
                    DateTime startOfMonth = new DateTime(timeNow.Year, timeNow.Month, 1);

                    // Lấy ngày cuối cùng của tháng hiện tại
                    DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                    // Chuyển đổi sang Timestamp (Unix time)
                    long startOfMonthTimestamp = new DateTimeOffset(startOfMonth).ToUnixTimeMilliseconds();
                    long endOfMonthTimestamp = new DateTimeOffset(endOfMonth).ToUnixTimeMilliseconds();

                    time.sTime = startOfMonthTimestamp;
                    time.eTime = endOfMonthTimestamp;
                    break;
                //- - - lọc theo quý - - -
                case 4:
                    int month = timeNow.Month;

                    // Xác định quý hiện tại
                    int startMonth = ((month - 1) / 4) * 4 + 1;
                    int endMonth = startMonth + 3;

                    // Lấy ngày bắt đầu của quý hiện tại
                    DateTime startOfQuarter = new DateTime(timeNow.Year, startMonth, 1);

                    // Lấy ngày kết thúc của quý hiện tại
                    DateTime endOfQuarter = new DateTime(timeNow.Year, endMonth, DateTime.DaysInMonth(timeNow.Year, endMonth));

                    // Chuyển đổi sang Timestamp (Unix time)
                    long startOfQuarterTimestamp = new DateTimeOffset(startOfQuarter).ToUnixTimeMilliseconds();
                    long endOfQuarterTimestamp = new DateTimeOffset(endOfQuarter).ToUnixTimeMilliseconds();

                    time.sTime = startOfQuarterTimestamp;
                    time.eTime = endOfQuarterTimestamp;
                    break;
                //- - - lọc theo năm - - -
                case 5:
                    // Lấy ngày bắt đầu của năm hiện tại
                    DateTime startOfYear = new DateTime(timeNow.Year, 1, 1);

                    // Lấy ngày kết thúc của năm hiện tại
                    DateTime endOfYear = new DateTime(timeNow.Year, 12, 31);

                    // Chuyển đổi sang Timestamp (Unix time)
                    long startOfYearTimestamp = new DateTimeOffset(startOfYear).ToUnixTimeMilliseconds();
                    long endOfYearTimestamp = new DateTimeOffset(endOfYear).ToUnixTimeMilliseconds();
                    time.sTime = startOfYearTimestamp;
                    time.eTime = endOfYearTimestamp;
                    break;
                //- - - tùy chọn - - -
                case 6:
                    time.sTime = sTime ?? Timestamp.Now();
                    time.eTime = eTime ?? Timestamp.Now();
                    break;
            }
            return time;
        }

        public override async Task UpdateItem(tbl_PaymentSession model)
        {
            await Validate(model);

            var user = LoginContext.Instance.CurrentUser;
            
            var entity = await Queryable
                 .Where(e => e.id == model.id && e.deleted == false)
                 .FirstOrDefaultAsync();

            var bill = await unitOfWork.Repository<tbl_Bill>().GetQueryable()
                .SingleOrDefaultAsync(x => x.id == entity.billId && x.deleted == false);
            if (bill == null)
                throw new AppException("Không tìm thấy công nợ");
            if (bill.debt <= 0)
                throw new AppException("Học viên đã thanh toán hết");
            if (entity.value > bill.debt)
                throw new AppException("Số tiền không hợp lệ");

            if (entity != null)
            {
                foreach (PropertyInfo item_old in entity.GetType().GetProperties())
                {
                    foreach (PropertyInfo item_new in model.GetType().GetProperties())
                    {
                        if (item_old.Name == item_new.Name)
                        {
                            var value_old = item_old.GetValue(entity);
                            var value_new = item_new.GetValue(model);
                            if (value_old != value_new)
                            {
                                item_old.SetValue(entity, value_new ?? value_old);
                            }
                            break;
                        }
                    }
                }
                entity.updatedBy = LoginContext.Instance.CurrentUser == null ? entity.updatedBy : LoginContext.Instance.CurrentUser.userId;
                entity.updated = Timestamp.Now();

                //nếu đơn được duyệt
                if(entity.status == 2)
                {
                    //cập nhật lại thông tin bill
                    //entity.printContent = await GetPrintContent(entity.type ?? 0, entity.branchId, entity.studentId, entity.reason, entity.value ?? 0, LoginContext.Instance.CurrentUser.fullName);
                    entity.printContent = await GetPrintContent(entity, bill);
                    bill.paid += entity.value;
                    bill.debt -= entity.value;
                    //tiền nợ > 0 thì cập nhật bill này chưa thanh toán xong
                    if(bill.debt > 0)
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
                }
                //nếu đơn không duyệt
                if (entity.status == 3)
                {
                    //cập nhật lại thông tin bill
                    //tiền nợ > 0 thì cập nhật bill này chưa thanh toán xong
                    if (bill.debt > 0)
                    {
                        bill.status = 3;
                        bill.statusName = tbl_Bill.GetStatusName(3);
                        if(bill.debt == bill.totalFinalPrice)
                        {
                            bill.status = 1;
                            bill.statusName = tbl_Bill.GetStatusName(1);
                        }
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
                }

                unitOfWork.Repository<tbl_PaymentSession>().Update(entity);
            }
            else
            {
                throw new AppException("Item không tồn tại");
            }
            await unitOfWork.SaveAsync();
        }

        /// <summary>
        /// vì phiên thanh toán có 1 loại là gắn vào bill 1 loại không gắn vào bill nên tách ra thành v1 v2 để xử lý in hóa đơn
        /// </summary>
        /// <param name="type"></param>
        /// <param name="paymentSession"></param>
        /// <param name="bill"></param>
        /// <returns></returns>
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
            if (bill != null)
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

        public override async Task<PagedList<tbl_PaymentSession>> GetPagedListData(PaymentSessionSearch baseSearch)
        {
            if(baseSearch == null) baseSearch = new PaymentSessionSearch();
            var time = GetTimeModelV2(baseSearch.type, baseSearch.sTime, baseSearch.eTime);
            baseSearch.sTime = time.sTime;
            baseSearch.eTime = time.eTime;
            PagedList<tbl_PaymentSession> pagedList = new PagedList<tbl_PaymentSession>();
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            pagedList = await this.unitOfWork.Repository<tbl_PaymentSession>().ExcuteQueryPagingAsync(this.GetStoreProcName(), parameters);
            pagedList.pageIndex = baseSearch.pageIndex;
            pagedList.pageSize = baseSearch.pageSize;
            return pagedList;
        }

        /*public async Task<string> GetPrintContent(int type, Guid? branchId, Guid? studentId, string reason, double value, string createBy)
        {
            string result = "";
            var template = await this.unitOfWork.Repository<tbl_Template>().GetQueryable().FirstOrDefaultAsync(x => x.type == type);
            if (template != null)
                result = template.content;
            var fullName = "";
            var userCode = "";
            var student = await this.unitOfWork.Repository<tbl_Student>().GetQueryable().FirstOrDefaultAsync(x => x.id == studentId);
            if (student != null)
            {
                fullName = student.fullName;
                userCode = student.code;
            }
            var branchName = "";
            var branch = await this.unitOfWork.Repository<tbl_Branch>().GetQueryable().FirstOrDefaultAsync(x => x.id == branchId);
            if (branch != null)
            {
                branchName = branch.name;
            }
            result = result.Replace("{ChiNhanh}", branchName);
            result = result.Replace("{HoVaTen}", fullName);
            result = result.Replace("{MaHocVien}", userCode);
            result = result.Replace("{Ngay}", DateTime.Now.Day.ToString());
            result = result.Replace("{Thang}", DateTime.Now.Month.ToString());
            result = result.Replace("{Nam}", DateTime.Now.Year.ToString());
            result = result.Replace("{LyDo}", reason);
            result = result.Replace("{SoTienThu}", String.Format("{0:0,0}", value));
            result = result.Replace("{SoTienChi}", String.Format("{0:0,0}", value));
            result = result.Replace("{NguoiThu}", createBy);
            result = result.Replace("{NguoiChi}", createBy);
            return result;
        }*/

        /*public async Task<PaymentSessionPagedList> GetDataAndStatistical(PaymentSessionSearch baseSearch)
        {
            PaymentSessionPagedList pagedList = new PaymentSessionPagedList();
            var userLog = LoginContext.Instance.CurrentUser ?? throw new AppException(MessageContants.auth_expiried);
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            pagedList = await ExcuteQueryWithStatisticalPagingAsync(this.GetStoreProcName(), parameters);
            pagedList.pageIndex = baseSearch.pageIndex;
            pagedList.pageSize = baseSearch.pageSize;
            return pagedList;
        }

        public Task<PaymentSessionPagedList> ExcuteQueryWithStatisticalPagingAsync(string commandText, SqlParameter[] sqlParameters)
        {
            return Task.Run(() =>
            {
                PaymentSessionPagedList pagedList = new PaymentSessionPagedList();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    connection = (SqlConnection)appDbContext.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = commandText;
                    command.Parameters.AddRange(sqlParameters);
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    pagedList.items = MappingDataTable.ConvertToList<tbl_PaymentSession>(dataTable);
                    if (pagedList.items != null && pagedList.items.Any())
                    {
                        var item = pagedList.items.FirstOrDefault();
                        pagedList.totalItem = item.totalItem;
                        pagedList.totalRevenue = item.totalRevenue;
                        pagedList.totalProfit = item.totalProfit;
                        pagedList.totalExpense = item.totalExpense;
                        if (item.id == Guid.Empty)
                            pagedList.items = new List<tbl_PaymentSession>();
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
        }*/
    }
}
