using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.View.Component
{
    public sealed class RemarkEvent
    {
        #region Instance
        private static readonly Lazy<RemarkEvent> _instance = new Lazy<RemarkEvent>(() => new RemarkEvent());
        public static RemarkEvent Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion Instance
        
        private RemarkEvent()
        {
            ActionRemark = (obj, value, args) => { if (null != Remark) Remark(obj, value, args); };
        }
        public Action<object, string, object> ActionRemark;
        public delegate void EventRemark(object sender, string value, object args);
        public event EventRemark Remark;
    }
}
