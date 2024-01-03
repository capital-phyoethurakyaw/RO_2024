using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
namespace RouteOptimizer
{
    public partial class VD_File_Converter : System.Windows.Forms.Form
    {
        public VD_File_Converter()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string installerPath = @"C:\Program Files\Wyzrs\RouteOptimizer\vdFileConverterEVAL.msi"; // @"C:\Program Files\Wyzrs\RouteOptimizer\setup.exe";// @"C:\Users\Asus\Downloads\msi-to-exe-demo.exe"; //vdFileConverterEVAL.msi
            if (!File.Exists(installerPath))
            {
                MessageBox.Show("There is no installer to be executed in specified directory.");
                return;
            }
            Process pr = new Process();
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = installerPath,
                    Verb = "runas"
                };

                pr = Process.Start(installerPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
                //Console.WriteLine($"Error: {ex.Message}");
            }
            //pr.WaitForExit();
            pr.WaitForExit();// += Pr_Exited;
            Exe();
        }

        private void Pr_Exited(object sender, EventArgs e)
        {
            
        }
        private void Exe(bool flg = false)
        {
            if (!File.Exists(@"C:\Program Files (x86)\VectorDraw\VectorDraw FileConverter 4.10.x Lite\VectorDrawFileConverter4.exe"))
            {
                label3.Text = "X";
                button2.Enabled = true;
                if (flg)
                 MessageBox.Show("There is no package Vdf converter Elite to be executed in specified directory."); 
            }
            else
            {
                label3.Text = "✔️";
                button2.Enabled = false;
                if (flg)
                    MessageBox.Show("Vdf converter Elite is already installed.");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        { 
            Exe(true); 
        }

        private void VD_File_Converter_Load(object sender, EventArgs e)
        {
            Exe();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
