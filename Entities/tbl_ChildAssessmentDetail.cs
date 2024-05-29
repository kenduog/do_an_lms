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
    public class tbl_ChildAssessmentDetail : DomainEntities.DomainEntities
    {
        public Guid? childAssessmentId { get; set; }
        public string name { get; set; }
        [NotMapped]
        public Guid? topicId{ get; set; }
        [NotMapped]
        public string topicName { get; set; }
        [NotMapped]
        public bool? status { get; set; }
        [NotMapped]
        public bool selected { get; set; } = false;
    }
}
