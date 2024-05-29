using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.CoreContants;

namespace Entities.Search
{
    public class TranscriptSearch : BaseSearch
    {
        public Guid? schoolYearId { get; set; }
        public Guid? semesterId { get; set; }
        public Guid? classId { get; set; }
        public Guid? subjectId { get; set; }
        /// <summary>
        /// 1: kiểm tra miệng
        /// 2: kiểm tra 15 phút
        /// 3: kiểm tra 1 tiết
        /// 4: thi giữa kỳ
        /// 5: thi học kỳ
        /// </summary>
        public int? type { get; set; }
    }
}
