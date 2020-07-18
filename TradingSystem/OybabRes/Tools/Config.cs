using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Oybab.Res.Exceptions;
using System.Windows.Forms;

namespace Oybab.Res.Tools
{
    /// <summary>
    /// 语言
    /// </summary>
    public sealed class Config
    {
        private static Config config = null;
        private Config() { }

        public static Config GetConfig()
        {
            if (null == config)
                config = new Config();
            return config;
        }
           
        /// <summary>
        /// 获取语言
        /// </summary>
        public void GetConfigs(bool IsPC = false)
        {
            try
            {
                bool IsSuccessServer = false;
                bool IsSuccessDisplayCursor = false;
                bool IsSuccessDisplaySecondMonitor = false;
                //bool IsSuccessMediaRoot = false;
                bool IsSuccessSyncTime = false;
                bool IsSuccessLocalPrintCustomOrder = false;
                bool IsSuccessCashDrawer = false;
                bool IsSuccessPriceMonitor = false;
                bool IsSuccessBarcodeReader = false;
                bool IsSuccessCardReader = false;
                bool IsSuccessLang = false;

                bool IsSuccessDemands = false;
                bool IsSuccessCallDevice = false;


                //下次记得增加去掉已测试过的判断,在配置文件增加是否隐藏开始菜单
                using (StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.txt"), Encoding.UTF8))
                {
                    string line = null;
                    string temp = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        //获取服务器地址
                        if (!IsSuccessServer && line.Trim().StartsWith("Server"))
                        {
                            try
                            {
                                temp = line.Trim().Split('=')[1];
                                Resources.GetRes().SERVER_ADDRESS = temp;
                                Resources.GetRes().ROOT = @"\\" + Resources.GetRes().SERVER_ADDRESS;
                                IsSuccessServer = true;
                            }
                            catch
                            {
                                ExceptionPro.ExpInfoLog("Unknow Server param.");
                            }
                        }
                        //获取鼠标是否显示
                        else if (Resources.GetRes().DevicesType == 2 && !IsSuccessDisplayCursor && line.Trim().StartsWith("DisplayCursor"))
                        {
                            temp = line.Trim().Split('=')[1];
                            if (temp == "0")
                            {
                                Resources.GetRes().DisplayCursor = false;
                                IsSuccessDisplayCursor = true;
                            }
                            else if (temp == "1")
                            {
                                Resources.GetRes().DisplayCursor = true;
                                IsSuccessDisplayCursor = true;
                            }
                            else
                            {
                                ExceptionPro.ExpInfoLog("Unknow DisplayCursor param.");
                            }
                        }
                        // 是否显示第二屏幕
                        else if (!IsSuccessDisplaySecondMonitor && line.Trim().StartsWith("DisplaySecondMonitor"))
                        {
                            temp = line.Trim().Split('=')[1];
                            if (temp == "0")
                            {
                                Resources.GetRes().DisplaySecondMonitor = false;
                                IsSuccessDisplaySecondMonitor = true;
                            }
                            else if (temp == "1")
                            {
                                Resources.GetRes().DisplaySecondMonitor = true;
                                IsSuccessDisplaySecondMonitor = true;
                            }
                            else
                            {
                                ExceptionPro.ExpInfoLog("Unknow DisplaySecondMonitor param.");
                            }
                        }
                        //是否自动同步服务器时间到客户端
                        else if (!IsSuccessSyncTime && line.Trim().StartsWith("AutoSyncClientTime"))
                        {
                            temp = line.Trim().Split('=')[1];
                            if (temp == "0")
                            {
                                Resources.GetRes().AutoSyncClientTime = false;
                                IsSuccessSyncTime = false;

                            }
                            else if (temp == "1")
                            {
                                Resources.GetRes().AutoSyncClientTime = true;
                                IsSuccessSyncTime = true;
                            }
                            else
                            {
                                ExceptionPro.ExpInfoLog("Unknow AutoSyncClientTime param.");
                            }
                        }
                        //是否本地打印客户订单
                        else if (!IsSuccessLocalPrintCustomOrder && line.Trim().StartsWith("IsLocalPrintCustomOrder"))
                        {
                            temp = line.Trim().Split('=')[1];
                            if (temp == "0")
                            {
                                Resources.GetRes().IsLocalPrintCustomOrder = false;
                                IsSuccessLocalPrintCustomOrder = true;

                            }
                            else if (temp == "1")
                            {
                                Resources.GetRes().IsLocalPrintCustomOrder = true;
                                IsSuccessLocalPrintCustomOrder = true;
                            }
                            else
                            {
                                ExceptionPro.ExpInfoLog("Unknow IsLocalPrintCustomOrder param.");
                            }
                        }
                        //钱箱
                        else if (!IsSuccessCashDrawer && line.Trim().StartsWith("CashDrawer"))
                        {
                            try 
	                        {	        
		                        temp = line.Trim().Split('=')[1];
                                Resources.GetRes().CashDrawer = temp;
                                IsSuccessCashDrawer = true;
	                        }
	                        catch
	                        {
                                ExceptionPro.ExpInfoLog("Unknow CashDrawer param.");
	                        }
                        }
                        //客显
                        else if (!IsSuccessPriceMonitor && line.Trim().StartsWith("PriceMonitor"))
                        {
                            try
                            {
                                temp = line.Trim().Split('=')[1];
                                Resources.GetRes().PriceMonitor = temp;
                                IsSuccessPriceMonitor = true;
                            }
                            catch
                            {
                                ExceptionPro.ExpInfoLog("Unknow PriceMonitor param.");
                            }
                        }
                        //条码阅读器
                        else if (!IsSuccessBarcodeReader && line.Trim().StartsWith("BarcodeReader"))
                        {
                            try
                            {
                                temp = line.Trim().Split('=')[1];
                                Resources.GetRes().BarcodeReader = temp;
                                IsSuccessBarcodeReader = true;
                            }
                            catch
                            {
                                ExceptionPro.ExpInfoLog("Unknow BarcodeReader param.");
                            }
                        }
                        //卡片阅读器
                        else if (!IsSuccessCardReader && line.Trim().StartsWith("CardReader"))
                        {
                            try
                            {
                                temp = line.Trim().Split('=')[1];
                                Resources.GetRes().CardReader = temp;
                                IsSuccessCardReader = true;
                            }
                            catch
                            {
                                ExceptionPro.ExpInfoLog("Unknow CardReader param.");
                            }
                        }
                        // 特殊功能
                        else if (!IsSuccessDemands && line.Trim().StartsWith("Demands"))
                        {
                            try
                            {
                                temp = line.Trim().Split('=')[1];
                                List<string> tempSplitSelect = temp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                                if (tempSplitSelect.Count > 0)
                                    Resources.GetRes().Demands.AddRange(tempSplitSelect);
                                IsSuccessDemands = true;
                            }
                            catch
                            {
                                ExceptionPro.ExpInfoLog("Unknow Demands param.");
                            }
                        }
                        // 呼叫器
                        else if (!IsSuccessCallDevice && line.Trim().StartsWith("CallDevice"))
                        {
                            try
                            {
                                temp = line.Trim().Split('=')[1];
                                Resources.GetRes().CallDevice = temp;
                                IsSuccessCallDevice = true;
                            }
                            catch
                            {
                                ExceptionPro.ExpInfoLog("Unknow CallDevice param.");
                            }
                        }
                        //获取语言
                        else if (Environment.GetCommandLineArgs().Length <= 1 && !IsSuccessLang && line.Trim().StartsWith("Language"))
                        {
                            temp = line.Trim().Split('=')[1];

                            int index = int.Parse(temp);

                            if (index == -1)
                            {
                                var currentLang = Resources.GetRes().AllLangList.Where(x => x.Value.Culture.Name == System.Globalization.CultureInfo.CurrentCulture.Name).Select(x => x.Value).FirstOrDefault();
                                if (null != currentLang)
                                    index = currentLang.LangIndex;
                                else
                                    index = 2; // default english
                            }


                            Resources.GetRes().ReloadResources(index);
                            IsSuccessLang = true;

                        }
                    }
                }

                //用命令行选择语言
                if (Environment.GetCommandLineArgs().Length > 1)
                {
                    string temp = Environment.GetCommandLineArgs()[1];
                    Resources.GetRes().ReloadResources(int.Parse(temp));
                    IsSuccessLang = true;
                    
                }


                if (!IsSuccessServer)
                    ExceptionPro.ExpInfoLog("undefined Server param.");

                if (Res.Resources.GetRes().DevicesType == 2 && !IsSuccessDisplayCursor)
                    ExceptionPro.ExpInfoLog("undefined DisplayCursor param.");

                if (!IsSuccessDisplaySecondMonitor)
                    ExceptionPro.ExpInfoLog("undefined DisplaySecondMonitor param.");

                
                if (!IsSuccessLocalPrintCustomOrder)
                    ExceptionPro.ExpInfoLog("undefined IsLocalPrintCustomOrder param.");

                if (!IsSuccessCashDrawer)
                    ExceptionPro.ExpInfoLog("undefined CashDrawer param.");

                if (!IsSuccessPriceMonitor)
                    ExceptionPro.ExpInfoLog("undefined PriceMonitor param.");

                if (!IsSuccessCardReader)
                    ExceptionPro.ExpInfoLog("undefined CardReader param.");

                if (!IsSuccessBarcodeReader)
                    ExceptionPro.ExpInfoLog("undefined BarcodeReader param.");

                
                if (!IsSuccessLang)
                    ExceptionPro.ExpInfoLog("undefined Lanugage param.");

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }

        /// <summary>
        /// 写入语言
        /// </summary>
        /// <param name="langIndex"></param>
        public bool SetConfig(int langIndex, string ServerIPAddress, bool IsLocalPrint, bool Immediately, string CashDrawer, string PriceMonitor, string BarcodeReader, string CardReader)
        {
            try
            {
                //先读取
                bool IsSuccessLanguage = false;
                bool IsSuccessServer = false;
                bool IsSuccessAutoSyncClientTime = false;
                bool IsSuccessDisplayCursor = false;
                bool IsSuccessDisplaySecondMonitor = false;
                bool IsSuccessCashDrawer = false;
                bool IsSuccessPriceMonitor = false;
                bool IsSuccessBarcodeReader = false;
                bool IsSuccessCardReader = false;
                bool IsSuccessIsLocalPrintCustomOrder = false;

                StringBuilder sb = new StringBuilder();
                bool IsReadSuccess = false;

                try
                {
                   
                    using (StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.txt"), Encoding.UTF8))
                    {
                        string line = null;

                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Trim().StartsWith("Server"))
                            {
                                sb.AppendLine("Server=" + ServerIPAddress);
                                IsSuccessServer = true;
                            }
                            else if (line.Trim().StartsWith("AutoSyncClientTime"))
                            {
                                sb.AppendLine("AutoSyncClientTime=" + (Resources.GetRes().AutoSyncClientTime ? "1" : "0"));
                                IsSuccessAutoSyncClientTime = true;
                            }
                            else if (line.Trim().StartsWith("DisplayCursor") &&  Resources.GetRes().DevicesType == 2)
                            {
                                sb.AppendLine("DisplayCursor=" + (Resources.GetRes().DisplayCursor ? "1" : "0"));
                                IsSuccessDisplayCursor = true;
                            }
                            else if (line.Trim().StartsWith("DisplaySecondMonitor"))
                            {
                                sb.AppendLine("DisplaySecondMonitor=" + (Resources.GetRes().DisplaySecondMonitor ? "1" : "0"));
                                IsSuccessDisplaySecondMonitor = true;
                            }
                            else if (line.Trim().StartsWith("IsLocalPrintCustomOrder"))
                            {
                                sb.AppendLine("IsLocalPrintCustomOrder=" + (IsLocalPrint ? "1" : "0"));
                                IsSuccessIsLocalPrintCustomOrder = true;
                            }
                            else if (line.Trim().StartsWith("CashDrawer"))
                            {
                                sb.AppendLine("CashDrawer=" + CashDrawer ?? "");
                                IsSuccessCashDrawer = true;
                            }
                            else if (line.Trim().StartsWith("PriceMonitor"))
                            {
                                sb.AppendLine("PriceMonitor=" + PriceMonitor ?? "");
                                IsSuccessPriceMonitor = true;
                            }
                            else if (line.Trim().StartsWith("BarcodeReader"))
                            {
                                sb.AppendLine("BarcodeReader=" + BarcodeReader ?? "");
                                IsSuccessBarcodeReader = true;
                            }
                            else if (line.Trim().StartsWith("CardReader"))
                            {
                                sb.AppendLine("CardReader=" + CardReader ?? "");
                                IsSuccessCardReader = true;
                            }
                            else if (line.Trim().StartsWith("Language"))
                            {
                                sb.AppendLine("Language=" + langIndex);
                                IsSuccessLanguage = true;
                            }
                            else
                            {
                                sb.AppendLine(line);
                            }
                        }

                        IsReadSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex);
                }

                if (!IsReadSuccess)
                {
                    sb.AppendLine("#提示:此配置文件用于配置系统功能,除非了解如何使用并有修改要求否则请勿随意修改."); //(注:详细请查看帮助文档)
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine();
                }

                if (!IsSuccessServer)
                    sb.AppendLine(Environment.NewLine + ("Server=" + ServerIPAddress));
                if (!IsSuccessAutoSyncClientTime)
                    sb.AppendLine(Environment.NewLine + ("AutoSyncClientTime=" + (Resources.GetRes().AutoSyncClientTime ? "1" : "0")));
                if (!IsSuccessDisplayCursor && Resources.GetRes().DevicesType == 2)
                    sb.AppendLine(Environment.NewLine + ("DisplayCursor=" + (Resources.GetRes().DisplayCursor ? "1" : "0")));
                if (!IsSuccessDisplaySecondMonitor)
                    sb.AppendLine(Environment.NewLine + ("DisplaySecondMonitor=" + (Resources.GetRes().DisplaySecondMonitor ? "1" : "0")));
                if (!IsSuccessIsLocalPrintCustomOrder)
                    sb.AppendLine(Environment.NewLine + ("IsLocalPrintCustomOrder=" + (IsLocalPrint ? "1" : "0")));
                if (!IsSuccessCashDrawer)
                    sb.AppendLine(Environment.NewLine + ("CashDrawer=" + CashDrawer ?? ""));
                if (!IsSuccessPriceMonitor)
                    sb.AppendLine(Environment.NewLine + ("PriceMonitor=" + PriceMonitor ?? ""));
                if (!IsSuccessBarcodeReader)
                    sb.AppendLine(Environment.NewLine + ("BarcodeReader=" + BarcodeReader ?? ""));
                if (!IsSuccessCardReader)
                    sb.AppendLine(Environment.NewLine + ("CardReader=" + CardReader ?? ""));
                if (!IsSuccessLanguage)
                    sb.AppendLine(Environment.NewLine + ("Language=" + langIndex));


                //写入
                using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.txt"), false, Encoding.UTF8))
                {
                    sw.Write(sb);
                }


                if (Immediately)
                {
                    Resources.GetRes().CashDrawer = CashDrawer;
                    Resources.GetRes().PriceMonitor = PriceMonitor;
                    Resources.GetRes().BarcodeReader = BarcodeReader;
                    Resources.GetRes().CardReader = CardReader;
                    Resources.GetRes().SERVER_ADDRESS = ServerIPAddress;
                    Resources.GetRes().ROOT = @"\\" + Resources.GetRes().SERVER_ADDRESS;
                    try
                    {
                        Common.GetCommon().Close();
                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);
                    }
                    Resources.GetRes().IsLocalPrintCustomOrder = IsLocalPrint;

                    Resources.GetRes().ReloadResources(langIndex);
                }

                return true;
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
                return false;
            }

        }










        /// <summary>
        /// 写入语言
        /// </summary>
        /// <param name="index"></param>
        internal bool SetLanguage(int index)
        {
            try
            {
                //先读取
                bool IsSuccess = false;
                StringBuilder sb = new StringBuilder();
                using (StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.txt"), Encoding.UTF8))
                {
                    string line = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Trim().StartsWith("Language"))
                        {
                            sb.AppendLine("Language=" + index);
                            IsSuccess = true;
                        }
                        else
                        {
                            sb.AppendLine(line);
                        }
                    }
                }

                if (!IsSuccess)
                    throw new Exception("undefined Lanugage.");

                IsSuccess = false;

                //写入
                using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.txt"), false, Encoding.UTF8))
                {
                    sw.Write(sb);
                    IsSuccess = true;
                }

                return IsSuccess;
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
                return false;
            }

        }
    }
}
