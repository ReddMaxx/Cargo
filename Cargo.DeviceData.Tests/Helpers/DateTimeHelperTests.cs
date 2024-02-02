using Cargo.DeviceData.Helpers;
using Xunit;

namespace Cargo.DeviceData.Tests.Helpers
{
    public class DateTimeHelperTests : BaseTest
    {
        [Theory, Trait("Category", "Unit")]
        [InlineData("01-01-2020 01:00:00", "01-01-2020 02:00:00", "01-01-2020 02:00:00")]
        [InlineData("01-01-2020 03:00:00", "01-01-2020 02:00:00", "01-01-2020 03:00:00")]
        public void CalcLastRead_ComparesDates_ReturnsAsExpected(string str1, string str2, string expectedStr)
        {
            DateTime? dateTime1 = DateTime.Parse(str1);
            DateTime? dateTime2 = DateTime.Parse(str2);
            DateTime? expectedDate = DateTime.Parse(expectedStr);

            DateTimeHelper helper = new DateTimeHelper();

            DateTime? actualDate = helper.LatestDateTime(dateTime1, dateTime2);

            Assert.Equal(expectedDate, actualDate);
        }


        [Theory, Trait("Category", "Unit")]
        [InlineData("01-01-2020 01:00:00", "01-01-2020 02:00:00", "01-01-2020 01:00:00")]
        [InlineData("01-01-2020 03:00:00", "01-01-2020 02:00:00", "01-01-2020 02:00:00")]
        public void CalcFirstRead_ComparesDates_ReturnsAsExpected(string str1, string str2, string expectedStr)
        {
            DateTime? dateTime1 = DateTime.Parse(str1);
            DateTime? dateTime2 = DateTime.Parse(str2);
            DateTime? expectedDate = DateTime.Parse(expectedStr);

            DateTimeHelper helper = new DateTimeHelper();

            DateTime? actualDate = helper.EarliestDateTime(dateTime1, dateTime2);

            Assert.Equal(expectedDate, actualDate);
        }

    }
}