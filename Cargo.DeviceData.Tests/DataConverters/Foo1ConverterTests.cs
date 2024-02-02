using Cargo.DeviceData.DataConverters;
using Cargo.DeviceData.Models;
using Xunit;

namespace Cargo.DeviceData.Tests.DataConverters
{
    public class Foo1ConverterTests : BaseTest
    {
        [Theory, Trait("Category", "Unit")]
        [InlineData(".\\Files", "DeviceDataFoo1.json")]
        public void Constructor_ValidFile_Success(string filePath, string fileName)
        {
            string inPath = this.ProjectDirectory() + $"{filePath}";
            string fullPath = $"{Path.GetFullPath(inPath)}\\{fileName}";

            MockFoo1Converter converter = new MockFoo1Converter(inPath, fileName);

            Assert.True(inPath == converter.MockFilePath, $"FilePath Expect[{inPath}] Actual[{converter.MockFilePath}");
            Assert.True(fileName == converter.MockFileName, $"FileName Expect[{fileName}] Actual[{converter.MockFileName}");
            Assert.True(fullPath == converter.MockFullFilePath, $"FullFilePath Expect[{fullPath}] Actual[{converter.MockFullFilePath}");
        }


        [Theory, Trait("Category", "Unit")]
        [InlineData("BadPath", "BadFileName.json")]
        public void Constructor_BadFile_ThrowsException(string filePath, string fileName)
        {
            Exception ex = Assert.Throws<Exception>(() => new Foo1Converter(filePath, fileName));

            Assert.True(ex.Message == $"Error: Could not find file: [{filePath}\\{fileName}", $"Actual Message [{ex.Message}]");
        }


