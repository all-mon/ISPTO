﻿using System.ComponentModel;

namespace Diplom.Models
{
    public class Placement
    {
        public int ID { get; set; }
        [DisplayName("Название")]
        public string Name { get; set; } = "Без названия";
        [DisplayName("Описание")]
        public string? Description { get; set; }
        [DisplayName("Используемое оборудование")]
        public ICollection<DevicePlacement>? DevicePlacements { get; set; }


    }
}
