using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.Tools
{
    /// <summary>
    /// 操作系统
    /// </summary>
    public static class OSCheck
    {
        /// <summary>
        /// 获取操作系统类型
        /// </summary>
        /// <returns></returns>
        public static string GetOSType()
        {
            if (Environment.Is64BitOperatingSystem)
                return "64";
            else
                return "32";
        }

        /// <summary>
        /// 获取操作系统
        /// </summary>
        /// <returns></returns>
        public static string GetOS()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32S:
                    return "Win 3.1";
                case PlatformID.Win32Windows:
                    switch (Environment.OSVersion.Version.Minor)
                    {
                        case 0:
                            return "Win95";
                        case 10:
                            return "Win98";
                        case 90:
                            return "WinME";
                    }
                    break;

                case PlatformID.Win32NT:
                    switch (ComputerInfo.WinMajorVersion)
                    {
                        case 3:
                            return "NT 3.51";
                        case 4:
                            return "NT 4.0";
                        case 5:
                            switch (ComputerInfo.WinMinorVersion)
                            {
                                case 0:
                                    return "Win2000";
                                case 1:
                                    return "WinXP";
                                case 2:
                                    return "Win2003";
                            }
                            break;

                        case 6:
                            switch (ComputerInfo.WinMinorVersion)
                            {
                                case 0:
                                    switch (ComputerInfo.IsServer)
                                    {
                                        case 0:
                                            return "Vista";
                                        case 1:
                                            return "Win2008Server";
                                    }
                                    return "Vista/Win2008Server";
                                case 1:
                                    switch (ComputerInfo.IsServer)
                                    {
                                        case 0:
                                            return "Win7";
                                        case 1:
                                            return "Win2008Server R2";
                                    }
                                    return "Win7/Win2008Server R2";
                                case 2:
                                    switch (ComputerInfo.IsServer)
                                    {
                                        case 0:
                                            return "Win8";
                                        case 1:
                                            return "Win2012Server";
                                    }
                                    return "Win8/Win2012Server";
                                case 3:
                                    switch (ComputerInfo.IsServer)
                                    {
                                        case 0:
                                            return "Win8.1";
                                        case 1:
                                            return "Win2012Server R2";
                                    }
                                    return "Win8.1/Win2012Server R2";
                            }
                            break;
                        case 10:
                            switch (ComputerInfo.WinMinorVersion)
                            {
                                case 0:
                                    switch (ComputerInfo.IsServer)
                                    {
                                        case 0:
                                            return "Win10 " + ComputerInfo.WinCurrentBuildNumber;
                                        case 1:
                                            return "Win2016Server " + ComputerInfo.WinCurrentBuildNumber;
                                    }
                                    return "Win10/Win2016Server " + ComputerInfo.WinCurrentBuildNumber;
                                default:
                                    break;
                            }
                            break;
                    }
                    break;

                case PlatformID.WinCE:
                    return "Win CE";
            }

            return string.Format("?. P:{0}. Ma:{1}. Mi:{2}. S:{3}. Rb:{4}", Environment.OSVersion.Platform, ComputerInfo.WinMajorVersion, ComputerInfo.WinMinorVersion, ComputerInfo.IsServer, ComputerInfo.WinCurrentBuildNumber);
        }




        private static class ComputerInfo
        {
            /// <summary>
            ///     Returns the Windows major version number for this computer.
            /// </summary>
            internal static uint WinMajorVersion
            {
                get
                {
                    dynamic major;
                    // The 'CurrentMajorVersionNumber' string value in the CurrentVersion key is new for Windows 10, 
                    // and will most likely (hopefully) be there for some time before MS decides to change this - again...
                    if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMajorVersionNumber", out major))
                    {
                        return (uint)major;
                    }

                    // When the 'CurrentMajorVersionNumber' value is not present we fallback to reading the previous key used for this: 'CurrentVersion'
                    dynamic version;
                    if (!TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out version))
                        return 0;

                    var versionParts = ((string)version).Split('.');
                    if (versionParts.Length != 2) return 0;
                    uint majorAsUInt;
                    return uint.TryParse(versionParts[0], out majorAsUInt) ? majorAsUInt : 0;
                }
            }

            /// <summary>
            /// Return release and build No
            /// </summary>
            internal static string WinCurrentBuildNumber
            {
                get
                {
                    dynamic release;
                    if (!TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", out release))
                        release = "?";

                    dynamic build;
                    if (!TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuildNumber", out build))
                        build = "?";

                    return release + " " + build;
                }
            }

            /// <summary>
            ///     Returns the Windows minor version number for this computer.
            /// </summary>
            internal static uint WinMinorVersion
            {
                get
                {
                    dynamic minor;
                    // The 'CurrentMinorVersionNumber' string value in the CurrentVersion key is new for Windows 10, 
                    // and will most likely (hopefully) be there for some time before MS decides to change this - again...
                    if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMinorVersionNumber",
                        out minor))
                    {
                        return (uint)minor;
                    }

                    // When the 'CurrentMinorVersionNumber' value is not present we fallback to reading the previous key used for this: 'CurrentVersion'
                    dynamic version;
                    if (!TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out version))
                        return 0;

                    var versionParts = ((string)version).Split('.');
                    if (versionParts.Length != 2) return 0;
                    uint minorAsUInt;
                    return uint.TryParse(versionParts[1], out minorAsUInt) ? minorAsUInt : 0;
                }
            }

            /// <summary>
            ///     Returns whether or not the current computer is a server or not.
            /// </summary>
            internal static uint IsServer
            {
                get
                {
                    dynamic installationType;
                    if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "InstallationType",
                        out installationType))
                    {
                        return (uint)(installationType.Equals("Client") ? 0 : 1);
                    }

                    return 0;
                }
            }

            private static bool TryGetRegistryKey(string path, string key, out dynamic value)
            {
                value = null;
                try
                {
                    var rk = Registry.LocalMachine.OpenSubKey(path);
                    if (rk == null) return false;
                    value = rk.GetValue(key);
                    return value != null;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