        [Theory, Trait("Category", "Unit")]
        [MemberData(nameof(ConvertValuesData))]
        public void Convert_GoodModel_ReturnDeviceModel(Foo1Model model, DeviceModel deviceModel)
        {
            string fileName = "DeviceDataFoo1.json";
            string filePath = $"{this.ProjectDirectory()}\\Files";
            MockFoo1Converter converter = new MockFoo1Converter(filePath, fileName);

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
        public void ReadStats_GoodTrackerData_Success(Foo1Model.Tracker tracker, SensorInfo expectedInfo)
        {
            string fileName = "DeviceDataFoo1.json";
            string filePath = $"{this.ProjectDirectory()}\\Files";
            MockFoo1Converter converter = new MockFoo1Converter(filePath, fileName);

            SensorInfo actualInfo = converter.TestReadStats(tracker, Foo1Converter.TEMPERATURE);

            Assert.Equal(expectedInfo.FirstReading, actualInfo.FirstReading);
            Assert.Equal(expectedInfo.LastReading, actualInfo.LastReading);
            Assert.Equal(expectedInfo.Count, actualInfo.Count);
            if (expectedInfo.Average == null)
            { Assert.Equal(expectedInfo.Average, actualInfo.Average); }
            else
            { Assert.Equal((double)(expectedInfo.Average), (double)(actualInfo.Average), 2); }
        }


        [Theory, Trait("Category", "Unit")]
        [InlineData(".\\Files", "DeviceDataFoo1.json")]
        public void Process_GoodJsonFile_GoodDeviceModel(string filePath, string fileName)
        {
            string inPath = this.ProjectDirectory() + $"{filePath}";
            Foo1Converter converter = new Foo1Converter(inPath, fileName);

            List<DeviceModel> list = converter.Process();

            Assert.Equal(2, list.Count);
        }


        /// <summary>
        ///   Test Data used for Convert test 
        /// </summary>
        public static IEnumerable<object[]> ConvertValuesData = new List<object[]>
        {
            new object[] {
                new Foo1Model()
                {
                    PartnerId = 100,
                    PartnerName = "Foo1",
                    Trackers = new List<Foo1Model.Tracker>()
                    {
                        new Foo1Model.Tracker()
                        {
                            Id = 1,
                            Model = "ABC-100",
                            ShipmentStartDtm = DateTime.Parse("01-01-2020 01:00:00"),
                            Sensors = new List<Foo1Model.Sensor>()
                            {
                                new Foo1Model.Sensor()
                                {
                                    Id = 100,
                                    Name = "Temperature",
                                    Crumbs = new List<Foo1Model.Crumb>()
                                    {
                                        new Foo1Model.Crumb()
                                        {
                                            CreatedDtm = DateTime.Parse("01-01-2020 01:01:00"),
                                            Value = 2.2
                                        }
                                    }
                                },
                                new Foo1Model.Sensor()
                                {
                                    Id = 100,
                                    Name = "Temperature",
                                    Crumbs = new List<Foo1Model.Crumb>()
                                    {
                                        new Foo1Model.Crumb()
                                        {
                                            CreatedDtm = DateTime.Parse("01-01-2020 02:02:00"),
                                            Value = 4.4
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new DeviceModel
                {
                    CompanyId = 100,
                    CompanyName = "Foo1",
                    DeviceId = 1,
                    DeviceName = "ABC-100",
                    FirstReadingDtm = DateTime.Parse("01-01-2020 01:01:00"),
                    LastReadingDtm = DateTime.Parse("01-01-2020 02:02:00"),
                    TemperatureCount = 2,
                    AverageTemperature = 3.3,
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
                new Foo1Model.Tracker()
                {
                    Sensors = new List<Foo1Model.Sensor>()
                    {
                        new Foo1Model.Sensor()
                        {
                            Name = "Temperature",
                            Crumbs = new List<Foo1Model.Crumb>()
                            {
                                new Foo1Model.Crumb()
                                {
                                    CreatedDtm = DateTime.Parse("01-01-2020 01:01:00"),
                                    Value = 1.1
                                },
                                new Foo1Model.Crumb()
                                {
                                    CreatedDtm = DateTime.Parse("01-01-2020 04:01:00"),
                                    Value = 2.2
                                },
                                new Foo1Model.Crumb()
                                {
                                    CreatedDtm = DateTime.Parse("01-01-2020 03:01:00"),
                                    Value = 3.3
                                },
                            }
                        },
                        new Foo1Model.Sensor()
                        {
                            Name = "Humidity",
                            Crumbs = new List<Foo1Model.Crumb>()
                            {
                                new Foo1Model.Crumb()
                                {
                                    CreatedDtm = DateTime.Parse("01-01-2020 01:00:00"),
                                    Value = 2.1
                                },
                                new Foo1Model.Crumb()
                                {
                                    CreatedDtm = DateTime.Parse("01-01-2020 02:01:00"),
                                    Value = 2.2
                                },
                                new Foo1Model.Crumb()
                                {
                                    CreatedDtm = DateTime.Parse("01-01-2020 03:01:00"),
                                    Value = null
                                },
                            }
                        },
                    }
                },
                new SensorInfo()
                {
                    FirstReading = DateTime.Parse("01-01-2020 01:01:00"),
                    LastReading = DateTime.Parse("01-01-2020 04:01:00"),
                    Count = 3,
                    Average = 2.2
                }
            }

        };

    }

    /// <summary>
    ///  This mock class allow testing of the protected members
    /// </summary>
    public class MockFoo1Converter : Foo1Converter
    {
        public MockFoo1Converter(string filePath, string fileName) : base(filePath, fileName) { }

        public string MockFilePath { get { return filePath; } protected set { } }
        public string MockFileName { get { return fileName; } protected set { } }
        public string MockFullFilePath { get { return fullFilePath; } protected set { } }

        public List<DeviceModel> TestConvertData(Foo1Model? foo1Model) { return this.ConvertData(foo1Model); }
        public SensorInfo TestReadStats(Foo1Model.Tracker tracker, string infoType) { return this.ReadStats(tracker, infoType); }
    }
}