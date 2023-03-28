using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using CommonService.Models;
using System.Text;

namespace CommonService
{
    public class BaseService : IBaseService
    {
        //定义数据库字符串
        //private static string conStr = "server=.;uid=sa;pwd=sa;database=master";
        private static SqlConnection mySqlConnection;
        private static bool IsCanConnectioned = false;

        /// <summary>
        /// 测试连接数据库是否成功
        /// </summary>
        /// <returns></returns>
        public bool ConnectionTest(string conStr)
        {
            //创建连接对象
            mySqlConnection = new SqlConnection(conStr);
            try
            {
                //Open DataBase
                //打开数据库
                mySqlConnection.Open();
                IsCanConnectioned = true;
            }
            catch
            {
                //Can not Open DataBase
                //打开不成功 则连接不成功
                IsCanConnectioned = false;
            }
            finally
            {
                //Close DataBase
                //关闭数据库连接
                mySqlConnection.Close();
            }
            //mySqlConnection   is   a   SqlConnection   object 
            if (mySqlConnection.State == ConnectionState.Closed || mySqlConnection.State == ConnectionState.Broken)
            {
                //Connection   is   not   available  
                return IsCanConnectioned;
            }
            else
            {
                //Connection   is   available  
                return IsCanConnectioned;
            }
        }
        /// <summary>
        /// 返回连接字符串
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public string GetConnectioning(string servername, string uid, string pwd, string port)
        {
            return string.Format("server={0};uid={1};pwd={2};database=master", servername, uid, pwd, port);
        }
        /// <summary>
        /// 获取数据库字符串
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public string GetConnectioning(string servername, string uid, string pwd, string db, string port)
        {
            return string.Format("server={0};uid={1};pwd={2};database={3}", servername, uid, pwd, db);
        }
        /// <summary>
        /// 获取数据库列表
        /// </summary>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public List<string> GetDBNameList(string conStr)
        {
            //List<DBName> list =new List<DBName>();
            string sql = "select [name] from master.dbo.sysdatabases where DBId>6 Order By [Name] ";
            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    var list = connection.Query<string>(sql).ToList();
                    return list;
                }
            }
            catch
            {
                return null;
            }

        }

        public List<DBModel> GetDBList(string conStr)
        {
            //List<DBName> list =new List<DBName>();
            string sql = "select [name] from master.dbo.sysdatabases where DBId>6 Order By [Name] ";
            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    var list = connection.Query<DBModel>(sql).ToList();
                    return list;
                }
            }
            catch
            {
                return null;
            }

        }
        /// <summary>
        /// 获取特定数据库的表名列表
        /// </summary>
        /// <param name="conStr"></param>
        /// <returns></returns>

        public List<TableModel> GetDBTableList(string conStr, string dbName = "")
        {
            var list = new List<TableModel>();
            //string sql = "SELECT TABLE_NAME as name FROM INFORMATION_SCHEMA.TABLES where TABLE_TYPE='BASE TABLE' ";
            string sql = "select a.name AS tableName,CONVERT(NVARCHAR(100),isnull(g.[value],'')) AS tableDesc from sys.tables a left join sys.extended_properties g on (a.object_id = g.major_id AND g.minor_id = 0)";
            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    list = connection.Query<TableModel>(sql).ToList();
                }
            }
            catch
            {

            }
            return list;
        }
        /// <summary>
        /// 获取特定数据库里面的存储过程
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<ProcModel> GetProcList(string conStr, string dbName = "")
        {
            var list = new List<ProcModel>();
            string sql = @"  select name as procName, (select text from syscomments where id=OBJECT_ID(name)) as proDerails
                         from dbo.sysobjects  o  where OBJECTPROPERTY(id, N'IsProcedure') = 1 order by name  ";
            try
            {               
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    list = connection.Query<ProcModel>(sql).ToList();
                }
            }
            catch
            {

            }
            return list;
        }
        /// <summary>
        /// 获取特定数据库里面的视图
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<ViewModel> GetViewList(string conStr, string dbName = "")
        {
            var list = new List<ViewModel>();
            string sql = @"  select name as viewName, (select text from syscomments where id=OBJECT_ID(name)) as viewDerails
                         from dbo.sysobjects  o  where OBJECTPROPERTY(id, N'IsView') = 1 order by name  ";
            try
            {              
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    list = connection.Query<ViewModel>(sql).ToList();
                }
            }
            catch
            {

            }
            return list;
        }

        /// <summary>
        /// 获取字段的信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public List<TableDetail> GetTableDetail(string tableName, string conStr, string dbName = "")
        {
            var list = new List<TableDetail>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT [index] = a.colorder,    Title = a.name,    isMark =        CASE    WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1 THEN '1' ELSE '0' END, ");
            sb.Append("isPK =  CASE   WHEN EXISTS(SELECT  1  FROM sysobjects WHERE xtype = 'PK' AND parent_obj = a.id AND name IN(SELECT name  FROM sysindexes WHERE indid IN(SELECT indid  FROM sysindexkeys  WHERE id = a.id AND colid = a.colid)) ) THEN '1' ELSE '0' END, ");
            sb.Append("	FieldType = b.name,fieldLenth = COLUMNPROPERTY(a.id, a.name, 'PRECISION'),isAllowEmpty =  CASE   WHEN a.isnullable = 1 THEN '1' ELSE '0' END, defaultValue = ISNULL(e.text, ''), fieldDesc = ISNULL(g.[value], '') ");
            sb.Append("FROM syscolumns a LEFT JOIN systypes b  ON a.xusertype = b.xusertype INNER JOIN sysobjects d ON a.id = d.id AND d.xtype = 'U' AND d.name <> 'dtproperties' LEFT JOIN syscomments e ON a.cdefault = e.id ");
            sb.Append("LEFT JOIN sys.extended_properties g ON a.id = G.major_id AND a.colid = g.minor_id LEFT JOIN sys.extended_properties f ON d.id = f.major_id AND f.minor_id = 0");
            //--如果只查询指定表,加上此红色where条件，tablename是要查询的表名；去除红色where条件查询说有的表信息
            sb.Append("WHERE d.name = '" + tableName + "' ORDER BY a.id, a.colorder, d.name");
            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    list = connection.Query<TableDetail>(sb.ToString()).ToList();
                }
            }
            catch
            { }

            return list;
        }

        public void BakDataBase(List<string> list, string conStr, string path)
        {

            foreach (var item in list)
            {
                string sql = string.Format("backup database {0} to disk='{1}{0}.bak'  ", item, path);

                // http://www.cnblogs.com/minideas/archive/2009/10/29/1591891.html
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    int count = connection.Execute(sql);
                }
            }

        }
        /// <summary>
        /// 获取建表SQL
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public string GetTableSQL(string tableName, string conStr)
        {
            string sql = string.Format(@"declare @tabname varchar(50)
set @tabname = '{0}'--表名

if (object_id('tempdb.dbo.#t') is not null)
                begin
                DROP TABLE #t
end

select  'create table [' + so.name + '] (' + o.list + ')'
    + CASE WHEN tc.Constraint_Name IS NULL THEN '' ELSE 'ALTER TABLE ' + so.Name + ' ADD CONSTRAINT ' + tc.Constraint_Name + ' PRIMARY KEY ' + ' (' + LEFT(j.List, Len(j.List) - 1) + ')' END
     TABLE_DDL
into #t from    sysobjects so
cross apply
    (SELECT
        '  [' + column_name + '] ' +
        data_type + case data_type
            when 'sql_variant' then ''
            when 'text' then ''
            when 'ntext' then ''
            when 'xml' then ''
            when 'decimal' then '(' + cast(numeric_precision as varchar) + ', ' + cast(numeric_scale as varchar) + ')'
            else coalesce('(' +case when character_maximum_length = -1 then 'MAX' else cast(character_maximum_length as varchar) end + ')','') end + ' ' +
        case when exists(
        select id from syscolumns
        where object_name(id)= so.name
        and name = column_name
        and columnproperty(id, name,'IsIdentity') = 1
        ) then
        'IDENTITY(' +
        cast(ident_seed(so.name) as varchar) + ',' +
        cast(ident_incr(so.name) as varchar) + ')'
        else ''
        end + ' ' +
         (case when IS_NULLABLE = 'No' then 'NOT ' else '' end ) +'NULL ' +
          case when information_schema.columns.COLUMN_DEFAULT IS NOT NULL THEN 'DEFAULT ' + information_schema.columns.COLUMN_DEFAULT ELSE '' END + ', '

     from information_schema.columns where table_name = so.name
     order by ordinal_position
    FOR XML PATH('')) o(list)
left join
    information_schema.table_constraints tc
on tc.Table_name = so.Name
AND tc.Constraint_Type = 'PRIMARY KEY'
cross apply
    (select '[' + Column_Name + '], '
     FROM information_schema.key_column_usage kcu
     WHERE kcu.Constraint_Name = tc.Constraint_Name
     ORDER BY
        ORDINAL_POSITION
     FOR XML PATH('')) j(list)
where xtype = 'U'
AND name = @tabname

select 'USE ' + db_name() + CHAR(13) + 'GO' + CHAR(13) +
(--区别有主键和没主键
case when(select count(a.constraint_type)
from information_schema.table_constraints a
inner
join information_schema.constraint_column_usage b
on a.constraint_name = b.constraint_name
where a.constraint_type = 'PRIMARY KEY'--主键
and a.table_name = @tabname) = 1 then
replace(table_ddl, ', )ALTER TABLE', ')' + CHAR(13) + 'ALTER TABLE')
else SUBSTRING(table_ddl, 1, len(table_ddl) - 3) + ')' end
) tableDesc from #t
drop table #t", tableName);
            string result = string.Empty;
            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    
                    var list = connection.Query<TableModel>(sql).ToList();
                    result = list[0].tableDesc;
                    
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            return result;
        }
    }
}
