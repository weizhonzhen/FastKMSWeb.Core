using FastElasticsearch.Core;
using FastElasticsearch.Core.Model;
using FastKMSWeb.Core.Model;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace FastKMSWeb.Core.Service
{
    public class DataService
    {
        private readonly string tableKey = "TableName";
        private readonly static string dbKey = "DbName";
        private readonly IElasticsearch elasticsearch;
        public DataService(IElasticsearch _elasticsearch)
        {
            elasticsearch = _elasticsearch;
        }

        public static PageResult TableList(string key, int pageId, int pageSize)
        {
            var result = new FastElasticsearch.Core.Model.PageResult();

            if (string.IsNullOrEmpty(key))
                return result;

            result.Page.PageSize = pageSize;
            result.Page.PageId = pageId;
            try
            {
                var config = AppSetting.DataConfig.Find(a => a.key == key);
                var sql = string.Empty;
                var sqlCount = string.Empty;

                if (config.dbType == DatadbType.MySql)
                {
                    sql = $"select table_name as tabName, table_comment as tabComments from information_schema.TABLES where table_schema='{config.schema}' and table_type='BASE TABLE' order by table_name";
                    sql = $"select * from ({sql}) field limit {(pageId - 1) * pageSize + 1}, {pageSize}";
                    sqlCount = $"select count(0) from information_schema.TABLES where table_schema='{config.schema}' and table_type='BASE TABLE'";
                }

                if (config.dbType == DatadbType.Oracle)
                {
                    sql = $"select a.table_name as tabName,comments as tabComments from all_tables a inner join all_tab_comments b on a.TABLE_NAME=b.TABLE_NAME and a.owner='{config.schema}' order by a.table_name";
                    sql = $"select * from(select field.*,ROWNUM RN from({sql}) field where rownum<={pageId * pageSize}) where rn>={(pageId - 1) * pageSize + 1}";
                    sqlCount = $"select count(0) from all_tables a inner join all_tab_comments b on a.TABLE_NAME=b.TABLE_NAME and a.owner='{config.schema}'";
                }

                if (config.dbType == DatadbType.SqlServer)
                {
                    sql = "select name as tabName,(select top 1 value from sys.extended_properties where major_id=object_id(a.name) and minor_id=0) as tabComments from sys.objects a where type='U' order by name";
                    sql = $"select top {pageSize} * from (select row_number()over(order by tempcolumn)temprownumber,* from(select tempcolumn = 0, * from ({sql})t)tt)ttt where temprownumber >= {(pageId - 1) * pageSize + 1}";
                    sqlCount = "select count(0) from sys.objects a where type='U'";
                }

                using (var conn = DbProviderFactories.GetFactory(key).CreateConnection())
                {
                    conn.ConnectionString = config.connStr;
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        using (var rd = cmd.ExecuteReader())
                        {
                            result.List = rd.ToDics();
                        }

                        cmd.CommandText = sqlCount;
                        using (var rd = cmd.ExecuteReader())
                        {
                            var dt = new DataTable();
                            dt.Load(rd);
                            result.Page.TotalRecord = dt.Rows[0][0].ToStr().ToInt(0);
                        }
                    }
                    conn.Close();
                }
                result.Page.TotalPage = result.Page.TotalRecord / pageSize + 1;

                if ((result.Page.TotalRecord % result.Page.PageSize) == 0)
                    result.Page.TotalPage = result.Page.TotalRecord / result.Page.PageSize;
                else
                    result.Page.TotalPage = (result.Page.TotalRecord / result.Page.PageSize) + 1;

                if (result.Page.PageId > result.Page.TotalPage)
                    result.Page.PageId = result.Page.TotalPage;

                return result;
            }
            catch
            {
                return result;
            }
        }

        public static PageResult ViewList(string key, int pageId, int pageSize)
        {
            var result = new FastElasticsearch.Core.Model.PageResult();

            if (string.IsNullOrEmpty(key))
                return result;

            result.Page.PageSize = pageSize;
            result.Page.PageId = pageId;
            try
            {
                var config = AppSetting.DataConfig.Find(a => a.key == key);
                var sql = string.Empty;
                var sqlCount = string.Empty;

                if (config.dbType == DatadbType.MySql)
                {
                    sql = $"select TABLE_NAME as tabName,'' as tabComments from information_schema.VIEWS v  where table_schema ='{config.schema}'";
                    sql = $"select * from ({sql}) field limit {(pageId - 1) * pageSize + 1}, {pageSize}";
                    sqlCount = $"select count(0) from information_schema.VIEWS where table_schema='{config.schema}'";
                }

                if (config.dbType == DatadbType.Oracle)
                {
                    sql = $"select a.VIEW_NAME,'' as tabComments from all_views a where a.owner='{config.schema}'";
                    sql = $"select * from(select field.*,ROWNUM RN from({sql}) field where rownum<={pageId * pageSize}) where rn>={(pageId - 1) * pageSize + 1}";
                    sqlCount = $"select count(0) from all_views a where a.owner='{config.schema}'";
                }

                if (config.dbType == DatadbType.SqlServer)
                {
                    sql = "select name as tabName,'' as tabComments from sys.objects where type='v'";
                    sql = $"select top {pageSize} * from (select row_number()over(order by tempcolumn)temprownumber,* from(select tempcolumn = 0, * from ({sql})t)tt)ttt where temprownumber >= {(pageId - 1) * pageSize + 1}";
                    sqlCount = "select count(0) from sys.objects a where type='v'";
                }

                using (var conn = DbProviderFactories.GetFactory(key).CreateConnection())
                {
                    conn.ConnectionString = config.connStr;
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        using (var rd = cmd.ExecuteReader())
                        {
                            result.List = rd.ToDics();
                        }

                        var viewList = ViewComments(key);
                        result.List.ForEach(a =>
                        {
                            var viewDic = viewList.Find(v => v.GetValue("tabName").ToStr() == a.GetValue("tabName").ToStr()) ?? new Dictionary<string, object>();
                            if (viewDic.Count > 0)
                                a["tabComments"] = viewDic.GetValue("tabComments").ToStr();
                        });

                        cmd.CommandText = sqlCount;
                        using (var rd = cmd.ExecuteReader())
                        {
                            var dt = new DataTable();
                            dt.Load(rd);
                            result.Page.TotalRecord = dt.Rows[0][0].ToStr().ToInt(0);
                        }
                    }
                    conn.Close();
                }
                result.Page.TotalPage = result.Page.TotalRecord / pageSize + 1;

                if ((result.Page.TotalRecord % result.Page.PageSize) == 0)
                    result.Page.TotalPage = result.Page.TotalRecord / result.Page.PageSize;
                else
                    result.Page.TotalPage = (result.Page.TotalRecord / result.Page.PageSize) + 1;

                if (result.Page.PageId > result.Page.TotalPage)
                    result.Page.PageId = result.Page.TotalPage;

                return result;
            }
            catch
            {
                return result;
            }
        }

        public List<ColumnInfo> ColumnList(string key, string tableName, bool isView = false)
        {
            try
            {
                var viewColumnList = new List<Dictionary<string, object>>();
                var config = AppSetting.DataConfig.Find(a => a.key == key);
                var list = new List<ColumnInfo>();
                var sql = string.Empty;
                var dt = new DataTable();

                if (config.dbType == DatadbType.MySql)
                    sql = @$"select column_name,data_type,character_maximum_length,column_comment,
                            (select count(0) from INFORMATION_schema.KEY_COLUMN_USAGE a where TABLE_schema='{config.schema}' and TABLE_NAME='{tableName}' and constraint_name='PRIMARY' and c.column_name=a.column_name),
                            (select count(0) from information_schema.statistics a where table_schema = '{config.schema}' and table_name = '{tableName}' and c.column_name=a.column_name),
                            is_nullable,numeric_precision,numeric_scale,column_type,default(column_name) from information_schema.columns c where table_name='{tableName}' and TABLE_schema='{config.schema}'
                            order by ordinal_position asc";

                if (config.dbType == DatadbType.Oracle)
                    sql = @$"select a.column_name,data_type,data_length,b.comments,
                            (select count(0) from all_cons_columns aa, all_constraints bb where aa.constraint_name = bb.constraint_name and bb.constraint_type = 'P' and bb.table_name = '{tableName}' and aa.column_name=a.column_name),
                            (select count(0) from all_ind_columns t,user_indexes i where t.index_name = i.index_name and t.table_name = i.table_name and t.table_name = '{tableName}' and t.column_name=a.column_name),
                            nullable,data_precision,data_scale,data_default
                            from all_tab_columns a 
                            inner join all_col_comments b on a.table_name='{tableName}' and a.table_name=b.table_name and a.column_name=b.column_name 
                            order by a.column_id asc";

                if (config.dbType == DatadbType.SqlServer)
                    sql = @$"select a.name,(select top 1 name from sys.systypes c where a.xtype=c.xtype) as type ,
                            length,b.value,(select count(0) from INFORMATION_schema.KEY_COLUMN_USAGE where TABLE_NAME='{tableName}' and COLUMN_NAME=a.name),
                            (SELECT count(0) FROM sysindexes aa JOIN sysindexkeys bb ON aa.id=bb.id AND aa.indid=bb.indid JOIN sysobjects cc ON bb.id=cc.id  JOIN syscolumns dd ON bb.id=dd.id AND bb.colid=dd.colid WHERE aa.indid NOT IN(0,255) AND cc.name='{tableName}' and dd.name=a.name),
                            isnullable,prec,scale,''
                            from syscolumns a 
                            left join sys.extended_properties b on major_id = id and minor_id = colid and b.name ='MS_Description' 
                            where a.id=object_id('{tableName}') 
                            order by a.colid asc";

                using (var conn = DbProviderFactories.GetFactory(key).CreateConnection())
                {
                    conn.ConnectionString = config.connStr;
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        using (var rd = cmd.ExecuteReader())
                        {
                            if (isView)
                                viewColumnList = ViewColumnComments(tableName);

                            dt.Load(rd);
                            foreach (DataRow item in dt.Rows)
                            {
                                var columnInfo = new ColumnInfo();
                                columnInfo.colName = (item.ItemArray[0] == DBNull.Value ? "" : item.ItemArray[0].ToString());
                                columnInfo.colType = item.ItemArray[1] == DBNull.Value ? "" : item.ItemArray[1].ToString();
                                columnInfo.colLength = item.ItemArray[2] == DBNull.Value ? 0 : decimal.Parse(item.ItemArray[2].ToString());
                                columnInfo.colComments = item.ItemArray[3] == DBNull.Value ? "" : item.ItemArray[3].ToString();
                                columnInfo.isKey = item.ItemArray[4].ToString() != "0" ? true : false;
                                columnInfo.isIndex = item.ItemArray[5].ToString() != "0" ? true : false;
                                columnInfo.isNull = (item.ItemArray[6].ToString().ToUpper().Trim() == "Y" || item.ItemArray[6].ToString().ToUpper().Trim() == "YES" || item.ItemArray[6].ToString().ToUpper().Trim() == "1") ? true : false;
                                columnInfo.precision = item.ItemArray[7] == DBNull.Value ? 0 : int.Parse(item.ItemArray[7].ToString());
                                columnInfo.scale = item.ItemArray[8] == DBNull.Value ? 0 : int.Parse(item.ItemArray[8].ToString());

                                if (isView)
                                {
                                    var viewDic = viewColumnList.Find(a => a.GetValue("colName").ToStr() == columnInfo.colName) ?? new Dictionary<string, object>();
                                    if (viewDic.Count > 0)
                                        columnInfo.colComments = viewDic.GetValue("colComments").ToStr();
                                    else
                                        UpdateColComments(key, tableName, columnInfo, isView);
                                }
                                    
                                columnInfo.showType = ShowColType(columnInfo);
                                list.Add(columnInfo);
                            }
                        }
                    }
                    conn.Close();
                }

                return list;
            }
            catch
            {
                return new List<ColumnInfo>();
            }
        }

        private string ShowColType(ColumnInfo column)
        {
            switch (column.colType.ToLower().Trim())
            {
                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":
                case "varchar2":
                case "nvarchar2":
                    return $"{column.colType}({(column.colLength == -1 ? "max" : column.colLength.ToString())})";
                case "decimal":
                case "numeric":
                case "number":
                    if (column.precision == 0 && column.scale == 0)
                        return column.colType;
                    else
                        return $"{column.colType}({column.precision},{column.scale})";
                case "bit":
                    return $"{column.colType}({column.precision})";
                default:
                    return column.colType;
            }
        }

        public bool UpdateColComments(string key, string table, ColumnInfo column, bool isView = false)
        {
            try
            {
                if (isView)
                {
                    var viewColList = ViewColumnComments(table);
                    var viewDic = viewColList.Find(a => a.GetValue("colName").ToStr() == column.colName) ?? new Dictionary<string, object>();

                    if (viewDic.Count == 0)
                    {
                        var json = JsonConvert.SerializeObject(column).ToStr();
                        var dic = json.JsonToDic(true);
                        dic.Add(tableKey, table);
                        elasticsearch.Add(AppSetting.ViewColumnIndex, Guid.NewGuid().ToStr(), dic);
                    }
                    else
                    {
                        if (viewDic.GetValue("colComments").ToStr() != column.colComments)
                        {
                            var id = viewDic.GetValue("_id").ToStr();
                            elasticsearch.Update(AppSetting.ViewColumnIndex, id, new { colComments = column.colComments });
                        }
                    }
                }
                else
                {
                    var config = AppSetting.DataConfig.Find(a => a.key == key);
                    var sql = string.Empty;

                    if (config.dbType == DatadbType.MySql)
                        sql = $"alter table {table} modify {column.colName} {column.showType} comment '{column.colComments}'";

                    if (config.dbType == DatadbType.Oracle)
                        sql = $"Comment on column {table}.{column.colName} is '{column.colComments}'";

                    if (config.dbType == DatadbType.SqlServer)
                        sql = @$"select count(0) from syscolumns where id = object_id('{table}') and name='{column.colName}'
                          and exists(select 1 from sys.extended_properties where object_id('{0}')=major_id and colid=minor_id)";

                    using (var conn = DbProviderFactories.GetFactory(key).CreateConnection())
                    {
                        conn.ConnectionString = config.connStr;
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            if (config.dbType == DatadbType.SqlServer)
                            {
                                var dt = new DataTable();
                                var rd = cmd.ExecuteReader();
                                dt.Load(rd);

                                sql = $"exec sys.sp_updateextendedproperty N'MS_Description',N'{column.colComments}',N'schema', N'dbo', N'TABLE',N'{table}',N'column',N'{column.colName}'";

                                if (dt.Rows[0][0].ToStr().ToInt(0) >= 1)
                                {
                                    cmd.CommandText = sql;
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd.CommandText = sql.Replace("sp_updateextendedproperty", "sp_addextendedproperty");
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        conn.Close();
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private List<Dictionary<string, object>> ViewColumnComments(string table)
        {
            var data = new List<Dictionary<string, object>>();
            var viewColumn = elasticsearch.GetList(AppSetting.ViewColumnIndex,
                                        new QueryModel
                                        {
                                            IsPhrase = true,
                                            Match = new Dictionary<string, object> { { tableKey, table } }
                                        }, 999);

            if (viewColumn.IsSuccess && viewColumn.List.Count > 0)
                data = viewColumn.List;

            return data;
        }

        private static List<Dictionary<string, object>> ViewComments(string key)
        {
            var elasticsearch = new FastElasticsearch.Core.Elasticsearch();
            var data = new List<Dictionary<string, object>>();
            var viewTable = elasticsearch.GetList(AppSetting.ViewTableIndex,
                                        new QueryModel
                                        {
                                            IsPhrase = true,
                                            Match = new Dictionary<string, object> { { dbKey, key } }
                                        }, 999);

            if (viewTable.IsSuccess && viewTable.List.Count > 0)
                data = viewTable.List;

            return data;
        }

        public bool UpdateTabComments(string key, TableInfo table, bool isView = false)
        {
            try
            {
                if (isView)
                {
                    var viewList = ViewComments(key);
                    var viewDic = viewList.Find(a => a.GetValue("tabName").ToStr() == table.tabName) ?? new Dictionary<string, object>();

                    if (viewDic.Count == 0)
                    {
                        var json = JsonConvert.SerializeObject(table).ToStr();
                        var dic = json.JsonToDic(true);
                        dic.Add(dbKey, key);
                        elasticsearch.Add(AppSetting.ViewTableIndex, Guid.NewGuid().ToStr(), dic);
                    }
                    else
                    {
                        var id = viewDic.GetValue("_id").ToStr();
                        elasticsearch.Update(AppSetting.ViewTableIndex, id, new { tabComments = table.tabComments });
                    }
                }
                else
                {
                    var config = AppSetting.DataConfig.Find(a => a.key == key);
                    var sql = string.Empty;

                    if (config.dbType == DatadbType.MySql)
                        sql = $"alter table {table.tabName} comment '{table.tabComments}'";

                    if (config.dbType == DatadbType.Oracle)
                        sql = $"Comment on table {table.tabName} is '{table.tabComments}'";

                    if (config.dbType == DatadbType.SqlServer)
                        sql = $"select count(0) from sys.extended_properties where object_id('{table.tabName}')=major_id and minor_id=0";

                    using (var conn = DbProviderFactories.GetFactory(key).CreateConnection())
                    {
                        conn.ConnectionString = config.connStr;
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            if (config.dbType == DatadbType.SqlServer)
                            {
                                var dt = new DataTable();
                                var rd = cmd.ExecuteReader();
                                dt.Load(rd);

                                sql = $"exec sys.sp_updateextendedproperty N'MS_Description',N'{table.tabComments}',N'schema', N'dbo', N'TABLE',N'{table.tabName}'";

                                if (dt.Rows[0][0].ToStr().ToInt(0) >= 1)
                                {
                                    cmd.CommandText = sql;
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd.CommandText = sql.Replace("sp_updateextendedproperty", "sp_addextendedproperty");
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        conn.Close();
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string TableSql(string key, List<string> tableName, bool isView = false)
        {
            try
            {
                var sql = new StringBuilder();
                foreach (var name in tableName)
                {
                    var columnList = ColumnList(key, name, isView);

                    sql.AppendLine($"create table {name}");
                    sql.AppendLine("(");
                    foreach (var column in columnList)
                    {
                        sql.AppendLine($"{column.colName} {column.showType} {(column.isKey ? "primary key" : string.Empty)} --{column.colComments}");
                    }

                    sql.AppendLine(");");
                }
                return sql.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public List<Dictionary<string, object>> NL2Data(string key, string sql, List<string> table, int top = 10, bool isView = false)
        {
            var result = new List<Dictionary<string, object>>();
            try
            {
                var tableName = string.Empty;
                table.ForEach(a => {
                    if (sql.Contains(a))
                        tableName = a;
                });

                var config = AppSetting.DataConfig.Find(a => a.key == key);
                var columnList = ColumnList(key, tableName, isView);
                using (var conn = DbProviderFactories.GetFactory(key).CreateConnection())
                {
                    conn.ConnectionString = config.connStr;
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        if (config.dbType == DatadbType.MySql)
                            cmd.CommandText = $"{sql} limit 0, {top}";

                        if (config.dbType == DatadbType.Oracle)
                            cmd.CommandText = $"select * from(select field.*,ROWNUM RN from({sql}) field where rownum<={top}) where rn>= 0";

                        if (config.dbType == DatadbType.SqlServer)
                            cmd.CommandText = $"select top {top} * from {sql}";

                        using (var rd = cmd.ExecuteReader())
                        {
                            result = rd.ToDics(columnList);
                        }
                    }
                    conn.Close();
                }
            }
            catch { }

            return result;
        }
    }
}