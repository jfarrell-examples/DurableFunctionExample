using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Farrellsoft.Example.FileApproval
{
    public static class ApproveFile
    {
        [FunctionName("ApproveFile_Start")]
        public static async Task HttpStart(
            [BlobTrigger("files/{id}", Connection = "StorageAccountConnectionString")] Stream fileBlob,
            string id,
            [Table("metadata", "{id}", "{id}", Connection = "TableConnectionString")] FileMetadata metadata,
            [Table("metadata", Connection = "TableConnectionString")] CloudTable metadataTable,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("ProcessFileFlow", new ApprovalWorkflowData { TargetId = id });
            metadata.WorkflowId = instanceId;

            var replaceOperation = TableOperation.Replace(metadata);
            var result = await metadataTable.ExecuteAsync(replaceOperation);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            log.LogInformation("Flow started");
        }

        [FunctionName("ProcessFileFlow")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            [Table("metadata", Connection = "TableConnectionString")] CloudTable metadataTable,
            ILogger log)
        {
            var input = context.GetInput<ApprovalWorkflowData>();

            var uploadApprovedEvent = context.WaitForExternalEvent<bool>("UploadApproved");
            await Task.WhenAny(uploadApprovedEvent);

            // run through OCR tools
            var ocrProcessTask = context.CallActivityAsync<bool>(nameof(ProcessFileFunction.ProcessFile), input.TargetId);
            await Task.WhenAny(ocrProcessTask);

            // ask for approval to download
            var downloadApprovedEvent = context.WaitForExternalEvent<bool>("DownloadApproved");
            await Task.WhenAny(downloadApprovedEvent);

            log.LogInformation("File Ready");
        }
    }
}