using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities.Search
{
    public class StudentDailyActivityRequest : BaseSearch
    {
        [Required(ErrorMessage = MessageContants.req_studentId)]
        public Guid? studentId { get; set; }
        [Required(ErrorMessage = MessageContants.req_currentDate)]
        public double? date { get; set; }
    }
}
