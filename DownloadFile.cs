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

namespace Farrellsoft.Example.FileApproval
{
    public static class DownloadFile
    {
        [FunctionName("DownloadFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "download/{fileId}")] HttpRequest req,
            [Table("metadata", "{fileId}", "{fileId}", Connection = "TableConnectionString")] FileMetadata fileMetadata,
            [Table("ocrdata", Connection = "TableConnectionString")] CloudTable fileOcrData,
            ILogger log)
        {
            if (!fileMetadata.ApprovedForDownload)
            {
                return new StatusCodeResult(403);
            }

            return new OkObjectResult(new DownloadResponse());
        }
    }
}
