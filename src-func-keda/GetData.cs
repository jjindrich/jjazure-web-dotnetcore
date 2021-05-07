using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JJ.Function
{

    public class TestData
    {
        public int Id;
        public string Name;
    }

    public static class GetData
    {
        [FunctionName("GetData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("GetData executed");            

            TestData data = new TestData() { Id = 1, Name = System.Environment.MachineName };

            return new JsonResult(data);
        }
    }
}
