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
    public class ScaleMeasureDetailSearch : BaseSearch
    {
        [Required(ErrorMessage = MessageContants.req_scaleMeasureId)]
        public Guid? scaleMeasureId { get; set; }
        public string classIds { get; set; }
        public string gradeIds { get; set; }
        /// <summary>
        /// Id của bé được chọn - 
        /// </summary>
        public Guid? studentId { get; set; }
    }
    public class ScaleMeasureDetailExportSearch
    { 
        public string classIds { get; set; }
        public string gradeIds { get; set; }
        
    }
}
