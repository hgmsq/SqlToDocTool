using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonService;
using System.IO;
namespace DataBaseToDocument
{
    public partial class FormToBak : Form
    {
        BaseService service = new BaseService();
        private string constr = "";
        public FormToBak()
        {
            InitializeComponent();
            constr = Form1.Form1Value;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            var list = CheckBox_GetData();
            //string path =@"D:\Bak\";
            string path = Environment.CurrentDirectory.ToString() + "\\Bak\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);//创建新路径
            }
            try
            {
                if (list != null && list.Count > 0)
                {
                    service.BakDataBase(list, constr, path);
                    MessageBox.Show("操作成功");
                }
                else
                {
                    MessageBox.Show("请先勾选数据库");
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
        }

        private void FormToBak_Load(object sender, EventArgs e)
        {
            //var list = service.GetDBList(constr);
            var list = service.GetDBNameList(constr);
            var dblist = new List<DBModel>();
            if(list!=null && list.Count>0)
            {
                foreach(var item in list)
                {
                    dblist.Add(new DBModel
                    {
                        name = item
                    });
                }
            }

            DataGridViewCheckBoxColumn columncb = new DataGridViewCheckBoxColumn();
            columncb.HeaderText = "选择";
            columncb.Name = "cb_check";
            columncb.TrueValue = true;
            columncb.FalseValue = false;
            columncb.DataPropertyName = "IsChecked";
            dataGridView1.Columns.Insert(0, columncb);    //添加的checkbox在第一列
           
            dataGridView1.DataSource = dblist;
            dataGridView1.ReadOnly = false;
            dataGridView1.Columns[1].HeaderCell.Value = "数据库名称";         
        }
        /// <summary>
        /// 获取复选框选中的数据库列表
        /// </summary>
        /// <returns></returns>
        private List<string> CheckBox_GetData()
        {
            var list = new List<string>();
            if (dataGridView1.Rows.Count > 0)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells[0];
                    Boolean flag = Convert.ToBoolean(checkCell.Value);
                    if (flag)
                    {
                        list.Add(dataGridView1.Rows[i].Cells[1].Value.ToString());
                    }
                }
            }
            return list;
        }
       
    }
    public class DBModel
    {
        public string name { get; set; }       

    }
}
