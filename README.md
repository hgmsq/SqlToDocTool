

## 一、软件介绍 

今天给大家分享我自己编写的数据库表结构文档生成工具，方便大家在实际开发当中，可以很方便导出业务系统的表结构，也可以作为项目验收文档中数据库设计文档使用。这样可以大大减少编写数据库表结构文档的时间，有需要的朋友欢迎下载或者沟通交流！

## 二、技术框架 

 *  编程语言：C\# （ Net Framework4.5.5）
 *  数据库技术框架：Dapper
 *  导出Word文档：NPOI
 *  访问方式：WinForm窗体应用程序，Windows操作系统可以直接运行

## 三、功能介绍 

 *  支持SQLServer、MySQL（5.7、8.0）、SQLite 三种类型的数据，持续更新
 *  支持Word、Html、MarkDown 三种格式的导出
 *  导出内容包含数据表（字段详情、字段注释、长度、默认值等）、创建表脚本、视图及视图脚本、存储过程及脚本
 *  支持生成文档的同时直接打开文档
 *  支持数据库备份（目前只支持SQLServer导出bak备份文件）

## 四、代码展示 

### 1、获取数据库信息部分代码 

```java
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
```

### 2、导出Html文档代码 

```java
/// <summary>
        /// 生成html文件
        /// </summary>
        /// <param name="list"></param>
        /// <param name="conStr"></param>
        /// <param name="db"></param>
        /// <param name="type"></param>
        public void CreateToHtml(List<TableModel> list, string conStr, string db, int type, List<string> checkList)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><meta charset=\"utf-8\" /><meta http-equiv = \"Content-Language\" content = \"zh-CN\" >");
            sb.Append("<head><title>数据库说明文档</title><body>");
            sb.Append("<style type=\"text/css\">\n");
            sb.Append("body { font-size: 9pt;}\n");
            sb.Append(".styledb { font-size: 14px; }\n");
            sb.Append(".styletab {font-size: 14px;padding-top: 15px; }\n</style></head><body>");
            sb.Append("<h1 style=\"text-align:center;\">" + db + "数据库说明文档</h1>");


            GetDBService(type);

            #region 创建一个表格
            if (checkList.Where(m => m.Equals("表")).Count() > 0)
            {
                sb.Append("<h2>一、表结构</h2>");
                sb.Append("");
                sb.Append("");
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (item.tableDesc != null && item.tableDesc != "")
                        {
                            sb.Append("<h3>表名:" + item.tableName + "(" + item.tableDesc + ")</h3>");
                        }
                        else
                        {
                            sb.Append("<h3>表名:" + item.tableName + "</h3>");
                        }
                        sb.Append(" <table cellspacing=\"0\" cellpadding=\"5\" border=\"1\" width=\"100%\" bordercolorlight=\"#4F7FC9\" bordercolordark=\"#D3D8E0\">");
                        sb.Append("<thead bgcolor=\"#E3EFFF\"> <th>序号</th><th>字段名称</th><th>标识</th><th>主键</th><th>字段类型</th><th>字段长度</th><th>允许空值</th><th>字段默认值</th><th>字段备注</th></thead>");
                        sb.Append("<tbody>");
                        //从第二行开始 因为第一行是表头
                        int i = 1;
                        var tabledetaillist = service.GetTableDetail(item.tableName, conStr, db);


                        if (tabledetaillist != null && tabledetaillist.Count > 0)
                        {
                            foreach (var itm in tabledetaillist)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td>" + itm.index + "</td>");
                                sb.Append("<td>" + itm.Title + "</td>");
                                sb.Append("<td>" + itm.isMark + "</td>");
                                sb.Append("<td>" + itm.isPK + "</td>");
                                sb.Append("<td>" + itm.FieldType + "</td>");
                                sb.Append("<td>" + itm.fieldLenth + "</td>");
                                sb.Append("<td>" + itm.isAllowEmpty + "</td>");
                                sb.Append("<td>" + itm.defaultValue + "</td>");
                                sb.Append("<td>" + itm.fieldDesc + "</td>");
                                sb.Append("</tr>");
                                i++;
                            }
                        }
                        sb.Append("</tbody></table>");

                        sb.Append("<h4>" + item.tableName + "建表脚本</h4><br/>");
                        sb.Append("<span>" + service.GetTableSQL(item.tableName, conStr) + "</span>");


                    }
                }
            }
            #endregion

            #region 存储过程
            if (checkList.Where(m => m.Equals("存储过程")).Count() > 0)
            {
                List<ProcModel> proclist = new List<ProcModel>();
                proclist = service.GetProcList(conStr, db);
                sb.Append("<h2>二、存储过程</h2>");
                if (proclist != null && proclist.Count > 0)
                {
                    foreach (var item in proclist)
                    {
                        sb.Append("<h3>存储过程名称：" + item.procName + "</h3>");
                        sb.Append("<span>" + item.proDerails + "</span>");
                    }
                }
            }
            #endregion

            #region 视图
            if (checkList.Where(m => m.Equals("视图")).Count() > 0)
            {
                List<ViewModel> viewlist = new List<ViewModel>();
                viewlist = service.GetViewList(conStr, db);
                sb.Append("<h2>三、视图</h2>");
                if (viewlist.Count > 0)
                {

                    foreach (var item in viewlist)
                    {
                        sb.Append("<h3>视图名称：" + item.viewName + "</h3>");
                        sb.Append("<span>" + item.viewDerails + "</span>");
                    }
                }
            }
            #endregion

            sb.Append("</body></html>");
            sb.ToString();
            string filename = db + "-数据库说明文档";//文件名
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "html";
            saveDialog.Filter = "html文件|*.html";
            saveDialog.FileName = filename;
            saveDialog.ShowDialog();
            filename = saveDialog.FileName;
            if (filename.IndexOf(":") < 0) return; //被点了取消         
            StreamWriter sw1 = new StreamWriter(saveDialog.FileName, false);
            sw1.WriteLine(sb);
            sw1.Close();
            System.Diagnostics.Process.Start(filename);

        }
```

## 五、运行效果 

应用程序主界面

![](https://img-blog.csdnimg.cn/img_convert/89541a77ec170e554b86e063642347e0.png)

支持三种生成文档类型：每次只能选择一种，推荐使用markdown格式

![](https://img-blog.csdnimg.cn/img_convert/02292df7f0e356a7a93a7c5a2f7ad22a.png)

Word文档生成效果

![](https://img-blog.csdnimg.cn/img_convert/aa6490b95dd6dc28811df5b61296d1f2.png)

Html文档生成效果

![](https://img-blog.csdnimg.cn/img_convert/4f35dadb8a53c93231fc33ce53ed95f7.png)

MarkDown文档效果

![](https://img-blog.csdnimg.cn/img_convert/e46cee6009df27b253512ef8c4856246.png)

针对SQLServer数据库备份

![](https://img-blog.csdnimg.cn/img_convert/34d4e2cee49deadc6c96747bbb1e53eb.png)

## 六、项目开源地址 

GitHub：https://github.com/hgmsq/SqlToDocTool

Gitee：https://gitee.com/hgm1989/SqlToDocTool

Gitcode：https://gitcode.net/xishining/SqlToDocTool




### CSDN：https://blog.csdn.net/xishining

### 微信:hgmyzhl

### 公众号：IT技术分享社区
