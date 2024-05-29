using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_ChildAssessmentTopic : DomainEntities.DomainEntities
    {
        public Guid? branchId { get; set; }
        public string name { get; set; }
        public Guid? gradeId { get; set; }
        [NotMapped]
        public string branchName { get; set; }
        [NotMapped]
        public string gradeName { get; set; }
    }

    public class AssessmentItem
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public List<AssessmentDetailItem> childs { get; set; }
    }
    public class AssessmentDetailItem 
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public bool selected { get; set; } = false;
    }
}
