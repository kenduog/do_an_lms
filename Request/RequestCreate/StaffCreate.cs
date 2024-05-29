using Newtonsoft.Json;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using static Utilities.CoreContants;

namespace Request.RequestCreate
{
    public class StaffCreate : DomainCreate
    {
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
        [Required(ErrorMessage = "Vui lòng chọn chi nhánh cho nhân viên")]
        public string branchIds { get; set; }
        public Guid? departmentId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn nhóm quyền")]
        public string groupIds { get; set; }
        /// <summary>
        /// Trình độ học vấn
        /// </summary>
        public Guid? highestLevelOfEducationId { get; set; }

        /// <summary>
        /// trường tốt nghiệp
        /// </summary>
        public string graduateSchool { get; set; }

        /// <summary>
        /// ngày tốt nghiệp
        /// </summary>
        public double? graduateTime { get; set; }

        /// <summary>
        /// văn bằng chứng chỉ khác
        /// </summary>
        public string otherCertificate { get; set; }

        /// <summary>
        /// Kinh nghiệm dạy học
        /// </summary>
        public string teachingExperience { get; set; }
        public string bankAccount { get; set; }
        public string bankName { get; set; }
        public string bankAddress { get; set; }
        public string bankNumber { get; set; }
        /// <summary>
        /// Ngày vào làm
        /// </summary>
        public double? joined { get; set; }

        public string Certificate { get; set; }
        
        public string Major { get; set; }
    }
}
