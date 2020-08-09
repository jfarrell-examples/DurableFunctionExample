namespace Farrellsoft.Example.FileApproval
{
    public class DownloadResponse
    {
        public string FileId { get; set; }
        public int ContentLength { get; set; }
        public object Metadata { get; set; }
        public string DownloadUrl { get; set; }
    }
}