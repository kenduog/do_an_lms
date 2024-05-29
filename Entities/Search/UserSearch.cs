using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using static Utilities.CoreContants;
namespace Entities.Search
{
    public class UserSearch : BaseSearch
    {
        /// <summary>
        /// mẫu: "code,code"
        /// </summary>
        public string roleCodes { get; set; }
        public int gender { get; set; } = 0;
        /// <summary>
        /// 0 - Giảm dần
        /// 1 - Tăng gần
        /// 2 - Tên tăng dần
        /// 3 - Tên giảm dần
        /// </summary>
        [DefaultValue(0)]
        public override int orderBy { set; get; }
    }
}
