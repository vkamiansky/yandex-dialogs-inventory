using System;
using System.Globalization;
using System.Threading.Tasks;
using AliceInventory.Controllers.AliceResponseRender;
using AliceInventory.Logic;
using Microsoft.AspNetCore.Mvc;

namespace AliceInventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private static readonly Random Random = new Random();

        public InventoryController(
            IInventoryDialogService inventoryDialogService,
            IConfigurationService configurationService)
        {
            InventoryDialogService = inventoryDialogService;
            ConfigurationService = configurationService;
        }

        public IInventoryDialogService InventoryDialogService { set; get; }
        public IConfigurationService ConfigurationService { set; get; }

        // GET api/inventory
        [HttpGet]
        public async Task<string> Get()
        {
            var configSuccessAnswer = await ConfigurationService.GetIsConfigured();
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
            var input = string.IsNullOrEmpty(request.Request.Command)
                ? request.Request.Payload
                : request.Request.Command;

            var answer =
                InventoryDialogService.ProcessInput(request.Session.UserId, input,
                    new CultureInfo(request.Meta.Locale));
            return AliceResponseRendererHelper.CreateAliceResponse(answer, request.Session, x => Random.Next(0, x));
        }
    }
}