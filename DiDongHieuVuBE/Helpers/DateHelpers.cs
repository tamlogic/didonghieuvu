using System;
namespace ManageEmployee.Helpers
{
    public class DateHelpers
    {
        public static int UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (int)timeSpan.TotalSeconds;
        }

        /// <summary>
        /// date : dd/MM/yyyy
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ConvertToDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }
            var dateSplit = date.Split('/');
            if (dateSplit.Length >= 3)
            {
                return new DateTime(Int32.Parse(dateSplit[2]), Int32.Parse(dateSplit[1]), Int32.Parse(dateSplit[0]));
            }
            else
            {
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static double DateTimeToUnixTimeStamp(DateTime time)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).ToLocalTime();
            var epoch = (time - dtDateTime).TotalSeconds;
            return epoch;
        }
    }
}
