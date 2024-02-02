using Cargo.DeviceData.Helpers;
using System.Text.Json.Serialization;

namespace Cargo.DeviceData.Models
{
    public class Foo2Model
    {
        public int CompanyId { get; set; }
        public string Company { get; set; } = "";
        public List<Device> Devices { get; set; } = new List<Device>();

        public class Device
        {
            public int? DeviceID { get; set; }
            public string Name { get; set; } = "";
            [JsonConverter(typeof(DateTimeNullableConverter))]
            public DateTime? StartDateTime { get; set; }

            [JsonPropertyName("SensorData")]
            public List<SensorData> SensorDataList { get; set; } = new List<SensorData>();
        }

        public class SensorData
        {
            public string SensorType { get; set; } = "";
            [JsonConverter(typeof(DateTimeNullableConverter))]
            public DateTime? DateTime { get; set; }
            public double? Value { get; set; }
        }
    }
}
