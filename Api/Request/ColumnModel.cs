using FastKMSApi.Core.Model;

namespace FastKMSApi.Core.Request
{
    public class ColumnModel
    {
        public required string key { get; set; }

        public required string tableName {  get; set; }

        public bool isView { get; set; }
    }
}