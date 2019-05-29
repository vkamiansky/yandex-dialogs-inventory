using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AliceInventory.Logic.AliceResponseRender;
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
        static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).
            UseStartup<AliceInventory.Startup>();

        public Logic.IInventoryDialogService InventoryDialogService { set; get; }

        public InventoryController(Logic.IInventoryDialogService inventoryDialogService)
        {
            this.InventoryDialogService = inventoryDialogService;
        }

        // GET api/inventory
        [HttpGet]
        public string Get()
        {
            var vars = Environment.GetEnvironmentVariables();

            string result = " --Vars-- \n";
            foreach (var key in vars.Keys.Cast<string>())
            {
                result += key + ": " + Environment.GetEnvironmentVariable(key) + "\n";
            }

            return $"Server is working...\n{result}";
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
