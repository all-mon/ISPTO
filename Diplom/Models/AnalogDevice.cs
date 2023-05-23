namespace Diplom.Models
{
    public class AnalogDevice
    {
        public int DeviceId { get; set; }
        public Device Device { get; set; }
        public int AnalogId { get; set; }
        public Device Analog { get; set; }
    }
}
