using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Oybab.Res.Exceptions;

namespace Oybab.Res.Tools
{
    /// <summary>
    /// 路径
    /// </summary>
    public static class Path
    {
        /// <summary>
        /// 路径合并
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string Combine(params string[] paths)
        {
            try
            {
                if (paths.Length == 0)
                {
                    throw new OybabException("please input path, paths.Length == 0");
                }
                else
                {
                    StringBuilder builder = new StringBuilder();
                    string spliter = "\\";

                    string firstPath = paths[0];

                    if (firstPath.StartsWith("HTTP", StringComparison.OrdinalIgnoreCase))
                    {
                        spliter = "/";
                    }

                    if (!firstPath.EndsWith(spliter))
                    {
                        firstPath = firstPath + spliter;
                    }
                    builder.Append(firstPath);

                    for (int i = 1; i < paths.Length; i++)
                    {
                        string nextPath = paths[i];
                        if (nextPath.StartsWith("/") || nextPath.StartsWith("\\"))
                        {
                            nextPath = nextPath.Substring(1);
                        }

                        if (i != paths.Length - 1)//not the last one
                        {
                            if (nextPath.EndsWith("/") || nextPath.EndsWith("\\"))
                            {
                                nextPath = nextPath.Substring(0, nextPath.Length - 1) + spliter;
                            }
                            else
                            {
                                nextPath = nextPath + spliter;
                            }
                        }

                        builder.Append(nextPath);
                    }

                    return builder.ToString();
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
                return null;
            }
        }



        /// <summary>
        /// 检查Mapping Driver
        /// </summary>
        /// <param name="path"></param>
        /// <param name="IsDirectory"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static bool PathExists(string path, bool IsDirectory, int time)
        {
            bool exists = true;
            System.Threading.Thread t = new System.Threading.Thread(() => {
             if(IsDirectory)
                 exists = System.IO.Directory.Exists(path);
             else
                 exists = System.IO.File.Exists(path);
            });
            
            t.Start();
            bool completed = t.Join(time);
            if (!completed) {
                exists = false;
                t.Abort();
            }
            return exists;
        }

       
    }
}
