using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcursionsDomain.Helpers
{
    public class Validatorcs
    {
        public static ValidationResult ValidateDate(DateTime date, ValidationContext context)
        {
            if (date.Date < DateTime.Today)
            {
                return new ValidationResult("Дата не може бути раніше сьогоднішньої", new List<string> { context.MemberName }.ToArray());
            }
            return ValidationResult.Success;
        }
    }
}
