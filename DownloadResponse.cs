using System.Collections.Generic;

namespace Farrellsoft.Example.FileApproval
{
    public class DownloadResponse
    {
        public string FileId { get; set; }
        public IList<OcrResult> Metadata { get; set; }
        public string DownloadUrl { get; set; }
    }
}