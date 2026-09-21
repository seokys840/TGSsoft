using System;
using System.Runtime.InteropServices;

namespace LAFLib
{
    public partial class LAF
    {
        /// <summary>
        /// Stops Motor immediately, clears pending motions, resets limit switch positions and sets motiontrack to 0.
        /// To be used in case the motor may have lost track/steps.
        /// </summary>
        public void Motor_motionreset()
        {
            try
            {
                WritePort(";uc motionreset");
                ReadPort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        ///Decellerates Motor immediately, clears pending motions and sets motiontrack to 0.
        ///To be used for prematurely ending a ongoing motion with “move” or “cmove” in a controlled way or stop tracking (set motiontrack to 0).
        ///If the motor lost track or steps the position counter is not valid anymore, 
        ///this command will not reset limit switch positions. Use “motionreset” in such cases.
        /// </summary>
        public void Motor_motionstop()
        {
            try
            {
                WritePort(";uc motionstop");
                ReadPort();

                bRunning = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// This command will be perform a motion if Signal Quality bigger than 0, 
        /// but returns immediately without waiting for the motion to be completed.
        /// </summary>
        /// <param name="relative">
        /// 0 =relative to current position
        /// 1 =relative to ls1 (ls1= limit switch1), only useful after ls1 was hit
        /// 2 =relative to motionzero point
        /// </param>
        /// <param name="step">target step</param>
        public void Motor_Move(int relative, int step)
        {
            try
            {
                WritePort(";uc move " + relative.ToString() + " " + step.ToString());
                ReadPort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// This command will perform a motion neglecting the quality signal (see 6.3.2. & 5.4.4) 
        /// and returns after the motion is completed, timeout 10s.
        /// </summary>
        /// <param name="relative">0 =relative to current position, 1 =relative to ls1 (ls1= limit switch1), only useful after ls1 was hit, 2 =relative to motionzero point</param>
        /// <param name="step">target steps</param>
        /// <param name="maxspeed">optional maximum speed in pulses/sec only for this movement</param>
        public void Motor_ForcedMove(int relative, int step, int maxspeed)
        {
            try
            {
                WritePort(";uc fmove " + relative.ToString() + " " + step.ToString()+ " " + maxspeed.ToString());
                ReadPort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This command will perform a motion neglecting the quality signaland returns immediately.
        /// </summary>
        /// <param name="relative">0 =relative to current position, 1 =relative to ls1 (ls1= limit switch1), only useful after ls1 was hit, 2 =relative to motionzero point</param>
        /// <param name="step">target steps</param>
        public void Motor_ConCurrentMove(int relative, int step)
        {
            try
            {
                //WritePort(";uc cmove " + relative.ToString() + " " + step.ToString());
                WritePort(";uc cmove " + relative.ToString() + " " + step.ToString());
                ReadPort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// This command moves to the target position with the optional maximum speed 
        /// regardless of the quality signal switching into tracking mode (motiontrack 1) 
        /// when a valid signal is detected but not before the specified percentage of the movement has been accomplished.
        /// It returns when tracking is activated, Timeout is 10s.
        /// </summary>
        /// <param name="relative">
        /// 0 =relative to current position
        /// 1 =relative to ls1 (ls1= limit switch1), only useful after ls1 was hit
        /// 2 =relative to motionzero point
        /// </param>
        /// <param name="steps">Target steps</param>
        /// <param name="maxspeed">optional maximum speed in pulses/sec only for this movement, 0 is default</param>
        /// <param name="percentage">percentage of movement accomplished before tracking is enabled</param>
        public void Motor_CatchMove(int relative, int steps, int maxspeed, int percentage)
        {
            try
            {
                WritePort(";uc catchmove"+ 
                    " "+relative.ToString()+ 
                    " "+steps.ToString() +
                    " "+maxspeed.ToString()+
                    " "+percentage.ToString());
                ReadPort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// This command moves to a target surface.
        /// Usually this is issued after motiontrack has been enabled to move to the first surface of the specimen.
        /// After the cog error is lower than Value 1 tracking is disabled and a movement with the relative distance Value 3 is started 
        /// and after the percentage Value 4 of this movement has been done tracking is enabled with the peakmode of Value 5.
        /// </summary>
        /// <param name="cogError">0 means use default</param>
        /// <param name="timeout">0 means use default</param>
        /// <param name="distance">relative distance</param>
        /// <param name="move">percentage of movement after tracking is enabled</param>
        /// <param name="peekmode">peakmode after first surface has been reached, -1 means no change</param>
        public void Motor_SurfaceMove(int cogError, int timeout, int distance, int move, int peekmode)
        {
            try
            {
                WritePort(";uc surfmove " +
                    " " + cogError.ToString() +
                    " " + timeout.ToString() +
                    " " + distance.ToString() +
                    " " + move.ToString() +
                    " " + peekmode.ToString());

                ReadPort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        
    }
}
