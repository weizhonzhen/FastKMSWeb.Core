using FastKMSApi.Core.Model;

namespace FastKMSApi.Core.Request
{
    public class Nl2SqlModel
    {
        public string message { get; set; }

        public DbInfo dbInfo { get; set; }

        public string chatIndex { get; set; }
    }
}
