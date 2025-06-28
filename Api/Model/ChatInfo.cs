using FastElasticsearch.Core;
using Newtonsoft.Json;

namespace FastKMSApi.Core.Model
{
    public class ChatInfo
    {
        [Column(type = "keyword")]
        public string name { get; set; }

        [Column(type = "keyword")]
        public string chatIndex { get; set; }

        [Column(type = "keyword")]
        public List<KmsModel> kms { get; set; } = new List<KmsModel>();

        [Column(type = "date")]
        public DateTime beginTime { get; set; }

        [Column(type = "keyword")]
        public bool isNL2Sql { get; set; }

        [Column(type = "keyword")]
        public DbInfo dbInfo { get; set; } = new DbInfo();

        [Column(type = "keyword")]
        public int total { get; set; }

        [JsonIgnore]
        public string _id { get; set; }

        [JsonIgnore]
        public string _index { get; set; }
    }

    public class DbInfo
    {
        public string key { get; set; }

        public List<string> tableName { get; set; } = new List<string>();

        public bool isView { get; set; }
    }
}