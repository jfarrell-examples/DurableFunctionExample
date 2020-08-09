using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Farrellsoft.Example.FileApproval.Extensions;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Farrellsoft.Example.FileApproval
{
    public static class ProcessFileFunction
    {
        private const string FileLengthKeyName = "ContentLength";

        [FunctionName("ProcessFile")]
        public static async Task<bool> ProcessFile(
            [ActivityTrigger] string fileId,
            [Blob("files/{fileId}", FileAccess.Read, Connection = "StorageAccountConnectionString")] Stream fileBlob,
            [Table("ocrdata", Connection = "TableConnectionString")] CloudTable ocrDataTable,
            ILogger log)
        {

            await ocrDataTable.CreateIfNotExistsAsync();

            // call the ocr functions
            var computerVisionResults = await ProcessWithComputerVision(fileBlob, fileId);

            // save the batch data
            var batchOperation = new TableBatchOperation();
            computerVisionResults.ForEach(result => batchOperation.Insert(result));
            batchOperation.Insert(new OcrResult(fileId) { KeyName = FileLengthKeyName, OcrValue = fileBlob.Length.ToString(), OcrType = OcrType.None });

            var executeReslt = await ocrDataTable.ExecuteBatchAsync(batchOperation);

            log.LogInformation("ProcessFile function fired");
            return true;
        }

        static async Task<List<OcrResult>> ProcessWithComputerVision(Stream blob, string fileId)
        {
            var visionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("CognitiveServicesKey")))
            {
                Endpoint = Environment.GetEnvironmentVariable("CognitiveServicesEndpoint")
            };

            var result = await visionClient.AnalyzeImageInStreamAsync(blob, new List<VisualFeatureTypes>()
            {
                //VisualFeatureTypes.Faces,
                VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color,
                //VisualFeatureTypes.Brands,
                VisualFeatureTypes.Description
            });

            return result.AsResultList(fileId);
        }
    }
}