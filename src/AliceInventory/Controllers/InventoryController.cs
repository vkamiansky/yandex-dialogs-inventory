using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AliceInventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        // GET api/inventory
        [HttpGet]
        public string Get()
        {
            return "Server is working...";
        }

        // POST api/inventory/alice
        [HttpPost]
        [Route("alice")]
        public ActionResult<AliceResponse> Post([FromBody] AliceRequest request)
        {
            var response = new AliceResponse()
            {
                Response = new Response()
                {
                    Text = request.Request.Command
                }, 
                Session = request.Session,
                Version = request.Version
            };
            return response;
        }
    }
}
