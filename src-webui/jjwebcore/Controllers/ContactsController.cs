using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using jjwebapicore;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.Models;
using System.Text.Json;
using System.Net.Http;
using Azure.Messaging.EventGrid.SystemEvents;
using System.Diagnostics.Contracts;

namespace jjwebcore.Controllers
{
    public class ContactsController : Controller
    {        
        private readonly IContactsClient cl;

        public ContactsController(IContactsClient contactsClient) : base()
        {
            cl = contactsClient;            
        }

        public async Task<IActionResult> Index()
        {
            var contacts = await cl.GetContactAllAsync();
            return View(contacts);
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]        
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                Contact createC = new Contact() { ContactId = Int32.Parse(collection["ContactId"]), FullName = collection["FullName"] };
                await cl.PostContactAsync(createC);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            var c = await cl.GetContactAsync(id);
            return View(c);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                Contact updateC = new Contact() { ContactId = id, FullName = collection["FullName"] };
                var c = await cl.PutContactAsync(id, updateC);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            var contact = await cl.GetContactAsync(id);
            return View(contact);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                await cl.DeleteContactAsync(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateWebhook([FromBody]EventGridEvent[] events, [FromServices]ILogger<WebhookController> logger)
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
                        default:
                            Contact createC = JsonSerializer.Deserialize<Contact>(ev.Data.ToString());
                            await cl.PostContactAsync(createC);
                            return Ok();
                            break;
                    }
                }                            
            }
            return BadRequest();
        }
    }
}
