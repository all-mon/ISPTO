﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Diplom.Models
{
    public class LogEntry
    {
        public int ID { get; set; }
        [DisplayName("Заголовок")]
        [MaxLength(255)]
        public string Name { get; set; } = "Без названия";
        [DisplayName("Описание")]
        public string? Description { get; set; }
        [DisplayName("Дата записи")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Дата выполнения")]
        public DateTime Date { get; set;}
        [DisplayName("Исполнитель")]
        public string Executor { get; set; } = "Не известно";
    }
}
