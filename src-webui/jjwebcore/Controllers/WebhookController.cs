using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                // Respond with a SubscriptionValidationResponse to complete the EventGrid subscription
                if (ev.EventType == EventTypes.EventGridSubscriptionValidationEvent)
                {
                    var eventValidationData = JsonConvert.DeserializeObject<SubscriptionValidationEventData>(ev.Data.ToString());
                    var response = new SubscriptionValidationResponse(eventValidationData.ValidationCode);
                    return Ok(response);
                }
            }

            //if (evt.EventType == EventTypes.MediaJobStateChangeEvent)
            //{
            //    var data = (evt.Data as JObject).ToObject<MediaJobStateChangeEventData>();
            //    logger.LogInformation(JsonConvert.SerializeObject(data, Formatting.Indented));
            //}

            return BadRequest();
        }
    }
}