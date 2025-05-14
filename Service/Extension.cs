using Newtonsoft.Json.Linq;

namespace FastKMSWeb.Core.Service
{
    public static class Extension
    {
        internal static string ToStr(this object strValue)
        {
            if (strValue == null)
                return string.Empty;
            else
                return strValue.ToString();
        }

        internal static DateTime ToDate(this string strValue)
        {
            if (strValue.ToStr() == "")
                return DateTime.MinValue;
            return Convert.ToDateTime(strValue);
        }

        internal static int ToInt(this string str, int defValue)
        {
            int tmp = 0;
            if (Int32.TryParse(str, out tmp))
                return (int)tmp;
            else
                return defValue;
        }

        internal static string GetConfig(string key, string fileName = "db.json")
        {
            var build = new ConfigurationBuilder();
            build.SetBasePath(Directory.GetCurrentDirectory());
            build.AddJsonFile(fileName, optional: true, reloadOnChange: true);
            var config = build.Build();
            return config.GetSection(key).Value;      
        }

        internal static Dictionary<string, object> JsonToDic(this string jsonValue, bool isString = false)
        {
            try
            {
                var item = new Dictionary<string, object>();

                if (string.IsNullOrEmpty(jsonValue))
                    return item;

                var jo = JObject.Parse(jsonValue);

                foreach (var temp in jo)
                {
                    if (isString)
                        item.Add(temp.Key, temp.Value.ToStr());
                    else
                        item.Add(temp.Key, temp.Value);
                }
                return item;
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }

        internal static List<Dictionary<string, object>> JsonToDics(this string jsonValue, bool isString = false)
        {
            try
            {
                var item = new List<Dictionary<string, object>>();

                if (string.IsNullOrEmpty(jsonValue))
                    return item;

                var ja = JArray.Parse(jsonValue);

                foreach (var jo in ja)
                {
                    item.Add(JsonToDic(jo.ToStr(),isString));
                }
                return item;
            }
            catch
            {
                return new List<Dictionary<string, object>>();
            }
        }
    }
}