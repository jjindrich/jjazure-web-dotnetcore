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
using jjwebapicore;

namespace jjwebcore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _cl;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _cl = httpClientFactory;
        }

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

            try
            {
                // call service
                var client = _cl.CreateClient("jjwebapicore");
                string result = await client.GetStringAsync("/api/values");
                ViewData["ServiceUrl"] = client.BaseAddress.ToString();
                ViewData["ApiResult"] = result;

                // call service windows
                var clientWin = _cl.CreateClient("jjwebapicore");
                string resultWin = await clientWin.GetStringAsync("/api/values");
                ViewData["ServiceWinUrl"] = clientWin.BaseAddress.ToString();
                ViewData["ApiWinResult"] = resultWin;

            }
            catch (Exception ex)
            {
                ViewData["Message"] = "Error calling service: " + ex.Message;
            }

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
