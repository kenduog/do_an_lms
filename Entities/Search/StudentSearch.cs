using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Utilities;

namespace Entities.Search
{
    public class StudentSearch : BaseSearch
    {
        public Guid? branchId { get; set; }
        /// <summary>
        /// Loại
        /// 1 - Đúng tuyến
        /// 2 - Trái tuyến
        /// </summary>
        public int? type { get; set; }
        /// <summary>
        /// Enum Gender
        /// </summary>
        public int? gender { get; set; }
        /// <summary>
        /// Hình thức
        /// 1 - Theo buổi
        /// 2 - Bán trú
        /// </summary>
        public int? method { get; set; }
        public Guid? gradeId { get; set; }
        public int? status { get; set; }
    }
    public class StudentSearchExport
    {
        public Guid? branchId { get; set; }
        /// <summary>
        /// Loại
        /// 1 - Đúng tuyến
        /// 2 - Trái tuyến
        /// </summary>
        public int? type { get; set; }
        /// <summary>
        /// Enum Gender
        /// </summary>
        public int? gender { get; set; }
        /// <summary>
        /// Hình thức
        /// 1 - Theo buổi
        /// 2 - Bán trú
        /// </summary>
        public int? method { get; set; }
        public Guid? gradeId { get; set; }
        public int? status { get; set; }
    }
    public class StudentOptionSearch
    {
        public Guid? branchId { get; set; }
        public Guid? gradeId { get; set; }
        public Guid? classId { get; set; }
    }

    public class ArrangeNewClassSearch : BaseSearch
    {
        public Guid? schoolYearId { get; set; }
        public Guid? gradeId { get; set; }
        public Guid? classId { get; set; }
    }

    public class GetStudentByGradeRequest : BaseSearch
    {
        public Guid? schoolYearId { get; set; }
        public Guid? gradeId { get; set; }
    }
}
