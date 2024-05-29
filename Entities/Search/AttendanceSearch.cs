using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.CoreContants;

namespace Entities.Search
{
    public class AttendanceSearch 
    {
        public Guid? classId { get; set; }
        public double? date { get; set; }
        /// <summary>
        /// 1 - Có mặt
        /// 2 - Có phép
        /// 3 - Không phép
        /// </summary>
        public int status { get; set; } = 0;
    }

    public class LearningDayResult
    {
        public double date { get; set; }
        public Guid? studentId { get; set; }
        public Guid? classId { get; set; }
    }
}
