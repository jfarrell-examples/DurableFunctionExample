using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Farrellsoft.Example.FileApproval
{
    public static class ApproveFileUpload
    {
        [FunctionName("ApproveFileUpload")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "approve/{fileId}")] HttpRequest req,
            [Table("metadata", "{fileId}", "{fileId}", Connection = "TableConnectionString")] FileMetadata fileMetadata,
            [Table("metadata", Connection = "TableConnectionString")] CloudTable metadataTable,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            var instanceId = fileMetadata.WorkflowId;

            fileMetadata.ApprovedForAnalysis = true;
            var replaceOperation = TableOperation.Replace(fileMetadata);
            await metadataTable.ExecuteAsync(replaceOperation);

            await client.RaiseEventAsync(instanceId, "UploadApproved", fileMetadata.ApprovedForAnalysis);
            return new AcceptedResult(string.Empty, fileMetadata.RowKey);
        }
    }
}
