using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using AliceInventory.Logic;
using AliceInventory.Data;

namespace AliceInventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureServices(srv => srv.AddMvc())
            .Configure(app => app.UseMvc());
            

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

            /*if (request.Session.New)
            {
            // надо вернуть AliceResponse-приветствие
            }
            var answer = InventoryDialogService.ProcessInput(request.Session.UserId,request.Request.Command);
            
            return Converter.MakeAliceResponse(request,answer); */
        }
    }
}
