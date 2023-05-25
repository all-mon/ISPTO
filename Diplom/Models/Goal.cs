using Diplom.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Diplom.Models
{
    public class Goal
    {
        public int ID { get; set; }
        [DisplayName("Название")]
        public string Name { get; set; } = "Empty";

        [DisplayName("Описание")]
        public string? Description { get; set; }

        [DisplayName("Дата выполнения")]
        [DataType(DataType.Date)]
        public DateTime TaskDate { get; set; }

        [DisplayName("Приоритет")]
        [EnumDataType(typeof(TaskPriority))]
        public TaskPriority Priority { get; set; }

        [Display(Name="Статус")]
        public bool IsCompleted { get; set; }
    }
   
}
