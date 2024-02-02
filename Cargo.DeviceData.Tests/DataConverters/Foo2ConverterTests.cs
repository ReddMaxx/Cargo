using Cargo.DeviceData.DataConverters;
using Cargo.DeviceData.Models;
using Xunit;

namespace Cargo.DeviceData.Tests.DataConverters
{
    public class Foo2ConverterTests : BaseTest
    {
        [Theory, Trait("Category", "Unit")]
        [InlineData(".\\Files", "DeviceDataFoo1.json")]
        public void Constructor_ValidFile_Success(string filePath, string fileName)
        {
            string inPath = this.ProjectDirectory() + $"{filePath}";
            string fullPath = $"{Path.GetFullPath(inPath)}\\{fileName}";

            MockFoo2Converter converter = new MockFoo2Converter(inPath, fileName);

            Assert.True(inPath == converter.MockFilePath, $"FilePath Expect[{inPath}] Actual[{converter.MockFilePath}");
            Assert.True(fileName == converter.MockFileName, $"FileName Expect[{fileName}] Actual[{converter.MockFileName}");
            Assert.True(fullPath == converter.MockFullFilePath, $"FullFilePath Expect[{fullPath}] Actual[{converter.MockFullFilePath}");
        }


        [Theory, Trait("Category", "Unit")]
        [InlineData("BadPath", "BadFileName.json")]
        public void Constructor_BadFile_ThrowsException(string filePath, string fileName)
        {
            Exception ex = Assert.Throws<Exception>(() => new Foo2Converter(filePath, fileName));

            Assert.True(ex.Message == $"Error: Could not find file: [{filePath}\\{fileName}", $"Actual Message [{ex.Message}]");
        }


        [Theory, Trait("Category", "Unit")]
        [MemberData(nameof(ConvertValuesData))]
        public void Convert_GoodModel_ReturnDeviceModel(Foo2Model model, DeviceModel deviceModel)
        {
            string fileName = "DeviceDataFoo2.json";
            string filePath = $"{this.ProjectDirectory()}\\Files";
            MockFoo2Converter converter = new MockFoo2Converter(filePath, fileName);

            List<DeviceModel> list = converter.TestConvertData(model);

            DeviceModel actualModel = list[0];
            Assert.Equal(deviceModel.CompanyId, actualModel.CompanyId);
            Assert.Equal(deviceModel.CompanyName, actualModel.CompanyName);
            Assert.Equal(deviceModel.DeviceId, actualModel.DeviceId);
            Assert.Equal(deviceModel.FirstReadingDtm, actualModel.FirstReadingDtm);
            Assert.Equal(deviceModel.LastReadingDtm, actualModel.LastReadingDtm);
            Assert.Equal(deviceModel.TemperatureCount, actualModel.TemperatureCount);
            Assert.Equal(deviceModel.HumidityCount, actualModel.HumidityCount);

            if (deviceModel.AverageTemperature == null)
            { Assert.Equal(deviceModel.AverageTemperature, actualModel.AverageTemperature); }
            else
            { Assert.Equal((double)(deviceModel.AverageTemperature), (double)(actualModel.AverageTemperature), 2); }

            if (deviceModel.AverageHumidity == null)
            { Assert.Equal(deviceModel.AverageHumidity, actualModel.AverageHumidity); }
            else
            { Assert.Equal((double)(deviceModel.AverageHumidity), (double)(actualModel.AverageHumidity), 2); }
        }


        [Theory, Trait("Category", "Unit")]
        [MemberData(nameof(ReadStatsValuesData))]
        public void ReadStats_GoodDeviceData_Success(Foo2Model.Device device, SensorInfo expectedInfo)
        {
            string fileName = "DeviceDataFoo2.json";
            string filePath = $"{this.ProjectDirectory()}\\Files";
            MockFoo2Converter converter = new MockFoo2Converter(filePath, fileName);

            SensorInfo actualInfo = converter.TestReadStats(device, Foo2Converter.TEMPERATURE);

            Assert.Equal(expectedInfo.FirstReading, actualInfo.FirstReading);
            Assert.Equal(expectedInfo.LastReading, actualInfo.LastReading);
            Assert.Equal(expectedInfo.Count, actualInfo.Count);
            if (expectedInfo.Average == null)
            { Assert.Equal(expectedInfo.Average, actualInfo.Average); }
            else
            { Assert.Equal((double)(expectedInfo.Average), (double)(actualInfo.Average), 2); }
        }


