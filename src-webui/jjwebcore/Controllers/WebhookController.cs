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
        [HttpPost]
        public IActionResult ProcessEventGridWebhook([FromBody]EventGridEvent[] ev, [FromServices]ILogger<WebhookController> logger)
        {
            var evt = ev.FirstOrDefault();
            if (evt == null) return BadRequest();

            if (evt.EventType == EventTypes.MediaJobStateChangeEvent)
            {
                var data = (evt.Data as JObject).ToObject<MediaJobStateChangeEventData>();
                logger.LogInformation(JsonConvert.SerializeObject(data, Formatting.Indented));
            }

            // Respond with a SubscriptionValidationResponse to complete the EventGrid subscription
            if (evt.EventType == EventTypes.EventGridSubscriptionValidationEvent)
            {
                var data = (evt.Data as JObject).ToObject<SubscriptionValidationEventData>();
                var response = new SubscriptionValidationResponse(data.ValidationCode);
                return Ok(response);
            }

            return BadRequest();
        }
    }
}