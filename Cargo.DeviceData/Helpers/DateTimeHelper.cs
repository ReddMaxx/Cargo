
namespace Cargo.DeviceData.Helpers
{
    public class DateTimeHelper
    {
        /// <summary>
        ///   Finds the earliest date
        /// </summary>
        /// 
        /// <param name="dateTime1">
        ///   Datetime to compare
        /// </param>
        /// 
        /// <param name="dateTime2">
        ///   Datetime to compare
        /// </param>
        /// 
        /// <returns>
        ///   Returns the earliest of the two dates
        /// </returns>
        public DateTime? EarliestDateTime(DateTime? dateTime1, DateTime? dateTime2)
        {
            if (dateTime1 == null)
            {
                return dateTime2;
            }
            else if (dateTime2 == null)
            {
                return dateTime1;
            }

            return (dateTime1 < dateTime2) ? dateTime1 : dateTime2;
        }

        /// <summary>
        ///   Finds the later date
        /// </summary>
        /// 
        /// <param name="dateTime1">
        ///   Datetime to compare
        /// </param>
        /// 
        /// <param name="dateTime2">
        ///   Datetime to compare
        /// </param>
        /// 
        /// <returns>
        ///   Returns the later of the two dates
        /// </returns>
        public DateTime? LatestDateTime(DateTime? dateTime1, DateTime? dateTime2)
        {
            if (dateTime1 == null)
            {
                return dateTime2;
            }
            else if (dateTime2 == null)
            {
                return dateTime1;
            }

            return (dateTime1 > dateTime2) ? dateTime1 : dateTime2;
        }
    }
}
