using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Hosting;

namespace PersonalWebsiteBackend.Services
{
    public class RecurringJobsService : BackgroundService
    {
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly IRecurringJobManager _recurringJobs; 
        
        public RecurringJobsService(
            // hangfire service for fire-and-forget jobs
            IBackgroundJobClient backgroundJobs,
            // hangfire service for recurring jobs
            IRecurringJobManager recurringJobs)
        {
            _backgroundJobs = backgroundJobs;
            _recurringJobs = recurringJobs;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            AddStartingMessageJob();
            // AddHelloSecondsJobs();
            AddProjectUpdateJob();
            AddDocumentUpdateJob();
            return Task.CompletedTask;
        }

        private void AddStartingMessageJob()
        { 
            _backgroundJobs.Enqueue<MessageService>(a => a.Send("Start Hangfire"));
        }
        public void AddHelloSecondsJobs()
        {
            string jobId = "seconds";
            _recurringJobs.RemoveIfExists(jobId);
            _recurringJobs.AddOrUpdate<MessageService>(jobId,a => a.Send("Message each Second"), "*/1 * * * * *");
        }

        public void AddProjectUpdateJob()
        {
            string jobId = "updateProjectJobId";
            _recurringJobs.RemoveIfExists(jobId);
            _recurringJobs.AddOrUpdate<ProjectService>(jobId, a => a.UpdateProjectsInDatabaseAsync(), Cron.Hourly);
        }

        public void AddDocumentUpdateJob()
        {
            string jobId = "updateDocumentJobId";
            _recurringJobs.RemoveIfExists(jobId);
            _recurringJobs.AddOrUpdate<DocumentService>(jobId, a => a.UpdateDocumentsInDatabaseAsync(), Cron.Hourly);
        }
    }
}