using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class sys_MultipleMessage
    {
        [Key]
        public string code { get; set; }
        public string en { get; set; }
        public string vi { get; set; }
    }
}
