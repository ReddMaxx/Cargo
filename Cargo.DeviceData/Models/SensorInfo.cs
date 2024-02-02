
namespace Cargo.DeviceData.Models
{
    /// <summary>
    ///   Common sensor info 
    /// </summary>
    public class SensorInfo
    {
        public DateTime? FirstReading { get; set; }
        public DateTime? LastReading { get; set; }
        public int? Count { get; set; }
        public double? Average { get; set; }
    }
}
