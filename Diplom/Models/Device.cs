using Diplom.Models.Validations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diplom.Models
{
    public class Device
    {
        public int ID { get; set; }

        [DisplayName("Название")]
        [Required(ErrorMessage = "Введите название")]
        [UniqueDevice(ErrorMessage = "Оборудование с таким названием уже существует.")]
        [MaxLength(100)]
        [MinLength(3)]
        public string Name { get; set; } = "Empty";

        [DisplayName("Описание")]
        [MaxLength(255)]
        public string? Description { get; set; }

        [DisplayName("Изображение")]
        [MaxLength(255)]
        public string? ImagePath { get; set; } = "/images/default_item_icon.png";

        [DisplayName("Файл изображения")]
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [DisplayName("Документация")]
        [MaxLength(255)]
        public string? DocumentPath { get; set; } = "/docs/default_item_pdf.pdf";

        [DisplayName("Файл документации")]
        [NotMapped]
        public IFormFile? DocumentationFile { get; set; }

        [DisplayName("Количество в запасе")]
        [Required(ErrorMessage = "Укажите число")]
        [Range(0,1000,ErrorMessage ="Недопустимое количество")]
        public int QuantityInStock { get; set; }

        [DisplayName("Места установки")]
        public ICollection<DevicePlacement> DevicePlacements { get; set; } = new List<DevicePlacement>();

        [DisplayName("Аналоги")]
        public ICollection<AnalogDevice>? AnalogDevice { get; set; } = new List<AnalogDevice>();

    }
}

