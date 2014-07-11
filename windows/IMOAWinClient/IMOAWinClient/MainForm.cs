using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IMOAWinClient
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            FormDockTemplate m_oDockFormTemplate = new FormDockTemplate(this);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UserInfo u = new UserInfo();
            treeList1.DataSource = u.UserInfoList();
            treeList1.KeyFieldName = "ID";
            treeList1.ParentFieldName = "parentID";
        }

        private void btnPhoto_Click(object sender, EventArgs e)
        {
            Form_SelfInfor frm = new Form_SelfInfor();
            frm.Show();
        }

        private void btnPhoto_EditValueChanged(object sender, EventArgs e)
        {
            
        }

        private void btnCLFBX_Click(object sender, EventArgs e)
        {
            Form_BX fm = new Form_BX();
            fm.Show();
        }
    }
}
