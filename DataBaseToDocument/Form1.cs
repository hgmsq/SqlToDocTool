using CommonService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
namespace DataBaseToDocument
{
    public partial class Form1 : Form
    {
        // LAPTOP-LV16R3PR\SQL2012
        //IBaseService service = new BaseServiceMysql();
        IBaseService service;
        NpoiToDoc docservice = new NpoiToDoc();
        public static string Form1Value; // 注意，必须申明为static变量
<<<<<<< HEAD
        private int docType = 0;       
=======
        //private int docType = 0;       
>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0
        public Form1()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
<<<<<<< HEAD
            {               
                string servername = txtServer.Text.Trim();
                string uid = txtUser.Text.Trim();
                string pwd = txtPwd.Text.Trim();
                string port = txtPort.Text.Trim();
                string constr = service.GetConnectioning(servername, uid, pwd, port);

                if (service.ConnectionTest(constr))
                {
                    MessageBox.Show("连接数据库成功！");
                    comboBox1.DataSource = service.GetDBNameList(constr);
                }
                else
                {
                    MessageBox.Show("连接数据库失败！");
=======
            {


                if (comboBox2.SelectedIndex != 2) // 不是sqlite
                {
                    string servername = txtServer.Text.Trim();
                    string uid = txtUser.Text.Trim();
                    string pwd = txtPwd.Text.Trim();
                    string port = txtPort.Text.Trim();

                    string constr = service.GetConnectioning(servername, uid, pwd, port);

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
                else
                {
                    string constr = service.GetConnectioning(txtSQLitePath.Text, "", "", "");
                    if (service.ConnectionTest(constr))
                    {
                        MessageBox.Show("连接数据库成功！");
                        //comboBox1.DataSource = service.GetDBNameList(constr);
                    }
                    else
                    {
                        MessageBox.Show("连接数据库失败！");
                    }
>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = 0;
<<<<<<< HEAD
            comboDocType.SelectedIndex = 0;           
=======
            comboDocType.SelectedIndex = 0;
>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0

            selVersion.Items.Add("无");
            selVersion.SelectedIndex = 0;
            txtPwd.Text = "123456";
            txtServer.Text = "LAPTOP-LV16R3PR\\SQL2012";
            txtUser.Text = "sa";
            txtPort.Text = "1433";
<<<<<<< HEAD
            CheckAll();           
        }
        private void CheckAll()
        {
            for(int i=0;i< checkedListBox1.Items.Count;i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
        }
        private List<string> GetCheckList()        
        {            
            var list= new List<string>();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    list.Add(checkedListBox1.GetItemText(checkedListBox1.Items[i]));                  
                }
            }         
            return list;
=======
            CheckAll();
>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0
        }
        private void CheckAll()
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
        }
        /// <summary>
        /// 获取选择导出的选项列表
        /// </summary>
        /// <returns></returns>
        private List<string> GetCheckList()
        {
            var list = new List<string>();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    list.Add(checkedListBox1.GetItemText(checkedListBox1.Items[i]));
                }
            }
            return list;
        }

