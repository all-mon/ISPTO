namespace Diplom.Models
{
    public class Placement
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public ICollection<DevicePlacement> DevicePlacements { get; set; }


    }
}
