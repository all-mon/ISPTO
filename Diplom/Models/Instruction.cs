using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Diplom.Models
{
    public class Instruction
    {
        public int ID { get; set; }

        [DisplayName("Заголовок")]
        public string Name { get; set; } = "Без названия";

        [DisplayName("Краткое описание")]
        public string? Description { get; set; }

        [DisplayName("Статья")]
        public string? Content { get; set; }

        [Display(Name ="Дата создания")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
    }
}
