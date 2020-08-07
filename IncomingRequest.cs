using System;
using Microsoft.AspNetCore.Http;

namespace FileApproval
{
    public class IncomingRequest
    {
        public Guid Id => Guid.NewGuid();
        public IFormFile File { get; set; }
    }
}