using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DomainEntities
{
    public class DomainReport
    {
        public double sDate { get; set; }
        public double eDate { get; set; }
    }
    public class PaymentReportRequest : DomainReport
    {
        [Required]
        public int type { get; set; }
    }

    public class StaffSalary: DomainReport
    {
        [Required]
        public Guid? id { get; set; }
    }
}
