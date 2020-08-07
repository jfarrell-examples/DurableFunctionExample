using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Blob;

namespace Farrellsoft.Example.FileApproval
{
    public static class UploadFile
    {
        [FunctionName("UploadFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "file/add")] HttpRequest uploadFile,
            [Blob("files", FileAccess.Write, Connection = "StorageAccountConnectionString")] CloudBlobContainer blobContainer,
            ILogger log)
        {
            await blobContainer.CreateIfNotExistsAsync();
    
            var fileName = Guid.NewGuid().ToString();
            var cloudBlockBlob = blobContainer.GetBlockBlobReference(fileName);
            await cloudBlockBlob.UploadFromStreamAsync(uploadFile.Body);

            return new CreatedResult(string.Empty, fileName);
        }
    }
}
