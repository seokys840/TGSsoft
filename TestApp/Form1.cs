using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace TestApp
{
    public partial class Form1 : Form
    {
        LAFLib.LAF sc;
        Timer timer = new Timer();
        Timer timer2 = new Timer();
        Timer timer3 = new Timer();     // Tracking Timer
        bool bWait_AnotherUse = false;
        bool bComport_Open = false;


        public Form1()
        {
            InitializeComponent();

            for (int i = 0; i < 50; i++)
            {
                string str;
                
                str = (i+1).ToString();
                cbb_comport.Items.Add(str);
            }
            cbb_comport.SelectedIndex = 2;
    
            
            cbb_baudrate.Items.Add("9600");
            //cbb_baudrate.Items.Add("115200");            
            cbb_baudrate.SelectedIndex = 0;
            cbb_lens.SelectedIndex = 0;

            comboBox1.Items.Add("Normal"); // 0
            //comboBox1.Items.Add("bottom prio");// -1
            //comboBox1.Items.Add("top prio"); // -2
            comboBox1.Items.Add("top prio");// -1
            comboBox1.Items.Add("bottom prio"); // -2
            comboBox1.SelectedIndex = 0;

            comboBox2.Items.Add("normal"); 
            comboBox2.Items.Add("multi");
            comboBox2.Items.Add("peak");
            comboBox2.SelectedIndex = 0;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            sc = new LAFLib.LAF();

            try
            {
                if (rb_lan.Checked)
                    sc.OpenLan(tb_ip.Text.Trim(), int.Parse(tb_tcpport.Text));
                else
                    sc.Open(int.Parse(cbb_comport.Text), int.Parse(cbb_baudrate.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connect Fail!!\r\n" + ex.Message);
                return;
            }

            bComport_Open = true;


            groupBox1.Visible = true;
            groupBox2.Visible = true;
            groupBox3.Visible = true;
            groupBox5.Visible = true;

            
            this.checkBox1.Checked = true;

                         
            timer.Interval = 1500;          // 1000ms => 1초


            labRunning.BackColor = Color.RoyalBlue;

            // Event
            timer.Start();

            timer.Tick += new EventHandler(fm_event_timer);

            // Timer 2
            timer2.Interval = 100;
            
            timer2.Start();
            timer2.Tick += new EventHandler(fm_event_timer2);

            // Timer 3
            timer3.Interval = 10000;        // 10초
            timer3.Tick += new EventHandler(fm_event_timer3);


            int default1 = 0;
            int default2 = 0;

            this.ED_PLUS_VALUE.Text = default1.ToString();
            this.ED_MINUS_VALUE.Text = default2.ToString();

        }

        private void rb_mode_CheckedChanged(object sender, EventArgs e)
        {
            bool lan = rb_lan.Checked;

            label24.Visible = !lan;
            cbb_comport.Visible = !lan;
            label12.Visible = !lan;
            cbb_baudrate.Visible = !lan;
            button14.Enabled = !lan;        // Baudrate 변경은 Serial 전용

            label_ip.Visible = lan;
            tb_ip.Visible = lan;
            label_tcpport.Visible = lan;
            tb_tcpport.Visible = lan;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
              
            int cogVal = Math.Abs(sc.ReadPara_Report("cog"));

            label1.Text = cogVal.ToString(); // cog
            textBox1.Text = tb_motionrefpos.Text; // offset

            if (tb_motionrefpos.Text == null)
            {
                MessageBox.Show("Offet Value is empty!!");
                return;
            }


            double calc = Math.Abs(cogVal - Math.Abs(int.Parse(textBox1.Text)));
            double tol = double.Parse(tb_Tol.Text);
            textBox3.Text = calc.ToString();

            if (calc <= tol)
            {
                label6.Text = "<";
                label9.Text = "in focus";
            }
            else
            {
                label6.Text = ">=";
                label9.Text = "out focus";
            }

            //if (sc.Func_Readfocus(double.Parse(tb_Tol.Text), double.Parse(tb_cogum.Text)))
            //{
             
            //}
            //else
            //{
                
            //}
            bWait_AnotherUse = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int timeout = 2000;         // 1000 => 1s
            bWait_AnotherUse = true;
            labRunning.BackColor = Color.Red;
                        
            if (checkBox2.Checked)      // Tracking Mode
            {
                int offset = int.Parse(textBox1.Text);
                int tol = int.Parse(tb_Tol.Text);
                
                sc.Func_Shot_L(offset, tol, timeout);
                labRunning.BackColor = Color.Red;
                sc.Func_FLaser();
                timer3.Start();
                button3.Enabled = false;
            }
            else
            {
                int offset = int.Parse(textBox1.Text);
                int tol = int.Parse(tb_Tol.Text);
                sc.Func_Shot_L(offset, tol, timeout);

                if (int.Parse(tb_jump.Text) != 0 && chk_Jump.Checked)
                {
                    int pos = int.Parse(tb_jump.Text) * (sc.GetPara_Stepspermm() / 1000);
                    sc.Motor_ConCurrentMove(0, pos);
                }
            }

            bWait_AnotherUse = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            try
            {                
                int Curcog = sc.ReadPara_Report("cog");
                tb_motionrefpos.Text = textBox1.Text = Curcog.ToString();
                sc.SetPara_Motionrefpos(Curcog);                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            bWait_AnotherUse = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            int pos = -1 * int.Parse(textBox4.Text) * (sc.GetPara_Stepspermm() / 1000);


            //if (chk_PlusLimit.Checked)
            //    if ((int.Parse(lb_mpos.Text) + int.Parse(textBox4.Text)) >= int.Parse(text_PlusLimit.Text))
            //        pos = int.Parse(text_PlusLimit.Text) * (sc.GetPara_Stepspermm() / 1000);


            if (chk_MinusLimit.Checked)
                if ((int.Parse(lb_mpos.Text) + -1 * int.Parse(textBox4.Text)) <= int.Parse(text_MinusLimit.Text))
                    pos = (int.Parse(text_MinusLimit.Text) - int.Parse(lb_mpos.Text)) * (sc.GetPara_Stepspermm() / 1000);

            sc.Motor_Move(0, pos);

            bWait_AnotherUse = false;

            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            int pos = int.Parse(textBox4.Text) * (sc.GetPara_Stepspermm() / 1000);


            if (chk_PlusLimit.Checked)
                if ((int.Parse(lb_mpos.Text) + int.Parse(textBox4.Text)) >= int.Parse(text_PlusLimit.Text))
                    pos = (int.Parse(text_PlusLimit.Text) - int.Parse(lb_mpos.Text)) * (sc.GetPara_Stepspermm() / 1000);


            //if (chk_MinusLimit.Checked)
            //    if ((int.Parse(lb_mpos.Text) + int.Parse(textBox4.Text)) <= int.Parse(text_MinusLimit.Text))
            //        pos = int.Parse(lb_mpos.Text) * (sc.GetPara_Stepspermm() / 1000);


            sc.Motor_Move(0, pos);

            bWait_AnotherUse = false;

            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            sc.Motor_motionstop();

            if (checkBox2.Checked)
            {
                timer3.Stop();
                button3.Enabled = true;
            }

            bWait_AnotherUse = false;

            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            
            int  MaxValue = -6250000;

           //if (chk_MinusLimit.Checked)
           //    if (int.Parse(lb_mpos.Text) >= int.Parse(text_MinusLimit.Text) * (sc.GetPara_Stepspermm() / 1000))
           //        MaxValue = int.Parse(text_MinusLimit.Text) * (sc.GetPara_Stepspermm() / 1000);

           sc.Motor_ConCurrentMove(0, MaxValue);

            bWait_AnotherUse = false;

            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;

            int MaxValue = 6250000;

            //if (chk_PlusLimit.Checked)
            //    if (int.Parse(lb_mpos.Text) <= int.Parse(text_PlusLimit.Text) * (sc.GetPara_Stepspermm() / 1000))
            //        MaxValue = int.Parse(text_PlusLimit.Text) * (sc.GetPara_Stepspermm() / 1000);

            sc.Motor_ConCurrentMove(0, MaxValue);

            bWait_AnotherUse = false;

            

        }

        private void button10_Click(object sender, EventArgs e)
        {
            // Motor Current Position set to "0" position.
            bWait_AnotherUse = true;
            sc.SetPara_MotionZero();
            lb_mpos.Text = sc.ReadPara_Report("mpos").ToString();
            bWait_AnotherUse = false;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            sc.Motor_ConCurrentMove(2,0);
            bWait_AnotherUse = false;

            
        }

        private void button12_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            int mpos = sc.ReadPara_Report("mpos");
            double dMpos = mpos / (sc.GetPara_Stepspermm() / 1000);
            lb_mpos.Text = dMpos.ToString();
            bWait_AnotherUse = false;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            // Event
            timer.Stop();
            timer2.Stop();
            timer3.Stop();

            if (bComport_Open)
                sc.Close();

            groupBox1.Visible = false;
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            groupBox5.Visible = false;

        }

        private void button14_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            sc.ChangeBaudRate(int.Parse(cbb_baudrate.Text));
            bWait_AnotherUse = false;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            this.button16.Enabled = false;
            bWait_AnotherUse = true;

            //int res = sc.SetPara_ChangeLens(cbb_lens.SelectedIndex);

            //if (res != 1)
            //   MessageBox.Show("Lens Change Fail!!");

            comboBox1.SelectedIndex = sc.GetMode();
            comboBox2.SelectedIndex = sc.GetSurface();
           
            tb_mag.Text = sc.GetPara_Magnification(cbb_lens.SelectedIndex).ToString();
            
            tb_vcogcenter.Text = sc.ReadPara_Report2("vcogcenter").ToString();

            double t = sc.ReadPara_Report2("wdi_lensjump " + cbb_lens.SelectedIndex.ToString()) / (sc.GetPara_Stepspermm() / 1000);
            tb_jump.Text = t.ToString();
            
            tb_motionrefpos.Text = sc.ReadPara_Report2("motionrefpos").ToString();
            //tb_motionrefpos.Text = sc.ReadPara_Report("cog").ToString();       // Offset -> cog로 대체
            
            tb_minpkp.Text = sc.ReadPara_Report2("minpkp").ToString();

            tb_tolerance.Text = sc.ReadPara_Report2("motionfoctol").ToString();
            
            tb_Tol.Text = tb_tolerance.Text;

            tb_cogum.Text = sc.GetPara_CogperStep().ToString();
            bWait_AnotherUse = false;

            this.button16.Enabled = true;
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            //sc.SetPara_SetLens(cbb_lens.SelectedIndex);
            //sc.WritePort(";uc motionfoctol 350");
            sc.SetPara_Motionfoctol(350);           // Modify 2025.9.15
            sc.Save(cbb_lens.SelectedIndex);
            bWait_AnotherUse = false;
        }

        private void button17_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            //sc.SetPara_SetLens(cbb_lens.SelectedIndex);
            sc.SetPara_SetLens(cbb_lens.SelectedIndex);
            sc.SetMode(comboBox1.SelectedIndex);
            sc.SetSurface(comboBox2.SelectedIndex);            
            //sc.Save(cbb_lens.SelectedIndex);
            bWait_AnotherUse = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            if (!checkBox2.Checked)
            {
                sc.Func_AutoFocusStop();
            }
            bWait_AnotherUse = false;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            // Move from Zero Position to Point.
            bWait_AnotherUse = true;
            int pos = int.Parse(textBox2.Text) * (sc.GetPara_Stepspermm() / 1000);

            if (chk_PlusLimit.Checked)
               if (int.Parse(textBox2.Text) >= int.Parse(text_PlusLimit.Text))
                   pos = int.Parse(text_PlusLimit.Text) * (sc.GetPara_Stepspermm() / 1000);

            if (chk_MinusLimit.Checked)
                if (int.Parse(textBox2.Text) <= int.Parse(text_MinusLimit.Text))
                    pos = int.Parse(text_MinusLimit.Text) * (sc.GetPara_Stepspermm() / 1000);

            sc.Motor_ConCurrentMove(2, pos);
            bWait_AnotherUse = false;

            
        }

        private void button19_Click(object sender, EventArgs e)
        {
            // Move from Zero Position to Point.
            bWait_AnotherUse = true;
            int pos = int.Parse(textBox5.Text) * (sc.GetPara_Stepspermm() / 1000);

            if (chk_PlusLimit.Checked)
                if (int.Parse(textBox5.Text) >= int.Parse(text_PlusLimit.Text))
                    pos = int.Parse(text_PlusLimit.Text) * (sc.GetPara_Stepspermm() / 1000);

            if (chk_MinusLimit.Checked)
                if (int.Parse(textBox5.Text) <= int.Parse(text_MinusLimit.Text))
                    pos = int.Parse(text_MinusLimit.Text) * (sc.GetPara_Stepspermm() / 1000);

            sc.Motor_ConCurrentMove(2, pos);
            bWait_AnotherUse = false;

            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {            
            bWait_AnotherUse = true;
            sc.SetPara_lasergate(checkBox1.Checked ? 1 : 0);
            bWait_AnotherUse = false;
        }

        private void fm_event_timer(object sender, EventArgs e)
        {
            // 2025.9.15
            /*
            if (!bWait_AnotherUse)
                ;//checkMotor_Limitswitch();
             */
            if (!bWait_AnotherUse)
            {
                checkMotor_Limitswitch();

                checkMotor_ReadPos();

            }
            // 2025.9.15


            if (sc.Func_IsRunning())
                labRunning.BackColor = Color.Red;
            else
                labRunning.BackColor = Color.RoyalBlue;

        }

        private void fm_event_timer2(object sender, EventArgs e)
        {
            cbb_lens_SelectedIndexChanged(sender, e);

            // Limit Default Setting Start
            Default_Limit_Set();

            timer2.Stop();
        }

        private void fm_event_timer3(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            sc.Func_FLaser();
            bWait_AnotherUse = false;
        }

        public void checkMotor_ReadPos()
        {
            try
            {
                int mpos = sc.ReadPara_Report("mpos");
                double dMpos = mpos / (sc.GetPara_Stepspermm() / 1000);
                lb_mpos.Text = dMpos.ToString();

                lb_mpos.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void checkMotor_Limitswitch()
        {
            try
            {
                int nValue = sc.ReadPara_Report("ls2");
                if (nValue == 1)
                    button8.BackColor = System.Drawing.Color.LightPink;
                else
                    button8.BackColor = System.Drawing.Color.LightGray;

                button8.Refresh();

                nValue = sc.ReadPara_Report("ls1");
                if (nValue == 1)
                    button7.BackColor = System.Drawing.Color.LightPink;
                else
                    button7.BackColor = System.Drawing.Color.LightGray;

                button7.Refresh();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            int pos = (5 * int.Parse(textBox4.Text)) * (sc.GetPara_Stepspermm() / 1000);


            if (chk_PlusLimit.Checked)
                if ((int.Parse(lb_mpos.Text) + 5 * int.Parse(textBox4.Text)) >= int.Parse(text_PlusLimit.Text))
                    pos = (int.Parse(text_PlusLimit.Text) - int.Parse(lb_mpos.Text)) * (sc.GetPara_Stepspermm() / 1000);


            //if (chk_MinusLimit.Checked)
            //    if ((int.Parse(lb_mpos.Text) + 5 * int.Parse(textBox4.Text)) <= int.Parse(text_MinusLimit.Text))
            //        pos = int.Parse(text_MinusLimit.Text) * (sc.GetPara_Stepspermm() / 1000);
            
            sc.Motor_Move(0, pos);

            bWait_AnotherUse = false;

            
        }

        private void button21_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            int pos = (-5 * int.Parse(textBox4.Text)) * (sc.GetPara_Stepspermm() / 1000);

            //if (chk_PlusLimit.Checked)
            //    if ((int.Parse(lb_mpos.Text) + -5 * int.Parse(textBox4.Text)) >= int.Parse(text_PlusLimit.Text))
            //        pos = int.Parse(text_PlusLimit.Text) * (sc.GetPara_Stepspermm() / 1000);


            if (chk_MinusLimit.Checked)
                if ((int.Parse(lb_mpos.Text) + -5 * int.Parse(textBox4.Text)) <= int.Parse(text_MinusLimit.Text))
                    pos = (int.Parse(text_MinusLimit.Text) - int.Parse(lb_mpos.Text)) * (sc.GetPara_Stepspermm() / 1000);

            sc.Motor_Move(0, pos);

            bWait_AnotherUse = false;

        }

        private void button23_Click(object sender, EventArgs e)
        {
            // Motor Move until tuch to Limit SW1.
            bWait_AnotherUse = true;

            int MaxValue = -6250000;

            sc.Motor_ConCurrentMove(1, MaxValue);

            bWait_AnotherUse = false;
            
        }

        private void button22_Click(object sender, EventArgs e)
        {
            // Motor Move until tuch to Limit SW1.
            bWait_AnotherUse = true;

            int MaxValue = 6250000;

            sc.Motor_ConCurrentMove(1, MaxValue);

            bWait_AnotherUse = false;

        }

        private void cbb_lens_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bComport_Open)
            {
                this.button16.Enabled = false;
                bWait_AnotherUse = true;

                int res = sc.SetPara_ChangeLens(cbb_lens.SelectedIndex);

                if (res != 1)
                    MessageBox.Show("Lens Change Fail!!");

                comboBox1.SelectedIndex = sc.GetMode();
	            comboBox2.SelectedIndex = sc.GetSurface();
           
              
	            tb_mag.Text = sc.GetPara_Magnification(cbb_lens.SelectedIndex).ToString();
            
	            tb_vcogcenter.Text = sc.ReadPara_Report2("vcogcenter").ToString();

	            double t = sc.ReadPara_Report2("wdi_lensjump " + cbb_lens.SelectedIndex.ToString()) / (sc.GetPara_Stepspermm() / 1000);
	            tb_jump.Text = t.ToString();
            
	            tb_motionrefpos.Text = sc.ReadPara_Report2("motionrefpos").ToString();// 명령어 없음.
                //tb_motionrefpos.Text = sc.ReadPara_Report("cog").ToString();       // Offset -> cog로 대체

            
	            tb_minpkp.Text = sc.ReadPara_Report2("minpkp").ToString();

	            tb_tolerance.Text = sc.ReadPara_Report2("motionfoctol").ToString();
            
	            tb_Tol.Text = tb_tolerance.Text;

	            tb_cogum.Text = sc.GetPara_CogperStep().ToString();
                 
                bWait_AnotherUse = false;
                this.button16.Enabled = true;
            }
        }

        private void MT_LASERON_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            sc.SetPara_lasergate(1);
            bWait_AnotherUse = false;
        }

        private void MT_LASEROFF_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            sc.SetPara_lasergate(0);
            bWait_AnotherUse = false;
        }

        private void MT_TRACKINGON_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;

            sc.SetPara_motiontrack(1);

            bWait_AnotherUse = false;
        }

        private void MT_TRACKINGOFF_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;

            sc.SetPara_motiontrack(0);

            bWait_AnotherUse = false;
        }

        private void MT_LIMITSET_Click(object sender, EventArgs e)
        {
            bWait_AnotherUse = true;
            int PlusValue = int.Parse(ED_PLUS_VALUE.Text);
            PlusValue *= (sc.GetPara_Stepspermm() / 1000);
            sc.SetPara_motionpluslimit(PlusValue);
             

            int MinusValue = int.Parse(ED_MINUS_VALUE.Text);
            MinusValue *= (sc.GetPara_Stepspermm() / 1000);
            sc.SetPara_motionminuslimit(MinusValue);
            bWait_AnotherUse = false;
        }

        public void Default_Limit_Set()
        {
            bWait_AnotherUse = true;
            sc.SetPara_motionpluslimit(0);
            sc.SetPara_motionminuslimit(0);
            bWait_AnotherUse = false;
        }
    }

    
}

