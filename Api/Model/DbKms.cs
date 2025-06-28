namespace FastKMSApi.Core.Model
{
    public class DbKms
    {
        public List<Dictionary<string, object>> record { get; set; } = new List<Dictionary<string, object>>();

        public List<Dictionary<string, object>> tableList { get; set; } = new List<Dictionary<string, object>>();
    }
}
