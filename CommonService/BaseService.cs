using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Dapper;
namespace CommonService
{
    public class BaseService
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
        public string GetConnectioning(string servername, string uid, string pwd)
        {
            return string.Format("server={0};uid={1};pwd={2};database=master", servername,uid,pwd);
        }
        /// <summary>
        /// 获取数据库字符串
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public string GetConnectioning(string servername, string uid, string pwd,string db)
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


        public List<string> GetDBTableList(string conStr)
        {
            var list = new List<string>();
            string sql = "SELECT TABLE_NAME as name FROM INFORMATION_SCHEMA.TABLES";
            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                     list = connection.Query<string>(sql).ToList();
                }
            }
            catch
            {
                
            }
            return list;
        } 
    }
}
