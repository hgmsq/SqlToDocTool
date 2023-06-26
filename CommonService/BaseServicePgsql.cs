using CommonService.Models;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CommonService
{
    /// <summary>
    /// Pgsql
    /// </summary>
    public class BaseServicePgsql : IBaseService
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
            return string.Format("host={0};User ID={1};password={2};port={3};pooling=false;", servername, uid, pwd, port);
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
            return string.Format("host={0};User ID={1};password={2};database={3};port={4};pooling=false;", servername, uid, pwd, db, port);
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
                using (NpgsqlConnection connection = new NpgsqlConnection(conStr))
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
            string sql = "SELECT datname FROM pg_database where datname not in ('postgres','template0','template1') ORDER BY datname; ";
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(conStr))
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
            string sql = "SELECT  relname AS tableName,  CAST ( obj_description ( relfilenode, 'pg_class' ) AS VARCHAR ) AS tableDesc FROM pg_class C WHERE  relkind = 'r' AND relname NOT LIKE'pg_%'  AND relname NOT LIKE'sql_%' ORDER BY  relname ; ";

            using (NpgsqlConnection connection = new NpgsqlConnection(conStr))
            {
                var result = new List<TableModel>();
                var list = connection.Query<dynamic>(sql).ToList();
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        var model = new TableModel();
                        var data = (IDictionary<string, object>)item;
                        model.tableDesc = data["tabledesc"].ToString();
                        model.tableName = data["tablename"].ToString();
                        result.Add(model);
                    }

                }
                return result;
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
            string sql = string.Format(@"SELECT proname procName,prosrc proSQl from pg_proc where pronamespace=2200 and prolang=14 and prorettype=1700;  ");
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(conStr))
                {
                    var result = new List<ProcModel>();
                    var list = connection.Query<dynamic>(sql).ToList();
                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            var model = new ProcModel();
                            var data = (IDictionary<string, object>)item;
                            model.proDerails = data["prosql"].ToString();
                            model.procName = data["procname"].ToString();
                            result.Add(model);
                        }
                       
                    }
                    return result;
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
            var result = new List<TableDetail>();
            string sql = @"SELECT col_description ( A.attrelid, A.attnum ) AS fieldDesc,  
            format_type ( A.atttypid, A.atttypmod ) AS FieldType,
            A.attname AS title,case WHEN  A.attnotnull='f' then '1' else '0' end AS isAllowEmpty
            ,case WHEN  A.attidentity='d' then '1' else '0' end AS isPK
            FROM  pg_class AS C,  pg_attribute AS A 
            WHERE  C.relname = '"+tableName+"'   AND A.attrelid = C.oid   AND A.attnum > 0;";
            using (NpgsqlConnection connection = new NpgsqlConnection(conStr))
            {
                var list = connection.Query<dynamic>(sql).ToList();
                if(list!=null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        var model = new TableDetail();
                        var data = (IDictionary<string, object>)item;
                        model.fieldDesc = data["fielddesc"].ToString();
                        model.FieldType = data["fieldtype"].ToString();
                        model.isAllowEmpty = Convert.ToInt32(data["isallowempty"]);
                        model.Title = data["title"].ToString();
                        model.isPK= Convert.ToInt32(data["ispk"]);
                        result.Add(model);
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
        public List<ViewModel> GetViewList(string conStr, string dbName)
        {
            var result = new List<ViewModel>();
            string sql = string.Format(@" SELECT viewname viewName,definition viewDerails from pg_views where schemaname='public'  ");
            try
            {       
                using (NpgsqlConnection connection = new NpgsqlConnection(conStr))
                {
                    var list = connection.Query<dynamic>(sql).ToList();
                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            var model = new ViewModel();
                            var data = (IDictionary<string, object>)item;
                            model.viewName = data["viewname"].ToString();
                            model.viewDerails = data["viewderails"].ToString();                            
                            result.Add(model);
                        }
                    }
                }
            }
            catch
            {

            }
            return result;
        }
        public void BakDataBase(List<string> list, string conStr, string path)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// pgsql 建表SQL
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public string GetTableSQL(string tableName, string conStr)
        {
            string result = string.Empty;
            //string sql = "show create table " + tableName;
            //try
            //{
            //    using (NpgsqlConnection connection = new NpgsqlConnection(conStr))
            //    {
            //        var model = connection.QueryFirst<dynamic>(sql);
            //        var data = (IDictionary<string, object>)model;
            //        result = data["Create Table"].ToString();
            //    }
            //}
            //catch (System.Exception)
            //{
            //    throw;
            //}
            return result;
            
        }
    }
}
