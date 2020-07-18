using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.ServerManager.Exceptions
{
    /// <summary>
    /// 异常处理器
    /// </summary>
    public static class ExceptionPro
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ExceptionPro));
        /// <summary>
        /// 检测异常类型并对应操作
        /// </summary>
        /// <param name="ex"></param>
        public static string HandleExceptionType(Exception ex, string message = null, string ExceptionMessage = null)
        {
            if (null == ex)
                return "Not caption exception param!";

            Type type = ex.GetType();
            string tempStr = "";
            //找到异常
            if (null != ex as OybabException)
            {
                return (ex as OybabException).ExceptionMessage;
            }
            if (null != message)
            {
                return message;
            }
            else
            {
                //如果没找到异常,直接获取异常自己的信息
                if (tempStr == "")
                {
                    StringBuilder tempMessage = new StringBuilder();
                    GetExceptionAllMessage(ex, tempMessage);
                    return tempMessage.ToString();
                }
                else
                {
                    return tempStr;
                }
            }
        }

        /// <summary>
        /// 获取所有异常
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="Message"></param>
        public static void GetExceptionAllMessage(Exception ex, StringBuilder Message)
        {
            Message.AppendLine(ex.Message);
            if (null != ex.InnerException)
            {
                GetExceptionAllMessage(ex.InnerException, Message);
            }
        }

        /// <summary>
        /// 输出异常
        /// </summary>
        /// <param name="message"></param>
        public static void ExpErrorLog(string message)
        {
            try
            {
                logger.Error(message);
            }
            catch { }
            
        }

        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="message"></param>
        public static void ExpInfoLog(string message)
        {
            try
            {
                logger.Info(message);
            }
            catch { }
        }

        /// <summary>
        /// 异常处理Exception
        /// </summary>
        /// <param name="ex"></param>
        public static void ExpLog(Exception ex, Action<string> action = null, bool IsInfo = false, string designationMessage = "")
        {
            OybabException vodEx = ex as OybabException;
            if (null != vodEx)
            {
                if (null != action)
                    AlertMessage(action, vodEx.ExceptionMessage, designationMessage);
            }
            else
            {
                string message = HandleExceptionType(ex);
                if (IsInfo)
                    ExpInfoLog(message + designationMessage + ex.ToString());
                else
                    ExpErrorLog(message + designationMessage + ex.ToString());
                if (null != action)
                    AlertMessage(action, message, designationMessage);
            }
        }


        /// <summary>
        /// 异常处理Exception
        /// </summary>
        /// <param name="ex"></param>
        public static void ExpLog(object exception, Action<string> action = null, bool IsInfo = false, string designationMessage = "")
        {

            Exception ex = exception as Exception;

            if (null != ex)
            {
                ExpLog(ex, action, IsInfo, designationMessage);
            }
            else
            {
                if (IsInfo)
                    ExpInfoLog(exception.ToString() + designationMessage);
                else
                    ExpErrorLog(exception.ToString() + designationMessage);
                if (null != action)
                    AlertMessage(action, exception.ToString(), designationMessage);
            }

        }

        /// <summary>
        /// 提醒用户
        /// </summary>
        /// <param name="action"></param>
        /// <param name="message"></param>
        /// <param name="designationMessage"></param>
        private static void AlertMessage(Action<string> action, string message, string designationMessage)
        {
            if (designationMessage != "")
                action(designationMessage);
            else
                action(message);
        }

    }
}
