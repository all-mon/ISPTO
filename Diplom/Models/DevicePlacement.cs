namespace Diplom.Models
{
    public class DevicePlacement
    {
        public int Id { get; set; }
        public int DeviceID { get; set; }
        public int PlacementID { get; set; }

        public Device Device { get; set; }
        public Placement Placement { get; set; }

    }
}