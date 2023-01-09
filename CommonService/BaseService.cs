using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using CommonService.Models;
using System.Text;

namespace CommonService
{
    public class BaseService:IBaseService
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
            return string.Format("server={0};uid={1};pwd={2};database=master", servername,uid,pwd,port);
        }
        /// <summary>
        /// 获取数据库字符串
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public string GetConnectioning(string servername, string uid, string pwd,string db, string port)
        {
            return string.Format("server={0};uid={1};pwd={2};database={3}", servername, uid, pwd,db);
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
               // http://www.cnblogs.com/minideas/archive/2009/10/29/1591891.html
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
                // http://www.cnblogs.com/minideas/archive/2009/10/29/1591891.html
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
            sb.Append("WHERE d.name = '"+ tableName + "' ORDER BY a.id, a.colorder, d.name");        
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

        public void BakDataBase(List<string> list, string conStr,string path)        {
        
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

    }
}
