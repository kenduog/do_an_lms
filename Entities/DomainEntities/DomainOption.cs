using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DomainEntities
{
    public class DomainOption
    {
        public Guid? id { get; set; }
        public string name { get; set; }
    }
    public class TuitionConfigOption
    {
        public Guid? id { get; set; }
        public Guid? tuitionConfigCategoryId { get; set; }
        public string tuitionConfigCategoryName { get; set; }
        public string name { get; set; }
        public string thumbnail { get; set; }
        public double? price { get; set; }
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// mô tả
        /// </summary>
        public string description { get; set; }
    }
    public class StudySessionOption
    {
        public Guid? id { get; set; }
        public Guid? branchId { get; set; }
        public int? index { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
    }
    public class StudyShiftOption
    {
        public Guid? id { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
    }
    public class DiscountOption : DomainOption
    {
        /// <summary>
        /// 1 - Giảm tiền
        /// 2 - Giảm phần trăm
        /// </summary>
        public int? type { get; set; }
        public string typeName { get; set; }
        /// <summary>
        /// Giá trị
        /// </summary>
        public double? value { get; set; }
        /// <summary>
        /// Mô tả
        /// </summary>
        public string description { get; set; }
        public double? maximumValue { get; set; }
    }
    public class SelectedOption : DomainOption
    {
        public bool selected { get; set; } = false;
    }
    public class SemesterOption : DomainOption
    {
        public double? sTime { get; set; }
        public double? eTime { get; set; }
        public bool selected { get; set; } = false;
    }
    public class SubjectOption : DomainOption
    {
        public string acronym { get; set; }
    }
    public class SchoolYearOption : DomainOption
    {
        public double? sTime { get; set; }
        public double? eTime { get; set; }
    }
    public class WeekOption
    {
        public Guid id { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
        public string name { get; set; }
        public bool selected { get; set; } = false;
    }
    public class UserOption : DomainOption
    {
        public string code { get; set; }
    }
    public class TeacherOption : UserOption
    {
        public int type { get; set; }
    }
    public class CurriculumDetailOption : DomainOption
    {
        public string code { get; set; }
        public int position { get; set; }
    }
    public class SalaryLevleOption : DomainOption
    {
        public double? price { get; set; }
    }
    public class GroupOption : DomainOption
    {
        public string code { get; set; }
        public bool? isStaff { get; set; }
    }
    public class StudyShiftFilterOption : DomainOption
    {
        public double? value { get; set; }
    }
    public class BranchOption : DomainOption
    {
        /// <summary>
        /// 1 - Mầm non
        /// 2 - Tiểu học
        /// ...
        /// </summary>
        public int? type { get; set; }
        public string logo { get; set; }
        public string description { get; set; }

    }
    public class LookupOption
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }
    public class ItemOption 
    {
        public Guid itemId { get; set; }
        public Guid itemSkuId { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public double? unitPrice { get; set; }
        public double? essenceRate { get; set; } = 0;
        public double? weightPerUnit { get; set; }
        public string unitOfMeasureName { get; set; }
        public double? convertQty { get; set; }
        public Guid? unitOfMeasureId { get; set; }
        public double? calo { get; set; }
        public double? lipit { get; set; }
        public double? gluxit { get; set; }
        public double? protein { get; set; }



    }

    public class FoodOption
    {
        public Guid foodId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public double? calo { get; set; }
        public double? protein { get; set; }
        public double? lipit { get; set; }
        public double? gluxit { get; set; }
    }

    public class ParentOption : DomainOption
    {
        public string code { get; set; }
        public string phone { get; set; }
    }

    public class MenuOption : DomainOption
    {
        public string gradeName { get; set; }
        public string nutritionGroupName { get; set; }
    }

    public class PurchaseOrderOption : DomainOption
    {
        public double? date { get; set; }
        public string code { get; set; }
        public string description { get; set; }
    }

    public class FeeOption : DomainOption
    {
        public Guid? feeId { get; set; }
        public string description { get; set; }
        /// <summary>
        /// Kỳ thu:
        /// 1 - Hàng tháng
        /// 2 - Quí
        /// 3 - Học kỳ
        /// 4 - Năm học
        /// </summary>
        public int? collectionType { get; set; }
        public string gradeName { get; set; }
        public double? price { get; set; }
    }

    public class PaymentBankOption : DomainOption
    {
        public string icon { get; set; }
    }

    public class PaymentMethodOption : DomainOption
    {
        public string location { get; set; }
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string description { get; set; }
    }
}
