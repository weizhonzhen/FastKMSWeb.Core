using FastKMSApi.Core.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace FastKMSApi.Core.Service
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

        internal static bool ToBool(this object strValue)
        {
            if (string.IsNullOrEmpty(strValue.ToStr()))
                return false;
            else
                return bool.Parse(strValue.ToStr());
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

        internal static List<T> ConfigList<T>(string key, string fileName = "db.json") where T : class, new()
        {
            var build = new ConfigurationBuilder();
            build.SetBasePath(Directory.GetCurrentDirectory());
            build.AddJsonFile(fileName, optional: true, reloadOnChange: true);
            var config = build.Build();
            return new ServiceCollection().AddOptions().Configure<List<T>>(config.GetSection(key)).BuildServiceProvider().GetService<IOptions<List<T>>>().Value;
        }

        internal static T Config<T>(string key, string fileName = "db.json") where T : class, new()
        {
            var build = new ConfigurationBuilder();
            build.SetBasePath(Directory.GetCurrentDirectory());
            build.AddJsonFile(fileName, optional: true, reloadOnChange: true);
            var config = build.Build();
            return new ServiceCollection().AddOptions().Configure<T>(config.GetSection(key)).BuildServiceProvider().GetService<IOptions<T>>().Value;
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
                    item.Add(JsonToDic(jo.ToStr(), isString));
                }
                return item;
            }
            catch
            {
                return new List<Dictionary<string, object>>();
            }
        }

        private static List<string> GetCol(DbDataReader dr)
        {
            var list = new List<string>();
            for (var i = 0; i < dr.FieldCount; i++)
            {
                var colName = dr.GetName(i);
                if (!list.Exists(a => string.Compare(a, colName, true) == 0))
                    list.Add(colName);
            }
            return list;
        }

        internal static List<Dictionary<string, object>> ToDics(this DbDataReader reader)
        {
            var result = new List<Dictionary<string, object>>();
            var cols = GetCol(reader);

            while (reader.Read())
            {
                var dic = new Dictionary<string, object>();
                cols.ForEach(a =>
                {
                    if (reader[a] is DBNull)
                        dic.Add(a, string.Empty);
                    else
                        dic.Add(a, reader[a]);
                });

                result.Add(dic);
            }
            return result;
        }

        internal static List<Dictionary<string, object>> ToDics(this DbDataReader reader, List<ColumnInfo> column)
        {
            var result = new List<Dictionary<string, object>>();
            var cols = GetCol(reader);

            while (reader.Read())
            {
                var dic = new Dictionary<string, object>();
                cols.ForEach(a =>
                {
                    if (string.Compare(a, "rn", true) == 0)
                        return;

                    var info = column.Find(b => string.Compare(a, b.colName, true) == 0);
                    var key = info == null ? a : info.colComments;
                    key = string.IsNullOrEmpty(key) ? a : key;
                    if (reader[a] is DBNull)
                        dic.Add(key, string.Empty);
                    else
                        dic.Add(key, reader[a]);
                });

                result.Add(dic);
            }
            return result;
        }
    }

    internal class DbProviderFactories : DbProviderFactory
    {
        private readonly static Dictionary<string, DbProviderFactory> dbCache = new Dictionary<string, DbProviderFactory>();

        public static DbProviderFactory GetFactory(string key)
        {
            try
            {
                DbProviderFactory factory;
                dbCache.TryGetValue(key, out factory);

                if (factory == null)
                {
                    var config = AppSetting.DataConfig.Find(a => a.key == key) ?? new DataConfig();

                    var assembly = AppDomain.CurrentDomain.GetAssemblies().ToList().Find(a => a.FullName.Split(',')[0] == config.providerName);
                    if (assembly == null)
                        assembly = Assembly.Load(config.providerName);

                    var model = assembly.GetType(config.factoryClient, false);
                    var field = model.GetField("Instance");
                    var instance = field.GetValue(model) as DbProviderFactory;

                    dbCache.Add(key, instance);
                    return instance;
                }
                else
                    return factory;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class DataDbType
    {
        public readonly static string Oracle = "Oracle";

        public readonly static string MySql = "MySql";

        public readonly static string SqlServer = "SqlServer";
    }
}