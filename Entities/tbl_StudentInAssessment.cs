using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_StudentInAssessment : DomainEntities.DomainEntities
    {
        public Guid? studentId { get; set; }
        public Guid? semesterId { get; set; }
        public Guid? assessmentTopicId { get; set; }
        public Guid? assessmentDetailId { get; set; }
        /// <summary>
        /// true: đạt
        /// false: chưa đạt
        /// </summary>
        public bool? status { get; set; }
    }
}
