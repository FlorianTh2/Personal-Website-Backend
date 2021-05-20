using System;

namespace PersonalWebsiteBackend.Services
{
    public class MessageService : IMessageService
    {
        private readonly IDateTimeService _dataTimeService;

        public MessageService(IDateTimeService dataTimeService)
        {
            _dataTimeService = dataTimeService;
        }
        public void Send(string message)
        {
            Console.WriteLine("[ " + _dataTimeService.Now + " ] " + message);
        }
    }
}