using System;
using System.Collections.Generic;
using System.Text;

namespace LAFLib
{
    public partial interface ILafLib
    {
        void Motor_motionreset();
        void Motor_motionstop();
        void Motor_Move(int relative, int step);
        void Motor_ForcedMove(int relative, int step, int maxspeed);
        void Motor_ConCurrentMove(int relative, int step);
        void Motor_CatchMove(int relative, int steps, int maxspeed, int percentage);
        void Motor_SurfaceMove(int cogError, int timeout, int distance, int move, int peekmode);
    }
}
