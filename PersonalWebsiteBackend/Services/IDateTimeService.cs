using System;

namespace PersonalWebsiteBackend.Services
{
    public interface IDateTimeService
    {
        public DateTime Now { get; }
    }
}