using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.Messaging.EventGrid.SystemEvents;

namespace jjwebcore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Hello from /api/webhook controller !";
        }

        [HttpPost]
        public IActionResult ProcessEventGridWebhook([FromBody]EventGridEvent[] events, [FromServices]ILogger<WebhookController> logger)
        {
            if (events == null) return BadRequest();

            foreach(EventGridEvent ev in events)
            {
                if (ev.TryGetSystemEventData(out object systemEvent))
                {
                    switch (systemEvent)
                    {
                        case SubscriptionValidationEventData subscriptionValidated:
                            var response = new SubscriptionValidationResponse();
                            response.ValidationResponse = subscriptionValidated.ValidationCode;
                            return Ok(response);
                            break;
                    }
                }
            }

            return BadRequest();
        }
    }
}