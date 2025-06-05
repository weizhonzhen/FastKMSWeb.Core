using Microsoft.AspNetCore.Components;

namespace FastKMSWeb.Core.Model
{
    public class DbKms
    {
        public List<Dictionary<string, object>> Record { get; set; } = new List<Dictionary<string, object>>();

        public string TableName { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public List<Dictionary<string, object>> TableList { get; set; } = new List<Dictionary<string, object>>();
    }
}
