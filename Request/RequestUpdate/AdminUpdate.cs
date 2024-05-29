using Newtonsoft.Json;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.CoreContants;

namespace Request.RequestUpdate
{
    public class AdminUpdate : DomainUpdate
    {
        public string code { get; set; }
        public string username { get; set; }
        public string fullName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public int status { get; set; }
        [JsonIgnore]
        public string statusName { get { return GetUserStatusName(status); } }
        public double? birthday { get; set; }
        /// <summary>
        /// 1 - Nam 
        /// 2 - Nữ 
        /// 3 - Khác
        /// </summary>
        public int? gender { get; set; }
        [JsonIgnore]
        public string genderName { get { return GetGenderName(gender ?? 0); } }
        public string thumbnail { get; set; }
        public Guid? cityId { get; set; }
        public Guid? districtId { get; set; }
        public Guid? wardId { get; set; }
        public string hashQRCode { get; set; }
        public string thumbnailResize { get; set; }
    }
}
