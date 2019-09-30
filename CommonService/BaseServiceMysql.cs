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
    class BaseServiceMysql : IBaseService
    {

        private static bool IsCanConnectioned = false;
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
            catch(Exception ex)
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
        public List<string> GetDBTableList(string conStr)
        {
            string sql = "show TABLES ";
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
        /// 获取存储过程列表
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="dbname"></param>
        /// <returns></returns>
        public List<ProcModel> GetProcList(string conStr,string dbName)
        {
            string sql =string.Format(@"select `name` from mysql.proc where type = 'PROCEDURE' and
            db = '{0}'  ",dbName);
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conStr))
                {
                    var list = connection.Query<ProcModel>(sql).ToList();
                    if(list!=null && list.Count>0)
                    {
                        //遍历获取存储过程明细
                        foreach(var item in list)
                        {
                            item.proDerails = connection.Query<string>("show create procedure "+item+"; ").ToList().FirstOrDefault();
                        }
                    }
                    return list;
                }
            }
            catch
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
        public List<TableDetail> GetTableDetail(string tableName, string conStr,string dbName)
        {
            var list = new List<TableDetail>();        
            string sql = @"select 
            ORDINAL_POSITION 'index',
            COLUMN_NAME title,0 fieldLenth,
            DATA_TYPE FieldType,
            case EXTRA when 'auto_increment' then '1' else '0' end isMark,
            case COLUMN_KEY when 'PRI' then '1' else '0' end isPK,
            case IS_NULLABLE when 'YES' then '1' else '0' end isAllowEmpty,
            COLUMN_DEFAULT defaultValue,COLUMN_COMMENT defaultValue
            from information_schema.columns 
            where table_schema ='"+dbName+"' and table_name = '"+tableName+"' ;";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conStr))
                {
                    list = connection.Query<TableDetail>(sql).ToList();
                }
            }
            catch
            { }

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
            string sql =string.Format(@" select TABLE_NAME viewName,VIEW_DEFINITION viewDerails from  information_schema.views 
            where TABLE_SCHEMA='{0}'  ",dbName);
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
    }
}
