using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.DomainRequests
{
    public class LargerStartTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            double? eDateValue = (double?)value;
            double? sDateValue = (double?)validationContext.ObjectType.GetProperty("sTime")?.GetValue(validationContext.ObjectInstance);

            if (sDateValue.HasValue && eDateValue.HasValue && sDateValue >= eDateValue)
            {
                return new ValidationResult(MessageContants.cp_date);
            }

            return ValidationResult.Success;
        }
    }

    public class LargerFromDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            double? eDateValue = (double?)value;
            double? sDateValue = (double?)validationContext.ObjectType.GetProperty("fromDate")?.GetValue(validationContext.ObjectInstance);

            if (sDateValue.HasValue && eDateValue.HasValue && sDateValue >= eDateValue)
            {
                return new ValidationResult(MessageContants.cp_date);
            }

            return ValidationResult.Success;
        }
    }

}
