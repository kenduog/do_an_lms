using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.CoreContants;
using Utilities;
using Request.DomainRequests;
using Newtonsoft.Json;

namespace Request.RequestUpdate
{
    public class ParentInfo : DomainCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập mã người dùng")]
        public string code { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50)]
        public string username { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(200)]
        public string fullName { get; set; }
        [StringLength(200)]
        public string firstName { get; set; }
        [StringLength(200)]
        public string lastName { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        [StringLength(20)]
        public string phone { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [StringLength(50)]
        public string email { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        [StringLength(1000)]
        public string address { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [JsonIgnore]
        public int status { get { return ((int)UserStatus.Active); } }
        [JsonIgnore]
        public string statusName { get { return GetUserStatusName(status); } }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public double? birthday { get; set; }
        /// <summary>
        /// Mật khẩu người dùng
        /// </summary>
        [StringLength(4000)]
        public string password
        {
            get { return string.IsNullOrEmpty(setPassword) ? null : SecurityUtilities.HashSHA1(setPassword); }
            set { setPassword = value; }
        }
        [JsonIgnore]
        private string setPassword { get; set; }
        /// <summary>
        /// 1 - Nam 
        /// 2 - Nữ 
        /// 3 - Khác
        /// </summary>
        public int? gender { get; set; }
        [JsonIgnore]
        public string genderName { get { return GetGenderName(gender ?? 0); } }
        public string thumbnail { get; set; }
        public Guid? districtId { get; set; }
        public Guid? cityId { get; set; }
        public Guid? wardId { get; set; }
        /// <summary>
        /// 1 - Cha 
        /// 2 - Mẹ
        /// 3 - Người giám hộ
        /// </summary>
        public int? type { get; set; }
        [JsonIgnore]
        public string typeName
        {
            get
            {
                return type == 1 ? "Cha"
                    : type == 2 ? "Mẹ"
                    : type == 3 ? "Người giám hộ" : "";
            }
        }
        public string note { get; set; }
        /// <summary>
        /// Nghề nghiệp
        /// </summary>
        public string job { get; set; }
    }
}
