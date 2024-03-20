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
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.DependencyInjection;

namespace jjwebcore.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IContactsClient cl;
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;

        public ContactsController(IContactsClient contactsClient, [FromKeyedServices("jjwebcore")] Kernel kernel, IChatCompletionService chatCompletionService, ILogger<HomeController> logger) : base()
        {
            cl = contactsClient;
            _kernel = kernel;
            _chatCompletionService = chatCompletionService;
            _logger = logger;
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
                int contactId = Int32.Parse(collection["ContactId"]);
                Contact createC = new Contact() { ContactId = contactId, FullName = collection["FullName"] };
                await cl.PostContactAsync(createC);

                _logger.LogInformation("Contact {contactId} created", contactId);

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
                Contact updateC = new Contact() { ContactId = id, FullName = collection["FullName"], Keywords = collection["Keywords"], Description = collection["Description"] };
                var c = await cl.PutContactAsync(id, updateC);

                int contactId = id;
                _logger.LogInformation("Contact {contactId} updated", contactId);

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

                int contactId = id;
                _logger.LogInformation("Contact {contactId} deleted", contactId);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateWebhook([FromBody] EventGridEvent[] events, [FromServices] ILogger<WebhookController> logger)
        {
            if (events == null) return BadRequest();

            foreach (EventGridEvent ev in events)
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

        // return description for keywords using Azure OpenAI
        [HttpPost]
        public async Task<string> GetDescription(string keywords, string fullname, string? additionalPrompt)
        {
            if (keywords == null) return "No keywords provided";

            string input = $"I'm going to write short description of person named {fullname}. Use following tags: {keywords} to describe this person. You are recruiter of IT company.";
            input += "Add information about his age at the end";
            if (additionalPrompt != null) input += additionalPrompt;

            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };
            ChatMessageContent result = await _chatCompletionService.GetChatMessageContentAsync(input, openAIPromptExecutionSettings, _kernel);

            return result.Content;
        }
    }
}
