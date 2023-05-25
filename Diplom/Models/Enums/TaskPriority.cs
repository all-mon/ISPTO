using System.ComponentModel.DataAnnotations;

namespace Diplom.Models.Enums
{
   public enum TaskPriority
    {
        [Display(Name ="Низкий")]
        Low,
        [Display(Name = "Средний")]
        Medium,
        [Display(Name = "Высокий")]
        High
    }
}