        [Theory, Trait("Category", "Unit")]
        [InlineData(".\\Files", "DeviceDataFoo2.json")]
        public void Process_GoodJsonFile_GoodDeviceModel(string filePath, string fileName)
        {
            string inPath = this.ProjectDirectory() + $"{filePath}";
            Foo2Converter converter = new Foo2Converter(inPath, fileName);

            List<DeviceModel> list = converter.Process();

            Assert.Equal(2, list.Count);
        }


        /// <summary>
        ///   Test Data used for Convert test 
        /// </summary>
        public static IEnumerable<object[]> ConvertValuesData = new List<object[]>
        {
            new object[] {
                new Foo2Model()
                {
                    CompanyId = 100,
                    Company = "Foo2",
                    Devices = new List<Foo2Model.Device>()
                    {
                        new Foo2Model.Device()
                        {
                            DeviceID = 10,
                            Name = "XYZ-100",
                            StartDateTime = DateTime.Parse("01-01-2020 01:00:00"),
                            SensorDataList = new List<Foo2Model.SensorData>()
                            {
                                new Foo2Model.SensorData()
                                {
                                    SensorType = "TEMP",
                                    DateTime = DateTime.Parse("01-01-2020 02:01:00"),
                                    Value = 2.2
                                },
                                new Foo2Model.SensorData()
                                {
                                    SensorType = "TEMP",
                                    DateTime = DateTime.Parse("01-01-2020 01:01:00"),
                                    Value = 3.2
                                },
                            }
                        }
                    }
                },
                new DeviceModel
                {
                    CompanyId = 100,
                    CompanyName = "Foo2",
                    DeviceId = 10,
                    DeviceName = "XYZ-100",
                    FirstReadingDtm = DateTime.Parse("01-01-2020 01:01:00"),
                    LastReadingDtm = DateTime.Parse("01-01-2020 02:01:00"),
                    TemperatureCount = 2,
                    AverageTemperature = 2.7,
                    HumidityCount = null,
                    AverageHumidity = null
                }
            }
        };


        /// <summary>
        ///   Test Data used for the ReadStats test
        /// </summary>
        public static IEnumerable<object[]> ReadStatsValuesData = new List<object[]>
        {
            new object[] {
                new Foo2Model.Device()
                {
                    DeviceID = 10,
                    Name = "XYZ-100",
                    StartDateTime = DateTime.Parse("01-01-2020 01:00:00"),
                    SensorDataList = new List<Foo2Model.SensorData>()
                    {
                        new Foo2Model.SensorData()
                        {
                            SensorType = "TEMP",
                            DateTime = DateTime.Parse("01-01-2020 01:01:00"),
                            Value = 2.2
                        },
                        new Foo2Model.SensorData()
                        {
                            SensorType = "TEMP",
                            DateTime = DateTime.Parse("01-01-2020 04:01:00"),
                            Value = 3.2
                        },
                        new Foo2Model.SensorData()
                        {
                            SensorType = "HUM",
                            DateTime = DateTime.Parse("01-01-2020 03:01:00"),
                            Value = 1.2
                        },
                        new Foo2Model.SensorData()
                        {
                            SensorType = "HUM",
                            DateTime = DateTime.Parse("01-01-2020 05:01:00"),
                            Value = 2.2
                        },
                    }
                },
                new SensorInfo
                {
                    FirstReading = DateTime.Parse("01-01-2020 01:01:00"),
                    LastReading = DateTime.Parse("01-01-2020 04:01:00"),
                    Count = 2,
                    Average = 2.7
                }
            }

        };

    }

    /// <summary>
    ///  This mock class allow testing of the protected members
    /// </summary>
    public class MockFoo2Converter : Foo2Converter
    {
        public MockFoo2Converter(string filePath, string fileName) : base(filePath, fileName) { }

        public string MockFilePath { get { return filePath; } protected set { } }
        public string MockFileName { get { return fileName; } protected set { } }
        public string MockFullFilePath { get { return fullFilePath; } protected set { } }

        public List<DeviceModel> TestConvertData(Foo2Model? foo2Model) { return this.ConvertData(foo2Model); }
        public SensorInfo TestReadStats(Foo2Model.Device device, string infoType) { return this.ReadStats(device, infoType); }
    }
}