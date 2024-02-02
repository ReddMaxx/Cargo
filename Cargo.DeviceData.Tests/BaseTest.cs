
using Xunit;

namespace Cargo.DeviceData.Tests
{
    // This turns off parallelization so we can test against the same files 
    [CollectionDefinition(nameof(DatabaseParallelizationCollection), DisableParallelization = true)]
    public class DatabaseParallelizationCollection { }

    [Collection(nameof(DatabaseParallelizationCollection))]
    public class BaseTest
    {
        /// <summary>
        ///   Finds the Project directory test was ran out of
        /// </summary>
        /// 
        /// <returns>
        ///   Returns the project directory test ran under
        /// </returns>
        public string ProjectDirectory()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            int pos = currentDirectory.IndexOf("\\bin\\");
            return currentDirectory.Substring(0, pos) + "\\";
        }
    }
}
