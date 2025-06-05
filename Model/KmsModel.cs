using Newtonsoft.Json;

namespace FastKMSWeb.Core.Model
{
    public class KmsModel
    {
        public string Name {  get; set; }

        public  DateTime DateTime { get; set; }

        public bool IsNL2Sql { get; set; }

        public string Remark {  get; set; }

        public string Index { get; set; } = $"kms-{Guid.NewGuid().ToString()}";

        public string VectorIndex {  get; set; } = $"vector-{Guid.NewGuid().ToString()}";

        [JsonIgnore]
        public string _id {  get; set; }

        [JsonIgnore]
        public string _index { get; set; }
    }
}