using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestUpdate
{
    public class NutritionGroupUpdate : DomainUpdate
    {
        /// <summary>
        /// Khối lớp áp dụng
        /// </summary>
        public string gradeIds { get; set; }
        public string name { get; set; }
        public double? fCalo { get; set; }
        public double? tCalo { get; set; }
        [Range(0, 100)]
        public double? fLipit { get; set; }
        [Range(0, 100)]
        public double? tLipit { get; set; }
        [Range(0, 100)]
        public double? fProtein { get; set; }
        [Range(0, 100)]
        public double? tProtein { get; set; }
        [Range(0, 100)]
        public double? fGluxit { get; set; }
        [Range(0, 100)]
        public double? tGluxit { get; set; }
    }
}
