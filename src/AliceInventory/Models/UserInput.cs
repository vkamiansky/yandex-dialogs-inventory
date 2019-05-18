using System;
using Newtonsoft.Json;

namespace TestApi
{
    public class UserInput
    {
       public class UserInputRequest
       {
           [JsonProperty("command")]
           public string Command {get;set;}

       }
       public UserInputRequest request{get;set;}


    }
}