using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Oybab.ServerManager.Exceptions;
using Oybab.ServerManager.Model.Models;
using Newtonsoft.Json;

namespace Oybab.ServerManager.Operate
{
    /// <summary>
    /// 配置
    /// </summary>
    internal sealed class Config
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
        /// 加载
        /// </summary>
        internal void GetConfigs()
        {
            try
            {

                bool IsSuccessBackupFolder = false;
                bool IsSuccessDbKey = false;
                bool IsSuccessUID = false;
                bool IsSuccessPrintInfo = false;


                using (StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServerConfig.txt"), Encoding.UTF8))
                {
                    string line = null;
                    string temp = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        //获取消费类型

                        if (!IsSuccessBackupFolder && line.Trim().StartsWith("BackupFolderPath"))
                        {
                            try
                            {
                                temp = line.Trim().Split('=')[1];
                                Resources.GetRes().BackupFolderPath = temp;
                                IsSuccessBackupFolder = true;
                            }
                            catch (Exception)
                            {
                                ExceptionPro.ExpInfoLog("Unknow BackupFolderPath param.");
                            }
                        }
                        else if (!IsSuccessDbKey && line.Trim().StartsWith("DbKey"))
                        {
                            try
                            {
                                temp = line.Trim().Split('=')[1];
                                if (string.IsNullOrWhiteSpace(Resources.GetRes().DB_KEY))
                                    Resources.GetRes().DB_KEY = temp;
                                IsSuccessDbKey = true;
                            }
                            catch (Exception)
                            {
                                ExceptionPro.ExpInfoLog("Unknow DB_KEY param.");
                            }
                        }
                        else if (!IsSuccessUID && line.Trim().StartsWith("UID"))
                        {
                            try
                            {
                                temp = line.Trim().Split('=')[1];
                                if (string.IsNullOrWhiteSpace(Resources.GetRes().UID))
                                    Resources.GetRes().UID = temp;
                                IsSuccessUID = true;
                            }
                            catch (Exception)
                            {
                                ExceptionPro.ExpInfoLog("Unknow UID param.");
                            }
                        }
                        else if (!IsSuccessPrintInfo && line.Trim().StartsWith("PrintInfo"))
                        {
                            try
                            {
                                temp = line.Trim().TrimStart("PrintInfo=");
                                try
                                {

                                    if (!string.IsNullOrWhiteSpace(temp))
                                        Resources.GetRes().PrintInfo = JsonConvert.DeserializeObject<PrintInfo>(Resources.GetRes().PrintInfo.Decrypt(temp, true));

                                    IsSuccessPrintInfo = true;
                                }
                                catch
                                {
                                    ExceptionPro.ExpInfoLog("Unknow PrintInfo param.");
                                }

                            }
                            catch (Exception)
                            {
                                ExceptionPro.ExpInfoLog("Unknow PhoneNo param.");
                            }
                        }
                    }

                }

                if (!IsSuccessBackupFolder)
                    ExceptionPro.ExpInfoLog("undefined BackupFolderPath param.");
                if (!IsSuccessDbKey)
                    ExceptionPro.ExpInfoLog("undefined DbKey param.");
                if (!IsSuccessUID)
                    ExceptionPro.ExpInfoLog("undefined UID param.");

                if (!IsSuccessPrintInfo)
                    ExceptionPro.ExpInfoLog("undefined PrintInfo param.");




            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }




       

        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="index"></param>
        internal bool SetConfig(List<string> config)
        {
            if (config.Count == 0 || !config.Any(x => x.Contains("PrintInfo=")))
                return false;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#Warning: This configuration file is used to configure system functions.Do not modify it unless you know how to use it and have modification requirements."); // (注:详细请查看帮助文档)
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("BackupFolderPath=" + Resources.GetRes().BackupFolderPath);
            sb.AppendLine();
            sb.AppendLine("DbKey=" + Resources.GetRes().DB_KEY);
            sb.AppendLine();
            sb.AppendLine("UID=" + Resources.GetRes().UID);
            sb.AppendLine();

            bool IsSuccessPrintInfo = false;

            PrintInfo PrintInfo = Resources.GetRes().PrintInfo;

            foreach (var item in config)
            {
                if (item.Contains("PrintInfo="))
                {
                    string temp = item.Trim().TrimStart("PrintInfo=");

                    if (!string.IsNullOrEmpty(temp))
                    {
                        PrintInfo = JsonConvert.DeserializeObject<PrintInfo>(temp);

                        sb.AppendLine("PrintInfo=" + Resources.GetRes().PrintInfo.Encrypt(temp, true));
                    }
                    else
                    {
                        PrintInfo = new PrintInfo();
                        sb.AppendLine("PrintInfo=");
                    }



                    sb.AppendLine();

                    IsSuccessPrintInfo = true;
                    
                }
            }

            if (!IsSuccessPrintInfo)
            {
                return false;
            }

            try
            {
                //写入
                using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServerConfig.txt"), false, Encoding.UTF8))
                {
                    sw.Write(sb);
                }


                Resources.GetRes().PrintInfo = PrintInfo;

                
                return true;
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
                return false;
            }
        }


        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <returns></returns>
        internal string ReadConfig()
        {
            string Config = null;
            try
            {
                using (StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServerConfig.txt"), Encoding.UTF8))
                {
                    Config = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
            return Config;
        }
    }
}
