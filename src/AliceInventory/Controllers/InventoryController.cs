using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AliceInventory.Logic.AliceResponseRender;
using AliceInventory.Logic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;


namespace AliceInventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        public Logic.IInventoryDialogService InventoryDialogService { set; get; }
        public Logic.IConfigurationService ConfigurationService { set; get; }

        public InventoryController(
            Logic.IInventoryDialogService inventoryDialogService,
            Logic.IConfigurationService configurationService)
        {
            this.InventoryDialogService = inventoryDialogService;
            this.ConfigurationService = configurationService;
        }

        // GET api/inventory
        [HttpGet]
        public async Task<string> Get()
        {
            string configSuccessAnswer = await this.ConfigurationService.GetIsConfigured();  
            return "Server is working...\n"
                + (string.IsNullOrWhiteSpace(configSuccessAnswer)
                    ? "Configuration OK!"
                    : "Not configured.");
        }

        // HEAD api/inventory
        [HttpHead]
        public ActionResult Head()
        { 
            return Ok(); 
        }

        // POST api/inventory/alice
        [HttpPost]
        [Route("alice")]
        public ActionResult<AliceResponse> Post([FromBody] AliceRequest request)
        {
            string input = string.IsNullOrEmpty(request.Request.Command)
                ? request.Request.Payload
                : request.Request.Command;

            var answer = InventoryDialogService.ProcessInput(request.Session.UserId, input, new CultureInfo(request.Meta.Locale));
            return AliceResponseRendererHelper.CreateAliceResponse(answer, request.Session);
        }
    }
}
