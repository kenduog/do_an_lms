using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities
{
    public class tbl_NutritionGroup : DomainEntities.DomainEntities
    {
        public Guid? branchId { get; set; }
        /// <summary>
        /// Khối lớp áp dụng
        /// </summary>
        public string gradeIds { get; set; }
        public string name { get; set; }
        public double? fCalo { get; set; }
        public double? tCalo { get; set; }
        public double? fLipit{ get; set; }
        public double? tLipit { get; set; }
        public double? fProtein { get; set; }
        public double? tProtein { get; set; }
        public double? fGluxit{ get; set; }
        public double? tGluxit { get; set; }
    }
}
