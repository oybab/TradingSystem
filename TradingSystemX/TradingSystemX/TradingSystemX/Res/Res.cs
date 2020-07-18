using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;

namespace Oybab.TradingSystemX
{
    public sealed class Res
    {

        private CultureInfo ci = null;
        private ResourceManager rm = null;



        #region Instance
        /// <summary>
        /// For _instance
        /// </summary>
        private static Res _instance;
        private Res()
        {

        }
        public static Res Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Res();
                return _instance;
            }
        }
        #endregion




        // 当前语言的索引
        private int _currentLangIndex = 2; // default en-US
        public int CurrentLangIndex
        {
            internal set
            {
                _currentLangIndex = value;
            }
            get { return _currentLangIndex; }
        }


        public Dictionary<int, Lang> AllLangList { private set; get; } = new Dictionary<int, Lang>();

        public Dictionary<int, Lang> MainLangList { private set; get; } = new Dictionary<int, Lang>();

        public class Lang
        {
            public CultureInfo Culture { get; set; }
            public int LangIndex { get; set; }
            public int MainLangIndex { get; set; } = -1;
            public string LangName { get; set; }
        }



        /// <summary>
        /// 初始化所有语言
        /// </summary>
        private void InitialAllLang()
        {
            int orderIndex = 0;
            AllLangList.Clear();


            // zh-CN
            CultureInfo ci = new CultureInfo("zh-CN");
            AllLangList.Add(orderIndex, new Lang() { Culture = ci, LangIndex = orderIndex, LangName = GetString("LangName", ci) }); // index 0


            // ug-CN
            ++orderIndex;
            ci = new CultureInfo("ug-CN");
            AllLangList.Add(orderIndex, new Lang() { Culture = ci, LangIndex = orderIndex, LangName = GetString("LangName", ci) }); // index 1


            //en-US
            ++orderIndex;
            ci = new CultureInfo("en-US");
            AllLangList.Add(orderIndex, new Lang() { Culture = ci, LangIndex = orderIndex, LangName = GetString("LangName", ci) }); // index 2


        }

        /// <summary>
        /// 初始化主要语言
        /// </summary>
        internal void InitialMainLang(string mainLangLists)
        {
            int orderIndex = 0;
            MainLangList.Clear();
            foreach (var item in mainLangLists.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                int index = int.Parse(item);
                CultureInfo ci = AllLangList[index].Culture;
                MainLangList.Add(index, new Lang() { Culture = ci, LangIndex = index, MainLangIndex = orderIndex, LangName = GetString("LangName", ci) }); // index 0  
                ++orderIndex;
            }

            MainLangIndex = CurrentLangIndex;
        }


        private int _mainLangIndex;
        /// <summary>
        /// 获取主语言索引
        /// </summary>
        public int MainLangIndex
        {
            internal set
            {

                if (MainLangList.ContainsKey(CurrentLangIndex))
                    _mainLangIndex = MainLangList[CurrentLangIndex].MainLangIndex;
                // 否则返回主语言索引
                else
                    _mainLangIndex = MainLangList[0].MainLangIndex;
            }
            get
            {
                return _mainLangIndex;
            }
        }

        /// <summary>
        /// 主语言名称
        /// </summary>
        public Lang MainLang
        {
            internal set { }
            get
            {
                return MainLangList.FirstOrDefault(x => x.Value.MainLangIndex == MainLangIndex).Value;
            }
        }


        /// <summary>
        /// 根据语言名返回索引
        /// </summary>
        /// <param name="LangName"></param>
        /// <returns></returns>
        public Lang GetLangByLangName(string LangName)
        {
            if (AllLangList.Any(x => x.Value.LangName == LangName))
                return AllLangList.FirstOrDefault(x => x.Value.LangName == LangName).Value;
            else
                return null;
        }

        // 根据语言索引返回主语言
        public Lang GetLangByLangIndex(int index)
        {
            if (AllLangList.Any(x => x.Value.LangIndex == index))
            {
                return AllLangList.FirstOrDefault(x => x.Value.LangIndex == index).Value;
            }
            return null;
        }


        /// <summary>
        /// 根据主语言名返回索引
        /// </summary>
        /// <param name="LangName"></param>
        /// <returns></returns>
        public Lang GetMainLangByLangName(string LangName)
        {
            if (MainLangList.Any(x => x.Value.LangName == LangName))
                return MainLangList.FirstOrDefault(x => x.Value.LangName == LangName).Value;
            else
                return MainLang;
        }


        // 根据主语言索引返回主语言
        public Lang GetMainLangByMainLangIndex(int index)
        {
            if (MainLangList.Any(x => x.Value.MainLangIndex == index))
            {
                return MainLangList.FirstOrDefault(x => x.Value.MainLangIndex == index).Value;
            }
            return MainLangList[0];
        }
        // 对比主语言里有没有该语言, 没有, 则返回主语言第一个语言.
        public Lang GetMainLangByLangIndex(int index)
        {
            if (MainLangList.ContainsKey(index))
            {
                return MainLangList[index];
            }
            return MainLangList[0];
        }


        /// <summary>
        /// 加载
        /// </summary>
        public void LoadResources(string AllLangIndex, int LangIndex = -1)
        {

            if (AllLangList.Count == 0)
            {
                rm = new ResourceManager("Oybab.TradingSystemX.Res.Resource", typeof(Res).GetTypeInfo().Assembly);
                InitialAllLang();
            }


            if (LangIndex != -1)
            {
                Res.Instance.CurrentLangIndex = LangIndex;
                InitialMainLang(AllLangIndex);

                ci = AllLangList[CurrentLangIndex].Culture;
            }



        }

        /// <summary>
        /// 返回对应的语言
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetString(string name)
        {

            return rm.GetString(name, ci);

        }


        /// <summary>
        /// 返回对应的语言
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetString(string name, CultureInfo ci)
        {

            return rm.GetString(name, ci);

        }


        

    }
}
