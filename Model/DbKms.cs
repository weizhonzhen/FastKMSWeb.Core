namespace FastKMSWeb.Core.Model
{
    public class DbKms
    {
        public List<Dictionary<string, object>> Record { get; set; } = new List<Dictionary<string, object>>();

        public List<Dictionary<string, object>> TableList { get; set; } = new List<Dictionary<string, object>>();
    }
}
