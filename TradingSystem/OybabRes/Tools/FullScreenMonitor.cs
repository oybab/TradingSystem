using Newtonsoft.Json;
using Oybab.Res.Exceptions;
using Oybab.Res.View.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Oybab.Res.Tools
{
    public sealed class FullScreenMonitor
    {
        #region Instance
        private FullScreenMonitor() { }
        private static readonly Lazy<FullScreenMonitor> lazy = new Lazy<FullScreenMonitor>(() => new FullScreenMonitor());
        public static FullScreenMonitor Instance { get { return lazy.Value; } }
        #endregion Instance

        private string WebComponentPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BsUI");
        private WebBrowser _webBrowser = new WebBrowser();
        private int SecondMoniterIndex = -1;
        private int _lastLangIndex = -1;
        private List<string> _imgList = new List<string>();

        JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
        {
            DefaultValueHandling = DefaultValueHandling.Include,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Include,
        };


        public bool _isInitialized { internal set; get; } = false;



        /// <summary>
        /// 刷新第二屏幕数据
        /// </summary>
        public void RefreshSecondMonitorList(BillModel model)
        {
            if (!_isInitialized) return;

            _webBrowser.Document.InvokeScript("ChangeListAndPriceInfo", new object[] { JsonConvert.SerializeObject(model, jsonSettings) });
        }



        /// <summary>
        /// 刷新第二屏幕语言(语言索引, 模式索引:0销售,1进货)
        /// </summary>
        public void RefreshSecondMonitorLanguage(int langIndex, int modeIndex)
        {
            if (!_isInitialized) return;

            if (_lastLangIndex == langIndex)
                return;

            _lastLangIndex = langIndex;
            _webBrowser.Document.InvokeScript("ChangeLanguageInfo", new object[] { langIndex, modeIndex });
        }


        /// <summary>
        /// 初始化第二屏幕
        /// </summary>
        public void Initial(int Left, int Top, int Width, int Height)
        {
            // 只有一个屏幕就没必要显示对吧
            if (!Resources.GetRes().DisplaySecondMonitor || System.Windows.Forms.SystemInformation.MonitorCount <= 1 || _isInitialized)
                return;


            // 从服务端获取图片地址集合
            
            string ServerImgListPath0 = Path.Combine(Resources.GetRes().ROOT, Resources.GetRes().ROOT_FOLDER, Resources.GetRes().BARS_FOLDER);
#if DEBUG
            //ServerImgListPath0 = @"E:\Project\Resources\TradingSystem\共享的图\Restaurant\2\bars";
#endif

            // 本地文件夹也存在(不连接服务器共享的场景)
            if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Resources.GetRes().ROOT_FOLDER, Resources.GetRes().BARS_FOLDER)))
                ServerImgListPath0 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Resources.GetRes().ROOT_FOLDER, Resources.GetRes().BARS_FOLDER);

            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(ServerImgListPath0);
            List<FileInfo> files = new List<FileInfo>();

            try
            {
                files = dirInfo.EnumerateFiles().ToList();
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }

            foreach (var item in files)
            {
                if (item.Extension.Equals(".jpg", StringComparison.CurrentCultureIgnoreCase) || item.Extension.Equals(".png", StringComparison.CurrentCultureIgnoreCase) || item.Extension.Equals(".gif", StringComparison.CurrentCultureIgnoreCase))
                    _imgList.Add(item.FullName);
            }


            // 现在初始化屏幕
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromRectangle(new System.Drawing.Rectangle(Left, Top, Width, Height));



            if (System.Windows.Forms.Screen.AllScreens[0].Equals(screen))
                SecondMoniterIndex = 1;
            else
                SecondMoniterIndex = 0;

            System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.AllScreens[SecondMoniterIndex].WorkingArea;

            Form _fullScreenMonitorWindow = new Form();



            _fullScreenMonitorWindow.SuspendLayout();

            _fullScreenMonitorWindow.Location = new System.Drawing.Point(workingArea.Left, workingArea.Top);

            _fullScreenMonitorWindow.Width = workingArea.Width;
            _fullScreenMonitorWindow.Height = workingArea.Height;

            _fullScreenMonitorWindow.StartPosition = FormStartPosition.Manual;
            _fullScreenMonitorWindow.FormBorderStyle = FormBorderStyle.None;
            _fullScreenMonitorWindow.WindowState = FormWindowState.Maximized;
            

            _webBrowser.Dock = DockStyle.Fill;
            //_webBrowser.Location = new System.Drawing.Point(0, 0);
            //_webBrowser.Width = 1024;
            //_webBrowser.Height = 768;

            _fullScreenMonitorWindow.Controls.Add(this._webBrowser);

            _fullScreenMonitorWindow.Enabled = false;
            _fullScreenMonitorWindow.TopMost = true;
            
            _fullScreenMonitorWindow.ShowInTaskbar = false;


            _fullScreenMonitorWindow.ResumeLayout(false);




            _fullScreenMonitorWindow.Shown += (x, y) =>
            {
                if (!_isShowed)
                {
                     _isShowed = true;
                    InitialAll();
                   
                }
            };

            _fullScreenMonitorWindow.Show();



        }
        private bool _isShowed = false;





        private void InitialAll()
        {

            string content = GetResourceFileContentAsString("index.html");

            content = content.Replace("@@@@@", string.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" ><link href=\"{1}\" type=\"text/css\"  rel=\"stylesheet\" /><style type=\"text/css\" > {2}</style>", System.IO.Path.Combine(WebComponentPath, @"bootstrap\css\bootstrap.css"), System.IO.Path.Combine(WebComponentPath, @"bootstrap\css\bootstrap-theme.css"), GetResourceFileContentAsString("index.css")));
            


            content = content.Replace("#####", string.Format("<script type=\"text/javascript\" src=\"{0}\"></script><script type=\"text/javascript\" src=\"{1}\" ></script> <script type=\"text/javascript\" > {2}</script>", System.IO.Path.Combine(WebComponentPath, @"js/jquery-1.12.4.min.js"), System.IO.Path.Combine(WebComponentPath, @"bootstrap/js/bootstrap.js"), GetResourceFileContentAsString("index.js")));

            ((Control)_webBrowser).Enabled = false;
            _webBrowser.WebBrowserShortcutsEnabled = false;
            _webBrowser.IsWebBrowserContextMenuEnabled = false;
            _webBrowser.ScriptErrorsSuppressed = true;
            _webBrowser.WebBrowserShortcutsEnabled = false;
            _webBrowser.AllowWebBrowserDrop = false;


            _webBrowser.DocumentText = content;



            _webBrowser.DocumentCompleted -= WebBrowser1_DocumentCompleted;
            _webBrowser.DocumentCompleted += WebBrowser1_DocumentCompleted;
        }



        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
            // 导入初始化参数和3种语言

            // 初始化数据
            string infoJson = GetJsonByInfo();

            // 语言:中文
            string li0Json = GetJsonByLang(Resources.GetRes().GetMainLangByMainLangIndex(0).Culture, Resources.GetRes().GetMainLangByMainLangIndex(0).MainLangIndex);
            // 语言:维文
            string li1Json = GetJsonByLang(Resources.GetRes().GetMainLangByMainLangIndex(1).Culture, Resources.GetRes().GetMainLangByMainLangIndex(1).MainLangIndex);
            // 语言:英文
            string li2Json = GetJsonByLang(Resources.GetRes().GetMainLangByMainLangIndex(2).Culture, Resources.GetRes().GetMainLangByMainLangIndex(2).MainLangIndex);

            _webBrowser.Document.InvokeScript("LoadAllInfo", new object[] { infoJson, li0Json, li1Json, li2Json });


            _isInitialized = true;

        }

        /// <summary>
        /// 根据语言返回语言模型
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        private string GetJsonByLang(CultureInfo ci, int langIndex)
        {
            LanguageInfo li = new LanguageInfo();
            li.ActualPrice = Resources.GetRes().GetString("ActualPrice", ci);
            li.BorrowPrice = Resources.GetRes().GetString("BorrowPrice", ci);
            li.CardPaidPrice = Resources.GetRes().GetString("CardPaidPrice", ci);
            li.Count = Resources.GetRes().GetString("Count", ci);
            li.DealsPrice = Resources.GetRes().GetString("DealsPrice", ci);
            li.Id = Resources.GetRes().GetString("Id", ci);
            li.MemberPaidPrice = Resources.GetRes().GetString("MemberPaidPrice", ci);
            li.MemberDealsPrice = Resources.GetRes().GetString("MemberDealsPrice", ci);
            li.Member = Resources.GetRes().GetString("Member", ci);
            li.PaidPrice = Resources.GetRes().GetString("PaidPrice", ci);
            li.Price = Resources.GetRes().GetString("Price", ci);
            li.ProductName = Resources.GetRes().GetString("ProductName", ci);
            li.ReturnPrice = Resources.GetRes().GetString("ReturnPrice", ci);
            li.RoomNo = Resources.GetRes().GetString("RoomNo", ci);
            li.RoomPrice = Resources.GetRes().GetString("RoomPrice", ci);
            li.SelectedCount = Resources.GetRes().GetString("SelectedCount", ci);
            li.SoftwareName = Resources.GetRes().GetString("SoftServiceName", ci);
            li.TotalPrice = Resources.GetRes().GetString("TotalPrice", ci);
            li.TotalTime = Resources.GetRes().GetString("TotalTime", ci);
            li.FontName =  Resources.GetRes().GetString("FontName", ci);
            li.LangIndex = langIndex;
            li.Culcure =  Resources.GetRes().GetString("Culcure", ci);
            li.Dir = Resources.GetRes().GetString("Dir", ci);
            return JsonConvert.SerializeObject(li, jsonSettings);
        }


        /// <summary>
        /// 根据信息获取JSON
        /// </summary>
        /// <returns></returns>
        private string GetJsonByInfo()
        {
            InitializationInfo info = new InitializationInfo();

            info.OwnerName0 = Resources.GetRes().KEY_NAME_0;
            info.OwnerName1 = Resources.GetRes().KEY_NAME_1;
            info.OwnerName2 = Resources.GetRes().KEY_NAME_2;
            info.PriceSymbol = Resources.GetRes().PrintInfo.PriceSymbol;
            info.imgList0 = _imgList.Select(x => x.Replace("\\", "\\\\")).ToArray();
            info.imgList1 = null;
            _lastLangIndex = info.LangIndex = Resources.GetRes().MainLangIndex;
            info.ModeIndex = 0;
            info.ThemeIndex = 0;

            return JsonConvert.SerializeObject(info, jsonSettings);
        }


        /// <summary>
        /// 增加CSS
        /// </summary>
        /// <param name="file"></param>
        private void AddStyle(string file, string content)
        {

            HtmlElement head = _webBrowser.Document.GetElementsByTagName("head")[0];
            if (head != null)
            {
                HtmlElement elm = _webBrowser.Document.CreateElement("style");

                elm.SetAttribute("type", "text/css");


                if (null != file)
                    elm.SetAttribute("text", System.IO.File.ReadAllText(file));
                else if (null != content)
                    elm.SetAttribute("text", content);

                head.AppendChild(elm);
            }
        }

        /// <summary>
        /// 增加JS
        /// </summary>
        /// <param name="file"></param>
        private void AddScript(string file, string content)
        {
            HtmlElement body = _webBrowser.Document.GetElementsByTagName("body")[0];

            if (body != null)
            {
                HtmlElement elm = _webBrowser.Document.CreateElement("script");
                elm.SetAttribute("type", "text/javascript");

                if (null != file)
                    elm.SetAttribute("text", System.IO.File.ReadAllText(file));
                else if (null != content)
                    elm.SetAttribute("text", content);

                body.AppendChild(elm);
            }
        }

        /// <summary>
        /// 从当前资源读取文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetResourceFileContentAsString(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Oybab.Res.Resources.BsUI." + fileName;

            string resource = null;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    resource = reader.ReadToEnd();
                }
            }
            return resource;
        }

       

    }


    internal sealed class InitializationInfo
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "imgList0")]
        public string[] imgList0 { get; set; } // 模式0图片集合
        [Newtonsoft.Json.JsonProperty(PropertyName = "imgList1")]
        public string[] imgList1 { get; set; } // 模式1图片集合
        [Newtonsoft.Json.JsonProperty(PropertyName = "OwnerName0")]
        public string OwnerName0 { get; set; } // 拥有者名(中文)
        [Newtonsoft.Json.JsonProperty(PropertyName = "OwnerName1")]
        public string OwnerName1 { get; set; } // 拥有者名(少数民族文)
        [Newtonsoft.Json.JsonProperty(PropertyName = "OwnerName2")]
        public string OwnerName2 { get; set; } // 拥有者名(英文)
        [Newtonsoft.Json.JsonProperty(PropertyName = "PriceSymbol")]
        public string PriceSymbol { get; set; } // 钱符号
         [Newtonsoft.Json.JsonProperty(PropertyName = "LangIndex")]
        public int LangIndex { get; set; } // 语言索引编号,0中文1少数民族文2英文
        [Newtonsoft.Json.JsonProperty(PropertyName = "ThemeIndex")]
        public int ThemeIndex { get; set; } // 风格索引,0亮1暗
        [Newtonsoft.Json.JsonProperty(PropertyName = "ModeIndex")]
        public int ModeIndex { get; set; } // 模式索引,0详细1全屏
    }


    internal sealed class LanguageInfo
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "SoftwareName")]
        public string SoftwareName { get; set; } // 软件名
        [Newtonsoft.Json.JsonProperty(PropertyName = "Id")]
        public string Id { get; set; } // 编号
        [Newtonsoft.Json.JsonProperty(PropertyName = "ProductName")]
        public string ProductName { get; set; } // 产品名称
        [Newtonsoft.Json.JsonProperty(PropertyName = "Count")]
        public string Count { get; set; } // 数量
        [Newtonsoft.Json.JsonProperty(PropertyName = "Price")]
        public string Price { get; set; } // 单价
        [Newtonsoft.Json.JsonProperty(PropertyName = "TotalPrice")]
        public string TotalPrice { get; set; } // 总价
        [Newtonsoft.Json.JsonProperty(PropertyName = "Member")]
        public string Member { get; set; } // 会员
        [Newtonsoft.Json.JsonProperty(PropertyName = "MemberDealsPrice")]
        // This is Obsolete
        public string MemberDealsPrice { get; set; } // 会员优惠价格
        [Newtonsoft.Json.JsonProperty(PropertyName = "DealsPrice")]
        // This is Obsolete
        public string DealsPrice { get; set; } // 优惠价格
        [Newtonsoft.Json.JsonProperty(PropertyName = "ActualPrice")]
        // This is Obsolete
        public string ActualPrice { get; set; } // 实际价格
        [Newtonsoft.Json.JsonProperty(PropertyName = "MemberPaidPrice")]
        public string MemberPaidPrice { get; set; } // 会员支付金额
        [Newtonsoft.Json.JsonProperty(PropertyName = "PaidPrice")]
        public string PaidPrice { get; set; } // 现金
        [Newtonsoft.Json.JsonProperty(PropertyName = "CardPaidPrice")]
        // This is Obsolete
        public string CardPaidPrice { get; set; } // 刷卡
        [Newtonsoft.Json.JsonProperty(PropertyName = "ReturnPrice")]
        // This is Obsolete
        public string ReturnPrice { get; set; } // 退还金额
        [Newtonsoft.Json.JsonProperty(PropertyName = "BorrowPrice")]
        public string BorrowPrice { get; set; } // 欠款金额
        [Newtonsoft.Json.JsonProperty(PropertyName = "RoomNo")]
        public string RoomNo { get; set; } // 包厢编号
        [Newtonsoft.Json.JsonProperty(PropertyName = "RoomPrice")]
        public string RoomPrice { get; set; } // 包厢价格
        [Newtonsoft.Json.JsonProperty(PropertyName = "TotalTime")]
        public string TotalTime { get; set; } // 包厢所用总时间
        [Newtonsoft.Json.JsonProperty(PropertyName = "SelectedCount")]
        public string SelectedCount { get; set; } // 已选
        [Newtonsoft.Json.JsonProperty(PropertyName = "FontName")]
        public string FontName { get; set; } // 字名
        [Newtonsoft.Json.JsonProperty(PropertyName = "LangIndex")]
        public int LangIndex { get; set; } // 语言索引编号
        [Newtonsoft.Json.JsonProperty(PropertyName = "Culcure")]
        public string Culcure { get; set; } // 语言
        [Newtonsoft.Json.JsonProperty(PropertyName = "Dir")]
        public string Dir { get; set; } // 文字方向

    }



}
