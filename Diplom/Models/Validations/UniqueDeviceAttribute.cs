using Diplom.Data;
using System.ComponentModel.DataAnnotations;

namespace Diplom.Models.Validations
{
    public class UniqueDeviceAttribute:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dbContext = (DiplomContext)validationContext.GetService(typeof(DiplomContext));
            var deviceName = (string)value;

            var existingDevice = dbContext.Device.FirstOrDefault(d => d.Name == deviceName);

            if (existingDevice != null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
