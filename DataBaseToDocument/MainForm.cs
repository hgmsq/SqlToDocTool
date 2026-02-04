using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataBaseToDocument
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void 关于作者ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 创建新的窗体实例
            AboutForm newForm = new AboutForm();
            // 显示新窗体
            newForm.Show();
        }

        /// <summary>
        /// 使用帮助窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 使用帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 创建新的窗体实例
            HelpForm newForm = new HelpForm();
            // 显示新窗体
            newForm.Show();
        }
    }
}
