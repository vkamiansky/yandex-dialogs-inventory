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
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Server is working...";
        }

        // POST api/values
        [HttpPost]
        public ActionResult<AliceResponse> Post([FromBody] AliceRequest request)
        {
            var response = new AliceResponse()
            {
                Response = new ResponseModel()
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
