using System;
using Google.Apis.Drive.v3.Data;
using PersonalWebsiteBackend.Domain;

namespace PersonalWebsiteBackend.Extensions
{
    public static class GoogleFileExtensions
    {
        public static Document ConvertToDocument(this File file)
        {
            var document = new Document()
            {
                DocumentId = file.Id,
                Name = file.Name,
                Description = file.Description,
                OwnersEmail = file.Owners?[0]?.EmailAddress,
                ThumbnailLink = file.ThumbnailLink,
                WebcontentLink = file.WebContentLink,
                WebviewLink = file.WebViewLink,
                FileExtension = file.FileExtension,
                FullFileExtension = file.FullFileExtension,
                Kind = file.Kind,
                Md5Checksum = file.Md5Checksum,
                Shared = file.Shared,
                Size = file.Size,
                Version = file.Version,
                DocumentCreatedTime = file.CreatedTime
            };
            return document;
        }
    }
}