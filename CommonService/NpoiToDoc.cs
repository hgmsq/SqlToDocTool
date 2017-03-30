using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NPOI.XWPF.UserModel;
namespace CommonService
{
    public class NpoiToDoc
    {
        BaseService service = new BaseService();
        /// <summary>
        /// 生成word文档
        /// </summary>
        /// <param name="list">数据库数据表的列表</param>
        public void CreateToWord(List<string> list,string conStr)
        {
            XWPFDocument doc = new XWPFDocument();      //创建新的word文档

            XWPFParagraph p1 = doc.CreateParagraph();   //向新文档中添加段落
         
            p1.Alignment = ParagraphAlignment.CENTER;
            XWPFRun r1 = p1.CreateRun();                //向该段落中添加文字
            r1.SetText("测试段落一");

            XWPFParagraph p2 = doc.CreateParagraph();  
            XWPFRun r2 = p2.CreateRun();
            r2.SetText("测试段落二");
            
            #region 创建一个表格
            if (list.Count > 0)
            {


                

  
                foreach (var item in list)
                {

                    XWPFParagraph p3 = doc.CreateParagraph();   //向新文档中添加段落
                    p3.Alignment = ParagraphAlignment.LEFT;
                    XWPFRun r3 = p3.CreateRun();                //向该段落中添加文字
                    r3.SetText("表名:"+item);

                    //从第二行开始 因为第一行是表头
                    int i = 1;
                    var tabledetaillist = service.GetTableDetail(item, conStr);
                    XWPFTable table = doc.CreateTable(tabledetaillist.Count + 1, 9);
                    table.Width = 5000;

                    #region 设置表头               

                    //table.GetRow(0).GetCell(0).SetText("数据库名称");
                    XWPFParagraph pI = table.GetRow(0).GetCell(0).AddParagraph();
                    XWPFRun rI = pI.CreateRun();
                    rI.FontFamily = "微软雅黑";
                    rI.FontSize = 12;
                    rI.IsBold = true;
                    rI.SetText("序号");

                
                    XWPFParagraph pI1 = table.GetRow(0).GetCell(1).AddParagraph();
                    XWPFRun rI1 = pI1.CreateRun();
                    rI1.FontFamily = "微软雅黑";
                    rI1.FontSize = 12;
                    rI1.IsBold = true;
                    rI1.SetText("字段名称");

                    XWPFParagraph pI2 = table.GetRow(0).GetCell(2).AddParagraph();
                    XWPFRun rI2 = pI2.CreateRun();
                    rI2.FontFamily = "微软雅黑";
                    rI2.FontSize = 12;
                    rI2.IsBold = true;
                    rI2.SetText("标识");

                    XWPFParagraph pI3 = table.GetRow(0).GetCell(3).AddParagraph();
                    XWPFRun rI3 = pI3.CreateRun();
                    rI3.FontFamily = "微软雅黑";
                    rI3.FontSize = 12;
                    rI3.IsBold = true;
                    rI3.SetText("主键");

                    XWPFParagraph pI4 = table.GetRow(0).GetCell(4).AddParagraph();
                    XWPFRun rI4 = pI4.CreateRun();
                    rI4.FontFamily = "微软雅黑";
                    rI4.FontSize = 12;
                    rI4.IsBold = true;
                    rI4.SetText("字段类型");

                    XWPFParagraph pI5 = table.GetRow(0).GetCell(5).AddParagraph();
                    XWPFRun rI5 = pI5.CreateRun();
                    rI5.FontFamily = "微软雅黑";
                    rI5.FontSize = 12;
                    rI5.IsBold = true;
                    rI5.SetText("字段长度");

                    XWPFParagraph pI6 = table.GetRow(0).GetCell(6).AddParagraph();
                    XWPFRun rI6 = pI6.CreateRun();
                    rI6.FontFamily = "微软雅黑";
                    rI6.FontSize = 12;
                    rI6.IsBold = true;
                    rI6.SetText("允许空");


                    XWPFParagraph pI7 = table.GetRow(0).GetCell(7).AddParagraph();
                    XWPFRun rI7 = pI7.CreateRun();
                    rI7.FontFamily = "微软雅黑";
                    rI7.FontSize = 12;
                    rI7.IsBold = true;
                    rI7.SetText("字段默认值");

                    XWPFParagraph pI8 = table.GetRow(0).GetCell(8).AddParagraph();
                    XWPFRun rI8 = pI8.CreateRun();
                    rI8.FontFamily = "微软雅黑";
                    rI8.FontSize = 12;
                    rI8.IsBold = true;
                    rI8.SetText("字段说明");

                    #endregion

                    
                    if (tabledetaillist != null && tabledetaillist.Count > 0)
                    {
                        foreach (var itm in tabledetaillist)
                        {
                            //第一列
                            XWPFParagraph pIO = table.GetRow(i).GetCell(0).AddParagraph();
                            XWPFRun rIO = pIO.CreateRun();
                            rIO.FontFamily = "微软雅黑";
                            rIO.FontSize = 12;
                            rIO.IsBold = true;
                            rIO.SetText(itm.index.ToString());

                            //第二列
                            XWPFParagraph pIO2 = table.GetRow(i).GetCell(1).AddParagraph();
                            XWPFRun rIO2 = pIO2.CreateRun();
                            rIO2.FontFamily = "微软雅黑";
                            rIO2.FontSize = 12;
                            rIO2.IsBold = true;
                            rIO2.SetText(itm.Title);


                            XWPFParagraph pIO3 = table.GetRow(i).GetCell(2).AddParagraph();
                            XWPFRun rIO3 = pIO3.CreateRun();
                            rIO3.FontFamily = "微软雅黑";
                            rIO3.FontSize = 12;
                            rIO3.IsBold = true;
                            rIO3.SetText(itm.isMark.ToString());

                            XWPFParagraph pIO4 = table.GetRow(i).GetCell(3).AddParagraph();
                            XWPFRun rIO4 = pIO4.CreateRun();
                            rIO4.FontFamily = "微软雅黑";
                            rIO4.FontSize = 12;
                            rIO4.IsBold = true;
                            rIO4.SetText(itm.isPK.ToString());

                            XWPFParagraph pIO5 = table.GetRow(i).GetCell(4).AddParagraph();
                            XWPFRun rIO5 = pIO5.CreateRun();
                            rIO5.FontFamily = "微软雅黑";
                            rIO5.FontSize = 12;
                            rIO5.IsBold = true;
                            rIO5.SetText(itm.FieldType);

                            XWPFParagraph pIO6 = table.GetRow(i).GetCell(5).AddParagraph();
                            XWPFRun rIO6 = pIO6.CreateRun();
                            rIO6.FontFamily = "微软雅黑";
                            rIO6.FontSize = 12;
                            rIO6.IsBold = true;
                            rIO6.SetText(itm.fieldLenth.ToString());

                            XWPFParagraph pIO7 = table.GetRow(i).GetCell(6).AddParagraph();
                            XWPFRun rIO7 = pIO7.CreateRun();
                            rIO7.FontFamily = "微软雅黑";
                            rIO7.FontSize = 12;
                            rIO7.IsBold = true;
                            rIO7.SetText(itm.isAllowEmpty.ToString());

                            XWPFParagraph pIO8 = table.GetRow(i).GetCell(7).AddParagraph();
                            XWPFRun rIO8 = pIO8.CreateRun();
                            rIO8.FontFamily = "微软雅黑";
                            rIO8.FontSize = 12;
                            rIO8.IsBold = true;
                            rIO8.SetText(itm.defaultValue.ToString());

                            XWPFParagraph pIO9 = table.GetRow(i).GetCell(8).AddParagraph();
                            XWPFRun rIO9 = pIO9.CreateRun();
                            rIO9.FontFamily = "微软雅黑";
                            rIO9.FontSize = 12;
                            rIO9.IsBold = true;
                            rIO9.SetText(itm.fieldDesc);

                            i++;
                        }
                    }
                    
                }
           


        

            }

            #endregion


            FileStream sw = File.Create("../../Doc/db.docx"); //...
            doc.Write(sw);                              //...
            sw.Close();                                 //在服务端生成文件

            FileInfo file = new FileInfo("../../Doc/db.docx");//文件保存路径及名称  
      
        }

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
            public int isAllowEmpty{ get; set; }
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
}
