using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace jjwebapicoredapr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// State store name.
        /// </summary>
        public const string StoreName = "statestore";

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value11", "value22" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get([FromState(StoreName)] StateEntry<string> id)
        {
            if (id.Value is null)
            {
                return this.NotFound();
            }

            return id.Value;
        }

        // POST api/values/5
        [HttpPost("{id}")]
        public async Task Post(int id, [FromBody] string value, [FromServices] DaprClient daprClient)
        {
            var state = await daprClient.GetStateEntryAsync<string>(StoreName, id.ToString());
            state.Value = value;
            await state.SaveAsync();
            return;
        }

        /*
        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        */
    }
}