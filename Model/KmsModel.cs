using FastElasticsearch.Core;
using Newtonsoft.Json;

namespace FastKMSWeb.Core.Model
{
    public class KmsModel
    {
        [Column(type = "keyword")]
        public string Name {  get; set; }

        [Column(type = "keyword")]
        public  DateTime DateTime { get; set; }

        [Column(type = "keyword")]
        public bool IsNL2Sql { get; set; }

        [Column(type = "keyword")]
        public string Remark {  get; set; }

        public string Index { get; set; } = $"kms-{Guid.NewGuid().ToString()}";

        public string VectorIndex {  get; set; } = $"vector-{Guid.NewGuid().ToString()}";

        [JsonIgnore]
        public string _id {  get; set; }

        [JsonIgnore]
        public string _index { get; set; }
    }
}