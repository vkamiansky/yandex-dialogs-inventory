// Alice.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;


namespace AliceInventory
{
    // Alice.cs
    public class Alice : Controller 
    {
        static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureServices(srv => srv.AddMvc())
            .Configure(app => app.UseMvc());

        [HttpPost("/alice")]
        public AliceResponse WebHook([FromBody] AliceRequest req) => req.Reply("Привет");

        [HttpGet("/alice/hello")]

        //public AliceResponse HelloHook([FromBody] AliceRequest req) => req.Reply("Привет");
        public ActionResult<string> Get()
        {
            return StringParser.GetReply("There are 4 numbers in this string: 32424, 12312, and 22.");
        }


    }
}
