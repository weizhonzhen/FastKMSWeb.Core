namespace FastKMSApi.Core.Request
{
    public class RequestPage
    {
        public string? vectorIndex { get; set; }
        public string? key { get; set; }
        public int pageId { get; set; }
        public int pageSize { get; set; }
    }
}
