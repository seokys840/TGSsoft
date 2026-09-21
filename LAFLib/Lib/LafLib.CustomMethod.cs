using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace LAFLib
{


    public partial class LAF
    {
        private bool bRunning = false;

        public void Func_FLaser()
        {
            WritePort(";uc motiontrack 1");
            byte[] ba = ReadPort(38);
        }

        public bool Func_Shot_L(int Offset, int Tolerance, int TimeOut)
        {
            lock (this)
            {
                bRunning = true;
                try
                {
                    //SetPara_Motionrefpos(Offset);
                    //SetPara_laserlineslant(0);
                    SetPara_lasergate(1);

                    WritePort(";uc motiontrack 1");
                    byte[] ba = ReadPort();

                    Stopwatch sw = Stopwatch.StartNew();
                    for (; ; Thread.Sleep(50))
                    {
                        int cog = ReadPara_Report("cog");
                        
                        if (Math.Abs(cog - Offset) <= Tolerance)
                            break;
                        if (sw.ElapsedMilliseconds > TimeOut)
                        {
                            bRunning = false;
                            return false;
                        }
                    }
                }                
                finally
                {
                    SetPara_lasergate(1);
                    Func_AutoFocusStop();
                    DiscardInBuffer();
                }

                bRunning = false;
                return true;
            }
        }

        public void Func_AutoFocusStop()
        {
            WritePort(";uc motiontrack 0");
            WritePort(";uc motionstop");
        }

        /// <summary>
        /// this method returns bool value that MSG program label UI 'to focus(um)'   
        /// </summary>
        /// <param name="foctol">foc.tol cog</param>
        /// <param name="cogum">cog/um</param>
        /// <returns></returns>
        public bool Func_Readfocus(double foctol, double cogum)
        {
            //WritePort(";uc motionfoctol " + foctol.ToString());

            int cog = ReadPara_Report("cog");
            int offset = ReadPara_Report2("motionrefpos");

            double diff = (double)(cog - offset) / cogum;
            if (diff < foctol / cogum)
                return false;
            else
                return true;
        }


        public bool Func_IsRunning()
        {
            return bRunning;
        }

        //public void Func_Jump(int pos)
        //{
        //    Func_Shot_L();
        //    Motor_Move(0, pos);
        //}
    }
}
