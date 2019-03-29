using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using jjwebcore.Models;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;

namespace jjwebcore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public async Task<IActionResult> Test()
        {
            ViewData["Message"] = "Test page.";

            var host = Dns.GetHostName();            
            ViewData["Host"] = host;

            Uri serviceUri = new Uri(Environment.GetEnvironmentVariable("SERVICEAPI_URL"));

            ViewData["ServiceUrl"] = serviceUri;

            // call service
            HttpClient client = new HttpClient();
            var serializer = new DataContractJsonSerializer(typeof(List<string>));
            var streamTask = client.GetStreamAsync(serviceUri);
            var res = serializer.ReadObject(await streamTask) as List<string>;
            string resString = string.Join(";", res);

            ViewData["ApiResult"] = resString;

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
