using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonService.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CommonService
{
    public class BaseServiceMysql : IBaseService
    {

        private static bool IsCanConnectioned = false;

        /// <summary>
        /// 返回连接字符串
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public string GetConnectioning(string servername, string uid, string pwd, string port)
        {
<<<<<<< HEAD
            return string.Format("data source={0};user id={1};password={2};port={3};pooling=false;charset=utf8", servername, uid, pwd, port);
=======
            return string.Format("data source={0};user id={1};password={2};port={3};pooling=false;charset=utf8;", servername, uid, pwd, port);
>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0
        }

        /// <summary>
        /// 返回连接字符串
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public string GetConnectioning(string servername, string uid, string pwd, string db, string port)
        {
<<<<<<< HEAD
            return string.Format("data source={0};user id={1};password={2};database={3};port={4};pooling=false;charset=utf8", servername, uid, pwd, db, port);
=======
            return string.Format("data source={0};user id={1};password={2};database={3};port={4};pooling=false;charset=utf8;", servername, uid, pwd, db, port);
>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0
        }
        /// <summary>
        /// 判断数据库服务器是否连接成功
        /// </summary>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public bool ConnectionTest(string conStr)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conStr))
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
        /// 获取当前数据库服务器对应的数据库列表
        /// </summary>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public List<string> GetDBNameList(string conStr)
        {
            string sql = "show databases ";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conStr))
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
        /// <summary>
        /// 获取当前数据库所有的数据表
        /// </summary>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public List<TableModel> GetDBTableList(string conStr, string db)
        {
            string sql = "select table_name tableName,table_comment tableDesc from information_schema.tables where table_schema='" + db + "' and table_type='BASE TABLE'; ";

            using (MySqlConnection connection = new MySqlConnection(conStr))
            {
                var list = connection.Query<TableModel>(sql).ToList();
                return list;
            }

        }
        /// <summary>
        /// 获取存储过程列表
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="dbname"></param>
        /// <returns></returns>
        public List<ProcModel> GetProcList(string conStr, string dbName)
        {
            string sql = string.Format(@"select `name` procName from mysql.proc where type = 'PROCEDURE' and
            db = '{0}'  ", dbName);
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conStr))
                {
                    var list = connection.Query<ProcModel>(sql).ToList();
                    if (list != null && list.Count > 0)
                    {
                        //遍历获取存储过程明细
                        foreach (var item in list)
                        {
<<<<<<< HEAD
                            //item.proDerails = connection.Query<string>("show create procedure "+item.procName+"; ").ToList().FirstOrDefault();
=======
                            var str = connection.Query<dynamic>("show create procedure " + item.procName + "; ").ToList();
                            var data = (IDictionary<string, object>)str[0];                         
                            item.proDerails = data["Create Procedure"].ToString(); 
>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0
                        }
                    }
                    return list;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 根据表名获取表字段信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public List<TableDetail> GetTableDetail(string tableName, string conStr, string dbName)
        {
            var list = new List<TableDetail>();
            string sql = @"select 
            ORDINAL_POSITION 'index',
            COLUMN_NAME title,0 fieldLenth,
            DATA_TYPE FieldType,
            case EXTRA when 'auto_increment' then '1' else '0' end isMark,
            case COLUMN_KEY when 'PRI' then '1' else '0' end isPK,
            case IS_NULLABLE when 'YES' then '1' else '0' end isAllowEmpty,
            ifnull(COLUMN_DEFAULT,'') defaultValue,COLUMN_COMMENT fieldDesc
            from information_schema.columns 
            where table_schema ='" + dbName + "' and table_name = '" + tableName + "' order by ORDINAL_POSITION;";
<<<<<<< HEAD
             using (MySqlConnection connection = new MySqlConnection(conStr))
             {
                list = connection.Query<TableDetail>(sql).ToList();
             }        
=======
            using (MySqlConnection connection = new MySqlConnection(conStr))
            {
                list = connection.Query<TableDetail>(sql).ToList();
            }
>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0
            return list;
        }
        /// <summary>
        /// 获取视图列表
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public List<ViewModel> GetViewList(string conStr, string dbName)
        {
            var list = new List<ViewModel>();
            string sql = string.Format(@" select TABLE_NAME viewName,VIEW_DEFINITION viewDerails from  information_schema.views 
            where TABLE_SCHEMA='{0}'  ", dbName);
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conStr))
                {
                    list = connection.Query<ViewModel>(sql).ToList();
                }
            }
            catch
            {

            }
            return list;
        }
        public void BakDataBase(List<string> list, string conStr, string path)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// mysql 建表SQL
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public string GetTableSQL(string tableName, string conStr)
        {
            string result = string.Empty;
            string sql = "show create table " + tableName;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conStr))
                {
                    var model = connection.QueryFirst<dynamic>(sql);
                    var data = (IDictionary<string, object>)model;
                    result = data["Create Table"].ToString();
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
