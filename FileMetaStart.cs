
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace Farrellsoft.Example.FileApproval
{
    public class FileMetaStart : TableEntity
    {
        public string FileSize { get; set; }

        public bool ApprovedForAnalysis => false;
        public bool ApprovedForDownload => false;
    }
}