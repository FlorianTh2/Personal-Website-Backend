using System;

namespace PersonalWebsiteBackend.Services
{
    public class DateTimeService : IDateTime
    {
        DateTime IDateTime.Now => DateTime.Now;
    }
}