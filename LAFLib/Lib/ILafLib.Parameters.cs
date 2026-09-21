using System;
using System.Collections.Generic;
using System.Text;

namespace LAFLib
{
    public partial interface ILafLib
    {
        void Restart();
        void Save(int LensNum);
        void Wait(long value);
        int ReadPara_Report(string _para);
        int ReadPara_Report2(string _para);
        void SetMode(int mode);
        int GetMode();
        
        void SetSurface(int mode);
        int GetSurface();

        void SetPara_SetLens(int LensNum);
        int SetPara_ChangeLens(int Num);
        void SetPara_Magnification(int LensNum, int value);
        int GetPara_Magnification(int LensNum);
        void SetPara_Calibration();
        void SetPara_pmlimit(int value);
        void SetPara_dacgain(int value);
        void SetPara_lasergate(int value);
        void SetPara_sensorgain(int value);
        void SetPara_maxlaseron(int value);
        void SetPara_laserlineslant(int value);
        void SetPara_AutomaticROI(int value);
        void SetPara_ywindows(int start, int height);
        void SetPara_xwindow(int start, int width);
        void SetPara_vcogcenter(int value);
        void SetPara_peakmode(int value);
        void SetPara_motiontrack(int value);
        void SetPara_motionenable(int value);
        void SetPara_limitsw1(int value);
        void SetPara_limitsw2(int value);
        void SetPara_motionresetlimit(int sensor, int para);
        void SetPara_motionloopgain(int gain);
        void SetPara_actorlatency(int value);
        void SetPara_cogperstepls16(int value);
        void SetPara_motionacctcms(int value);
        void SetPara_motionstartstop(int value);
        void SetPara_motionmaxspeed(int value);
        void SetPara_motionpluslimit(int value);
        void SetPara_motionminuslimit(int value);
        void SetPara_maxretdb(int value);
        void SetPara_MotionZero();
        void SetPara_Motionrefpos(int value);
        void SetPara_Motiontimeout(int value);
        void SetPara_Motiondeadzone(int value);
        void SetPara_Monitortrackingerror(int error, int timeout);
        void SetPara_minretdb(int value);
        void SetPara_Stepspermm(int value);
        int GetPara_Stepspermm();
        double GetPara_CogperStep();
        void SetPara_Motionfoctol(int value);       //2025.9.15
    }
}
