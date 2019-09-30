using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonService.Models
{
    /// <summary>
    /// 数据表详情
    /// </summary>
    public class TableDetail
    {
        //字段序号 字段名 标识 主键  类型   长度 允许空 默认值 字段说明
        /// <summary>
        /// 序号
        /// </summary>
        public int index { get; set; }
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 标识 0 不是， 1 是
        /// </summary>
        public int isMark { get; set; }

        /// <summary>
        /// 是否是主键 0 不是， 1 是
        /// </summary>
        public int isPK { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        public string FieldType { get; set; }
        /// <summary>
        /// 字段长度
        /// </summary>
        public int fieldLenth { get; set; }

        /// <summary>
        /// 允许空 0 不， 1 是
        /// </summary>
        public int isAllowEmpty { get; set; }
        /// <summary>
        /// 字段默认值
        /// </summary>
        public string defaultValue { get; set; }
        /// <summary>
        /// 字段说明
        /// </summary>
        public string fieldDesc { get; set; }
    }
}
