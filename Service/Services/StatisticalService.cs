using AutoMapper;
using Entities.DataTransferObject;
using Entities.Search;
using Entities;
using Extensions;
using Interface.Services;
using Interface.UnitOfWork;
using Service.Services.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Interface.Services.DomainServices;
using Entities.Model.Statistical;
using Interface.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Service.Services
{
    public class StatisticalService : IStatisticalService
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IMapper mapper;
        public StatisticalService(IAppUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        #region chuẩn bị data
        public class Time
        {
            public int Month { get; set; }
            public int Year { get; set; }
            public int LastMonth { get; set; }
            public int YearOfLastMonth { get; set; }
            public int LastYear { get; set; }
            public int Day { get; set; }
        }

        public static Time GetTimeModel(int? month, int? year)
        {
            DateTime timeNow = DateTime.Now;
            Time time = new Time();
            time.Month = month ?? DateTime.Now.Month;
            time.Year = year ?? DateTime.Now.Year;
            time.LastMonth = time.Month - 1 == 0 ? 12 : time.Month - 1;
            time.YearOfLastMonth = time.LastMonth == 12 ? time.Year - 1 : time.Year;
            time.LastYear = time.Year - 1;
            time.Day = timeNow.Day;
            return time;
        }

        public class TimeV2
        {
            public double sTime { get; set; }
            public double eTime { get; set; }
        }

        public static TimeV2 GetTimeModelV2(int? type, double? sTime, double? eTime)
        {
            DateTime timeNow = DateTime.Now;            
            TimeV2 time = new TimeV2();
            switch (type)
            {
                //- - - lọc theo ngày - - -
                case 1:
                    time.sTime = Timestamp.Now();
                    time.eTime = Timestamp.Now();
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
        public class CompareModel
        {
            /// <summary>
            /// số lượng chênh lệch
            /// </summary>
            public double? DifferenceQuantity { get; set; }
            /// <summary>
            /// tỷ lệ chênh lệch
            /// </summary>
            public double? DifferenceValue { get; set; }
            /// <summary>
            /// 1 - tăng
            /// 2 - giảm
            /// 3 - không đổi
            /// </summary>
            public int? Status { get; set; }
        }
        public static CompareModel CompareProgress(double thisMonth, double lastMonth)
        {
            double differenceQuantityValue = 0;
            double differenceRateValue = 0;
            int status = 3;
            //tháng này > 0 && tháng trước = 0 ( tăng 100% )
            if (thisMonth > 0 && lastMonth == 0)
            {
                differenceQuantityValue = thisMonth;
                differenceRateValue = 100;
                status = 1;
            }
            //tháng này = 0 && tháng trước > 0 ( giảm 100% )
            if (thisMonth == 0 && lastMonth > 0)
            {
                differenceQuantityValue = lastMonth;
                differenceRateValue = 100;
                status = 2;
            }

            //tháng này > 0 && tháng trước > 0
            if (thisMonth > 0 && lastMonth > 0)
            {
                //chênh lệch
                differenceQuantityValue = thisMonth - lastMonth;
                differenceRateValue = Math.Round(Math.Abs(differenceQuantityValue / lastMonth * 100), 2);
                //tháng này > tháng trước ( tăng percent% )
                if (thisMonth > lastMonth)
                {
                    status = 1;
                }
                //tháng này < tháng trước (giảm percent% )
                if (thisMonth < lastMonth)
                {
                    status = 2;
                }
            }
            return new CompareModel { DifferenceQuantity = differenceQuantityValue, DifferenceValue = differenceRateValue, Status = status };
        }
        #endregion

        #region báo cáo số liệu tổng quan
        public async Task<List<StatisticalModel>> FinanceOverview(StatisticalSearch baseSearch)
        {
            if (baseSearch == null) baseSearch = new StatisticalSearch();
            var time = GetTimeModel(baseSearch.month, baseSearch.year);
            var result = new List<StatisticalModel>();
            var data = new StatisticalModel();
            double totalData = 0;
            double totalDataInMonth = 0;
            double totalDataPreMonth = 0;
            var compare = new CompareModel();

            #region chuẩn bị data
            //phiên thanh toán trong tháng
            var listPaymentSessionInMonth = await this.unitOfWork.Repository<tbl_PaymentSession>().GetQueryable()
                    .Where(x => x.deleted == false
                        && x.branchId != null 
                        && (baseSearch.branchId == null || x.branchId == baseSearch.branchId)
                        && x.paymentDate != null)
                    .ToListAsync();
            listPaymentSessionInMonth = listPaymentSessionInMonth.Where(
                        x => DateTimeOffset.FromUnixTimeMilliseconds((long)x.paymentDate).Year == time.Year
                        && DateTimeOffset.FromUnixTimeMilliseconds((long)x.paymentDate).Month == time.Month)
                    .ToList();

            //phiên thanh toán tháng trước
            var listPaymentSessionPreMonth = await this.unitOfWork.Repository<tbl_PaymentSession>().GetQueryable()
                    .Where(x => x.deleted == false
                        && x.branchId != null
                        && (baseSearch.branchId == null || x.branchId == baseSearch.branchId)
                        && x.paymentDate != null)
                    .ToListAsync();
            listPaymentSessionPreMonth = listPaymentSessionPreMonth.Where(
                        x => DateTimeOffset.FromUnixTimeMilliseconds((long)x.paymentDate).Year == time.Year
                        && DateTimeOffset.FromUnixTimeMilliseconds((long)x.paymentDate).Month == time.LastMonth)
                    .ToList();

            //đợt thu trong tháng
            var listBillInMonth = await this.unitOfWork.Repository<tbl_Bill>().GetQueryable()
                    .Where(x => x.deleted == false
                        && x.branchId != null
                        && (baseSearch.branchId == null || x.branchId == baseSearch.branchId)
                        && x.paymentDate != null)
                    .ToListAsync();
            listBillInMonth = listBillInMonth.Where(
                        x => DateTimeOffset.FromUnixTimeMilliseconds((long)x.created).Year == time.Year
                        && DateTimeOffset.FromUnixTimeMilliseconds((long)x.created).Month == time.Month)
                    .ToList();

            //đợt thu tháng trước
            var listBillPreMonth = await this.unitOfWork.Repository<tbl_Bill>().GetQueryable()
                    .Where(x => x.deleted == false
                        && x.branchId != null
                        && (baseSearch.branchId == null || x.branchId == baseSearch.branchId)
                        && x.paymentDate != null)
                    .ToListAsync();
            listBillPreMonth = listBillPreMonth.Where(
                        x => DateTimeOffset.FromUnixTimeMilliseconds((long)x.created).Year == time.Year
                        && DateTimeOffset.FromUnixTimeMilliseconds((long)x.created).Month == time.LastMonth)
                    .ToList();
            #endregion

            // Doanh thu trong tháng
            totalDataInMonth = 0;
            if (listPaymentSessionInMonth.Count > 0)
            {
                totalDataInMonth = listPaymentSessionInMonth.Where(x => x.type == 1).Sum(x => x.value ?? 0);
            }

            totalDataPreMonth = 0;
            if (listPaymentSessionPreMonth.Count > 0)
            {
                totalDataPreMonth = listPaymentSessionPreMonth.Where(x => x.type == 1).Sum(x => x.value ?? 0);
            }

            compare = CompareProgress(totalDataInMonth, totalDataPreMonth);
            data = new StatisticalCommentModel
            {
                Type = "Doanh thu trong tháng",
                Value = totalDataInMonth,
                DifferenceQuantity = compare.DifferenceQuantity,
                DifferenceValue = compare.DifferenceValue,
                Status = compare.Status
            };
            result.Add(data);

            // Chi phí trong tháng
            totalDataInMonth = 0;
            if (listPaymentSessionInMonth.Count > 0)
            {
                totalDataInMonth = listPaymentSessionInMonth.Where(x => x.type == 2).Sum(x => x.value ?? 0);
            }

            totalDataPreMonth = 0;
            if (listPaymentSessionPreMonth.Count > 0)
            {
                totalDataPreMonth = listPaymentSessionPreMonth.Where(x => x.type == 2).Sum(x => x.value ?? 0);
            }

            compare = CompareProgress(totalDataInMonth, totalDataPreMonth);
            data = new StatisticalCommentModel
            {
                Type = "Chi phí trong tháng",
                Value = totalDataInMonth,
                DifferenceQuantity = compare.DifferenceQuantity,
                DifferenceValue = compare.DifferenceValue,
                Status = compare.Status
            };
            result.Add(data);

            // Tổng nợ trong tháng
            totalDataInMonth = 0;
            if (listBillInMonth.Count > 0)
            {
                totalDataInMonth = listBillInMonth.Sum(x => x.debt ?? 0);
            }

            totalDataPreMonth = 0;
            if (listBillPreMonth.Count > 0)
            {
                totalDataPreMonth = listBillPreMonth.Sum(x => x.debt ?? 0);
            }

            compare = CompareProgress(totalDataInMonth, totalDataPreMonth);
            data = new StatisticalCommentModel
            {
                Type = "Tổng nợ trong tháng",
                Value = totalDataInMonth,
                DifferenceQuantity = compare.DifferenceQuantity,
                DifferenceValue = compare.DifferenceValue,
                Status = compare.Status
            };
            result.Add(data);

            // Tổng giảm giá trong tháng
            totalDataInMonth = 0;
            if (listBillInMonth.Count > 0)
            {
                totalDataInMonth = listBillInMonth.Sum(x => x.totalReduced ?? 0);
            }

            totalDataPreMonth = 0;
            if (listBillPreMonth.Count > 0)
            {
                totalDataPreMonth = listBillPreMonth.Sum(x => x.totalReduced ?? 0);
            }

            compare = CompareProgress(totalDataInMonth, totalDataPreMonth);
            data = new StatisticalCommentModel
            {
                Type = "Tổng giảm giá trong tháng",
                Value = totalDataInMonth,
                DifferenceQuantity = compare.DifferenceQuantity,
                DifferenceValue = compare.DifferenceValue,
                Status = compare.Status
            };
            result.Add(data);

            return result;
        }
        #endregion

        #region báo cáo doanh thu theo 12 tháng
        public async Task<List<Statistical12MonthModel>> Revenue12Month(StatisticalSearch baseSearch)
        {
            if (baseSearch == null) baseSearch = new StatisticalSearch();
            var time = GetTimeModel(baseSearch.month, baseSearch.year);
            var result = new List<Statistical12MonthModel>();

            var filteredList = new List<tbl_PaymentSession>();
            var dataItem = new Statistical12MonthModel();

            var listPaymentSessionInYear = await this.unitOfWork.Repository<tbl_PaymentSession>().GetQueryable()
                    .Where(x => x.deleted == false
                        && x.branchId != null
                        && (baseSearch.branchId == null || x.branchId == baseSearch.branchId)
                        && x.paymentDate != null)
                    .ToListAsync();
            listPaymentSessionInYear = listPaymentSessionInYear.Where(x => DateTimeOffset.FromUnixTimeMilliseconds((long)x.paymentDate).Year == time.Year).ToList();

            var listPaymentSessionPreYear = await this.unitOfWork.Repository<tbl_PaymentSession>().GetQueryable()
                    .Where(x => x.deleted == false
                        && x.branchId != null
                        && (baseSearch.branchId == null || x.branchId == baseSearch.branchId)
                        && x.paymentDate != null)
                    .ToListAsync();
            listPaymentSessionPreYear = listPaymentSessionPreYear.Where(x => DateTimeOffset.FromUnixTimeMilliseconds((long)x.paymentDate).Year == time.LastYear).ToList();

            //type = 1 là phiếu thu
            //type = 2 là phiếu chi
            for (int i = 1; i <= 12; i++)
            {
                //doanh thu trong năm được chọn tháng thứ i
                dataItem = new Statistical12MonthModel();
                dataItem.Month = "Tháng " + i;
                dataItem.Type = "Doanh thu năm nay";
                filteredList = listPaymentSessionInYear
                    .Where(x => DateTimeOffset.FromUnixTimeMilliseconds((long)x.paymentDate).Month == i && x.type == 1).ToList();
                dataItem.Value = filteredList.Any() ? filteredList.Sum(x => x.value ?? 0) : 0;
                result.Add(dataItem);
            }
            for (int i = 1; i <= 12; i++)
            {
                //doanh thu trong năm trước tháng thứ i
                dataItem = new Statistical12MonthModel();
                dataItem.Month = "Tháng " + i;
                dataItem.Type = "Doanh thu năm ngoái";
                filteredList = listPaymentSessionPreYear
                    .Where(x => DateTimeOffset.FromUnixTimeMilliseconds((long)x.paymentDate).Month == i && x.type == 1).ToList();
                dataItem.Value = filteredList.Any() ? filteredList.Sum(x => x.value ?? 0) : 0;
                result.Add(dataItem);
            }
            return result;
        }
        #endregion

        #region báo cáo tỉ lệ khoản thu
        public async Task<List<StatisticalModel>> RateTutionConfig(StatisticalSearch baseSearch)
        {
            if (baseSearch == null) baseSearch = new StatisticalSearch();
            var time = GetTimeModel(baseSearch.month, baseSearch.year);
            var result = new List<StatisticalModel>();

            var listBillDetail = await this.unitOfWork.Repository<tbl_BillDetail>().GetQueryable()
                    .Where(x => x.deleted == false)
                    .ToListAsync();
            listBillDetail = listBillDetail.Where(
                        x => DateTimeOffset.FromUnixTimeMilliseconds((long)x.created).Year == time.Year
                        && DateTimeOffset.FromUnixTimeMilliseconds((long)x.created).Month == time.Month)
                    .ToList();

            var listTutionConfig = await this.unitOfWork.Repository<tbl_TuitionConfig>().GetQueryable()
                    .Where(x => x.deleted == false)
                    .ToListAsync();

            var listTutionConfigCategory = await this.unitOfWork.Repository<tbl_TuitionConfigCategory>().GetQueryable()
                    .Where(x => x.deleted == false)
                    .ToListAsync();

            if(listTutionConfigCategory.Count > 0)
            {
                foreach (var item in listTutionConfigCategory)
                {
                    var data = new StatisticalModel();
                    data.Type = item.name;
                    var countData = listBillDetail.Where(x => listTutionConfig.Any(y => y.id == x.tuitionConfigId && y.tuitionConfigCategoryId == item.id) == true).Count();
                    if(listBillDetail.Count > 0)
                        data.Value = Math.Round(((double)countData / (double)listBillDetail.Count * 100), 2);
                    result.Add(data);
                }
            }
            
            return result;
        }
        #endregion

        #region báo cáo các đợt thu còn nợ
        public async Task<PagedList<BillInDebtModel>> ReportBillInDebt(StatisticalSearch baseSearch)
        {
            if (baseSearch == null) baseSearch = new StatisticalSearch();
            var time = GetTimeModel(baseSearch.month, baseSearch.year);
            var result = new PagedList<BillInDebtModel>();
            var items = new List<BillInDebtModel>();

            var listStudent = await this.unitOfWork.Repository<tbl_Student>().GetQueryable()
                    .Where(x => x.deleted == false)
                    .ToListAsync();
            var listBill = await this.unitOfWork.Repository<tbl_Bill>().GetQueryable()
                    .Where(x => x.deleted == false && x.debt > 0 && x.branchId != null
                        && (baseSearch.branchId == null || x.branchId == baseSearch.branchId))
                    .ToListAsync();
            listBill = listBill.Where(
                        x => DateTimeOffset.FromUnixTimeMilliseconds((long)x.created).Year == time.Year
                        && DateTimeOffset.FromUnixTimeMilliseconds((long)x.created).Month == time.Month)
                    .ToList();
            if(listBill.Count <= 0) return new PagedList<BillInDebtModel> { totalItem = 0, items = null, pageIndex = 0, pageSize = 0};

            listBill = listBill.OrderByDescending(x => x.created).ToList();
            result.totalItem = listBill.Count;
            // Phân trang
            int startIndex = (baseSearch.pageIndex - 1) * baseSearch.pageSize;
            listBill = listBill.Skip(startIndex).Take(baseSearch.pageSize).ToList();
            result.pageSize = baseSearch.pageSize;
            result.pageIndex = baseSearch.pageIndex;

            foreach (var item in listBill)
            {
                var data = new BillInDebtModel();
                var student = listStudent.SingleOrDefault(x => x.id == item.studentId);
                if (student == null)
                    continue;
                data.studentName = student.fullName;
                data.studentCode = student.code;
                data.studentThumbnail = student.thumbnail;
                data.code = item.code;
                data.totalFinalPrice = item.totalFinalPrice;
                data.paid = item.paid;
                data.debt = item.debt;
                data.date = item.date; 
                data.note = item.note;
                items.Add(data);           
            }
            result.items = items;
            return result;
        }
        #endregion
    }
}
