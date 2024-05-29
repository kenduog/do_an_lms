using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_Parent : DomainEntities.DomainEntities
    {
        public Guid? userId { get; set; }
        /// <summary>
        /// 1 - Cha 
        /// 2 - Mẹ
        /// 3 - Người giám hộ
        /// </summary>
        public int? type { get; set; }
        public string typeName { get; set; }
        public string note { get; set;}
        /// <summary>
        /// Nghề nghiệp
        /// </summary>
        public string job { get; set; }
        [NotMapped]
        public string code { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        [StringLength(50)]
        [NotMapped]
        public string username { get; set; }
        /// <summary>
        /// Họ và tên
        /// </summary>
        [StringLength(200)]
        [NotMapped]
        public string fullName { get; set; }
        [StringLength(200)]
        [NotMapped]
        public string firstName { get; set; }
        [StringLength(200)]
        [NotMapped]
        public string lastName { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        [StringLength(20)]
        [NotMapped]
        public string phone { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [StringLength(50)]
        [NotMapped]
        public string email { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        [StringLength(1000)]
        [NotMapped]
        public string address { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [NotMapped]
        public int? status { get; set; }
        [NotMapped]
        public string statusName { get; set; }
        /// <summary>
        /// Ngày sinh
        /// </summary>
        [NotMapped]
        public double? birthday { get; set; }
        /// <summary>
        /// </summary>
        [NotMapped]
        public int? gender { get; set; }
        [NotMapped]
        public string genderName { get; set; }
        [NotMapped]
        public string thumbnail { get; set; }
        [NotMapped]
        public string thumbnailResize { get; set; }
        [NotMapped]
        public Guid? districtId { get; set; }
        [NotMapped]
        public string districtName { get; set; }
        [NotMapped]
        public Guid? cityId { get; set; }
        [NotMapped]
        public string cityName { get; set; }
        [NotMapped]
        public Guid? wardId { get; set; }
        [NotMapped]
        public string wardName { get; set; }
        [NotMapped]
        public string branchIds { get; set; }
        [NotMapped]
        public List<StudentByParent> students { get; set; }
    }
    public class StudentByParent
    {
        public Guid id { get; set; }
        public string fullName { get; set; }
        public string code { get; set; }
        public string thumbnail { get; set; }
    }
    public class ParentByStudent
    { 
        public tbl_Parent mother { get; set; }
        public tbl_Parent father { get; set; }
        public tbl_Parent guardian { get; set; }
    }
}
