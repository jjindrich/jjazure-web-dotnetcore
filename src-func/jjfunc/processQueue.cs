using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace jjfunc
{
    public static class processQueue
    {
        [FunctionName("processQueue")]
        public static void Run([QueueTrigger("orders", Connection = "")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
