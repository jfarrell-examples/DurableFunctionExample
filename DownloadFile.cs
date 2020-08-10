using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage.Blob;

namespace Farrellsoft.Example.FileApproval
{
    public static class DownloadFile
    {
        [FunctionName("DownloadFile")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "download/{fileId}")] HttpRequest req,
            string fileId,
            [Table("metadata", "{fileId}", "{fileId}", Connection = "TableConnectionString")] FileMetadata fileMetadata,
            [Table("ocrdata", Connection = "TableConnectionString")] CloudTable fileOcrDataTable,
            [Blob("files/{fileId}", FileAccess.Read, Connection = "StorageAccountConnectionString")] CloudBlockBlob fileBlob,
            ILogger log)
        {
            if (!fileMetadata.ApprovedForDownload)
            {
                return new StatusCodeResult(403);
            }

            var readQuery = new TableQuery<OcrResult>();
            TableQuery.GenerateFilterCondition(nameof(OcrResult.PartitionKey), QueryComparisons.Equal, fileId);

            var ocrResults = fileOcrDataTable.ExecuteQuery(readQuery).ToList();

            return new OkObjectResult(new DownloadResponse
            {
                Metadata = ocrResults,
                FileId = fileId,
                DownloadUrl = GenerateSasUrlForFileDownload(fileBlob, fileId)
            });
        }

        static string GenerateSasUrlForFileDownload(CloudBlockBlob blob, string fileId)
        {
            var policy = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.Now.AddHours(1),
                Permissions = SharedAccessBlobPermissions.Read
            };

            return blob.Uri + blob.GetSharedAccessSignature(policy);
        }
    }
}
