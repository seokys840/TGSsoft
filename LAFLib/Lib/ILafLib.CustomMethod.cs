
namespace LAFLib
{ 
    public partial interface ILafLib
    {
        void Func_FLaser();
        bool Func_Shot_L(int Offset, int Tolerance, int TimeOut = 2000);
        void Func_AutoFocusStop();
        bool Func_Readfocus(double foctol, double cogum);
        bool Func_IsRunning();
        //void Func_Jump(int pos);
    }
}
