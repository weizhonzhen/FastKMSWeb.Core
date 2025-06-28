namespace FastKMSApi.Core.Request
{
    public class TableModel
    {
        public required string key { get; set; }

        public bool isView { get; set; }

        public required string tabName { get; set; }

        public string? tabComments { get; set; }
    }
}
