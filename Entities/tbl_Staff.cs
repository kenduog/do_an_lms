using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities
{
    public class tbl_Staff : DomainEntities.DomainEntities
    {
        public Guid? branchId { get; set; }

        public Guid? userId { get; set; }
        /// <summary>
        /// Trình độ học vấn
        /// </summary>
        public Guid? highestLevelOfEducationId { get; set; }

        /// <summary>
        /// trường tốt nghiệp
        /// </summary>
        public string graduateSchool { get; set; }
        public string certificate { get; set; }
        public string major { get; set; }
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
        public Guid? departmentId { get; set; }
        [NotMapped]
        public string departmentName { get; set; }
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
        public string groupIds { get; set; }
        [NotMapped]
        public List<SubjectStaff> subject { get; set; }
        [NotMapped]
        public List<DomainOption> branchs { get; set; }
        [NotMapped]
        public List<GroupOption> groups { get; set; }
        [NotMapped]
        public string itemCode { get; set; }
        [NotMapped]
        public string itemName { get; set; }
        [NotMapped]
        public string itemUserName { get; set; }
        [NotMapped]
        public string itemPhone { get; set; }
        [NotMapped]
        public string itemEmail { get; set; }
        [NotMapped]
        public string itemBOD { get; set; }
        [NotMapped]
        public string itemCity { get; set; }
        [NotMapped]
        public string itemDepartmentName { get; set; }
        [NotMapped]
        public string itemGroup { get; set; }
        [NotMapped]
        public string itemStatus { get; set; }
        [NotMapped]
        public string itemSubject { get; set; }
        [NotMapped]
        public string itemBranchName { get; set; }

    }
    public class ItemStaffImport {
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUserName { get; set; }
        public string itemPhone { get; set; }
        public string itemEmail { get; set; }
        public double? itemBOD { get; set; }
        public string itemCity { get; set; }
        public string itemDepartmentName { get; set; }
        public string itemBranchName { get; set; }
        public string itemStatus { get; set; }
        public string itemCertificate { get; set; }
        public string itemMajor { get; set; }
        public string itemGroupUser { get; set; }
        public string itemGroup { get; set; }
    }
    public class ItemStaffExportTemplete
    {
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUserName { get; set; }
        public string itemPhone { get; set; }
        public string itemEmail { get; set; }
        public string itemBOD { get; set; }
        public string itemCity { get; set; }
        public string itemDepartmentName { get; set; }
        public string itemMajor { get; set; }
        public string itemCertificate { get; set; }
        public string itemBranchName { get; set; }
        public string itemGroupUser { get; set; }
    }
    public class ItemTeacherExportTemplete
    {
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUserName { get; set; }
        public string itemPhone { get; set; }
        public string itemEmail { get; set; }
        public string itemBOD { get; set; }
        public string itemCity { get; set; }
        public string itemDepartmentName { get; set; }
        public string itemSubject { get; set; }
        public string itemGroup { get; set; }
        public string itemStatus { get; set; }
        public string itemCertificate { get; set; }
        public string itemMajor { get; set; }
        public string itemBranchName { get; set; }
        public string itemGroupUser { get; set; }
    }
    public class ItemStaffExport
    {
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUserName { get; set; }
        public string itemPhone { get; set; }
        public string itemEmail { get; set; }
        public string itemBOD { get; set; }
        public string itemCity { get; set; }
        public string itemDepartmentName { get; set; }
        public string itemBranchName { get; set; }
        public string itemGroup { get; set; }
    }
    public class ItemTeacherImport
    {
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUserName { get; set; }
        public string itemPhone { get; set; }
        public string itemEmail { get; set; }
        public double? itemBOD { get; set; }
        public string itemCity { get; set; }
        public string itemDepartmentName { get; set; }
        public string itemSubject { get; set; }
        public string itemGroup { get; set; }
        public string itemStatus { get; set; }
        public string itemCertificate { get; set; }
        public string itemMajor { get; set; }
        public string itemBranchName { get; set; }
        public string itemGroupUser { get; set; }
    }
    public class ItemTeacherExport
    {
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUserName { get; set; }
        public string itemPhone { get; set; }
        public string itemEmail { get; set; }
        public double? itemBOD { get; set; }
        public string itemCity { get; set; }
        public string itemDepartmentName { get; set; }
        public string itemSubject { get; set; }
        public string itemGroup { get; set; }
        public string itemStatus { get; set; }
        public string itemCertificate { get; set; }
        public string itemMajor { get; set; }
        public string itemBranchName { get; set; }
        public string itemGroupUser { get; set; }
    }
    public class SubjectStaff {
        public Guid? id { get; set; }
        public string name { get; set; }
    }
}
