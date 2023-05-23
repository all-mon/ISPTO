using Diplom.Models.Enums;
using System.ComponentModel;

namespace Diplom.Models
{
    public class Goal
    {
        public int ID { get; set; }
        [DisplayName("Название")]
        public string Name { get; set; } = "Empty";
        [DisplayName("Описание")]
        public string? Description { get; set; }
        [DisplayName("Дата создания")]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Приоритет")]
        public TaskPriority Priority { get; set; }

       
    }
   
}
