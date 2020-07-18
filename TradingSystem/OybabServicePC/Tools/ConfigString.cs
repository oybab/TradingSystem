using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oybab.Res;
using Oybab.Res.Exceptions;

namespace Oybab.ServicePC.Pattern
{
    /// <summary>
    /// 配置文字
    /// </summary>
    internal sealed class ConfigString
    {
        private static ConfigString configString = null;
        private ConfigString() { }
        internal static ConfigString GetConfigString()
        {
            if (null == configString)
                configString = new ConfigString();
            return configString;
        }
        internal void Config()
        {
            try
            {
                KryptonManager.Strings.Abort = Resources.GetRes().GetString("Abort");
                KryptonManager.Strings.Cancel = Resources.GetRes().GetString("Cancel");
                KryptonManager.Strings.Close = Resources.GetRes().GetString("Close");
                KryptonManager.Strings.Ignore = Resources.GetRes().GetString("Ignore");
                KryptonManager.Strings.No = Resources.GetRes().GetString("No");
                KryptonManager.Strings.OK = Resources.GetRes().GetString("OK");
                KryptonManager.Strings.Yes = Resources.GetRes().GetString("Yes");
                KryptonManager.Strings.Retry = Resources.GetRes().GetString("Retry");
            }
            catch (Exception ex)
            {
                throw new OybabException(ex);
            }
        }
    }
}
