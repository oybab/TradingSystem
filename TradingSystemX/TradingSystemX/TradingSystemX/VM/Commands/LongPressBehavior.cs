using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace Oybab.TradingSystemX.VM.Commands
{
    public class LongPressBehavior : Behavior<Button>
    {
        private readonly object _syncObject = new object();
        private const int Duration = 2000;

        //timer to track long press
        private Timer _timer;
        //the timeout value for long press
        private readonly int _duration;
        //whether the button was released after press
        private volatile bool _isReleased;
        private Button _button;

        /// <summary>
        /// Occurs when the associated button is long pressed.
        /// </summary>
        public event EventHandler LongPressed;

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command),
            typeof(ICommand), typeof(LongPressBehavior), default(ICommand));

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(LongPressBehavior));

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttachedTo(Button button)
        {
            base.OnAttachedTo(button);
            _button = button;
            button.Pressed += Button_Pressed;
            button.Released += Button_Released;
        }

        protected override void OnDetachingFrom(Button button)
        {
            base.OnDetachingFrom(button);
            _button = null;
            button.Pressed -= Button_Pressed;
            button.Released -= Button_Released;
        }

        /// <summary>
        /// DeInitializes and disposes the timer.
        /// </summary>
        private void DeInitializeTimer()
        {
            lock (_syncObject)
            {
                if (_timer == null)
                {
                    return;
                }
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _timer.Dispose();
                _timer = null;
            }
        }

        /// <summary>
        /// Initializes the timer.
        /// </summary>
        private void InitializeTimer()
        {
            lock (_syncObject)
            {
                _timer = new Timer(Timer_Elapsed, null, _duration, Timeout.Infinite);
            }
        }

        private void Button_Pressed(object sender, EventArgs e)
        {
            _isReleased = false;
            InitializeTimer();
        }

        private void Button_Released(object sender, EventArgs e)
        {
            _isReleased = true;
            DeInitializeTimer();
        }

        protected virtual void OnLongPressed()
        {
            var handler = LongPressed;
            handler?.Invoke(this, EventArgs.Empty);

            this.BindingContext = _button.BindingContext;
            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }
        }

        public LongPressBehavior()
        {
            _isReleased = true;
            _duration = Duration;
        }

        public LongPressBehavior(int duration) : this()
        {
            _duration = duration;
        }

        private void Timer_Elapsed(object state)
        {
            DeInitializeTimer();
            if (_isReleased)
            {
                return;
            }
            Device.BeginInvokeOnMainThread(OnLongPressed);
        }
    }
}
