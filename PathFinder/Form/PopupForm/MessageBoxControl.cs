using RouteOptimizer.Object;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RouteOptimizer
{
    public partial class MessageBoxControl : System.Windows.Forms.Form
    {
        public RouteInfo RouteInfo = new RouteInfo();
        public vdControls.vdFramedControl vdFramedControl;
        public List<int> ids = new List<int>();
        public MessageBoxControl(string caption, string header, List<int> ids,  RouteInfo routeInfo, vdControls.vdFramedControl vdFramedControl)
        {
            InitializeComponent();
            lblMessage.Text = caption;
            this.Text = header;
            this.ids = ids;
            this.RouteInfo = routeInfo;
            this.vdFramedControl = vdFramedControl;
        }

        private void MessageBox_Load(object sender, EventArgs e)
        { 
            btn_Ignore.Focus();
        }

        private void btn_Ignore_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCheckList_Click(object sender, EventArgs e)
        {
            DuplicateItem duplicateItem = new DuplicateItem(RouteInfo, vdFramedControl,false, ids);
            duplicateItem.Show();
           // this.DialogResult = DialogResult.OK;
          //  this.Close();
        }
    }
}
