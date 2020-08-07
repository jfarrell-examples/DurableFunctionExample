using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Farrellsoft.Example.FileApproval
{
    public static class ApproveFile
    {
        [FunctionName("ApproveFile")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("ApproveFile_Hello", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("ApproveFile_Hello", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("ApproveFile_Hello", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("ApproveFile_Hello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

        [FunctionName("ApproveFile_Start")]
        public static async Task HttpStart(
            [BlobTrigger("files/{id}", Connection = "StorageAccountConnectionString")] Stream fileBlob,
            string id,
            [Table("metadata", "Http", "{id}", Connection = "TableConnectionString")] FileMetaStart metadata,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            /*(string instanceId = await starter.StartNewAsync("ApproveFile", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");*/
            log.LogInformation("Flow started");
        }
    }
}