using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AliceInventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        // POST api/values
        [HttpPost]
        public ActionResult<string> Post([FromBody] AliceRequest request)
        {
            var response = new AliceResponse()
            {
                Response = new ResponseModel()
                {
                    Text = request.Request.Command
                },
                Session = new SessionModel()
                {
                    SessionId = request.Session.SessionId,
                    MessageId = request.Session.MessageId,
                    UserId = request.Session.UserId
                }
            };

            return JsonConvert.SerializeObject(response);
        }
    }
}
