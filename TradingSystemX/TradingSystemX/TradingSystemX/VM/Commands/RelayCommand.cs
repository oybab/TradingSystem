using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Oybab.TradingSystemX.VM.Commands
{
    public class RelayCommand : Command
    {
        public RelayCommand(Action<object> execute)
            : base(execute)
        {
        }

        public RelayCommand(Action execute)
            : this(o => execute())
        {
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute, INotifyPropertyChanged npc = null)
            : base(execute, canExecute)
        {
            if (npc != null)
                npc.PropertyChanged += delegate { ChangeCanExecute(); };
        }

        public RelayCommand(Action execute, Func<bool> canExecute, INotifyPropertyChanged npc = null)
            : this(o => execute(), o => canExecute(), npc)
        {
        }
    }
}
