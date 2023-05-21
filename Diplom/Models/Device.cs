﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Diplom.Models
{
    public class Device
    {
        public int ID { get; set; }

        [DisplayName("Название")]
        [Required(ErrorMessage = "Введите название")]
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

        [DisplayName("Количество в запасе")]
        [Required(ErrorMessage = "Укажите число")]
        public int QuantityInStock { get; set; }

        [DisplayName("Места установки")]
        public ICollection<DevicePlacement>? DevicePlacements { get; set; }

        [DisplayName("Аналоги")]
        public ICollection<Device> Analogues { get; set; } = new List<Device>();
    }
}

