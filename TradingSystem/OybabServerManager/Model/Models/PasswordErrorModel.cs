using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.ServerManager.Model.Models
{
    internal class PasswordErrorModel
    {
        public string AdminNo { get; set; }
        public  int ErrorCount { get; set; }
        public int TotalErrorCount { get; set; }
        public DateTime LastErrorData { get; set; }
    }
}
