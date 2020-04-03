using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using jjwebapicore;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.EventGrid;
using Newtonsoft.Json;

namespace jjwebcore.Controllers
{
    public class ContactsController : Controller
    {
        private jjwebapicore.ContactsClient cl = new jjwebapicore.ContactsClient();

        public ContactsController() : base()
        {
            Uri serviceUri = null;
            try
            {
                serviceUri = new Uri(Environment.GetEnvironmentVariable("SERVICEAPIROOT_URL"));
                cl.BaseUrl = serviceUri.ToString();
            }
            catch { }
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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> CreateWebhook([FromBody]EventGridEvent[] events, [FromServices]ILogger<WebhookController> logger)
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
                else
                // process message to create contact
                {
                    Contact createC = JsonConvert.DeserializeObject<Contact>(ev.Data.ToString());                    
                    await cl.PostContactAsync(createC);
                }
            }
            return BadRequest();
        }
    }
}