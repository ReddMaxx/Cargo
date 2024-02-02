using Cargo.DeviceData.DataConverters;
using Cargo.DeviceData.Interfaces;
using Cargo.DeviceData.Processors;
using Xunit;

namespace Cargo.DeviceData.Tests.Processors
{
    public class DeviceProcessorTests : BaseTest
    {
        [Theory, Trait("Category", "Unit")]
        [InlineData(".\\Files", "DeviceDataFoo1.json", ".\\Temp")]
        public void Constructor_ValidFile_Success(string filePath, string fileName, string reportPath)
        {
            string inPath = this.ProjectDirectory() + $"{filePath}";
            string fullPath = $"{Path.GetFullPath(inPath)}";
            string outPath = this.ProjectDirectory() + $"{reportPath}";
            string fullOutPath = $"{Path.GetFullPath(outPath)}";

            MockDeviceProcessor converter = new MockDeviceProcessor(inPath, outPath, fileName);

            Assert.True(inPath == converter.MockFilePath, $"FilePath Expect[{inPath}] Actual[{converter.MockFilePath}");
            Assert.True(fullPath == converter.MockFullFilePath, $"FullFilePath Expect[{fullPath}] Actual[{converter.MockFullFilePath}");
            Assert.True(outPath == converter.MockOutputPath, $"FilePath Expect[{outPath}] Actual[{converter.MockOutputPath}");
            Assert.True(fullOutPath == converter.MockFullOutputPath, $"FullOutputPath Expect[{fullOutPath}] Actual[{converter.MockFullOutputPath}");
            Assert.True(fileName == converter.MockOutputFileName, $"FileName Expect[{fileName}] Actual[{converter.MockOutputFileName}");
        }


        [Theory, Trait("Category", "Unit")]
        [InlineData("BadPath", "BadOutPath", "report.json")]
        public void Constructor_BadInPath_ThrowsException(string filePath, string outPath, string outputFileName)
        {
            Exception ex = Assert.Throws<Exception>(() => new DeviceProcessor(filePath, outPath, outputFileName));

            Assert.True(ex.Message == $"Error: Could not find directory: [{filePath}]", $"Actual Message [{ex.Message}]");
        }

        [Theory, Trait("Category", "Unit")]
        [InlineData(".\\Files", "C:\\BadOutPath", "report.json")]
        public void Constructor_BadOutPath_ThrowsException(string filePath, string outPath, string outputFileName)
        {
            string inPath = this.ProjectDirectory() + $"{filePath}";
            
            Exception ex = Assert.Throws<Exception>(() => new DeviceProcessor(inPath, outPath, outputFileName));

            Assert.True(ex.Message == $"Error: Could not find directory: [{outPath}]", $"Actual Message [{ex.Message}]");
        }


        [Theory, Trait("Category", "Unit")]
        [InlineData("DeviceDataFoo1.json", typeof(Foo1Converter))]
        [InlineData("DeviceDataFoo2.json", typeof(Foo2Converter))]
        public void DetermineConverter_ValidFile_FindsConverter(string inFileName, Type type)
        {
            string inPath = $"{this.ProjectDirectory()}\\Files";
            string outPath = $"{this.ProjectDirectory()}\\Temp";

            MockDeviceProcessor processor = new MockDeviceProcessor(inPath, outPath, "noname" );

            IDataConverter? dataConverter = processor.TestDetermineConverter(inPath, inFileName);

            Assert.True(dataConverter != null);
            Assert.True(type == dataConverter.GetType());
        }


        [Theory, Trait("Category", "Unit")]
        [InlineData("Merged.json", "MergedCompare.json")]
        public void ProcessFile_ValidDirectory_CreateFile(string outputFileName, string compareFile)
        {
            string inPath = $"{this.ProjectDirectory()}\\Files";
            string outPath = $"{this.ProjectDirectory()}\\Temp";
            DeviceProcessor processor = new DeviceProcessor(inPath, outPath, outputFileName );

            processor.ProcessFiles();
        
            // Compare create file to a known good runn
            int lineCount = 0;
            using (StreamReader expectedReader = new StreamReader($"{inPath}\\{compareFile}"))
            {
                using (StreamReader reader = new StreamReader($"{outPath}\\{outputFileName}"))
                {
                    string? expected = "";
                    string? actual = "";

                    do
                    {
                        lineCount++;
                        expected = expectedReader.ReadLine();
                        actual = reader.ReadLine();

                        Assert.True(string.Compare(expected, actual) == 0,
                          $"Line: {lineCount} \r\n" +
                          $"Expected: [{expected}] \r\n" +
                          $"Actual:   [{actual}] \r\n"
                        );
                    } while ((expected != null) || (actual != null));

                }
            }
        }

    }

    /// <summary>
    ///  This mock class allow testing of the protected members
    /// </summary>
    public class MockDeviceProcessor : DeviceProcessor
    {
        public MockDeviceProcessor(string filePath, string outputPath, string outputFileName) : base(filePath, outputPath, outputFileName) { }

        public string MockFilePath { get { return filePath; } protected set { } }
        public string MockFullFilePath { get { return fullFilePath; } protected set { } }
        public string MockOutputPath { get { return outputPath; } protected set { } }
        public string MockFullOutputPath { get { return fullOutputPath; } protected set { } }
        public string MockOutputFileName { get { return outputFileName; } protected set { } }

        public IDataConverter? TestDetermineConverter(string filePath, string fileName) { return this.DetermineConverter(filePath, fileName); }
    }
}