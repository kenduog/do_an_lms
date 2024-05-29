using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_ScaleMeasureDetail : DomainEntities.DomainEntities
    {
        public Guid? scaleMeasureId { get; set; }
        public Guid? studentId { get; set; }
        [NotMapped]
        public string studentCode { get; set; }
        [NotMapped]
        public string studentFullName { get; set; }
        [NotMapped]
        public string studentFirstName { get; set; }
        [NotMapped]
        public string studentLastName { get; set; }
        public double? studentBirthDay { get; set; }
        [NotMapped]
        public int? studentGender { get; set; }
        [NotMapped]
        public string className { get; set; }
        public Guid? classId { get; set; }
        [NotMapped]
        public string gradeName { get; set; }
        public Guid? gradeId { get; set; }
        public int? monthOfAge { get; set; }
        public double? weight { get; set; }
        public double? height { get; set; }
        /// <summary>
        /// 0: Dưới 5: Dưới cân
        /// 1: 5 đến 85: Bình thường
        /// 2: 85 đến 95: Thừa cân
        /// 3: 95 trở lên: Béo phì
        /// </summary>
        public double? bmi { get; set; }
        public string bmiResult { get; set; }
        public double? weightMustHave { get; set; }
        [NotMapped]
        public string scaleMeasureName { get; set; }
        [NotMapped]
        public double? scaleMeasureDate { get; set; }
        /// <summary>
        /// Nhận xét người dùng nhập, trường hợp không dùng kết quả BMI
        /// </summary>
        public string evaluation { get; set; }
    }

    public class ScaleMeasureDetailExport
    {
        public string studentCode { get; set; }
        public string studentFullName { get; set; }
        public string studentFirstName { get; set; }
        public string studentLastName { get; set; }
        public string studentBirthDay { get; set; }
        public int? monthOfAge { get; set; }
        public string studentGenderName { get; set; }
        public string gradeName { get; set; }
        public string className { get; set; }
        public double? height { get; set; }
        public double? weight { get; set; }
        public string evaluation { get; set; }
    }
    public class ScaleMeasureDetailExportWithClient
    {
        public string studentCode { get; set; }
        public string studentFullName { get; set; }
        public string studentFirstName { get; set; }
        public string studentLastName { get; set; }
        public double studentBirthDay { get; set; }
        public int? monthOfAge { get; set; }
        public string studentGenderName { get; set; }
        public string gradeName { get; set; }
        public string className { get; set; }
        public double? height { get; set; }
        public double? weight { get; set; }
        public string evaluation { get; set; }
    }
    public class ScaleMeasureDetailModel
    {
        public Guid? scaleMeasureId { get; set; }
        public string scaleMeasureName { get; set; }
        public double? scaleMeasureDate { get; set; }
        public double? weight { get; set; }
        public double? height { get; set; }
        public double? bmi { get; set; }
        public string bmiResult { get; set; }
        public double? weightMustHave { get; set; }
    }
    public class StudentScaleMeasureDetail
    {
        public Guid? studentId { get; set; }
        public string studentCode { get; set; }
        public string studentFullName { get; set; }
        public string studentFirstName { get; set; }
        public string studentLastName { get; set; }
        public double? studentBirthDay { get; set; }
        public string studentGenderName { get; set; }
        public List<ScaleMeasureDetailModel> details { get; set; }
    }
}
