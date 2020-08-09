
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace Farrellsoft.Example.FileApproval
{
    public class FileMetadata : TableEntity
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Include, DefaultValueHandling = DefaultValueHandling.Include)]
        public string WorkflowId { get; set; }
        public bool ApprovedForAnalysis { get; set; } = false;
        public bool ApprovedForDownload { get; set; } = false;
    }
}