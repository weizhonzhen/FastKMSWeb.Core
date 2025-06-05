namespace FastKMSWeb.Core.Model
{
    public class ColumnInfo
    {
        public string colName { get; set; }

        public string colComments { get; set; }

        public string colType { get; set; }

        public decimal colLength { get; set; }

        public bool isKey { get; set; }

        public bool isIndex { get; set; }

        public bool isNull { get; set; }

        public int precision { get; set; }

        public int scale { get; set; }

        public string showType {  get; set; }
    }

    public class TableInfo
    {
        public string tabName { get; set; }

        public string tabComments { get; set; }
    }
}
