using Newtonsoft.Json;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using static Utilities.CoreContants;

namespace Request.RequestUpdate
{
    public class ParentUpdate : DomainUpdate
    {
        public string code { get; set; }
        public string username { get; set; }
        /// <summary>
        /// Họ và tên
        /// </summary>
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
        public int status { get; set; }
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
        public string setPassword { get; set; }
        /// <summary>
        /// </summary>
        public int? gender { get; set; }
        [JsonIgnore]
        public string genderName { get { return GetGenderName(gender ?? 0); } }
        public string thumbnail { get; set; }
        public string thumbnailResize { get; set; }
        public Guid? districtId { get; set; }
        public Guid? cityId { get; set; }
        public Guid? wardId { get; set; }
        /// <summary>
        /// Chi nhánh
        /// </summary>
        public string branchIds { get; set; }
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
        public Guid? studentId { get; set; }
    }
}
