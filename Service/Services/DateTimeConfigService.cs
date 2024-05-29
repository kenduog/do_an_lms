using Entities;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using static ClosedXML.Excel.XLPredefinedFormat;
using DateTime = System.DateTime;

namespace Service.Services
{
    public class DateTimeConfigService : IDateTimeConfigService
    {
        public DateTimeConfigService(IAppUnitOfWork unitOfWork)
        {
        }

        public async Task<DateTime?> DoubleToDateTime(double value)
        {
            try
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds((long)value);

                return dateTimeOffset.DateTime;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<double?> DateTimeToMilliseconds(DateTime value)
        {
            try
            {
                TimeSpan timeSpan = value - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                // Convert the TimeSpan to milliseconds
                return timeSpan.TotalMilliseconds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<string> DoubleToString(double value, string type)
        {
            try
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds((long)value);
                return dateTimeOffset.Date.ToString(type);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public async Task<string> DoubleToStringExcel(string value, string type)
        {
            try
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds((long)Convert.ToDouble(value));
                return dateTimeOffset.Date.ToString(type);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public async Task<double?> StringToDouble(string value, string type)
        {
            try
            {
                DateTime dateTime = DateTime.ParseExact(value, type, null);
                return (long)(dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static double? StringToDoubleExcel(string value, string type)
        {
            try
            {
                var arrValue = value.Split(' ');
                DateTime dateTime = DateTime.ParseExact(arrValue[0], type, null);
                return (long)(dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
                public static double ConvertStringToDouble(string s)
        {
            DateTime date;
            if (DateTime.TryParseExact(s, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out date))
            {
                // Chuyển đổi ngày thành số double
                double milliseconds = (date - new DateTime(1900, 1, 1)).TotalMilliseconds;
                return milliseconds;
            }
            throw new ArgumentException("Invalid date format.");
        }
        public static string ConvertMillisecondsToDateString(double milliseconds)
        {
            // Tạo một đối tượng DateTime từ số milliseconds
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddMilliseconds(milliseconds);

            // Chuyển đổi thành chuỗi theo định dạng "DD/MM/YYYY"
            return date.ToString("dd/MM/yyyy");
        }
        public static double ConvertDoubleToMilliseconds(double input)
        {
            DateTime date = DateTime.FromOADate(input); // Chuyển đổi từ số double thành ngày
            double milliseconds = (date - new DateTime(1900, 1, 1)).TotalMilliseconds; // Tính số milisecond từ ngày 1/1/1900
            return milliseconds;
        }
    }
}
