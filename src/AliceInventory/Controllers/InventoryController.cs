using System;
using System.Globalization;
using System.Threading.Tasks;
using AliceInventory.Controllers.AliceResponseRender;
using Microsoft.AspNetCore.Mvc;


namespace AliceInventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private static readonly Random Random = new Random();
        private readonly Logic.IInventoryDialogService _inventoryDialogService;
        private readonly Logic.IConfigurationService _configurationService;
        private readonly Logic.Tracing.ITracingProvider _tracingProvider;

        public InventoryController(
            Logic.IInventoryDialogService inventoryDialogService,
            Logic.IConfigurationService configurationService,
            Logic.Tracing.ITracingProvider tracingProvider)
        {
            _inventoryDialogService = inventoryDialogService;
            _configurationService = configurationService;
            _tracingProvider = tracingProvider;
        }

        // GET api/inventory
        [HttpGet]
        public async Task<string> Get()
        {
            return await _tracingProvider.TryTraceAsync(
                "Alice::Get",
                async scope =>
                {
                    string configSuccessAnswer = await this._configurationService.GetIsConfigured();

                    var result = "Server is working...\n"
                                    + (string.IsNullOrWhiteSpace(configSuccessAnswer)
                                        ? "Configuration OK!"
                                        : "Not configured.");
                    scope?.Log($"Server config tested. The result is: {result}");
                    return result;
                });
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

            return _tracingProvider.TryTrace(
                "Alice::Post",
                scope =>
                {
                    var answer = _inventoryDialogService.ProcessInput(request.Session.UserId, input, new CultureInfo(request.Meta.Locale));
                    if (answer.Error != null) scope?.Log($"Error:\"{answer.Error.Message}\", input: \"{input}\"");
                    else if (answer.Exception != null) scope?.Log($"Error:\"{answer.Exception.Message}\", input: \"{input}\"");
                    else scope?.Log($"Result:\"{answer.Type.ToString()}\", input: \"{input}\"");

                    var response = AliceResponseRendererHelper.CreateAliceResponse(answer, request.Session, x => Random.Next(0, x));
                    return response;
                });
        }
    }
}
