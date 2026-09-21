using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LAFLib;
using System.Threading;
using System.Diagnostics;

namespace WindowsFormsApplication1
{    
    public partial class Form1 : Form
    {
        LAF af = new LAF();
        public Form1()
        {
            InitializeComponent();

            comboBox1.Items.Add(";uc rep cog");
            comboBox1.Items.Add(";uc rep ls1");
            comboBox1.Items.Add(";uc rep ls2");
            comboBox1.Items.Add(";uc rep mpos");
            comboBox1.Items.Add(";uc rep mspd");
            comboBox1.Items.Add(";uc rep pkp");
            comboBox1.Items.Add(";uc rep cogx");
            comboBox1.Items.Add(";uc rep icog");
            comboBox1.Items.Add(";uc rep tret");
            comboBox1.Items.Add(";uc rep ret");

        }
        private void btn_open_Click(object sender, EventArgs e)
        {
            af.Open(3, 9600);
            af.ChangeBaudRate(115200);
            //af.Open(4, 7600);
        }

        private void btn_write_Click(object sender, EventArgs e)
        {
            try
            {
                af.WritePort(comboBox1.Text);
                Thread.Sleep(50);
                byte[] ba = af.ReadPort();

                ASCIIEncoding asc = new ASCIIEncoding();

                string str = asc.GetString(ba);
                richTextBox1.AppendText(str);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            af.Func_AutoFocusStop();
            af.Motor_motionstop();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                af.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                af.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_nores_write_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int CCC = af.ReadPara_Report("cog");

                string a = CCC.ToString();
                richTextBox1.AppendText(a);
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message);
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                label1.Text = af.ReadPara_Report("apos").ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //label2.Text = af.ReadPara_Report("mpos").ToString();
        }
    }
}
