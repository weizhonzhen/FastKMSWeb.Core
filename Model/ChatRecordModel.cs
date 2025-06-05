using Newtonsoft.Json;
using FastElasticsearch.Core;

namespace FastKMSWeb.Core.Model
{
    public class ChatRecordModel
    {
        [Column(type = "keyword")]
        public string charIndex {  get; set; }

        [Column(type = "keyword")]
        public string request { get; set; }

        [Column(type = "keyword")]
        public DateTime beginTime { get; set; }

        [Column(type = "keyword")]
        public string response { get; set; }

        [Column(type = "keyword")]
        public string vectorContent { get; set; }

        [Column(type = "keyword")]
        public bool isPrompt { get; set; }

        [Column(type = "keyword")]
        public string nL2Sql { get; set; }

        [Column(type = "keyword")]
        public bool isNL2Sql { get; set; }

        [Column(type = "keyword")]
        public DateTime endTime { get; set; }

        [Column(type = "keyword")]
        public string model { get; set; }

        [JsonIgnore]
        public string _id { get; set; }

        [JsonIgnore]
        public string _index { get; set; }
    }
}
