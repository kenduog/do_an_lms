
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class tbl_Item : DomainEntities.DomainEntities
    {
        public Guid? branchId { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string nameShort { get; set; }
        public Guid? unitOfMeasureId { get; set; }
        public Guid? itemGroupId { get; set; }
        public double? unitPrice { get; set; } = 0;

        //bổ sung thông tin trên 1 đơn vị sản phẩm
        public double? calo { get; set; } = 0;
        public double? protein { get; set; } = 0;
        public double? lipit { get; set; } = 0;
        public double? gluxit { get; set; } = 0;
        /// <summary>
        /// Tỷ lệ thải bỏ
        /// </summary>
        public double? essenceRate { get; set; } = 0;
        /// <summary>
        /// Trọng lượng (gram) / 1 đơn vị tính
        /// </summary>
        public double? weightPerUnit { get; set; } = 1;

        [NotMapped]
        public string itemGroupName { get; set; }
        [NotMapped]
        public string unitOfMeasureName { get; set; }
    }
}
