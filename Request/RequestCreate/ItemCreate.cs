using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class ItemCreate :  DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        public string code { get; set; }
        [Required(ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
        public string nameShort { get; set; }
        [Required(ErrorMessage = MessageContants.req_unitOfMeasureId)]
        public Guid? unitOfMeasureId { get; set; }
        [Required(ErrorMessage = MessageContants.req_itemGroupId)]
        public Guid? itemGroupId { get; set; }

        public double? calo { get; set; }
        public double? protein { get; set; }
        public double? lipit { get; set; }
        public double? gluxit { get; set; }
        /// <summary>
        /// Tỷ lệ thải bỏ
        /// </summary>
        public double? essenceRate { get; set; }
        public double? unitPrice { get; set; }
        public double? weightPerUnit { get; set; }
    }
}
