using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class Timestamp
    {
        public static double Now()
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            return timestamp;
        }
        public static double TimestampDateTime(DateTime? date)
        {
            if (!date.HasValue)
                return 0;
            var timestamp = new DateTimeOffset(date.Value).ToUnixTimeMilliseconds();
            return timestamp;
        }

        public static double TimestampDate(long timestamp)
        {
            DateTime rslt = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).Date;
            var result = new DateTimeOffset(rslt).ToUnixTimeMilliseconds();
            return result;
        }
        public static DateTime ToUTCDatetime(long timestamp)
        {
            DateTime rslt = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
            return rslt;
        }
        public static DateTime ToDatetime(long timestamp)
        {
            DateTime rslt = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
            return rslt;
        }

        public static DateTime ToLocalDateTime(double? timestamp)
        {
            if (!timestamp.HasValue)
                return DateTime.MinValue;

            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds((long)timestamp);
            return dateTimeOffset.LocalDateTime;
        }


        public static double AddYear(double time, double year) => time + (year * 31536000 * 1000); //1 year = 31536000 seconds
        public static double AddMinutes(double time, double minutes) => time + (minutes * 60000);
        public static double AddSeconds(double time, double second) => time + (second * 1000);
        public static double AddDay(double time, int day) => time + (day * 24 * 60 * 60000); //day*hours*minutes*miliseconds

        public static double FirstDayOfLastMonth(double time)
        {
            var currentDate = ToDatetime((long)time - 1000 * 60 * 60 * 25);
            var lastMonth = currentDate.AddMonths(-1);
            var firstDay = new DateTime(lastMonth.Year, lastMonth.Month, 1);
            return TimestampDateTime(firstDay);
        }
        public static double LastDayOfLastMonth(double time)
        {
            var currentDate = ToDatetime((long)time);
            var thisMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var lastDay = thisMonth.Date.AddMilliseconds(-1);
            return TimestampDateTime(lastDay);
        }

        public static double MonDay()
        {
            DateTime nowDatetime = DateTime.Today;
            var timeRange = DateTime.Now - DateTime.UtcNow;
            int dayOfWeek = (int)nowDatetime.DayOfWeek;
            DateTime monday = nowDatetime.AddDays(-dayOfWeek + 1);
            double mondayUTC = TimestampDateTime(monday) + timeRange.TotalMilliseconds;
            return Math.Round(mondayUTC);
        }

        public static string ToString(double? timeStamp, string format)
        {
            if (!timeStamp.HasValue)
                return null;

            return ToLocalDateTime(timeStamp).ToString(format);
        }
    }
}
