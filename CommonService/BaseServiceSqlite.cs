using CommonService.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace CommonService
{
    /// <summary>
    /// SQLLite类库
    /// </summary>
    public class BaseServiceSqlite : IBaseService
    {
        private static bool IsCanConnectioned = false;

        public bool ConnectionTest(string sqlitePath)
        {
            try
            {
                //using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + sqlitePath + ""))
                using (SQLiteConnection connection = new SQLiteConnection(sqlitePath))
                {
                    connection.Open();
                    IsCanConnectioned = true;
                    connection.Close();
                    return IsCanConnectioned;
                }
            }
            catch (Exception ex)
            {
                return IsCanConnectioned;
            }
        }
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="sqlitePath"></param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <param name="port"></param>
        /// <returns></returns>

        public string GetConnectioning(string sqlitePath, string uid, string pwd, string port)
        {
            return string.Format("Data Source = {0}", sqlitePath);
        }
        /// <summary>
        /// 针对SQLite两者方法一致
        /// </summary>
        /// <param name="sqlitePath"></param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <param name="db"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public string GetConnectioning(string sqlitePath, string uid, string pwd, string db, string port)
        {
            return string.Format("Data Source = {0}", sqlitePath);
        }
        /// <summary>
        /// 获取表名数据
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public List<TableModel> GetDBTableList(string conStr, string dbName = "")
        {
            string sql = "SELECT name tableName,'' tableDesc,sql  from sqlite_master where type='table' and name !='sqlite_sequence'; ";

            using (SQLiteConnection connection = new SQLiteConnection(conStr))
            {

                var list = connection.Query<TableModel>(sql).ToList();
                return list;
            }

        }
        /// <summary>
        /// 获取表结构信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="conStr"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public List<TableDetail> GetTableDetail(string tableName, string conStr, string dbName = "")
        {
            var result = new List<TableDetail>();
            var list = new List<SqliteMode>();
            string sql = @"pragma table_info('" + tableName + "') ;";
            using (SQLiteConnection connection = new SQLiteConnection(conStr))
            {
                list = connection.Query<SqliteMode>(sql).ToList();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        result.Add(new TableDetail
                        {
                            index = item.cid,
                            isPK = item.pk,
                            defaultValue = item.dflt_value == null ? "" : item.dflt_value,
                            FieldType = item.type,
                            Title = item.name,
                            isMark = 0,
                            fieldLenth = 0,
                            isAllowEmpty = 0,
                            fieldDesc = ""
                        });
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 获取视图列表
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public List<ViewModel> GetViewList(string conStr, string dbName = "")
        {
            string sql = "SELECT name viewName,sql viewDerails,sql  from sqlite_master where type='view' and name !='sqlite_sequence'; ";

            using (SQLiteConnection connection = new SQLiteConnection(conStr))
            {

                var list = connection.Query<ViewModel>(sql).ToList();
                return list;
            }
        }
        /// <summary>
        /// SQLite 没有存储过程
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public List<ProcModel> GetProcList(string conStr, string dbName = "")
        {
            return new List<ProcModel>();
        }
        public void BakDataBase(List<string> list, string conStr, string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 针对SQLite数据库用不到获取数据库列表的方法
        /// </summary>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public List<string> GetDBNameList(string conStr)
        {
            return new List<string>();
        }
        /// <summary>
        /// sqlite 建表SQL
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public string GetTableSQL(string tableName, string conStr)
        {
            string result = string.Empty;
            string sql = "SELECT sql from sqlite_master where type='table' and name !='sqlite_sequence' and tbl_name='" + tableName+"' " ;
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(conStr))
                {
                    var list = connection.Query<dynamic>(sql).ToList();
                    var data = (IDictionary<string, object>)list[0];                    
                    result = data["sql"].ToString();
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
