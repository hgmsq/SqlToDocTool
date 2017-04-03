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
namespace DataBaseToDocument
{
    public partial class Form1 : Form
    {
        BaseService service = new BaseService();
        NpoiToDoc docservice = new NpoiToDoc();
        public Form1()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            string servername = txtServer.Text.Trim();
            string uid = txtUser.Text.Trim();
            string pwd = txtPwd.Text.Trim();
            string constr =service.GetConnectioning(servername,uid,pwd);
            if (service.ConnectionTest(constr))
            {
                MessageBox.Show("连接数据库成功！");
                comboBox1.DataSource = service.GetDBNameList(constr);         
            }
            else
            {
                MessageBox.Show("连接数据库失败！");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnToDoc_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue==null)
            {
                MessageBox.Show("请选择数据库");
            }
            else
            {
                string db = comboBox1.SelectedValue.ToString();
                string servername = txtServer.Text.Trim();
                string uid = txtUser.Text.Trim();
                string pwd = txtPwd.Text.Trim();
                string constr = service.GetConnectioning(servername, uid, pwd,db);
                var listnew = service.GetTableDetail("UserInfo", constr);
                var list = service.GetDBTableList(constr);
                
                docservice.CreateToWord(list,constr, db);
                MessageBox.Show("生成成功");
            }

        }
    }
}
