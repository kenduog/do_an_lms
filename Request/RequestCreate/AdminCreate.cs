using Newtonsoft.Json;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.CoreContants;

namespace Request.RequestCreate
{
    public class AdminCreate : DomainCreate
    {
        public string code { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tài khoản")]
        public string username { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        public string fullName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        [JsonIgnore]
        public int status { get { return ((int)UserStatus.Active); } }
        [JsonIgnore]
        public string statusName { get { return GetUserStatusName(((int)UserStatus.Active)); } }
        public double? birthday { get; set; }
        public int gender { get; set; }
        [JsonIgnore]
        public string genderName { get { return GetGenderName(gender); }}
        public string thumbnail { get; set; }
        public Guid? cityId { get; set; }
        public Guid? districtId { get; set; }
        public Guid? wardId { get; set; }
        public string hashQRCode { get; set; }
        public string thumbnailResize { get; set; }
    }
}
