using System;

namespace PersonalWebsiteBackend.Services
{
    public class DateTimeServiceService : IDateTimeService
    {
        DateTime IDateTimeService.Now => DateTime.Now;
    }
}