using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace jjwebapicore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StressController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            int cpuUsage = 70;
            int time = 40000;
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < Environment.ProcessorCount + 1; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(CPUKill));
                t.Start(cpuUsage);
                threads.Add(t);
            }
            Thread.Sleep(time);

            return new string[] { string.Format("CPU stressed on {1}% for {0}ms.",time,cpuUsage) };
        }

        public static void CPUKill(object cpuUsage)
        {
            Parallel.For(0, 1, new Action<int>((int i) =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (true)
                {
                    if (watch.ElapsedMilliseconds > (int)cpuUsage)
                    {
                        Thread.Sleep(100 - (int)cpuUsage);
                        watch.Reset();
                        watch.Start();
                    }
                }
            }));

        }
    }
}
