using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObject
{
    public class StudentAvailableDTO
    {
        public Guid id { get; set; }
        public string code { get; set; }
        public string fullName { get; set; }
        public double? birthday { get; set; }
        /// <summary>
        /// 1 - Nam 
        /// 2 - Nữ 
        /// 3 - Khác
        /// </summary>
        public int? gender { get; set; }
        public string genderName { get; set; }
        public string oldClass { get; set; }
        [JsonIgnore]
        public int totalItem { get; set; }
    }
}
