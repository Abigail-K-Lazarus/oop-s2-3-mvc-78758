using System;
using System.Globalization;

namespace AcmeGlobalCollege.Helpers
{
    public static class DateTimeExtensions
    {
        public static int GetWeekOfMonth(this DateTime date)
        {
            var firstDay = new DateTime(date.Year, date.Month, 1);

            int firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                firstDay,
                CalendarWeekRule.FirstDay,
                DayOfWeek.Monday);

            int currentWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                date,
                CalendarWeekRule.FirstDay,
                DayOfWeek.Monday);

            return currentWeek - firstWeek + 1;
        }
    }
}
