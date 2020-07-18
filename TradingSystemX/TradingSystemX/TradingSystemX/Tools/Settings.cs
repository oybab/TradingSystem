using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Oybab.TradingSystemX.Tools
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    internal class Settings
    {


        #region Instance
        private Settings() { }

        private static readonly Lazy<Settings> _instance = new Lazy<Settings>(() => new Settings());
        public static Settings Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion Instance






        #region Setting Constants

        internal const string SettingsLangKey = "OYBAB_LANG_INDEX";
        internal readonly int SettingsLangDefault = -1;

        internal const string SettingsAdminKey = "OYBAB_ADMIN_NO";
        internal readonly string SettingsAdminDefault = string.Empty;

        internal const string SettingsPasswordKey = "OYBAB_PASSWORD_NO";
        internal readonly string SettingsPasswordDefault = string.Empty;

        internal const string SettingsIsSavePassword = "OYBAB_ISSAVEPASSWORD";
        internal readonly bool SettingsIsSavePasswordDefault = false;

        internal const string SettingsIPKey = "OYBAB_SERVER_IP";
        internal readonly string SettingsIPDefault = "192.168.1.200";

        #endregion


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal async Task<string> GetValueOrDefault(string key, string defaultValue)
        {
            try
            {
                string value = await SecureStorage.GetAsync(key);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return defaultValue;
                }
                else
                {
                    return value;
                }
            }
            catch
            {
                return defaultValue;
            }
        }


        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal async Task<bool> AddOrUpdateValue(string key, string value)
        {
            try
            {
                if (null != value)
                {
                    await SecureStorage.SetAsync(key, value);
                    return true;
                }
                else
                    return SecureStorage.Remove(key);
            }
            catch
            {
                return false;
            }
            
        }



        

    }
}
