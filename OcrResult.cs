using System;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace Farrellsoft.Example.FileApproval
{
    public class OcrResult : TableEntity
    {
        public OcrResult()
        {
            // used by TableQuery reconstruction
        }

        public OcrResult(string fileId)
        {
            RowKey = Guid.NewGuid().ToString();
            PartitionKey = fileId;
        }

        public string KeyName { get; set; }
        public string OcrValue { get; set; }
        public OcrType OcrType { get; set; }
    }

    public enum OcrType
    {
        ComputerVision,
        None
    }
}