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
            var responce = new AliceResponse() {Response = new ResponseModel() {Text = request.Request.Command}};
            return JsonConvert.SerializeObject(responce);
        }
    }
}
