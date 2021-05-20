using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using PersonalWebsiteBackend.Options;

namespace PersonalWebsiteBackend.Helpers
{
    public class FetchGoogleDriveDocuments
    {
        public static async Task<IList<File>> FetchAllDocumentsWithAllProperties(
            GoogleDriveAPISettings googleDriveAPISettings)
        {
            // Create Drive API service
            var credentials = GoogleCredential.FromJson(JsonSerializer.Serialize(googleDriveAPISettings))
                .CreateScoped(DriveService.ScopeConstants.Drive);
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials
            });

            // Define parameters of request.
            var listFilesRequest = service.Files.List();
            listFilesRequest.PageSize = 1000;
            listFilesRequest.Fields = "*";

            // List files.
            var listFilesResponse = await listFilesRequest.ExecuteAsync();
            return listFilesResponse.Files;
        }
    }
}