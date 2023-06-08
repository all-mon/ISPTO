using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Diplom.Models
{
    public class Placement
    {
        public int ID { get; set; }
        [DisplayName("Название")]
        [MaxLength(255)]
        public string Name { get; set; } = "Без названия";
        [DisplayName("Описание")]
        [MaxLength(600)]
        public string? Description { get; set; }
        [DisplayName("Используемое оборудование")]
        public ICollection<DevicePlacement>? DevicePlacements { get; set; }


    }
}
