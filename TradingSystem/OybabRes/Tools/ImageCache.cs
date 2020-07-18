using Oybab.Res.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Oybab.Res.Tools
{
    public static class ImageCache
    {
        private static string ImageCacheDirectory { get; set; }
        private static string ImageLocalDirectory { get; set; }

        static ImageCache()
        {

            //先创建主目录
            string TempRootDirectory = Path.Combine(System.IO.Path.GetTempPath(), "OYBAB");
            if (!Directory.Exists(TempRootDirectory))
            {
                Directory.CreateDirectory(TempRootDirectory);
                DirectoryInfo di = new DirectoryInfo(TempRootDirectory);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }


            //创建附目录
            string TempSecondRootDirectory = Path.Combine(TempRootDirectory, "TradingSystem");
            if (!Directory.Exists(TempSecondRootDirectory))
            {
                Directory.CreateDirectory(TempSecondRootDirectory);
                DirectoryInfo di = new DirectoryInfo(TempSecondRootDirectory);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }


            // default cache directory, can be changed in de app.xaml.
            ImageCacheDirectory = Path.Combine(TempSecondRootDirectory, "ImagesCache");
            if (!System.IO.Directory.Exists(ImageCacheDirectory))
            {
                //Create it 
                System.IO.Directory.CreateDirectory(ImageCacheDirectory);
                DirectoryInfo di = new DirectoryInfo(ImageCacheDirectory);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden; 
            }

            // 本地文件夹也存在(不连接服务器共享的场景)
            if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Resources.GetRes().ROOT_FOLDER, Resources.GetRes().PRODUCTS_FOLDER)))
                ImageLocalDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Resources.GetRes().ROOT_FOLDER, Resources.GetRes().PRODUCTS_FOLDER);
        }

        /// <summary>
        /// 获取本地缓存文件或先放到缓存
        /// </summary>
        /// <param name="ImagePath"></param>
        /// <param name="ImageName"></param>
        /// <returns></returns>
        public static string GetImage(string ImagePath, string ImageName)
        {

            if (string.IsNullOrWhiteSpace(ImagePath) || string.IsNullOrWhiteSpace(ImageName))
                return null;

            if (!string.IsNullOrWhiteSpace(ImageLocalDirectory))
                return Path.Combine(ImageLocalDirectory, ImageName);

            //Cast the string into a Uri so we can access the image name without regex 
            var localFile = Path.Combine(ImageCacheDirectory, ImageName);
            if (!System.IO.File.Exists(localFile))
            {
                if (ImagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    if (HttpHelper.GetAndSaveToFile(ImagePath, localFile))
                        return localFile;
                    else
                        return null;
                }
                else
                {
                    try
                    {
                        System.IO.File.Copy(ImagePath, localFile);
                        return localFile;
                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);
                        return null;
                    }
                }
            }
            else
            {
                //The full path of the image on the local computer 
                return localFile;
            }
        }
    }





    /// <summary>
    /// HTTP辅助(下载保存)
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] Get(string url)
        {
            WebRequest request = HttpWebRequest.Create(url);
            WebResponse response = request.GetResponse();

            return response.ReadToEnd();
        }

        /// <summary>
        /// 保存到指定文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool GetAndSaveToFile(string url, string filename)
        {
            try
            {
                using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = Get(url);
                    stream.Write(data, 0, data.Length);
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpErrorLog(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 读取到内存(WebResponse扩展)
        /// </summary>
        /// <param name="webresponse"></param>
        /// <returns></returns>
        public static byte[] ReadToEnd(this WebResponse webresponse)
        {
            Stream responseStream = webresponse.GetResponseStream();

            using (MemoryStream memoryStream = new MemoryStream((int)webresponse.ContentLength))
            {
                responseStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
