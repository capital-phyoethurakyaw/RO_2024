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
    public partial class BoxSizing : System.Windows.Forms.Form
    {
        public int Width = 0;
        public int Height = 0;
        public string TBColor = "";
        public string IOColor = "";
        public string MCCColor = "";
        public BoxSizing(int width, int height,string io, string tb, string mcc)
        {
            InitializeComponent();
            BindCombo();
            IOColor = io;
            TBColor = tb;
            MCCColor = mcc;
            //if (width <= 1000) width = 3000;
            //if (height <= 1000) height = 3000;
            Width = width;
            Height = height;

        }
        private void BindCombo()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Type");
            dt.Rows.Add("DarkBlue");
            dt.Rows.Add("Cyan");
            dt.Rows.Add("Red");
            dt.Rows.Add("Green");
            dt.Rows.Add("Yellow");

            cbo_IOColor.ValueMember = cbo_IOColor.DisplayMember = "Type";
             cbo_IOColor.DataSource = dt;

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Type");
            dt2.Rows.Add("DarkBlue");
            dt2.Rows.Add("Cyan");
            dt2.Rows.Add("Red");
            dt2.Rows.Add("Green");
            dt2.Rows.Add("Yellow");
            cbo_MCCColor.ValueMember = cbo_MCCColor.DisplayMember = "Type";
            cbo_MCCColor.DataSource  = dt2;

            DataTable dt3 = new DataTable();
            dt3.Columns.Add("Type");
            dt3.Rows.Add("DarkBlue");
            dt3.Rows.Add("Cyan");
            dt3.Rows.Add("Red");
            dt3.Rows.Add("Green");
            dt3.Rows.Add("Yellow");
            cbo_TBColor.ValueMember = cbo_TBColor.DisplayMember = "Type";

            cbo_TBColor.DataSource =  dt3;


        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtHeight.Text) || string.IsNullOrEmpty(txtWidth.Text) )
            {
                MessageBox.Show("There is an empty field at input. Please fix this.");
                return;
            }
            bool IsError = false; 
            try
            {
                Convert.ToInt32(txtHeight.Text);
                Convert.ToInt32(txtWidth.Text);
            }
            catch
            {
                IsError = true;
            }
            if (IsError)
            {
                MessageBox.Show("There is an error at user's entered input. Please fix this.");
                return;
            }

            if (Convert.ToInt32(txtWidth.Text) < 1000 || Convert.ToInt32(txtWidth.Text) > 100000)
            {
                IsError = true;
            }
            if (Convert.ToInt32(txtHeight.Text) < 1000 || Convert.ToInt32(txtHeight.Text) > 100000)
            {
                IsError = true;
            }
            if (IsError)
            {
                MessageBox.Show("Please enter the value which is inputable range between 1000 and 100000. Please fix this.");
                return;
            }

            Width = Convert.ToInt32(txtWidth.Text);
            Height = Convert.ToInt32(txtHeight.Text);
            TBColor = cbo_TBColor.SelectedValue.ToString();
            MCCColor = cbo_MCCColor.SelectedValue.ToString();
            IOColor = cbo_IOColor.SelectedValue.ToString();
            MessageBox.Show("Saved Successfully!");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BoxSizing_Load(object sender, EventArgs e)
        {
            txtWidth.Text = Width.ToString();
            txtHeight.Text = Height.ToString();
            cbo_IOColor.SelectedValue = IOColor;
            cbo_MCCColor.SelectedValue = MCCColor;
            cbo_TBColor.SelectedValue = TBColor;
        }
    }
}
