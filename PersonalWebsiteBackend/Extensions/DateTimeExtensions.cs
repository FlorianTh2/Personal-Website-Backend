using System;

namespace PersonalWebsiteBackend.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ConvertToISO8601String(this DateTime datetime)
        {
            return datetime.ToString("o");
        }
    }
}