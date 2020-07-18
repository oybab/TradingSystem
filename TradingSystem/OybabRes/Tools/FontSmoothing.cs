using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Oybab.Res.Exceptions;

namespace Oybab.Res.Tools
{
    /// <summary>
    /// 开启清晰字体
    /// </summary>
    public static class FontSmoothing
    {

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int pvParam, uint fWinIni);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        /* Constants used for User32 calls. */
        const uint SPI_GETFONTSMOOTHING = 74;
        const uint SPI_SETFONTSMOOTHING = 75;
        const uint SPI_GETFONTSMOOTHINGTYPE = 0x200A;
        const uint SPI_SETFONTSMOOTHINGTYPE = 0x200B;

        const uint FE_FONTSMOOTHINGCLEARTYPE = 2;
        const uint FE_FONTSMOOTHINGSTANDARTTYPE = 1;
        const uint FE_FONTSMOOTHINGNORMALTYPE = 0;

        //const uint SPIF_UPDATEINIFILE = 0x1;

        //Writes the new system-wide parameter setting to the user profile.
	    const int SPIF_UPDATEINIFILE = 1;
		//Broadcasts the WM_SETTINGCHANGE message after updating the user profile.
		const int SPIF_SENDCHANGE = 2;


        const uint SPI_GETFONTSMOOTHINGCONTRAST = 0x200C;
        const uint SPI_SETFONTSMOOTHINGCONTRAST = 0x200D;

        private static bool GetFontSmoothing()
        {
            bool iResult;
            int pv = 0;
            /* Call to systemparametersinfo to get the font smoothing value. */
            iResult = SystemParametersInfo(SPI_GETFONTSMOOTHING, 0, ref pv, 0);
            
            if (pv > 0)
            {
                //pv > 0 means font smoothing is on.
                return true;
            }
            else
            {
                //pv == 0 means font smoothing is off.
                return false;
            }
        }

        private static void DisableFontSmoothing()
        {
            bool iResult;
            int pv = 0;
            /* Call to systemparametersinfo to set the font smoothing value. */
            iResult = SystemParametersInfo(SPI_SETFONTSMOOTHING, 0, ref pv, SPIF_UPDATEINIFILE);
        }

        private static void EnableFontSmoothing()
        {
            bool iResult;
            int pv = 0;
            /* Call to systemparametersinfo to set the font smoothing value. */
            iResult = SystemParametersInfo(SPI_SETFONTSMOOTHING, 1, ref pv, SPIF_UPDATEINIFILE);
        }



        private static bool GetFontType()
        {
            bool iResult;
            int uiType = 0;

            iResult = SystemParametersInfo(SPI_GETFONTSMOOTHINGTYPE, 0, ref uiType, 0);

            if (uiType == FE_FONTSMOOTHINGCLEARTYPE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private static void UseFontNormal()
        {
            
        }

        private static void UseFontClearType()
        {
            bool iResult;
            uint pv = 0;
            //MessageBox.Show("UseFontClearType" + pv);
            iResult = SystemParametersInfo(SPI_SETFONTSMOOTHINGTYPE, pv, (IntPtr)FE_FONTSMOOTHINGCLEARTYPE, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }




        private static int GetSmoothHintConstracter()
        {
            bool iResult;
            int pv = 0;
            SystemParametersInfo(SPI_GETFONTSMOOTHING, 0, ref pv, 0);
            iResult = SystemParametersInfo(SPI_GETFONTSMOOTHINGCONTRAST, 0, ref pv,SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

            return pv;
        }

        private static void SetSmoothHintContractorNormal()
        {

        }

        private static void SetSmoothHintContractorPerfect()
        {
            bool iResult;
            uint pv = 0;
            //MessageBox.Show("SetSmoothHintContractorPerfect" + pv);
            iResult = SystemParametersInfo(SPI_SETFONTSMOOTHINGCONTRAST, pv, (IntPtr)1200, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }


        /// <summary>
        /// 打开效果
        /// </summary>
        public static void OpenFontEffect()
        {
            try
            {
                if (!GetFontSmoothing())
                {
                    EnableFontSmoothing();
                }

                if (!GetFontType())
                {
                    UseFontClearType();
                }

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }

        }
    }
}
