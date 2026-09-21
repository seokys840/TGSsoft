using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;

namespace LAFLib
{
    public partial class LAF
    {
        byte del = 0x11;
        byte del2 = 0x95;
        byte cha = 0x5F;

        public void Restart()
        {
            try
            {
                WritePort(";coldboot");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Wait(long value)
        {
            try
            {
                WritePort(";uc wait " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // ------------------ public ---------------------------
        //Available parameter list
        //cog crf sig2 pkp lpw ls1 ls2 mpos apos srt ret tret segs cogrms pkprms mposrms ftus ftrv ibrt
        /// <summary>
        /// use rep
        /// </summary>
        /// <param name="_para"></param>
        /// <returns></returns>
        /// 
        ///  ####### Response Data ##########
        /// MLLAF3:uc rep mpos
        /// 
        /// 1
        /// mpos:         +1
        /// 
        /// MLLAF3:
        ///  ####### Response Data ##########
        
        object obj = new object();

        public int ReadPara_Report(string _para)
        {
            Monitor.Enter(obj);
            DiscardInBuffer();

            /////////////////////////////////////////////////////

            WritePort(";uc rep " + _para);
  
            byte[] ba = ReadPort();

            // Add by 2025.9.22
            for (int i = 0; i < ba.Length; i++)
            {
                if (ba[i] == del)
                    ba[i] = cha;

                if (ba[i] == del2)
                    ba[i] = cha;
            }
            // Add by 2025.9.22  

            string str = asc.GetString(ba);
            str = str.Replace("_", "");          // Add by 2025.9.22
            string[] sa = str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < sa.Length; i++)
            {
                if (sa[i].Contains(_para + ":"))
                {
                    string position = sa[i].Substring(sa[i].IndexOf(':') + 1);
                    int rv = int.Parse(position);
                   
                    Monitor.Exit(obj);
                    return rv;
                }
            }

            Monitor.Exit(obj);
            return int.MinValue;

        }
        /// <summary>
        /// use repo
        /// </summary>
        /// <param name="_para"></param>
        /// <returns></returns>
        public int ReadPara_Report2(string _para)
        {
            Monitor.Enter(obj);
            DiscardInBuffer();

            WritePort(";uc " + _para);
            Thread.Sleep(100);
            byte[] ba = ReadPort();

            if (ba[0] == 0x00) return int.MinValue;

            // Add by 2025.9.22
            for (int i = 0; i < ba.Length; i++)
            {
                if (ba[i] == del)
                    ba[i] = cha;

                if (ba[i] == del2)
                    ba[i] = cha;
            }
            // Add by 2025.9.22  
            
            string str = asc.GetString(ba);
            str = str.Replace("_", "");          // Add by 2025.9.22
            string[] sa = str.Split(new string[]{"\n"}, StringSplitOptions.RemoveEmptyEntries);
           
            if (sa.Length > 2)
            {
                string Res = "";
                for (int i = 0; i < sa.Length; i++)
                {
                    _para = _para.Replace("_", ""); // Add by 2025.9.22
                    if (sa[i].Contains(_para))
                    {
                       
                        Res = sa[i + 1];
                        int blankindex = 0;
                        if (Res.Contains(" "))
                        {
                            blankindex = Res.IndexOf(' ');
                         
                            Monitor.Exit(obj);
                            return int.Parse(Res.Remove(blankindex));     
                        }
                        else
                        {
              
                            Monitor.Exit(obj);
                            return int.Parse(Res);                                                    
                        }                        
                    }
                }

     
                Monitor.Exit(obj);
                return int.MinValue;
            }
            else
            {
 
                Monitor.Exit(obj);
                return int.MinValue;
            }
        }
       
        /// <summary>
        /// Make parameters permanent except the ones which are marked non-permanent. Returns in 500-1000ms. Should not be used during normal sensor operation as it disturbs sensor operation and pending motions.
        /// </summary>
        public void SetPara_SetLens(int LensNum)
        {
            
            try
            {
                string cmd = ";uc wdi_setlens " + LensNum.ToString();
                WritePort(cmd);
                byte[] ba = ReadPort();

                // Add by 2025.9.22
                for (int i = 0; i < ba.Length; i++)
                {
                    if (ba[i] == del)
                        ba[i] = cha;

                    if (ba[i] == del2)
                        ba[i] = cha;
                }
                // Add by 2025.9.22   

                string g = asc.GetString(ba);
                
                string[] sa = g.Split(new string[] { "\n", " " }, StringSplitOptions.RemoveEmptyEntries);


            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }


        /// <summary>
        /// normal 0, multi 1, peak 2
        /// </summary>
        public void SetSurface(int mode)
        {
            try
            {
                if (mode == 0)
                {
                    //WritePort(";uc alternatemode 0");
                    WritePort(";uc peakmode 0");
                    WritePort(";uc multisurfacemode 0");
                    ReadPort();
                }
                else if (mode == 1)
                {
                    //WritePort(";uc alternatemode 0");
                    WritePort(";uc peakmode 0");
                    WritePort(";uc multisurfacemode 1");
                    ReadPort();
                }
                else if (mode == 2)
                {
                    //WritePort(";uc alternatemode 0");
                    WritePort(";uc peakmode 1");
                    WritePort(";uc multisurfacemode 0");
                    ReadPort();
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// normal 0, multi 1, peak 2
        /// </summary>
        public int GetSurface()
        {
            try
            {
                //WritePort(";uc alternatemode");
                //byte[] ba = ReadPort();
                //string g = asc.GetString(ba);
                //string[] sa = g.Split(new string[] { "\n", " " }, StringSplitOptions.RemoveEmptyEntries);

                WritePort(";uc multisurfacemode");
                
                byte[] ba = ReadPort();

                if (ba[0] == 0x00)
                    return 2;

                // Add by 2025.9.22
                for (int i = 0; i < ba.Length; i++)
                {
                    if (ba[i] == del)
                        ba[i] = cha;

                    if (ba[i] == del2)
                        ba[i] = cha;
                }
                // Add by 2025.9.22                        
                               

                string g = asc.GetString(ba);
                g = g.Replace("_", "");                
                string[] sa = g.Split(new string[] { "\n", " " }, StringSplitOptions.RemoveEmptyEntries);

                WritePort(";uc peakmode");
                
                byte[] ba1 = ReadPort();


                // Add by 2025.9.22
                for (int i = 0; i < ba1.Length; i++)
                {
                    if (ba1[i] == del)
                        ba1[i] = cha;

                    if (ba1[i] == del2)
                        ba1[i] = cha;
                }
                // Add by 2025.9.22   

                string g1 = asc.GetString(ba1);
                g1 = g1.Replace("_", "");   
                string[] sa1 = g1.Split(new string[] { "\n", " " }, StringSplitOptions.RemoveEmptyEntries);


                if (sa1.Length <= 0) return 0;
                if (sa.Length <= 0) return 0;

                int[] nResMode = { 0, 0 };
                int blankindex = sa[4].IndexOf('');

                if (sa.Length == 7)
                    nResMode[0] = Convert.ToInt32(sa[4], 16);
                else
                    nResMode[0] = Convert.ToInt32(sa[3], 16);

                if (sa1.Length == 7)
                    nResMode[1] = Convert.ToInt32(sa1[4], 16);
                else
                    nResMode[1] = Convert.ToInt32(sa1[3], 16);
                
                if (nResMode[0] == 0 && nResMode[1] == 0)
                {
                    return 0;
                }
                else if (nResMode[0] == 1 && nResMode[1] == 0)
                {
                    return 1;
                }
                else if (nResMode[0] == 0 && nResMode[1] == 1)
                {
                    return 2;
                }              
                else
                {
                    throw new Exception("known excpetion 211");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// normal 0, bottom 1, top 2
        /// </summary>
        public void SetMode(int mode) 
        {
            try
            {
                //string cmd = ";uc topsurfprio -" + mode.ToString();
                string cmd = ";uc topsurfprio " + mode.ToString();
                WritePort(cmd);
                byte[] ba = ReadPort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// normal 0, bottom 1, top 2
        /// </summary>
        public int GetMode()
        { 
            try
            { 
                string cmd = ";uc topsurfprio";
                WritePort(cmd);
                byte[] ba = ReadPort();

                // Add by 2025.9.22
                for (int i = 0; i < ba.Length; i++)
                {
                    if (ba[i] == del)
                        ba[i] = cha;

                    if (ba[i] == del2)
                        ba[i] = cha;
                }
                // Add by 2025.9.22  

                string g = asc.GetString(ba);

                string[] sa = g.Split(new string[] { "\n", " "}, StringSplitOptions.RemoveEmptyEntries);

                int nResult = 0;
                //return -int.Parse(sa[3]);
                if (sa.Length == 6)
                    nResult = Convert.ToInt32(sa[4], 16);
                else if(sa.Length == 7)
                    nResult = Convert.ToInt32(sa[5], 16); 

                if (!(-2 <= nResult && nResult <= 0))
                    return 0;


                return nResult * -1;
                //return nResult;
            }
            
            catch (Exception ex)
            {
                throw ex;
            }
             


        }

        /// <summary>
        /// Make parameters permanent except the ones which are marked non-permanent. Returns in 500-1000ms. Should not be used during normal sensor operation as it disturbs sensor operation and pending motions.
        /// </summary>
        public void Save(int LensNum)
        {
            try
            {
                WritePort(";write_all_parameters");
                byte[] ba = ReadPort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int SetPara_ChangeLens(int Num)
        {
            try
            {
                WritePort(";uc wdi_getlens " + Num.ToString());
                byte[] ba = ReadPort();

                if(ba.Length <= 0)
                    return 0;

                // Add by 2025.9.22
                for (int i = 0; i < ba.Length; i++)
                {
                    if (ba[i] == del)
                        ba[i] = cha;

                    if (ba[i] == del2)
                        ba[i] = cha;
                }
                // Add by 2025.9.22  

                string str = asc.GetString(ba);
                str = str.Replace("_", "");          // Add by 2025.9.22
                string[] sa = str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (sa.Length == 3)
                    return int.Parse(sa[1]);
                if (sa.Length == 6)
                    return int.Parse(sa[5]);

                return int.Parse(sa[2]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetPara_Magnification(int LensNum, int value)
        {
            try
            {
                WritePort(";uc wdi_lensmag " + LensNum.ToString() + " " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int GetPara_Magnification(int LensNum)
        {
            try
            {
                DiscardInBuffer();
                WritePort(";uc wdi_lensmag " + LensNum.ToString());
                byte[] ba = ReadPort();

                // Add by 2025.9.22
                for (int i = 0; i < ba.Length; i++)
                {
                    if (ba[i] == del)
                        ba[i] = cha;

                    if (ba[i] == del2)
                        ba[i] = cha;
                }
                // Add by 2025.9.22  

                string str = asc.GetString(ba);
                str = str.Replace("_", "");          // Add by 2025.9.22
                string[] sa = str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                string Res = "";
                for (int i = 0; i < sa.Length; i++)
                {
                    if (sa[i].Contains("lensmag"))
                    {
                        Res = sa[i + 1];
                        return int.Parse(Res);
                    }
                }
                return int.MinValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Image sensor calibration. This command is performed automatically after system start and certain periods of time , but can be performed manually if needed. Returns after about 3000ms.
        /// </summary>
        public void SetPara_Calibration()
        {
            try
            {
                WritePort(";uc patternmeasure");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>        
        /// This parameter limits the number of sensor calibration cycles (automatic patternmeasure) to a certain number after system start.
        /// </summary>
        /// <param name="value">range 0 ~ 255, default = 3</param>
        public void SetPara_pmlimit(int value)
        {
            try
            {
                WritePort(";uc pmlimit " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// This value sets the factor for digital to analogue conversion of the distance signal. 
        /// This command applies only if the analog output or the DAC report is used i.e. the sensor is operated with an external motion controller        
        /// </summary>        
        /// <param name="value">range 1~10000, default = 64</param>
        public void SetPara_dacgain(int value)
        {
            try
            {
                WritePort(";uc dacgain " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// This command gates the laser on or off. The laser has to be enabled by the proper control signals to see an effect.
        /// </summary>
        /// <param name="value">1= gate laser on, 0= gate laser off</param>
        public void SetPara_lasergate(int value)
        {
            try
            {
                WritePort(";uc lasergate " + value.ToString());
                byte[] ba = ReadPort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This command sets the light amplification level of the sensor. This value also effects the signal to noise ratio.
        /// Usually the default 0 is best, high optical losses could be compensated with higher values up to 3. Image Sensor needs to be calibrated “paternmeasure” after a change.
        /// </summary>
        /// <param name="value">range 0 ~ 3, default = 3</param>
        public void SetPara_sensorgain(int value)
        {
            try
            {
                WritePort(";uc sensorgain " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>        
        /// This command limits the max laser energy. Higher values than 20 are not recommended and may affect reliable LAF operation
        /// </summary>        
        /// <param name="value">range 1~20, default = 20, means percent</param>
        public void SetPara_maxlaseron(int value)
        {
            try
            {
                WritePort(";uc maxlaseron " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>        
        /// This command sets a correction value for the laser line processing. For more information please contact MSG.
        /// </summary>
        /// <param name="value">range -10000 ~ 10000, Default = 0</param>
        public void SetPara_laserlineslant(int value)
        {
            try
            {
                //WritePort(";uc laserlineslant " + value.ToString());
                //byte[] ba = ReadPort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>        
        /// Sets the Region of interest auto control.The larger the window is the slower the samplerate an the higher the latency gets. 
        /// This command enables or disables automatic control for the window height.
        /// </summary>
        /// <param name="value">0 = off, 1 = on</param>
        public void SetPara_AutomaticROI(int value)
        {
            try
            {
                WritePort(";uc vro " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// This command sets the height and position (y direction) of the Region of interest window if VRO is off. 
        /// This directly controls samplerate and latency.
        /// </summary>
        /// <param name="start">start position, range 0 ~ 458</param>
        /// <param name="height">range 22 ~ 480</param>
        public void SetPara_ywindows(int start, int height)
        {
            try
            {
                WritePort(";uc ywindow " + start.ToString() + " " + height.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>        
        /// This command sets position and width (x direction) of the Region of interest. No effect on samplerate and latency.
        /// <param name="value"></param>
        /// </summary>
        /// <param name="start">start position, range 0 ~ 638</param>
        /// <param name="width">range 1 ~ 640</param>
        public void SetPara_xwindow(int start, int width)
        {
            try
            {
                WritePort(";uc xwindow " + start.ToString() + " " + width.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>                
        /// The value should be set that the VCOG reading value (see 6.3.2. & 5.4.4) gets close to 0 when the laser line width reading 
        /// (see 6.3.2. & 5.4.4) is the smallest. 
        /// This is the measurement laser focus position. In cases of changing objective lenses this value has to set properly for every objective lens.
        /// You don’t have to change this if you want to track in offset to the measurement laser focus position, you can use “motionrefpos” parameter for 
        /// this so the measurement laser focus position can stay correct.
        /// </summary>
        /// <param name="value">range 200000 ~ 300000</param>
        public void SetPara_vcogcenter(int value)
        {
            try
            {
                WritePort(";uc vcogcenter " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>        
        /// If peakmode is 1 only the strongest reflections are used for focus measurement. 
        /// This helps distinguishing multiple reflective surfaces, but reduces accuracy.
        /// Used when focusing on certain surfaces of specimens with multiple reflective surfaces.
        /// </summary>
        /// <param name="value">0 = OFF, 1 = ON</param>
        public void SetPara_peakmode(int value)
        {
            try
            {
                WritePort(";uc peakmode " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //이거 반드시 테스트 할 것
        /// <summary>        
        /// value 0 = ON, 1 = OFF, 2 = video mode, 4 = hybrid mode        
        /// Enable/disable closed loop focusing also called motiontracking or autofocusing.
        /// Example value 2 : video focus, value 4 : hybrid focus.
        /// Video option has to be licensed to work.
        /// </summary>
        /// <param name="value">0 = ON, 1 = OFF,2 = video mode, 4 = hybrid mode</param>
        public void SetPara_motiontrack(int value)
        {
            try
            {
                WritePort(";uc motiontrack " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>                
        /// Enable/disable any motion of the motor immediately.
        /// </summary>
        /// <param name="value">0 = OFF, 1 = ON</param>
        public void SetPara_motionenable(int value)
        {
            try
            {
                WritePort(";uc motionenable " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>                
        /// This command assigns the limit switch to the physical I/O connector at the LAF-C2 controller interface
        /// </summary>
        /// <param name="value">0 = OFF, 1 = ON</param>
        public void SetPara_limitsw1(int value)
        {
            try
            {
                WritePort(";uc limitsw1 " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>                
        /// This command assigns the limit switch to the physical I/O connector at the LAF-C2 controller interface.(Figure 15, table 5, coninector 6)
        /// </summary>
        /// <param name="value">        
        /// value 
        /// 0 = Connector X6, Limit switch CW, pin 1-3
        /// 1 = Connector X6, Limit switch CWW, pin 4-6,
        /// -1 = disable
        /// </param>
        public void SetPara_limitsw2(int value)
        {
            try
            {
                WritePort(";uc limitsw2 " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// After the Motor was turned manually or lost steps or hit some obstacle the stored limits are no longer valid and should be reset.
        /// The limits are non-permantent, reset at power on or reboot.
        /// </summary>
        /// <param name="sensor">0= ls1 , 1= ls2 (ls=limit switch)</param>
        /// <param name="para">0=Read, 1=Reset</param>
        public void SetPara_motionresetlimit(int sensor, int para)
        {
            try
            {
                WritePort(";uc motionresetlimit " + sensor.ToString() + " " + para.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        /// <summary>
        /// This parameter sets the dynamics for the auto focus control loop.
        /// Absolute higher values make the autofocus control loop faster until oscillation may occur. Sign depends on Motor/Driver wiring.
        /// </summary>
        /// <param name="gain">range -1000000 ~ 1000000, default = -500</param>
        public void SetPara_motionloopgain(int gain)
        {
            try
            {
                WritePort(";uc motionloopgain " + gain.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// This parameter sets the dynamics for the closed auto focus control loop. 
        /// More precisely it represents the isolated part of the motor/actor/stage.
        /// As the sensor speed is variable the effective (internal in LAF) loop gain is calculated by the LAF depending on motionloopgain and actorlatency.
        /// When the LAF is operating at maximum speed usually the actor latency controls loop dynamics as when the LAF is operated at minimum speed the actor latency usually gets insignificant.
        /// A actorlatency of zero means the actor is assumed to be ideally fast or much faster than the sensor.
        /// Higher values mean more actor latency.
        /// This should not be confused with the maximum speed the motor can reach. 
        /// It is more like a measure for the time delay from the motor/actor/stage receiving a step impulse to the actual movement of the focus.
        /// The longer it takes for the actor/motor to settle after a step impulse the higher the actorlatency needs to be.
        /// </summary>
        /// <param name="value">range 0 ~ 100000, default = 5000</param>
        public void SetPara_actorlatency(int value)
        {
            try
            {
                WritePort(";uc actorlatency " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// This parameter sets the gain of the used lens in relation to the motor/stage resolution for the control loop.
        /// It is specified as a integer number in change of cog units per motor step multiplied by 2^16.
        /// For example if you have a change in cog of 1300 per 100 motor steps you get cogperstepls16=851968 (1300/100*65536).
        /// For 6400 steps per mm and a 10x lens you usually get 13 cog per step.
        /// This is needed by the control loop as a compensation value and should not be changed in order to tune the control loop or make the control loop slower or more insensitive to disturbances of the specimen.
        /// It should represent the actual cog change per motor step and not be used for any loop tuning or adapting,
        /// the loopgain or actorlatency should be used to tune the loop or make it slower/faster on purpose.
        /// </summary>
        /// <param name="value">range 0 ~ 2^32-1,default = 851968</param>
        public void SetPara_cogperstepls16(int value)
        {
            try
            {
                WritePort(";uc cogperstepls16 " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// This parameter sets the acceleration/deceleration time (Figure 16)
        /// </summary>
        /// <param name="value">range = 1~200, default = 100</param>
        public void SetPara_motionacctcms(int value)
        {
            try
            {
                WritePort(";uc motionacctcms " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// This parameter sets the max start/stop speed without a acceleration time constant as a fraction of maximum speed.
        /// The effect of smaller values is to speed up the control loop for slow motor speeds.
        /// Normally this doesn’t require changes, using smaller values could make the motor loose steps.
        /// </summary>
        /// <param name="value">default 20 </param>
        public void SetPara_motionstartstop(int value)
        {
            try
            {
                WritePort(";uc motionstartstop " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// The command sets the max. speed in the velocity part of the motion profile (Figure 16).
        /// The step pulse width is less than 0.5/motionmaxspeed in s and less than 20us
        /// </summary>
        /// <param name="value">range 1- 500000, Maximum frequency of steps in Hz, default = 15000</param>
        public void SetPara_motionmaxspeed(int value)
        {
            try
            {

                WritePort(";uc motionmaxspeed " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// This parameter sets a limit for any movement in motor steps. As long as it is active (non-zero) the motor will not move to a higher position,
        /// if activated and the motor is at a higher position it will move to this specified position.
        /// </summary>
        /// <param name="value">-+ movement range of axis in motor steps, 0 means inactive, default = 0</param>
        public void SetPara_motionpluslimit(int value)
        {
            try
            {

                WritePort(";uc motionpluslimit " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// This parameter sets a limit for any movement in motor steps. As long as it is active (non-zero) the motor will not move to a lower position, 
        /// if activated and the motor is at a lower position it will move to this specified position.
        /// </summary>
        /// <param name="value">-+ movement range of axis in motor steps, 0 means inactive, default = 0</param>
        public void SetPara_motionminuslimit(int value)
        {
            try
            {

                WritePort(";uc motionminuslimit " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// This parameter sets a upper limit for the returned laser energy above which the motor stops
        /// This parameters can help to discriminate different surfaces with different reflectivity and only focus on those with
        /// a reflectivity in a certain range but stop the motor in other cases.
        /// Note that the “ret” report has limited accuracy and is most accurate in the linear measurement range of the sensor.
        /// You can do a profile graph with the “ret” report to evaluate.
        /// </summary>
        /// <param name="value"></param>
        public void SetPara_maxretdb(int value)
        {
            try
            {
                WritePort(";uc maxretdb " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Sets the motionzero reference to the current position. This makes mpos zero.
        /// </summary>
        public void SetPara_MotionZero()
        {
            try
            {
                WritePort(";uc motionzero");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// This parameter sets the focus offset position in COG units the control loop targets when auotofocus is active.
        /// Useful if the actual focus for external camera/laser differs from the measurement laser focus due to special lenses or special specimens.
        /// The LAF has best accuracy in the measurement laser focus position which should be correctly set by the parameter ‘vcogcenter’.
        /// Specifying a offset with this parameter can reduce accuracy or repeatability.
        /// </summary>
        /// <param name="value">range -10000 ~ 10000, default 0</param>
        public void SetPara_Motionrefpos(int value)
        {
            try
            {
                WritePort(";uc motionrefpos " + value.ToString());
                byte[] ba = ReadPort();
                //string a = asc.GetString(ba);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Switches off the motiontracking if laser signal is interrupted or motion is disturbed longer then set value.
        /// Also helps when the stepper motor is losing steps and doesn’t move anymore so it can not reach the focus position when autofocusing.
        /// In such a case it will stop the pulses to the motor driver and accelerate again.
        /// </summary>
        /// <param name="value">range 0 ~ 5000, 2ms/unit, default 1000</param>
        public void SetPara_Motiontimeout(int value)
        {
            try
            {
                WritePort(";uc motiontimeout " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// This is the deadzone in COG around the measurement laser focus point before the motor starts to move. 
        /// This “calms down” the autofocus control loop but reduces accuracy.
        /// </summary>
        /// <param name="value">range 0-2000, default 0</param>
        public void SetPara_Motiondeadzone(int value)
        {
            try
            {
                WritePort(";uc motiondeadzone " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Waits for the tracking error to be less than the value, and returns then. Returns immediately if tracking is not enabled.
        /// </summary>
        /// <param name="error">Optional COG error value, default 1000</param>
        /// <param name="timeout">Optional timeout in 2ms units, default 5000 (10s).</param>
        public void SetPara_Monitortrackingerror(int error, int timeout)
        {
            try
            {
                WritePort(";uc waittrack " + error.ToString() +
                    " " + timeout.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// This parameter sets a lower limit for the returned laser energy above which the motor stops.
        /// This parameters can help to discriminate different surfaces with different reflectivity and only focus on those with
        /// a reflectivity in a certain range but stop the motor in other cases.
        /// Note that the “ret” report has limited accuracy and is most accurate in the linear measurement range of the sensor.
        /// You can do a profile graph with the “ret” report to evaluate.
        /// </summary>
        /// <param name="value"></param>
        public void SetPara_minretdb(int value)
        {
            try
            {
                WritePort(";uc minretdb " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SetPara_Stepspermm(int value)
        {
            try
            {
                WritePort(";uc wdi_stepspermm " + value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int GetPara_Stepspermm()
        {
            try
            {
                WritePort(";uc wdi_stepspermm");
                byte[] ba = ReadPort();
                
                // Add by 2025.9.22
                for (int i = 0; i < ba.Length; i++)
                {
                    if (ba[i] == del)
                        ba[i] = cha;

                    if (ba[i] == del2)
                        ba[i] = cha;
                }
                // Add by 2025.9.22  

                string str = asc.GetString(ba);
                str = str.Replace("_", "");          // Add by 2025.9.22
                string[] sa = str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                string Res = "";
                for (int i = 0; i < sa.Length; i++)
                {
                    if (sa[i].Contains("wdistepspermm"))
                    {
                        Res = sa[i + 1];
                        return int.Parse(Res);
                    }
                }
                return int.MinValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public double GetPara_CogperStep()
        {
            try
            {
                int cg = ReadPara_Report2("cogperstepls16");
                int stepspermm = GetPara_Stepspermm();
                double result = (double)cg / 65536;//(double)cg / 65536 * (stepspermm / 1000);
                return Math.Round(result,2);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public void SetPara_Motionfoctol(int value)
        {
            try
            {
        

                /////////////////////////////////////////////////////

                WritePort(";uc motionfoctol " + value.ToString());
               
				byte[] ba = ReadPort();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }


}
