using System.ComponentModel;

namespace Diplom.Models
{
    public class Instruction
    {
        public int ID { get; set; }
        [DisplayName("Название")]
        public string Name { get; set; } = "Без названия";
        [DisplayName("Описание")]
        public string? Description { get; set; }
        [DisplayName("Информация")]
        public string? Content { get; set; }

    }
}
