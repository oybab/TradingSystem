using Ionic.Zip;
using Newtonsoft.Json;
using Oybab.DAL;
using Oybab.ServerManager.Exceptions;
using Oybab.ServerManager.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;

namespace Oybab.ServerManager.Operate
{
    internal sealed class Backup
    {
        #region Instance
        private Backup() { }

        private static readonly Lazy<Backup> _instance = new Lazy<Backup>(() => new Backup());
        public static Backup Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion Instance

        private int BackCount = 1;
        private DateTime lastBackup = DateTime.MinValue;


        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="IsAuto"></param>
        internal void BackupFile(bool IsAutoBackup = false, bool IsForce = false)
        {
            try
            {
                // 如果获取到的备份位置不是空的, 则继续
                if (!string.IsNullOrWhiteSpace(Resources.GetRes().BackupFolderPath))
                {

                    if (new Regex(@"^(([a-zA-Z]:\\)|(//)).*").Match((Resources.GetRes().BackupFolderPath)).Success)
                    {
                        string disk = Path.GetPathRoot(Resources.GetRes().BackupFolderPath);
                        if (!Directory.Exists(disk))
                        {
                            ExceptionPro.ExpErrorLog("Backup failed! backup path disk name not exists!");
                            return;
                        }

                    }
                    else
                    {
                        Resources.GetRes().BackupFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Resources.GetRes().BackupFolderPath.Replace('/','\\').TrimStart("\\"));
                    }

                  
                    // 没有文件夹就先创建
                    if (!Directory.Exists(Resources.GetRes().BackupFolderPath))
                    {
                        Directory.CreateDirectory(Resources.GetRes().BackupFolderPath);
                        //DirectoryInfo di = new DirectoryInfo(Resources.GetRes().BackupFolderPath);
                        //di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                    }

                    // 如果上次备份时间不是今天, 则把数量重置
                    if (lastBackup.Date != DateTime.Today)
                    {
                        BackCount = 1;
                    }


                    DateTime systemDateTimeNow = System.DateTime.Now;

                    // 检查是否备份
                    if (IsForce || CheckforSave())
                    {

                        // 首次打开, 非自动模式要求备份时, 也不是强制的. 也是第一次备份时(时间为0说明刚启动). 暂时停留数秒. 以便防止有些电脑还没启动好就备份导致出现问题(不过已通过改Windows服务启动类型设置为自动(延迟)来暂时解决这个问题, 
                        // 不过可能导致启动速度变慢, 所以下次改造需要直接把整个Windows改成自动后, 把这个程序启动后等待1分钟后再继续执行, 免得启动太快出现硬盘还未识别之类的奇葩错误!
                        if(!IsAutoBackup && !IsForce && lastBackup == DateTime.MinValue)
                        {
#if !DEBUG
                            //System.Threading.Thread.Sleep(1000 * 15);
#endif
                        }

                        int systemDateTimeNowDayOfWeek = (int)systemDateTimeNow.DayOfWeek;

                        string fileName = "";

                        if (!IsForce)
                            fileName = "backup{0}-{1}.bak";
                        else
                            fileName = "backup{0}-c.bak";

                        fileName = string.Format(fileName, systemDateTimeNowDayOfWeek == 0 ? 7 : systemDateTimeNowDayOfWeek, BackCount);
                        string fullFilePath = Path.Combine(Resources.GetRes().BackupFolderPath, fileName);

                        try
                        {
                            using (ZipFile zip = new ZipFile())
                            {

                                //zip.UseUnicodeAsNecessary = true;  // utf-8

                                //zip.Encryption = EncryptionAlgorithm.WinZipAes256;
                                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestSpeed;
                                zip.AddFile(Path.Combine(Resources.GetRes().BackupFolderPath, AppDomain.CurrentDomain.BaseDirectory, "ts.db"), "");
                                zip.AddFile(Path.Combine(Resources.GetRes().BackupFolderPath, AppDomain.CurrentDomain.BaseDirectory, "ServerConfig.txt"), "");
                                zip.Comment = "This file was created at " + systemDateTimeNow.ToString("yyyy-MM-dd HH:mm:ss");

                                zip.Save(fullFilePath);
                            }

                            lastBackup = systemDateTimeNow;
                            Session.Instance.ChangeInterval(true);
                        }
                        catch (Exception ex)
                        {
                            Session.Instance.ChangeInterval(false);
                            ExceptionPro.ExpLog(ex, null, false, "Backup failed!");
                        }
                    }
                    else
                    {
                        lastBackup = systemDateTimeNow;
                        Session.Instance.ChangeInterval(true);
                    }
                }
                
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Backup check operate failed!");
            }
        }

        /// <summary>
        /// 为备份检查
        /// </summary>
        /// <returns></returns>
        private bool CheckforSave()
        {
            DateTime systemDateTimeNow = System.DateTime.Now;
            int systemDateTimeNowDayOfWeek = (int)systemDateTimeNow.DayOfWeek;

            string fileName = string.Format("backup{0}-{1}.bak", systemDateTimeNowDayOfWeek == 0 ? 7 : systemDateTimeNowDayOfWeek, BackCount);
            string fullFilePath = Path.Combine(Resources.GetRes().BackupFolderPath, fileName);

            // 如果文件存在, 则需要对比创建时间
            if (File.Exists(fullFilePath))
            {
                DateTime fileGetLastWriteTime = File.GetLastWriteTime(fullFilePath);
                // 写入时间不是今天就备份
                if (fileGetLastWriteTime.Date != DateTime.Today) 
                {
                    return true;
                }else
                {
                    // 如果是今天创建的, 则判断一下上一次写入时间是否在3小时内, 如果超出3小时就备份并且没有下一个数的文件则备份
                    if ((DateTime.Now - fileGetLastWriteTime).TotalHours > Resources.GetRes().DB_BACKUP_TIME - 1)
                    {
                        // 如果当前没有当天的下一个文件, 则直接备份
                        if (!File.Exists(Path.Combine(Resources.GetRes().BackupFolderPath, string.Format("backup{0}-{1}.bak", systemDateTimeNowDayOfWeek == 0 ? 7 : systemDateTimeNowDayOfWeek, (BackCount + (1))))))
                        {
                            ++BackCount;
                            return true;
                        }else
                        {
                            ++BackCount;
                            return CheckforSave();
                        }
                        
                    }
                    // 否则递增一个备份数
                    else
                    {
                        ++BackCount;
                        return false;
                    }
                }
            }
            else
            {
                return true;
            }
        }

        
    }

}
