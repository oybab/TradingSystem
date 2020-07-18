using Oybab.Res.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.Tools
{
    /// <summary>
    /// 还原
    /// </summary>
    internal sealed class Restore
    {
        #region Instance
        private Restore() { }
        private static readonly Lazy<Restore> lazy = new Lazy<Restore>(() => new Restore());
        public static Restore Instance { get { return lazy.Value; } }
        #endregion Instance


        private string BakFile;

        /// <summary>
        /// 创建路径
        /// </summary>
        private void CreateBakFilePath()
        {
            //先创建主目录
            string TempRootDirectory = Path.Combine(System.IO.Path.GetTempPath(), "OYBAB");
            if (!System.IO.Directory.Exists(TempRootDirectory))
            {
                System.IO.Directory.CreateDirectory(TempRootDirectory);
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(TempRootDirectory);
                di.Attributes = System.IO.FileAttributes.Directory | System.IO.FileAttributes.Hidden;
            }


            //创建附目录
            string TempSecondRootDirectory = Path.Combine(TempRootDirectory, "TradingSystem");
            if (!System.IO.Directory.Exists(TempSecondRootDirectory))
            {
                System.IO.Directory.CreateDirectory(TempSecondRootDirectory);
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(TempSecondRootDirectory);
                di.Attributes = System.IO.FileAttributes.Directory | System.IO.FileAttributes.Hidden;
            }

            //创建备份目录
            string TempDirectory = Path.Combine(TempSecondRootDirectory, "Bak");
            if (!System.IO.Directory.Exists(TempDirectory))
            {
                System.IO.Directory.CreateDirectory(TempDirectory);
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(TempDirectory);
                di.Attributes = System.IO.FileAttributes.Directory | System.IO.FileAttributes.Hidden;
            }

            BakFile = Path.Combine(TempDirectory, "Trading.bak");
        }
        /// <summary>
        ///  读取备份(读取完配置文件后)
        /// </summary>
        internal void ReadBak()
        {

            // 创建路径
            if (string.IsNullOrWhiteSpace(BakFile))
                CreateBakFilePath();


            // 如果文件不存在写入到错误日志
            if (System.IO.File.Exists(BakFile))
            {

                try
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(BakFile, Encoding.UTF8))
                    {

                        // 上次登录的管理员
                        Resources.GetRes().LastLoginAdminNo = sr.ReadLine();
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex);
                }
            }

        }



        private string Last;
        /// <summary>
        /// 写入备份
        /// </summary>
        internal void WriteBak()
        {
            // 创建路径
            if (string.IsNullOrWhiteSpace(BakFile))
                CreateBakFilePath();

            try
            {
                StringBuilder sb = new StringBuilder();
                // 管理员
                sb.AppendLine(Resources.GetRes().LastLoginAdminNo);

                string temp = sb.ToString();

                if (temp != Last)
                {
                    Last = temp;
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(BakFile, false, Encoding.UTF8))
                    {
                        sw.Write(temp);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }
    }
}

