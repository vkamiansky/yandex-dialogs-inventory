// Alice.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

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
            return GetReply("There are 4 numbers in this string: 32424, 12312, and 22.");
        }

        private string GetReply(string input = "Hello!")
        {
            // Split on one or more non-digit characters.
            string[] numbers = Regex.Split(input, @"\D+");
            var ret=new List<string>();
            foreach (string value in numbers)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    int i = int.Parse(value);
                    ret.Add($"Number: {i}, ");
                }
            }
            return ret.Any()? string.Concat(ret.ToArray()) : "no numbers in string";
        }       
    }
}
