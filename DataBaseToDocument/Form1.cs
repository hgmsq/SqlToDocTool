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
        // LAPTOP-LV16R3PR\SQL2012
        //IBaseService service = new BaseServiceMysql();
        IBaseService service;
        NpoiToDoc docservice = new NpoiToDoc();
        public static string Form1Value; // 注意，必须申明为static变量
        private int docType = 0;       
        public Form1()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = 0;
            comboDocType.SelectedIndex = 0;           

            selVersion.Items.Add("无");
            selVersion.SelectedIndex = 0;
            txtPwd.Text = "123456";
            txtServer.Text = "LAPTOP-LV16R3PR\\SQL2012";
            txtUser.Text = "sa";
            txtPort.Text = "1433";
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
        }

        private void btnToDoc_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedValue == null)
                {
                    
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
                        
                        if (docTypeIndex == 0)// 生成word
                        {
                            docservice.CreateToWord(list, constr, db, comboBox2.SelectedIndex, checkList);
                        }
                        else if(docTypeIndex==1)// 生成html
                        {
                            docservice.CreateToHtml(list, constr, db, comboBox2.SelectedIndex, checkList);
                        }
                        else if (docTypeIndex == 2) // 生成md文件
                        {
                            docservice.CreateToMarkDown(list, constr, db, comboBox2.SelectedIndex, checkList);
                        }
                        else 
                        {
                            docservice.CreateToHtml(list, constr, db, comboBox2.SelectedIndex, checkList);
                        }
                    }                
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

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

        private void Label5_Click(object sender, EventArgs e)
        {

        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0) //sqlserver
            {
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
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.DataSource = null;
            }
        }

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
    }
}
