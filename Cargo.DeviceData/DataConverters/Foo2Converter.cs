using Cargo.DeviceData.Helpers;
using Cargo.DeviceData.Interfaces;
using Cargo.DeviceData.Models;
using System.Text.Json;

namespace Cargo.DeviceData.DataConverters
{
    public class Foo2Converter :IDataConverter
    {
        // Compile time constants
        public const string TEMPERATURE = "temp";
        public const string HUMIDITY = "hum";

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
        public Foo2Converter(string filePath, string fileName)
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
            Foo2Model? model;

            using (FileStream fileStream = new FileStream($"{fullFilePath}", FileMode.Open))
            {
                model = JsonSerializer.Deserialize<Foo2Model>(fileStream);
            }

            return ConvertData(model);
        }

        /// <summary>
        ///   Converts Foo2Model to DeviceModel 
        /// </summary>
        /// 
        /// <param name="foo2Model">
        ///   A foo2Model that will converted
        /// </param>
        /// 
        /// <returns>
        ///   Null - If the model is empty
        ///   DeviceModel - Converted from a Foo2Model
        /// </returns>
        protected List<DeviceModel> ConvertData(Foo2Model? foo2Model)
        {
            List<DeviceModel> devices = new List<DeviceModel>();

            // If model is empty 
            if (foo2Model == null)
            {
                return devices;
            }

            // Convert trackers to devices
            foreach (Foo2Model.Device foo2Device in foo2Model.Devices)
            {
                DateTimeHelper dateTimeHelper = new DateTimeHelper();
                SensorInfo temperatureInfo = ReadStats(foo2Device, TEMPERATURE);
                SensorInfo humidityInfo = ReadStats(foo2Device, HUMIDITY);

                DeviceModel device = new DeviceModel();
                device.CompanyId = foo2Model.CompanyId;
                device.CompanyName = foo2Model.Company;
                device.DeviceId = foo2Device.DeviceID;
                device.DeviceName = foo2Device.Name;
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
        ///   Read the stats from a device 
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
        protected SensorInfo ReadStats(Foo2Model.Device device, string infoType)
        {
            SensorInfo info = new SensorInfo();

            if (device.SensorDataList.Count == 0)
            {
                return info;
            }

            DateTime? firstReading = DateTime.MaxValue;
            DateTime? lastReading = DateTime.MinValue;
            int count = 0;
            double? value = 0.0;

            // Loop through all the senors
            foreach (Foo2Model.SensorData sensorData in device.SensorDataList)
            {
                // If it is not the correct sensor type then skip
                if (sensorData.SensorType.ToLower() != infoType) { continue; }

                // Check for Last Reading
                if (sensorData.DateTime > lastReading)
                {
                    lastReading = sensorData.DateTime;
                }

                // Check for First Reading
                if (sensorData.DateTime < firstReading)
                {
                    firstReading = sensorData.DateTime;
                }

                if (sensorData.Value != null)
                {
                    count++;
                    value += sensorData.Value;
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
