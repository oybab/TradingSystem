using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Oybab.Res.Exceptions
{
    /// <summary>
    /// VOD异常
    /// </summary>
    public sealed class OybabException : Exception
    {
        
        public bool HasWrite;
        public string ExceptionMessage;
        private OybabException(){}

        public OybabException(string message, bool IsInfo = false)
            :base(message)
        {
            ExceptionMessage = message;
            WriteExpLog(null, message, IsInfo);
        }


        public OybabException(Exception ex, bool IsInfo = false)
            : base(null,ex)
        {
            ExceptionMessage = ExceptionPro.HandleExceptionType(ex, null, ExceptionMessage);
            WriteExpLog(ex, ex.ToString(), IsInfo);
        }


        public OybabException(string message, Exception ex, bool IsInfo = false)
            : base(message, ex)
        {
            ExceptionMessage = ExceptionPro.HandleExceptionType(ex, message, ExceptionMessage);
            WriteExpLog(ex, message + ex.ToString(), IsInfo);
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        private void WriteExpLog(Exception ex, string message, bool IsInfo)
        {
            if (null != ex as OybabException)
            {
                if ((ex as OybabException).HasWrite == false)
                {
                    if (IsInfo)
                        ExceptionPro.ExpInfoLog(message);
                    else
                        ExceptionPro.ExpErrorLog(message);
                }
            }
            else
            {
                if (IsInfo)
                    ExceptionPro.ExpInfoLog(message);
                else
                    ExceptionPro.ExpErrorLog(message);
            }
            HasWrite = true;
        }


    }
}
