using CommonService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
                                         //private int docType = 0;   

        // 新增：进度条控件
        private BackgroundWorker backgroundWorker1;
        private ProgressBar progressBar1;
        private Label lblStatus;

        public Form1()
        {
            InitializeComponent();
            InitializeProgressComponents();
        }

        /// <summary>
        /// 初始化进度条相关控件
        /// </summary>
        private void InitializeProgressComponents()
        {
            // 创建进度条
            progressBar1 = new ProgressBar
            {
                Location = new System.Drawing.Point(20, 400),
                Size = new System.Drawing.Size(600, 30),
                Visible = false,
                Style = ProgressBarStyle.Marquee
            };

            // 创建状态标签
            lblStatus = new Label
            {
                Location = new System.Drawing.Point(20, 435),
                Size = new System.Drawing.Size(600, 20),
                Text = "",
                Visible = false
            };

            // 创建后台工作组件
            backgroundWorker1 = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            // 绑定后台工作事件        

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;

            // 将控件添加到窗体
            this.Controls.Add(progressBar1);
            this.Controls.Add(lblStatus);
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
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
        /// 导出文档方法 - 修改为异步方式
        /// </summary>
        private void btnToDoc_Click(object sender, EventArgs e)
        {
            try
            {

                if (comboBox1.SelectedValue == null && comboBox2.SelectedIndex != 2)
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
                        MessageBox.Show("请选择要导出的文档的项目");
                        return;
                    }

                    // 检查是否已经有后台任务在运行
                    if (backgroundWorker1.IsBusy)
                    {
                        MessageBox.Show("正在导出中，请稍候...");
                        return;
                    }

                    // 准备导出参数
                    ExportParameters exportParams = new ExportParameters
                    {
                        CheckList = checkList,
                        DbTypeIndex = comboBox2.SelectedIndex,
                        DocTypeIndex = comboDocType.SelectedIndex
                    };

                    // 获取连接字符串和数据库名称
                    if (comboBox2.SelectedIndex == 2) // SQLite
                    {
                        exportParams.Constr = service.GetConnectioning(txtSQLitePath.Text, "", "", "", "");
                        var arr = exportParams.Constr.Split('\\').ToArray();
                        exportParams.Db = arr[arr.Length - 1];
                    }
                    else if (comboBox2.SelectedIndex != 2) // 不是sqlite
                    {
                        exportParams.Db = comboBox1.SelectedValue?.ToString();
                        if (string.IsNullOrEmpty(exportParams.Db))
                        {
                            MessageBox.Show("请先选择数据库");
                            return;
                        }
                        string servername = txtServer.Text.Trim();
                        string uid = txtUser.Text.Trim();
                        string pwd = txtPwd.Text.Trim();
                        string port = txtPort.Text.Trim();
                        exportParams.Constr = service.GetConnectioning(servername, uid, pwd, exportParams.Db, port);
                    }

                    // 禁用UI控件
                    SetUIEnabled(false);

                    // 显示进度条
                    progressBar1.Visible = true;
                    lblStatus.Visible = true;
                    lblStatus.Text = "准备导出数据...";
                    progressBar1.Style = ProgressBarStyle.Marquee;

                    // 启动后台工作
                    backgroundWorker1.RunWorkerAsync(exportParams);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SetUIEnabled(true);
            }
        }

        /// <summary>
        /// 后台工作线程执行导出操作
        /// </summary>
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ExportParameters exportParams = e.Argument as ExportParameters;
            if (exportParams == null)
            {
                throw new ArgumentException("导出参数无效");
            }

            try
            {
                // 更新状态
                backgroundWorker1.ReportProgress(0, "正在获取数据库表列表...");

                // 获取表列表
                var list = service.GetDBTableList(exportParams.Constr, exportParams.Db);

                // 更新进度
                backgroundWorker1.ReportProgress(30, $"已获取 {list.Count} 个表，正在生成文档...");

                // 根据文档类型生成文档
                if (exportParams.DocTypeIndex == 0) // 生成word
                {
                    backgroundWorker1.ReportProgress(50, "正在生成Word文档...");
                    byte[] docBytes = docservice.CreateToWord(list, exportParams.Constr, exportParams.Db, exportParams.DbTypeIndex, exportParams.CheckList);
                    // 封装结果（含默认文件名）
                    e.Result = new ExportResult
                    {
                        DocumentBytes = docBytes,
                        Ext="doc",
                        DefaultFileName = $"{exportParams.Db}-数据库说明文档.doc",
                    };
                    //docservice.CreateToWord(list, exportParams.Constr, exportParams.Db, exportParams.DbTypeIndex, exportParams.CheckList);
                }               
                else if (exportParams.DocTypeIndex == 2) // 生成md文件
                {
                    backgroundWorker1.ReportProgress(50, "正在生成Markdown文档...");                  
                    byte[] docBytes = docservice.CreateToMarkDown(list, exportParams.Constr, exportParams.Db, exportParams.DbTypeIndex, exportParams.CheckList);
                    // 封装结果（含默认文件名）
                    e.Result = new ExportResult
                    {
                        DocumentBytes = docBytes,
                        Ext = "md",
                        DefaultFileName = $"{exportParams.Db}-数据库说明文档.md",
                    };
                }
                else
                {
                    backgroundWorker1.ReportProgress(50, "正在生成HTML文档...");
                    byte[] docBytes = docservice.CreateToHtml(list, exportParams.Constr, exportParams.Db, exportParams.DbTypeIndex, exportParams.CheckList);
                    // 封装结果（含默认文件名）
                    e.Result = new ExportResult
                    {
                        DocumentBytes = docBytes,
                        Ext="html",
                        DefaultFileName = $"{exportParams.Db}-数据库说明文档.html",
                    };
                }

                // 完成
                backgroundWorker1.ReportProgress(100, "文档导出完成！");

            }
            catch (Exception ex)
            {
                // 异常处理
                e.Result = $"导出失败: {ex.Message}";
                throw;
            }
        }

        /// <summary>
        /// 进度更新事件
        /// </summary>
        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 更新进度条
            if (progressBar1.Style == ProgressBarStyle.Blocks)
            {
                progressBar1.Value = e.ProgressPercentage;
            }

            // 更新状态文本
            if (e.UserState != null)
            {
                lblStatus.Text = e.UserState.ToString();
            }

            // 强制刷新界面
            Application.DoEvents();
        }

        // 完成后在 UI 线程处理保存（STA 安全）
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                // 优先处理 BackgroundWorker 自身异常
                if (e.Error != null)
                {
                    MessageBox.Show($"导出过程发生系统错误: {e.Error.Message}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (e.Result is ExportResult result)
                {
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show(result.ErrorMessage, "导出失败",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }


                    // ✅ 所有 UI/COM 操作在此执行（STA 线程）
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.DefaultExt = result.Ext;
                        if (result.Ext == "doc")
                        {
                            sfd.Filter = "Word文档 (*.doc)|*.doc|所有文件 (*.*)|*.*";
                        }
                        else if (result.Ext == "html")
                        {
                            sfd.Filter = "html文件 | *.html";
                        }
                        else if (result.Ext == "md")
                        {
                            sfd.Filter = "md文件|*.md";
                        }
                        sfd.FileName = result.DefaultFileName;
                        sfd.Title = "保存数据库说明文档";

                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                File.WriteAllBytes(sfd.FileName, result.DocumentBytes);

                                // 安全打开文件（处理路径空格/特殊字符）
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = sfd.FileName,
                                    UseShellExecute = true
                                });

                                MessageBox.Show("文档已成功保存并打开！", "操作成功",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"保存或打开文件时出错:\n{ex.Message}", "操作失败",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("未获取到有效的导出结果", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                // ✅ 关键：无论成功/失败/取消，统一清理UI状态（STA线程安全）
                ResetExportUI();

            }
        }

        /// <summary>
        /// 重置导出相关UI状态，准备下一次操作
        /// </summary>
        private void ResetExportUI()
        {
            // 隐藏并重置进度条
            if (progressBar1 != null && progressBar1.IsHandleCreated)
            {
                progressBar1.Invoke(new Action(() =>
                {
                    progressBar1.Visible = false;
                    progressBar1.Value = 0;
                }));
            }

            // 重置状态标签
            if (lblStatus != null && lblStatus.IsHandleCreated)
            {
                lblStatus.Invoke(new Action(() =>
                {
                    lblStatus.Text = "就绪";
                }));
            }

            // 重新启用导出按钮（假设按钮名为 btnExport）
            if (btnToDoc != null && btnToDoc.IsHandleCreated)
            {
                btnToDoc.Invoke(new Action(() =>
                {
                    btnToDoc.Enabled = true;
                    btnToDoc.Text = "导出文档";
                }));
            }

            SetUIEnabled(true);
        }

        /// <summary>
        /// 启用/禁用UI控件
        /// </summary>
        private void SetUIEnabled(bool enabled)
        {
            btnToDoc.Enabled = enabled;
            btnTest.Enabled = enabled;
            button1.Enabled = enabled;
            comboBox1.Enabled = enabled;
            comboBox2.Enabled = enabled;
            comboDocType.Enabled = enabled;
            checkedListBox1.Enabled = enabled;
            txtServer.Enabled = enabled;
            txtPort.Enabled = enabled;
            txtUser.Enabled = enabled;
            txtPwd.Enabled = enabled;
            txtSQLitePath.Enabled = enabled;
            btnSelect.Enabled = enabled;
            selVersion.Enabled = enabled;
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


        /// <summary>
        /// 根据数据库类型切换显示的控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0) //sqlserver
            {
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
            else if (comboBox2.SelectedIndex == 2)// sqlite
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
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.DataSource = null;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtSQLitePath.Text = openFileDialog1.FileName;
            }
        }

        /// <summary>
        /// 导出参数类
        /// </summary>
        private class ExportParameters
        {
            public string Constr { get; set; }
            public string Db { get; set; }
            public int DbTypeIndex { get; set; }
            public List<string> CheckList { get; set; }
            public int DocTypeIndex { get; set; }
        }
        /// <summary>
        /// 导出结果封装类
        /// </summary>
        public class ExportResult
        {
            public byte[] DocumentBytes { get; set; }      // Word 文档二进制数据
            public string DefaultFileName { get; set; }    // 建议的默认文件名（不含扩展名）
            public string ErrorMessage { get; set; }       // 错误信息（如有）
            public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
            public string Ext { get; set; } // doc  html  md
        }
    }
}
