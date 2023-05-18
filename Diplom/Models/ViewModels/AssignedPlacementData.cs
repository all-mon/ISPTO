namespace Diplom.Models.ViewModels
{
    //вспомогательная модель для выбора мест установки устройства
    public class AssignedPlacementData
    {
        public int PlacementID { get; set; }
        public string? Title { get; set; }
        public bool Assigned { get; set; }
    }
}
