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
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < Environment.ProcessorCount + 1; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(CPUKill));
                t.Start(cpuUsage);
                threads.Add(t);
            }
            return new string[] { string.Format("CPU stress {0}% ",cpuUsage) };
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
