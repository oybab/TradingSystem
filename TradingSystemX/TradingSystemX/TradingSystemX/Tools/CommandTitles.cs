using System;
using System.Collections.Generic;
using System.Text;

namespace Oybab.TradingSystemX.Tools
{
    internal sealed class CommandTitles
    {

        #region Instance
        private CommandTitles()
        {
        }

        private static readonly Lazy<CommandTitles> _instance = new Lazy<CommandTitles>(() => new CommandTitles());
        public static CommandTitles Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion Instance

        /// <summary>
        /// Abort
        /// </summary>
        /// <returns></returns>
        internal string Abort
        {
            get
            {
                return Resources.Instance.GetString("Abort");
            }
        }

        /// <summary>
        /// Cancel
        /// </summary>
        /// <returns></returns>
        internal string Cancel
        {
            get
            {
                return Resources.Instance.GetString("Cancel");
            }
        }


        /// <summary>
        /// Close
        /// </summary>
        /// <returns></returns>
        internal string Close
        {
            get
            {
                return Resources.Instance.GetString("Close");
            }
        }


        /// <summary>
        /// Ignore
        /// </summary>
        /// <returns></returns>
        internal string Ignore
        {
            get
            {
                return Resources.Instance.GetString("Ignore");
            }
        }


        /// <summary>
        /// No
        /// </summary>
        /// <returns></returns>
        internal string No
        {
            get
            {
                return Resources.Instance.GetString("No");
            }
        }


        /// <summary>
        /// OK
        /// </summary>
        /// <returns></returns>
        internal string OK
        {
            get
            {
                return Resources.Instance.GetString("OK");
            }
        }

        /// <summary>
        /// Yes
        /// </summary>
        /// <returns></returns>
        internal string Yes
        {
            get
            {
                return Resources.Instance.GetString("Yes");
            }
        }


        /// <summary>
        /// Retry
        /// </summary>
        /// <returns></returns>
        internal string Retry
        {
            get
            {
                return Resources.Instance.GetString("Retry");
            }
        }


        /// <summary>
        /// Warn
        /// </summary>
        /// <returns></returns>
        internal string Warn
        {
            get
            {
                return Resources.Instance.GetString("Warn");
            }
        }


        /// <summary>
        /// Information
        /// </summary>
        /// <returns></returns>
        internal string Information
        {
            get
            {
                return Resources.Instance.GetString("Information");
            }
        }


        /// <summary>
        /// Error
        /// </summary>
        /// <returns></returns>
        internal string Error
        {
            get
            {
                return Resources.Instance.GetString("Error");
            }
        }


        /// <summary>
        /// Exit
        /// </summary>
        /// <returns></returns>
        internal string Exit
        {
            get
            {
                return Resources.Instance.GetString("Exit");
            }
        }

    }
}
