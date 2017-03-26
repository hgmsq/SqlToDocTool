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
        /// <summary>
        /// 生成word文档
        /// </summary>
        public void CreateToWord(List<string> list)
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
                XWPFTable table = doc.CreateTable(list.Count+1, 2);
                table.Width = 800;

                #region 设置表头               

                //table.GetRow(0).GetCell(0).SetText("数据库名称");
                XWPFParagraph pI = table.GetRow(0).GetCell(0).AddParagraph();
                XWPFRun rI = pI.CreateRun();
                rI.FontFamily = "微软雅黑";
                rI.FontSize = 12;
                rI.IsBold = true;
                rI.SetText("数据库名称");

                //table.GetRow(0).GetCell(1).SetText("属性");
                XWPFParagraph pI1 = table.GetRow(0).GetCell(1).AddParagraph();
                XWPFRun rI1 = pI1.CreateRun();
                rI1.FontFamily = "微软雅黑";
                rI1.FontSize = 12;
                rI1.IsBold = true;
                rI1.SetText("属性");

                #endregion

                //从第二行开始 因为第一行是表头
                int i = 1;
                foreach (var item in list)
                {                   
                    //第一列
                    XWPFParagraph pIO = table.GetRow(i).GetCell(0).AddParagraph();
                    XWPFRun rIO = pIO.CreateRun();
                    rIO.FontFamily = "微软雅黑";
                    rIO.FontSize = 12;
                    rIO.IsBold = true;
                    rIO.SetText(item);

                    //第二列
                    XWPFParagraph pIO2 = table.GetRow(i).GetCell(1).AddParagraph();
                    XWPFRun rIO2 = pIO2.CreateRun();
                    rIO2.FontFamily = "微软雅黑";
                    rIO2.FontSize = 12;
                    rIO2.IsBold = true;
                    rIO2.SetText(item+"2222");

                    i++;
                }
           


            //XWPFParagraph pIO = table.GetRow(0).GetCell(0).AddParagraph();
            //XWPFRun rIO = pIO.CreateRun();        
            //rIO.FontFamily="微软雅黑";
            //rIO.FontSize = 12;
            //rIO.IsBold = true;    
            //rIO.SetText("第一行1");


            //XWPFParagraph pIO01 = table.GetRow(0).GetCell(1).AddParagraph();
            //XWPFRun rIO01 = pIO01.CreateRun();
            //rIO01.FontFamily = "微软雅黑";
            //rIO01.FontSize = 12;
            //rIO01.IsBold = true;
            //rIO01.SetText("第一行2");


            //XWPFParagraph pIO1 = table.GetRow(1).GetCell(0).AddParagraph();
            //XWPFRun rIO1 = pIO1.CreateRun();
            //rIO1.FontFamily = "微软雅黑";
            //rIO1.FontSize = 12;
            //rIO1.IsBold = true;
            //rIO1.SetText("第二行1");

            //XWPFParagraph pIO12 = table.GetRow(1).GetCell(1).AddParagraph();
            //XWPFRun rIO12 = pIO12.CreateRun();
            //rIO12.FontFamily = "微软雅黑";
            //rIO12.FontSize = 12;
            //rIO12.IsBold = true;
            //rIO12.SetText("第二行2");

            }

            #endregion


            FileStream sw = File.Create("../../Doc/db.docx"); //...
            doc.Write(sw);                              //...
            sw.Close();                                 //在服务端生成文件

            FileInfo file = new FileInfo("../../Doc/db.docx");//文件保存路径及名称  
      


            //File.Delete("cutput.docx");                 //清除服务端生成的word文件
        }
    }
}
