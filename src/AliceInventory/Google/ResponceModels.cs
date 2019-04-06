using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


    public class Button
    {

        [JsonProperty("text")]
        public string text { get; set; }

        [JsonProperty("postback")]
        public string postback { get; set; }
    }

    public class Card
    {
        [JsonProperty("title")]
         public string title { get; set; }

        [JsonProperty("subtitle")]
        public string subtitle { get; set; }

        [JsonProperty("imageUri")]
        public string imageUri { get; set; }

      //  [JsonProperty("buttons")]
      //  public IList<Button> buttons { get; set; }
    }

    public class OutputMessage
    {

        [JsonProperty("card")]
        public Card card { get; set; }
    }

    public class SimpleResponse
    {

        [JsonProperty("textToSpeech")]
        public string textToSpeech { get; set; }
    }

    public class Item
    {

        [JsonProperty("simpleResponse")]
        public SimpleResponse simpleResponse { get; set; }
    }

    public class RichResponse
    {

      //  [JsonProperty("items")]
       // public IList<Item> items { get; set; }
    }

    public class Google
    {

        [JsonProperty("expectUserResponse")]
        public bool expectUserResponse { get; set; }

      //  [JsonProperty("richResponse")]
      //  public RichResponse richResponse { get; set; }
    }

    public class Facebook
    {

        [JsonProperty("text")]
        public string text { get; set; }
    }

    public class Slack
    {

        [JsonProperty("text")]
        public string text { get; set; }
    }

    public class Payload
    {

        [JsonProperty("google")]
        public Google google { get; set; }

        [JsonProperty("facebook")]
        public Facebook facebook { get; set; }

        [JsonProperty("slack")]
        public Slack slack { get; set; }
    }

    public class FollowupEventInput
    {

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("languageCode")]
        public string languageCode { get; set; }

  //      [JsonProperty("parameters")]
  //      public Parameters parameters { get; set; }
    }

    public class GoolgeResponse
    {

        [JsonProperty("fulfillmentText")]
        public string fulfillmentText { get; set; }

       // [JsonProperty("fulfillmentMessages")]
       // public IList<OutputMessage> fulfillmentMessages { get; set; }

        [JsonProperty("source")]
        public string source { get; set; }

        [JsonProperty("payload")]
        public Payload payload { get; set; }

       // [JsonProperty("outputContexts")]
       // public IList<OutputContext> outputContexts { get; set; }

        [JsonProperty("followupEventInput")]
        public FollowupEventInput followupEventInput { get; set; }
    }