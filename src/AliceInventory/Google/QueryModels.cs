using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class Parameters
{
    
    [JsonProperty("parameters")]
    public Dictionary<string, string> parameters { get; set; }
}

public class Text
{
    
    [JsonProperty("text")]
    public List<string> text { get; set; }
}

public class FulfillmentMessage
{
    
    [JsonProperty("displayName")]
    public Text text { get; set; }
}

public class OutputContext
{
    
    [JsonProperty("displayName")]
    public string name { get; set; }
    
    [JsonProperty("displayName")]
    public int lifespanCount { get; set; }
    
    [JsonProperty("displayName")]
    public Parameters parameters { get; set; }
}

public class Intent
{
    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("displayName")]
    public string displayName { get; set; }
}

public class DiagnosticInfo
{
}

public class QueryResult
{ 
    [JsonProperty("queryText")]
    public string queryText { get; set; }

    [JsonProperty("parameters")]
    public Parameters parameters { get; set; }

    [JsonProperty("allRequiredParamsPresent")]
    public bool allRequiredParamsPresent { get; set; }

    [JsonProperty("fulfillmentText")]
    public string fulfillmentText { get; set; }

    [JsonProperty("fulfillmentMessages")]
    public List<FulfillmentMessage> fulfillmentMessages { get; set; }

    [JsonProperty("outputContexts")]
    public List<OutputContext> outputContexts { get; set; }

    [JsonProperty("intent")]
    public Intent intent { get; set; }

    [JsonProperty("intentDetectionConfidence")]
    public int intentDetectionConfidence { get; set; }

    [JsonProperty("diagnosticInfo")]
    public DiagnosticInfo diagnosticInfo { get; set; }

    [JsonProperty("languageCode")]
    public string languageCode { get; set; }
}

public class OriginalDetectIntentRequest
{
}

public class GoogleRequest
{

    [JsonProperty("responseId")]
    public string responseId { get; set; }

    [JsonProperty("session")]
    public string session { get; set; }

    [JsonProperty("queryResult")]
    public QueryResult queryResult { get; set; }

    [JsonProperty("originalDetectIntentRequest")]
    public OriginalDetectIntentRequest originalDetectIntentRequest { get; set; }
}