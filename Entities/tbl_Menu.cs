using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class tbl_Menu : DomainEntities.DomainEntities
    {
        public Guid? gradeId { get; set; }
        public Guid? nutritionGroupId { get; set; }
        public Guid? branchId { get; set; }
        public string name { get; set; }
        public double? calo { get; set; }
        public double? lipit { get; set; }
        public double? protein { get; set; }
        public double? gluxit { get; set; }

        public double? lipitPercent { get; set; }
        public double? proteinPercent { get; set; }
        public double? gluxitPercent { get; set; }

        [NotMapped]
        public string gradeName { get; set; }
        [NotMapped]
        public string nutritionGroupName { get; set; }
        [NotMapped]
        public virtual IList<tbl_MenuItem> items { get; set; }
        [NotMapped]
        public virtual IList<tbl_MenuFood> foods { get; set; }
        [NotMapped]
        public virtual IList<FoodItem> foodItems{ get; set; }
    }
}
