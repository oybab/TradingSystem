using Oybab.Res.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace Oybab.TradingSystemX.Tools
{
    internal sealed class Restore
    {
        #region Instance
        private Restore() { }
        private static readonly Lazy<Restore> lazy = new Lazy<Restore>(() => new Restore());
        public static Restore Instance { get { return lazy.Value; } }
        #endregion Instance


        /// <summary>
        ///  读取备份(读取完配置文件后)
        /// </summary>
        internal async Task ReadBak()
        {
           
                try
                {
                    string langIndex = await Settings.Instance.GetValueOrDefault(Settings.SettingsLangKey, Settings.Instance.SettingsLangDefault.ToString());

                    if (langIndex == "-1")
                    {
                        var currentLang = Res.Instance.AllLangList.Where(x => x.Value.Culture.Name == System.Globalization.CultureInfo.CurrentCulture.Name).Select(x => x.Value).FirstOrDefault();
                        if (null != currentLang)
                            langIndex = currentLang.LangIndex.ToString();
                        else
                            langIndex = "2"; // default english
                    }

                    Resources.Instance.ReloadResources(int.Parse(langIndex));
                    Resources.Instance.LastLoginAdminNo = await Settings.Instance.GetValueOrDefault(Settings.SettingsAdminKey, Settings.Instance.SettingsAdminDefault.ToString());
                    Resources.Instance.LastLoginPassword = await Settings.Instance.GetValueOrDefault(Settings.SettingsPasswordKey, Settings.Instance.SettingsPasswordDefault.ToString());
                    Resources.Instance.IsSavePassword = bool.Parse(await Settings.Instance.GetValueOrDefault(Settings.SettingsIsSavePassword, Settings.Instance.SettingsIsSavePasswordDefault.ToString()));
                    Resources.Instance.SERVER_ADDRESS = await Settings.Instance.GetValueOrDefault(Settings.SettingsIPKey, Settings.Instance.SettingsIPDefault.ToString());
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex);
                }
            

        }


        /// <summary>
        /// 写入备份
        /// </summary>
        internal async Task WriteBak()
        {

            try
            {
                await Settings.Instance.AddOrUpdateValue(Settings.SettingsLangKey, Res.Instance.CurrentLangIndex.ToString());
                await Settings.Instance.AddOrUpdateValue(Settings.SettingsAdminKey, Resources.Instance.LastLoginAdminNo);
                await Settings.Instance.AddOrUpdateValue(Settings.SettingsPasswordKey, Resources.Instance.LastLoginPassword);
                await Settings.Instance.AddOrUpdateValue(Settings.SettingsIsSavePassword, Resources.Instance.IsSavePassword.ToString());
                await Settings.Instance.AddOrUpdateValue(Settings.SettingsIPKey, Resources.Instance.SERVER_ADDRESS);


            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }






      
    }
}
