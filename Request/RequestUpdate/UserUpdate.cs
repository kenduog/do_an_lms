using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;
using Request.DomainRequests;
using Utilities;
using static Utilities.CoreContants;

namespace Request.RequestUpdate
{
    public class UserUpdate : DomainUpdate
    {
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
        private string setPassword { get; set; }
        /// <summary>
        /// </summary>
        public int? gender { get; set; }
        [JsonIgnore]
        public string genderName { get { return GetGenderName(gender ?? 0); } }
        public string thumbnail { get; set; }
        public Guid? districtId { get; set; }
        public Guid? cityId { get; set; }
        public Guid? wardId { get; set; }
    }
    public class StateUpdate : DomainUpdate
    {
        public int? state { get; set; }
    }
    public class OneSignalUpdate: DomainUpdate
    {
        public string playerId { get; set; }
    }
    public class UserPaymentUpdate : DomainUpdate
    {
        public bool? allowPayment { get; set; }
    }
}
