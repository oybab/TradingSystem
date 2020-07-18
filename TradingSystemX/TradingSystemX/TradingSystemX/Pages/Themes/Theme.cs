using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages.Themes
{
    public class Theme
    {


        #region Instance
        private Theme() { }

        private static readonly Lazy<Theme> _instance = new Lazy<Theme>(() => new Theme());
        public static Theme Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion Instance



        public ThemeMode CurrentTheme { get; set; } = ThemeMode.Light;

        /// <summary>
        /// 更改风格
        /// </summary>
        /// <param name="mode"></param>
        public void ChangeTheme(ThemeMode mode)
        {
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                
                if (mode == ThemeMode.Light && CurrentTheme != ThemeMode.Light)
                {
                    mergedDictionaries.Clear();
                    mergedDictionaries.Add(new LightTheme());
                    CurrentTheme = ThemeMode.Light;
                }
                else if (mode == ThemeMode.Dard && CurrentTheme != ThemeMode.Dard)
                {
                    mergedDictionaries.Clear();
                    mergedDictionaries.Add(new DarkTheme());
                    CurrentTheme = ThemeMode.Dard;
                }
            }
        }
    }
}
