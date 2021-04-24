using System;
using Octokit;
using Project = PersonalWebsiteBackend.Domain.Project;

namespace PersonalWebsiteBackend.Extensions
{
    public static class RepositoryExtensions
    {
        public static Project ConvertToProject(this Repository repository)
        {
            var project = new Project()
            {
                ProjectId = repository.Id,
                Name = repository.Name,
                Description = repository.Description,
                Archived = repository.Archived,
                forksCount = repository.ForksCount,
                HtmLUrl = repository.HtmlUrl,
                Language = repository.Language,
                Licence = repository.License?.Name,
                OwnerName = repository.Owner.Login,
                OwnerHtmlUrl = repository.Owner.HtmlUrl,
                Size = repository.Size,
                stars = repository.StargazersCount,
                WatchersCount = repository.WatchersCount,
                Visibility = (Domain.Visibility) repository.Visibility,
                ProjectCreatedOn = repository.CreatedAt.DateTime,
                ProjectUpdatedOn = repository.UpdatedAt.DateTime
            };
            return project;
        }
    }
}