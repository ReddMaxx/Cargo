using Cargo.DeviceData.Helpers;
using System.Text.Json.Serialization;

namespace Cargo.DeviceData.Models
{
    public class Foo1Model
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; } = "";
        public List<Tracker> Trackers { get; set; } = new List<Tracker>();

        public class Tracker
        {
            public int? Id { get; set; }
            public string Model { get; set; } = "";

            [JsonConverter(typeof(DateTimeNullableConverter))]
            public DateTime? ShipmentStartDtm { get; set; }
            public List<Sensor> Sensors { get; set; } = new List<Sensor>();
        }

        public class Sensor
        {
            public int? Id { get; set; }
            public string Name { get; set; } = "";
            public List<Crumb> Crumbs { get; set; } = new List<Crumb>();
        }

        public class Crumb
        {
            [JsonConverter(typeof(DateTimeNullableConverter))]
            public DateTime? CreatedDtm { get; set; }
            public double? Value { get; set; }
        }
    }

}
