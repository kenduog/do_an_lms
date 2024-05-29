using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using static Utilities.CoreContants;

namespace Entities
{
    public class tbl_Student : DomainEntities.DomainEntities
    {
        public string code { get; set; }
        public string thumbnail { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
        /// <summary>
        /// Biệt danh
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// Loại
        /// 1 - Đúng tuyến
        /// 2 - Trái tuyến
        /// </summary>
        public int? type { get; set; }
        public string typeName { get; set; }
        /// <summary>
        /// 1 - Nam 
        /// 2 - Nữ 
        /// 3 - Khác
        /// </summary>
        public int? gender { get; set; }
        public string genderName { get; set; }
        /// <summary>
        /// Dân tộc
        /// </summary>
        public string ethnicity { get; set; }
        /// <summary>
        /// Ngày sinh
        /// </summary>
        public double? birthday { get; set; }
        /// <summary>
        /// Nơi sinh
        /// </summary>
        public string placeOfBirth { get; set; }
        /// <summary>
        /// Địa chỉ hiện tại
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// Quê quán
        /// </summary>
        public string hometown { get; set; }
        /// <summary>
        /// Hình thức
        /// 1 - Theo buổi
        /// 2 - Bán trú
        /// </summary>
        public int? method { get; set; }
        /// <summary>
        /// Phụ huynh
        /// </summary>
        public Guid? fatherId { get; set; }
        public Guid? motherId { get; set; }
        public Guid? guardianId { get; set; }
        /// <summary>
        /// Ngày nhập học
        /// </summary>
        public double? enrollmentDate { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string note { get; set; }

        /// <summary>
        /// Ngày bắt đầu học
        /// </summary>
        public double? startLearningDate { get; set; }
        /// <summary>
        /// Tình trạng học sinh
        /// 1 - Mới
        /// 2 - Đang học
        /// 3 - Bảo lưu
        /// 4 - Đã học xong
        /// 5 - Bỏ học
        /// </summary>
        public int? status { get; set; }
        public string statusName { get; set; }
        public static string GetStatusName(int status) =>
            status == 1 ? "Mới"
            : status == 2 ? "Đang học"
            : status == 3 ? "Bảo lưu"
            : status == 4 ? "Đã học xong"
            : status == 5 ? "Bỏ học" : "";
        /// <summary>
        /// Khối
        /// </summary>
        public Guid? gradeId { get; set; }
        [NotMapped]
        public string gradeName { get; set; }

        public Guid? branchId { get; set; }
        [NotMapped]
        public Guid? classId { get; set; }
        [NotMapped]
        public string className { get; set; }
        [NotMapped]
        public int? statusAssessment { get; set; }
        [NotMapped]
        public Guid? schoolYearId { get; set; }
    }

    public class StudentSelection
    {
        public Guid id { get; set; }
        public string thumbnail { get; set; }
        public string fullName { get; set; }
        public Guid? classId { get; set; }
        public string className { get; set; }
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
    }
    public class ParentModel
    {
        public Guid? id { get; set; }
        public string thumbnail { get; set; }
        public string fullName { get; set; }
        public string phone { get; set; }
        public string job { get; set; }
        public double? bod { get; set; }
        public int? type { get; set; }
        public string typeName { get; set; }

    }
    public class ProfileStudentForMobile : tbl_Student
    {
        public ParentModel father { get; set; }
        public ParentModel mother { get; set; }
        public ParentModel guardian { get; set; }
        public string nameBranch { get; set; }
        public string phoneBranch { get; set; }
        public string emailBranch { get; set; }
        public string addressBranch { get; set; }
    }
    public class SchoolReportCardModel
    {
        public Guid? studentId { get; set; }
        public string studentName { get; set; }
        public string studentThumbnail { get; set; }
        public Guid? classId { get; set; }
        public string className { get; set; }
        public int? status { get; set; }
        public string statusName { get; set; }
        public Guid? gradeId { get; set; }
        public string gradeName { get; set; }
        public Attendance attendance { get; set; }
        public List<Comment> comments { get; set; }
        public GoodBehaviorCertificate goodBehaviorCertificate { get; set; }
        public List<ChildAssessmentTopic> childAssessment { get; set; }
    }
    public class GoodBehaviorCertificate {
        public int total { get; set; }
        public int receive { get; set; }
    }
    public class Attendance {
        public int? total { get; set; }
        public int? attendanced { get; set; }
        public int? dayOff { get; set; }
        public int? dayOffAllow { get; set; }
        public int? dayOffNotAllow { get; set; }
    }
    public class Comment {
        public Guid? id { get; set; }
        public double? date { get; set; }
        public string dayComment { get; set; }
        public string lunch { get; set; }
        public string afternoonSnack { get; set; }
        public string afternoon { get; set; }
        public string sleep { get; set; }
        public string hygiene { get; set; }
    }
    public class ChildAssessmentTopic
{
        public Guid? id { get; set; }
        public string name { get; set; }
        public List<ChildAssessmentDetail> details { get; set; }
    }
    public class ChildAssessmentDetail
    {
        public Guid? id { get; set; }
        public string name { get; set; }
        public bool? status { get; set; }
        public string statusName { get; set; }
    }

    public class ItemStudentExport
    {
        public string firstName { get; set; }
        public string thumbnail { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
        public string nickname { get; set; }
        public int type { get; set; }
        public int gender { get; set; }
        public string ethnicity { get; set; }
        public double birthday { get; set; }
        public string placeOfBirth { get; set; }
        public string address { get; set; }
        public string hometown { get; set; }
        public int method { get; set; }
        public double enrollmentDate { get; set; }
        public string note { get; set; }
        public double startLearningDate { get; set; }


    }
    public class ItemStudentExportWithClient
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
        public string nickname { get; set; }
        public string type { get; set; }
        public string gender { get; set; }
        public string ethnicity { get; set; }
        public string birthday { get; set; }
        public string placeOfBirth { get; set; }
        public string address { get; set; }
        public string hometown { get; set; }
        public string method { get; set; }
        public string enrollmentDate { get; set; }
        public string note { get; set; }
        public string startLearningDate { get; set; }


    }
    public class ItemStudentImport
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
        public string nickname { get; set; }
        public int type { get; set; }
        public int gender { get; set; }
        public string ethnicity { get; set; }
        public double birthday { get; set; }
        public string placeOfBirth { get; set; }
        public string address { get; set; }
        public string hometown { get; set; }
        public int method { get; set; }
        public double enrollmentDate { get; set; }
        public string note { get; set; }
        public double startLearningDate { get; set; }
    }
}