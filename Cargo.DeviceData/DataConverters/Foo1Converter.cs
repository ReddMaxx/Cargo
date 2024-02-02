using Cargo.DeviceData.Helpers;
using Cargo.DeviceData.Interfaces;
using Cargo.DeviceData.Models;
using System.Text.Json;

namespace Cargo.DeviceData.DataConverters
{
    public class Foo1Converter : IDataConverter
    {
        // Compile time constants
        public const string TEMPERATURE = "temperature";
        public const string HUMIDITY = "humidty";

        // Runtime constants
        protected readonly string filePath;
        protected readonly string fileName;
        protected readonly string fullFilePath;

        /// <summary>
        ///   Constructor that takes a json file 
        /// </summary>
        /// 
        /// <param name="filePath">
        ///   Full or relative path to the json file
        /// </param>
        /// 
        /// <param name="fileName">
        ///   Name of the json file
        /// </param>
        /// 
        /// <exception cref="Exception">
        ///   If file is not found then an exception is thrown
        /// </exception>
        public Foo1Converter(string filePath, string fileName)
        {
            this.filePath = filePath;
            this.fileName = fileName;

            // This will allow for relative pathing like ..\\path
            this.fullFilePath = $"{Path.GetFullPath(filePath)}\\{fileName}";

            // Check if file exists
            if (!File.Exists($"{fullFilePath}"))
            {
                throw new Exception($"Error: Could not find file: [{filePath}\\{fileName}");
            }
        }

        /// <summary>
        ///   Reads a json file and converts the data to a list of Device Models
        /// </summary>
        /// 
        /// <returns>
        ///   A list of device models 
        /// </returns>
        public List<DeviceModel> Process()
        {
            Foo1Model? model;

            using (FileStream fileStream = new FileStream($"{fullFilePath}", FileMode.Open))
            {
                model = JsonSerializer.Deserialize<Foo1Model>(fileStream);
            }

            return ConvertData(model);
        }

        /// <summary>
        ///   Converts Foo1Model to DeviceModel 
        /// </summary>
        /// 
        /// <param name="foo1Model">
        ///   A foo1Model that will converted
        /// </param>
        /// 
        /// <returns>
        ///   Null - If the model is empty
        ///   DeviceModel - Converted from a Foo1Model
        /// </returns>
        protected List<DeviceModel> ConvertData(Foo1Model? foo1Model)
        {
            List<DeviceModel> devices = new List<DeviceModel>();

            // If model is empty 
            if (foo1Model == null)
            {
                return devices;
            }

            // Convert trackers to devices
            foreach (Foo1Model.Tracker tracker in foo1Model.Trackers)
            {
                DateTimeHelper dateTimeHelper = new DateTimeHelper();
                SensorInfo temperatureInfo = ReadStats(tracker, TEMPERATURE);
                SensorInfo humidityInfo = ReadStats(tracker, HUMIDITY);

                DeviceModel device = new DeviceModel();
                device.CompanyId = foo1Model.PartnerId;
                device.CompanyName = foo1Model.PartnerName;
                device.DeviceId = tracker.Id;
                device.DeviceName = tracker.Model;
                device.FirstReadingDtm = dateTimeHelper.EarliestDateTime(temperatureInfo.FirstReading, humidityInfo.FirstReading);
                device.LastReadingDtm = dateTimeHelper.LatestDateTime(temperatureInfo.LastReading, humidityInfo.LastReading);
                device.TemperatureCount = temperatureInfo.Count;
                device.AverageTemperature = temperatureInfo.Average;
                device.HumidityCount = humidityInfo.Count;
                device.AverageHumidity = humidityInfo.Average;

                devices.Add(device);
            }

            return devices;
        }


        /// <summary>
        ///   Read the stats from a tracker 
        /// </summary>
        /// 
        /// <param name="tracker">
        ///   A tracker with a list of sensor data
        /// </param>
        /// 
        /// <param name="infoType">
        ///   Type of sensor:
        ///     - Temperature
        ///     - Humidity
        /// </param>
        /// 
        /// <returns>
        ///   Returns the compiled info on the sensor
        /// </returns>
        protected SensorInfo ReadStats(Foo1Model.Tracker tracker, string infoType)
        {
            SensorInfo info = new SensorInfo();

            if (tracker.Sensors.Count == 0)
            {
                return info;
            }

            DateTime? firstReading = DateTime.MaxValue;
            DateTime? lastReading = DateTime.MinValue;
            int count = 0;
            double? value = 0.0;

            // Loop through all the senors
            foreach (Foo1Model.Sensor sensor in tracker.Sensors)
            {
                // If it is not the correct sensor type then skip
                if (sensor.Name.ToLower() != infoType) { continue; }

                // Loop through all the dat
                foreach (Foo1Model.Crumb crumb in sensor.Crumbs)
                {
                    if (crumb.CreatedDtm != null)
                    {
                        // Check for Last Reading
                        if (crumb.CreatedDtm > lastReading)
                        {
                            lastReading = crumb.CreatedDtm;
                        }

                        // Check for First Reading
                        if (crumb.CreatedDtm < firstReading)
                        {
                            firstReading = crumb.CreatedDtm;
                        }

                        if (crumb.Value != null)
                        {
                            count++;
                            value += crumb.Value;
                        }
                    }
                }
            }

            info.FirstReading = (firstReading == DateTime.MaxValue) ? null : firstReading;
            info.LastReading = (lastReading == DateTime.MinValue) ? null : lastReading;
            info.Count = (count == 0) ? null : count;
            info.Average = (count == 0) ? null : (value / count);

            return info;
        }

    }

}
