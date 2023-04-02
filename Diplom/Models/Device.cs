using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Diplom.Models
{
    public class Device
    {

        public int ID { get; set; }
        [DisplayName("Название")]
        public string? Name { get; set; }
        [DisplayName("Описание")]
        [MaxLength(40)]
        public string? Description { get; set; }
        
        [DisplayName("Изображение")]
        public string? ImagePath { get; set; }
        [DisplayName("Документация")]
        public string? DocumentPath { get; set; }


        public ICollection<DevicePlacement> DevicePlacements { get; set; }



    }
}
