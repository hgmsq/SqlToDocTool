using CommonService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonService
{
    public interface IBaseService
    {
        /// <summary>
        /// 获取数据库连接字符串不指定数据库
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        string GetConnectioning(string servername, string uid, string pwd, string port);
        /// <summary>
        /// 获取数据库连接字符串 指定数据库
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        string GetConnectioning(string servername, string uid, string pwd,string db, string port);
        /// <summary>
        /// 服务器连接是否成功
        /// </summary>
        /// <param name="conStr"></param>
        /// <returns></returns>
        bool ConnectionTest(string conStr);
        /// <summary>
        /// 获取数据库列表
        /// </summary>
        /// <param name="conStr"></param>
        /// <returns></returns>
        List<string> GetDBNameList(string conStr);
        /// <summary>
        /// 获取特定数据库的表名列表
        /// </summary>
        /// <param name="conStr"></param>
        /// <returns></returns>
        List<TableModel> GetDBTableList(string conStr, string dbName = "");
        /// <summary>
        /// 获取特定数据库里面的存储过程
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        List<ProcModel> GetProcList(string conStr,string dbName="");
        /// <summary>
        /// 获取特定数据库里面的视图
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        List<ViewModel> GetViewList(string conStr, string dbName = "");
        /// <summary>
        /// 获取特定数据表字段的信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        List<TableDetail> GetTableDetail(string tableName, string conStr, string dbName = "");
        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="list"></param>
        /// <param name="conStr"></param>
        /// <param name="path"></param>
        void BakDataBase(List<string> list, string conStr, string path);

    }
}
