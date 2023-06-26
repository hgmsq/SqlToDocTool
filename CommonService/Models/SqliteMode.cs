using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonService.Models
{
    public class SqliteMode
    {
        /// <summary>
        /// 列索引值
        /// </summary>
        public int cid { get; set; }
        /// <summary>
        /// 字段名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 数据库字段类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 是否不能为null 1 是 0 否
        /// </summary>
        public int notnull { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string dflt_value { get; set; }
        /// <summary>
        /// 是否为主键 1 是主键，0 否则
        /// </summary>
        public int pk { get; set; }
    }
}
