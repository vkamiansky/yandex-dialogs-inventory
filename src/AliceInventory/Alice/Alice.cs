// Alice.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ConsoleApp;
using System.Text;

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
        public AliceResponse WebHook([FromBody] AliceRequest req) =>
        req.Reply(GetAliceReply(req.Request.OriginalUtterance));

        private string GetAliceReply(string input)
        {
            var session = new UserSession();
            ChatResponse response = session.ProcessInput(input);
           return $"{response.TextResponse}\n[VOICE:] {response.VoiceResponse}";
        }

        private static UserSession localSession=new UserSession();

        [HttpGet("/alice/hello")]

        //public AliceResponse HelloHook([FromBody] AliceRequest req) => req.Reply("Привет");
        public ActionResult<string> Get()
        {
           var input="add 3 cats";
            var dice=System.DateTime.Now.Second % 5;
            switch(dice)
            {
                case 0:input="add 2 tigers";
                    break;
                case 1:input="add 6 cars";
                    break;
                case 2:input="add 5 tanks";
                    break;
                case 3:input="add 5 метров кабеля";
                    break;
                case 4:input="add 50 см кабеля";
                    break;
                default:input="add 5 кг свинца";
                    break;
            }
            var response=localSession.ProcessInput(input).TextResponse;
/*             var response=localSession.ProcessInput("list").TextResponse;
            return $"{System.DateTime.Now.ToLongTimeString()} {response}!";
*/

            return $"Hello! {response}";
        }
        
        [HttpPost("/google")]
        public JsonResult GetGoogleResponse([FromBody] GoogleRequest req) 
        {
            var response= new GoolgeResponse();
            response.fulfillmentText = $"{DateTime.Now.ToLongTimeString()} Hello!";
            return new JsonResult(response);
        }
    }
}
