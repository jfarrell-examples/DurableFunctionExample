using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Farrellsoft.Example.FileApproval
{
    public static class DownloadFile
    {
        [FunctionName("DownloadFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "download/{fileId}")] HttpRequest req,
            [Table("metadata", "{fileId}", "{fileId}", Connection = "TableConnectionString")] FileMetadata fileMetadata,
            ILogger log)
        {
            return new OkObjectResult(new DownloadResponse());
        }
    }
}
