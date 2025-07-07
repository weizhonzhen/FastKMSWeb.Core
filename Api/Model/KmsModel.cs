using FastElasticsearch.Core;
using Newtonsoft.Json;

namespace FastKMSApi.Core.Model
{
    public class KmsModel
    {
        [Column(type = "keyword")]
        public string? name {  get; set; }

        [Column(type = "keyword")]
        public DateTime? dateTime { get; set; } = DateTime.Now;

        [Column(type = "keyword")]
        public bool? isNL2Sql { get; set; }

        [Column(type = "keyword")]
        public string? remark {  get; set; }

        public string? index { get; set; } = $"kms-{Guid.NewGuid().ToString()}";

        public string? vectorIndex {  get; set; } = $"vector-{Guid.NewGuid().ToString()}";

        [JsonIgnore]
        public string? _id {  get; set; }

        [JsonIgnore]
        public string? _index { get; set; }
    }
}