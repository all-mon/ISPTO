using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Diplom.Models
{
    public class Device
    {

        public int ID { get; set; }
        [DisplayName("Название")]
        [Required(ErrorMessage ="Введите название")]
        [MaxLength(100)]
        public string Name { get; set; } = "Empty";
        [DisplayName("Описание")]
        [MaxLength(255)]
        public string? Description { get; set; }
        
        [DisplayName("Изображение")]
        [MaxLength(255)]
        public string? ImagePath { get; set; }
        [DisplayName("Документация")]
        [MaxLength(255)]
        public string? DocumentPath { get; set; }
        [DisplayName("Аналоги")]
        public string? Analogue { get; set; }
        [DisplayName("Количество в запасе")]
        public int QuantityInStock { get; set; }

        [DisplayName("Места установки")]
        public ICollection<DevicePlacement>? DevicePlacements { get; set; }



    }
}
