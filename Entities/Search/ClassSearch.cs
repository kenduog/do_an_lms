using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities.Search
{
    public class ClassSearch : BaseSearch
    {
        public Guid? gradeId { get; set; }
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
        /// <summary>
        /// 0 - Giảm dần
        /// 1 - Tăng gần
        /// 2 - Tên tăng dần
        /// 3 - Tên giảm dần
        /// </summary>
        [DefaultValue(0)]
        public override int orderBy { set; get; }
    }

    public class ClassPrepare
    {
        public Guid? gradeId { get; set; }
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
    }


    public class PrivateClassSearch : ClassSearch
    {
        public string myBranchIds { get; set; }
        public Guid? userId { get; set; }

    }
}
