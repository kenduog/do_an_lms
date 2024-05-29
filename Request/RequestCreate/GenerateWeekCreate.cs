using Newtonsoft.Json;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using static Utilities.CoreContants;

namespace Request.RequestCreate
{
    public class GenerateWeekCreate : DomainCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn năm học")]
        public Guid? schoolYearId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn học kỳ")]
        public Guid? semesterId { get; set; }
    }
}
