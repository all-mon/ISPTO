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
        //[UniqueDevice(ErrorMessage = "Оборудование с таким названием уже существует.")]
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
        public IFormFile ImageFile { get; set; }

        [DisplayName("Документация")]
        [MaxLength(255)]
        public string? DocumentPath { get; set; } = "/docs/default_pdf_icon.pdf";

        [DisplayName("Файл документации")]
        [NotMapped]
        public IFormFile DocumentationFile { get; set; }

        [DisplayName("Количество в запасе")]
        [Required(ErrorMessage = "Укажите число")]
        public int QuantityInStock { get; set; }

        [DisplayName("Места установки")]
        public ICollection<DevicePlacement>? DevicePlacements { get; set; }

        [DisplayName("Аналоги")]
        public ICollection<Device> Analogues { get; set; } = new List<Device>();
    }
}

