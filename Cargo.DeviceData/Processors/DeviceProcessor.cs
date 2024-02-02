using Cargo.DeviceData.Models;
using System.Text.Json;
using Cargo.DeviceData.Helpers;
using Cargo.DeviceData.Interfaces;
using Cargo.DeviceData.DataConverters;
using System.Reflection;

namespace Cargo.DeviceData.Processors
{
    public class DeviceProcessor
    {
        // Runtime constants
        protected readonly string filePath;
        protected readonly string fullFilePath;
        protected readonly string outputPath;
        protected readonly string fullOutputPath;
        protected readonly string outputFileName;

        /// <summary>
        ///   Constructor that takes a json file 
        /// </summary>
        /// 
        /// <param name="filePath">
        ///   Full or relative path to the json file
        /// </param>
        /// 
        /// <exception cref="Exception">
        ///   If file is not found then an exception is thrown
        /// </exception>
        public DeviceProcessor(string filePath, string outputPath, string outputFileName)
        {
            this.filePath = filePath;

            // This will allow for relative pathing like ..\\path
            this.fullFilePath = $"{Path.GetFullPath(filePath)}";

            // Check if diretory exists
            if (!Directory.Exists($"{fullFilePath}"))
            {
                throw new Exception($"Error: Could not find directory: [{filePath}]");
            }

            this.outputPath = outputPath;
            this.outputFileName = outputFileName;

            // This will allow for relative pathing like ..\\path
            this.fullOutputPath = $"{Path.GetFullPath(outputPath)}";

            // Check if directory exists
            if (!Directory.Exists($"{fullOutputPath}"))
            {
                throw new Exception($"Error: Could not find directory: [{fullOutputPath}]");
            }
        }

        /// <summary>
        ///   Reads a json files from the give directory and processes them 
        /// </summary>
        ///
        ///  Creates a new file with all the devices normalized 
        /// 
        public void ProcessFiles()
        {
            string[] files = Directory.GetFiles(this.fullFilePath, "*.json");
            List<DeviceModel> deviceList = new List<DeviceModel>();

            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                IDataConverter? dataConverter = this.DetermineConverter(this.fullFilePath, fileName);

                if (dataConverter != null)
                {
                    deviceList.AddRange(dataConverter.Process());
                }
            }

            this.SerializeDeviceList(deviceList);
        }

        /// <summary>
        ///   Determines which converter to use for a file
        /// </summary>
        /// 
        /// <param name="filePath">
        ///   Path of the file to check
        /// </param>
        /// 
        /// <param name="fileName">
        ///   Name of the file to check
        /// </param>
        /// 
        /// <returns>
        ///    Null - if ho converter is fould
        ///    IDataConverter to process that file with
        /// </returns>
        protected IDataConverter? DetermineConverter(string filePath, string fileName)
        {
            try
            {
                using (Stream stream = File.OpenRead($"{filePath}\\{fileName}"))
                {
                    JsonDocument doc = JsonDocument.Parse(stream, new JsonDocumentOptions());

                    JsonElement dummyValue;
                    var node = doc.RootElement;

                    if (node.TryGetProperty("PartnerId", out dummyValue))
                    {
                        return new Foo1Converter(filePath, fileName);

                    }
                    else if (node.TryGetProperty("CompanyId", out dummyValue))
                    {
                        return new Foo2Converter(filePath, fileName);
                    }

                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        /// <summary>
        ///   Serializes a list of Device Models to a json file 
        /// </summary>
        /// 
        /// <param name="deviceList">
        ///   A list of device models to write to disk
        /// </param>
        protected void SerializeDeviceList(List<DeviceModel> deviceList)
        {
            using (FileStream fileStream = new FileStream($"{fullOutputPath}\\{outputFileName}", FileMode.Create))
            {
                JsonSerializer.Serialize<List<DeviceModel>>(fileStream, deviceList, new JsonSerializerOptions { WriteIndented = true });
            }
        }

    }

}