        /// <summary>
        /// 导出文档方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnToDoc_Click(object sender, EventArgs e)
        {
            try
            {
<<<<<<< HEAD
                if (comboBox1.SelectedValue == null)
                {
                    
=======
                if (comboBox1.SelectedValue == null && comboBox2.SelectedIndex != 2)
                {
>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0
                    string servername = txtServer.Text.Trim();
                    string uid = txtUser.Text.Trim();
                    string pwd = txtPwd.Text.Trim();
                    string port = txtPort.Text.Trim();
                    string constr = service.GetConnectioning(servername, uid, pwd, port);
                    if (service.ConnectionTest(constr))
                    {
                        comboBox1.DataSource = service.GetDBNameList(constr);
                    }
                }
                else
                {
                    var checkList = GetCheckList();
                    if (checkList.Count == 0)
                    {
<<<<<<< HEAD
                        MessageBox.Show("请选择要导出的项");
                    }
                    else
                    {
                        string checkStr = checkedListBox1.CheckedItems.ToString();
                        string db = comboBox1.SelectedValue.ToString();
                        string servername = txtServer.Text.Trim();
                        string uid = txtUser.Text.Trim();
                        string pwd = txtPwd.Text.Trim();
                        string port = txtPort.Text.Trim();
                        string constr = service.GetConnectioning(servername, uid, pwd, db, port);
                        var listnew = service.GetTableDetail("UserInfo", constr);
                        var list = service.GetDBTableList(constr, db);
                        int docTypeIndex = comboDocType.SelectedIndex;
                        
=======
                        MessageBox.Show("请选择要导出的文档的项目");
                    }
                    else
                    {
                       
                        string constr = string.Empty;
                        string db = string.Empty;
                        //var listnew = service.GetTableDetail("UserInfo", constr);
                        if (comboBox2.SelectedIndex == 2) 
                        {
                            constr = service.GetConnectioning(txtSQLitePath.Text, "", "", "", "");
                            // 获取文件名
                            var arr = constr.Split('\\').ToArray();
                            db = arr[arr.Length - 1];
                        }
                        else if(comboBox2.SelectedIndex!=2) // 不是sqlite
                        {
                            string checkStr = checkedListBox1.CheckedItems.ToString();
                            db = comboBox1.SelectedValue.ToString();
                            string servername = txtServer.Text.Trim();
                            string uid = txtUser.Text.Trim();
                            string pwd = txtPwd.Text.Trim();
                            string port = txtPort.Text.Trim();
                            constr = service.GetConnectioning(servername, uid, pwd, db, port);
                        }                     

                        var list = service.GetDBTableList(constr, db);
                        int docTypeIndex = comboDocType.SelectedIndex;

>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0
                        if (docTypeIndex == 0)// 生成word
                        {
                            docservice.CreateToWord(list, constr, db, comboBox2.SelectedIndex, checkList);
                        }
<<<<<<< HEAD
                        else if(docTypeIndex==1)// 生成html
=======
                        else if (docTypeIndex == 1)// 生成html
>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0
                        {
                            docservice.CreateToHtml(list, constr, db, comboBox2.SelectedIndex, checkList);
                        }
                        else if (docTypeIndex == 2) // 生成md文件
                        {
                            docservice.CreateToMarkDown(list, constr, db, comboBox2.SelectedIndex, checkList);
                        }
<<<<<<< HEAD
                        else 
                        {
                            docservice.CreateToHtml(list, constr, db, comboBox2.SelectedIndex, checkList);
                        }
                    }                
=======
                        else
                        {
                            docservice.CreateToHtml(list, constr, db, comboBox2.SelectedIndex, checkList);
                        }
                    }
>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        /// <summary>
        /// 导出数据库备份文件 目前只支持SQLServer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {           
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show("请先保证服务器连接成功");
            }
            else
            {
                if (comboBox2.SelectedIndex == 0)
                {
                    string db = comboBox1.SelectedValue.ToString();
                    string servername = txtServer.Text.Trim();
                    string uid = txtUser.Text.Trim();
                    string pwd = txtPwd.Text.Trim();
                    string constr = service.GetConnectioning(servername, uid, pwd, db);
                    Form1Value = constr;
                    //this.Hide();
                    FormToBak fr = new FormToBak();
                    fr.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("导出数据库备份文件只支持本地服务器的SQLServer版本");
                }
            }

        }

<<<<<<< HEAD
        private void Label5_Click(object sender, EventArgs e)
        {

        }

=======

        /// <summary>
        /// 根据数据库类型切换显示的控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0
        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0) //sqlserver
            {
<<<<<<< HEAD
                selVersion.Items.Clear();
                selVersion.Items.Add("无");
                selVersion.SelectedIndex = 0;
                txtPwd.Text = "123456";
                txtServer.Text = "LAPTOP-LV16R3PR\\SQL2012";
                txtUser.Text = "sa";
                txtPort.Enabled = false;
                txtPort.Text = "1433";
                service = new BaseService();
            }
            else //mysql
            {
=======
                txtSQLitePath.Text = "";
                txtPort.ReadOnly = false;
                txtPwd.ReadOnly = false;
                txtUser.ReadOnly = false;
                txtServer.ReadOnly = false;
                comboBox1.Enabled = true;
                txtSQLitePath.ReadOnly = true;
                btnSelect.Visible = false;
                selVersion.Items.Clear();
                selVersion.Items.Add("");
                selVersion.SelectedIndex = 0;
                txtPwd.Text = "123456";
                txtServer.Text = "127.0.0.1";
                txtUser.Text = "sa";
                txtPort.Enabled = true;
                txtPort.Text = "1433";
                selVersion.Enabled = false;
                service = new BaseService();
            }
            else if (comboBox2.SelectedIndex == 1)//mysql
            {
                txtSQLitePath.Text = "";
                selVersion.Enabled = true;
                txtPort.ReadOnly = false;
                txtPwd.ReadOnly = false;
                txtUser.ReadOnly = false;
                txtServer.ReadOnly = false;
                comboBox1.Enabled = true;
                txtSQLitePath.ReadOnly = true;
                btnSelect.Visible = false;
>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0
                selVersion.Items.Clear();
                selVersion.Items.Add("mysql5.7");
                selVersion.Items.Add("mysql8.0");
                selVersion.SelectedIndex = 0;
                txtPwd.Text = "root";
                txtServer.Text = "127.0.0.1";
                txtUser.Text = "root";
                txtPort.Enabled = true;
                txtPort.Text = "3306";
                service = new BaseServiceMysql();
            }
<<<<<<< HEAD
=======
            else if(comboBox2.SelectedIndex == 2)// sqlite
            {
                txtPort.ReadOnly = true;
                txtPwd.ReadOnly = true;
                txtUser.ReadOnly = true;
                txtServer.ReadOnly = true;
                comboBox1.Enabled = false;
                txtSQLitePath.ReadOnly = false;
                btnSelect.Visible = true;
                selVersion.Items.Clear();
                selVersion.Items.Add("");
                selVersion.Enabled = false;
                txtPwd.Text = "";
                txtServer.Text = "";
                txtUser.Text = "";
                txtPort.Text = "";
                service = new BaseServiceSqlite();

            }
            else //PostgreSQL
            {
                txtSQLitePath.Text = "";
                selVersion.Enabled = true;
                txtPort.ReadOnly = false;
                txtPwd.ReadOnly = false;
                txtUser.ReadOnly = false;
                txtServer.ReadOnly = false;
                comboBox1.Enabled = true;
                txtSQLitePath.ReadOnly = true;
                btnSelect.Visible = false;
                selVersion.Items.Clear();               
                //selVersion.SelectedIndex = 0;
                txtPwd.Text = "123456";
                txtServer.Text = "127.0.0.1";
                txtUser.Text = "postgres";
                txtPort.Enabled = true;
                txtPort.Text = "5432";
                service = new BaseServicePgsql();
            }
>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.DataSource = null;
            }
        }
<<<<<<< HEAD

        private void ComboDocType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboDocType.SelectedIndex == 0) //word
            {
                docType = 0;
            }
            else // html
            {
                docType = 1;
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
=======
        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtSQLitePath.Text = openFileDialog1.FileName;
            }
        }
>>>>>>> 6ed2518c9e89cf04b20cc3fb0d71a7f2815be3c0
    }
}
