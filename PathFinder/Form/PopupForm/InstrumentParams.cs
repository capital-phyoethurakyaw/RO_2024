using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using vdControls;
using VectorDraw.Professional.vdCollections;
using VectorDraw.Professional.vdFigures;

namespace RouteOptimizer
{
    public partial class InstrumentParams : System.Windows.Forms.Form
    {
        public string T1 = "";
        public string T2 = "";
        public string InsertName = "";
        List<string> vdInsertList = null;
        public   vdControls.vdFramedControl vdFrameControl1 = null;
        public InstrumentParams(List<string> vdInsertList =null,   vdControls.vdFramedControl  vdFrameControl = null)
        {
            InitializeComponent();
            this.vdInsertList = vdInsertList;
            vdFrameControl1 = vdFrameControl;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtT1.Text) || string.IsNullOrEmpty(txtT2.Text) || string.IsNullOrEmpty(cboBlocks.Text)  )
            {
                MessageBox.Show("Please set a suible value at inputs");
                return;
            }
            T1 = txtT1.Text;
            T2 = txtT2.Text;
            InsertName = cboBlocks.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void InstrumentParams_Load(object sender, EventArgs e)
        {
            cboBlocks.DataSource = vdInsertList;
        }
        
        private void cboBlocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cboBlocks.Text))
            {
                 
                var blk = vdFrameControl1.BaseControl.ActiveDocument.Blocks.FindName(cboBlocks.Text);
                //var blk = vdFrameControl1.BaseControl.ActiveDocument.GetUsedBlocks().FindName(cboBlocks.Text);

                blk.SetUnRegisterDocument(vdFrameControl1.BaseControl.ActiveDocument);
                blk.setDocumentDefaults();
                vdFrameControl1.BaseControl.ActiveDocument.Redraw(true);
                vdFrameControl1.BaseControl.ActiveDocument.Update();
                blk.Entities.SetUnRegisterDocument(vdFrameControl1.BaseControl.ActiveDocument);
                blk.Update();
                if (Check_1C_2A(blk.Entities) || Chek_1C_2A_1L(blk.Entities))
                {
                    txtT1.Enabled = true;
                    txtT2.Enabled = true;
                    btn_ok.Enabled = true;
                }
                else
                {
                    txtT1.Enabled = false;
                    txtT2.Enabled = false;
                    btn_ok.Enabled = false;
                }
                vdFrameControl1.BaseControl.ActiveDocument.Redraw(true);
                vdFrameControl1.BaseControl.ActiveDocument.Update();

            }
        }
        private bool Check_1C_2A(vdEntities ve)
        {
            vdAttribDef atb1 = null;
            vdAttribDef atb2 = null;
            //vdLeader lder = null;
            vdCircle cir = null;
            foreach (var lines in ve)
            {
                if (!(lines is vdCircle) && !(lines is vdAttribDef) )
                    return false;
                if (lines is vdAttribDef l)
                {
                    if (atb1 == null)
                        atb1 = l;
                    else if (atb2 == null)
                        atb2 = l;
                    else
                        return false; 
                } 
                if (lines is vdCircle c)
                {
                    if (cir == null)
                        cir = c;
                    else
                        return false;
                } 
            }
            if (atb1 != null && atb2 != null && cir != null  )
            {
                return true;
            }
            return false;
        }
        private bool Chek_1C_2A_1L(vdEntities ve)
        {
            vdAttribDef atb1 = null;
            vdAttribDef atb2 = null;
            vdLeader lder = null;
            vdCircle cir = null;
            foreach (var lines in ve)
            {
                if (!(lines is vdCircle) && !(lines is vdAttribDef) && !(lines is vdLeader))
                    return false;
                if (lines is vdAttribDef l)
                {
                    if (atb1 == null)
                        atb1 = l;
                    else if (atb2 == null)
                        atb2 = l;
                    else
                        return false;

                }
                if (lines is vdLeader e)
                {
                    if (lder == null)
                        lder = e;
                    else
                        return false;
                }
                if (lines is vdCircle c)
                {
                    if (cir == null)
                        cir = c;
                    else
                        return false;
                }


            }
            if (atb1 != null && atb2 != null && cir != null && lder != null)
            {
                return true;
            }
            return false;
        }
    }
}
