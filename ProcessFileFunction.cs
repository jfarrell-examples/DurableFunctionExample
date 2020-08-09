using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Farrellsoft.Example.FileApproval
{
    public static class ProcessFileFunction
    {
        [FunctionName("ProcessFile")]
        public static async Task<bool> ProcessFile(
            [ActivityTrigger] string fileId,
            ILogger log)
        {
            log.LogInformation("ProcessFile function fired");
            return true;
        }
    }
}