using FastElasticsearch.Core;
using Newtonsoft.Json;

namespace FastKMSWeb.Core.Model
{
    public class ChatInfo
    {
        [Column(type = "keyword")]
        public string Name { get; set; }

        [Column(type = "keyword")]
        public string ChatIndex { get; set; }

        [Column(type = "keyword")]
        public List<KmsModel> Kms { get; set; } = new List<KmsModel>();

        [Column(type = "keyword")]
        public DateTime BeginTime { get; set; }

        [Column(type = "keyword")]
        public bool IsNL2Sql { get; set; }

        [Column(type = "keyword")]
        public DbInfo DbInfo { get; set; } = new DbInfo();

        [Column(type = "keyword")]
        public int Total { get; set; }

        [JsonIgnore]
        public string _id { get; set; }

        [JsonIgnore]
        public string _index { get; set; }
    }

    public class DbInfo
    {
        public string Key { get; set; }

        public List<string> TableName { get; set; } = new List<string>();

        public bool IsView { get; set; }
    }
}