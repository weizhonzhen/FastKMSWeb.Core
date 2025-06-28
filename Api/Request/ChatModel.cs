using FastKMSApi.Core.Model;
using Newtonsoft.Json;

namespace FastKMSApi.Core.Request
{
    public class ChatModel
    {
        public string? _id { get; set; }

        public string chatIndex { get; set; }

        public string? message {  get; set; }
        
        public List<KmsModel>? kmsModel { get; set; }
    }
}
