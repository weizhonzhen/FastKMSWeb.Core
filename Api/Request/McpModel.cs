using Newtonsoft.Json;

namespace FastKMSApi.Core.Request
{
    public class McpModel
    {
        public string name { get; set; }

        public string description { get;set; }

        public List<Parameters> parameters { get; set; }

        public string httpMethod { get; set; }

        public string httpUrl { get; set; }
                
        public string? _id { get; set; }

        [JsonIgnore]
        public string? _index { get; set; }
    }



    public class Parameters
    {
        public string type { get; set; }

        public string name { get; set; }

        public string description { get; set; }
    }

    public class chatMcpModel
    {
        public List<McpModel> mcp { get; set; }

        public string message { get; set; }

        public string chatIndex { get; set; }
    }
}