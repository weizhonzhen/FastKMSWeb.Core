using Newtonsoft.Json;

namespace FastKMSWeb.Core.Model
{
    public class ChatInfo
    {
        public string Name { get; set; }

        public string ChatIndex { get; set; }

        public List<KmsModel> Kms { get; set; } = new List<KmsModel>();

        public DateTime BeginTime { get; set; }

        public int Total { get; set; }

        [JsonIgnore]
        public string _id { get; set; }

        [JsonIgnore]
        public string _index { get; set; }
    }
}