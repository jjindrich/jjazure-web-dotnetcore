using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace jjfunc
{
    [StorageAccount("QueueStorageAccount")]
    public static class processQueue
    {
        [FunctionName("processQueue")]
        [return: Queue("processed")]
        public static string Run([QueueTrigger("orders", Connection = "QueueStorageAccount")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"JJ trigger function processed: {myQueueItem}");
            return myQueueItem;
        }
    }
}
