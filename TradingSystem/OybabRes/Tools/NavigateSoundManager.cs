using Oybab.Res.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.Tools
{
    public class NavigateSoundManager
    {

        #region Instance
        private NavigateSoundManager() { }
        private static readonly Lazy<NavigateSoundManager> lazy = new Lazy<NavigateSoundManager>(() => new NavigateSoundManager());
        public static NavigateSoundManager Instance { get { return lazy.Value; } }
        #endregion Instance


        /// <summary>
        /// 关闭声音
        /// </summary>
        public void DisableSound()
        {
            try
            {
                string keyValue = null;
                keyValue = "%SystemRoot%\\Media\\";
                if (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor > 0)
                {
                    keyValue += "Windows XP Start.wav";
                }
                else if (Environment.OSVersion.Version.Major >= 6)
                {
                    keyValue += "Windows Navigation Start.wav";
                }
                else
                {
                    return;
                }
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("AppEvents\\Schemes\\Apps\\Explorer\\Navigating\\.Current", true);
                key.SetValue(null, "", Microsoft.Win32.RegistryValueKind.ExpandString);
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }

        /// <summary>
        /// 开启声音
        /// </summary>
        public void EnableSound()
        {
            try
            {
                string keyValue = null;
                keyValue = "%SystemRoot%\\Media\\";
                //xp and 2003
                if (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor > 0)
                {
                    keyValue += "Windows XP Start.wav";
                }
                else if (Environment.OSVersion.Version.Major >= 6)
                {
                    keyValue += "Windows Navigation Start.wav";
                }
                else
                {
                    return;
                }
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("AppEvents\\Schemes\\Apps\\Explorer\\Navigating\\.Current", true);
                key.SetValue(null, keyValue, Microsoft.Win32.RegistryValueKind.ExpandString);
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }
    }
}
