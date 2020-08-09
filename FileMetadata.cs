
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace Farrellsoft.Example.FileApproval
{
    public class FileMetadata : TableEntity
    {
        public string FileSize { get; set; }
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public string WorkflowId { get; set; }
        public bool ApprovedForAnalysis { get; set; } = false;
        public bool ApprovedForDownload { get; set; } = false;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public OcrResult OcrData { get; set; }
    }
}