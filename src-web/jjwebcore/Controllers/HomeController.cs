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
            ViewData["Message"] = "Test API page.";

            var host = Dns.GetHostName();            
            ViewData["Host"] = host;

            Uri serviceUri = null;
            Uri serviceUriWin = null;

            try
            {
                serviceUri = new Uri(Environment.GetEnvironmentVariable("SERVICEAPI_URL"));
                serviceUriWin = new Uri(Environment.GetEnvironmentVariable("SERVICEWINAPI_URL"));
            }
            catch (Exception ex)
            {
                ViewData["Message"] = "Error loading Environment variables" + ex.Message;
            }

            // call service            
            if (serviceUri != null)
            {
                ViewData["ServiceUrl"] = serviceUri;
                ViewData["ApiResult"] = await CallApi(serviceUri);
            }

            // call service windows
            if (serviceUriWin != null)
            {
                ViewData["ServiceWinUrl"] = serviceUriWin;
                ViewData["ApiWinResult"] = await CallApi(serviceUriWin);
            }
            
            return View();
        }

        private async Task<string> CallApi(Uri serviceUri)
        {
            HttpClient client = new HttpClient();
            var serializer = new DataContractJsonSerializer(typeof(List<string>));
            var streamTask = client.GetStreamAsync(serviceUri);
            var res = serializer.ReadObject(await streamTask) as List<string>;
            return string.Join(";", res);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
