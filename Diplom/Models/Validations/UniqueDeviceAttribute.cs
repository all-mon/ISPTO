using Diplom.Data;
using System.ComponentModel.DataAnnotations;

namespace Diplom.Models.Validations
{
    public class UniqueDeviceAttribute:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
           
            var context = (DiplomContext)validationContext.GetService(typeof(DiplomContext));
            var device = (Device)validationContext.ObjectInstance;

            if (context.Device.Any(d => d.Name == device.Name && d.ID != device.ID))
            {
                // Игнорировать проверку уникальности при изменении существующего оборудования
                if (device.ID != 0)
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    
    }
}
