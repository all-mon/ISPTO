using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Diplom.Models
{
    public class Device
    {

        public int ID { get; set; }
        [DisplayName("Название")]
        [MaxLength(255)]
        public string? Name { get; set; }
        [DisplayName("Описание")]
        [MaxLength(255)]
        public string? Description { get; set; }
        
        [DisplayName("Изображение")]
        [MaxLength(255)]
        public string? ImagePath { get; set; }
        [DisplayName("Документация")]
        [MaxLength(255)]
        public string? DocumentPath { get; set; }


        public ICollection<DevicePlacement>? DevicePlacements { get; set; }



    }
}
