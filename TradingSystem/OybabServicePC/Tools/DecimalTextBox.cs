using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Oybab.ServicePC.Tools
{
    internal sealed partial class DecimalTextBox : KryptonTextBox
    {
        public DecimalTextBox()
        {

        }


        protected override void OnTextChanged(EventArgs e)
        {
            if (IsDecimal())
                base.OnTextChanged(e);
        }



        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar)
                && ((Keys)e.KeyChar != Keys.Back)
                && (e.KeyChar != '.'))
                e.Handled = true;



            if (e.KeyChar == '.' && Text.IndexOf('.') > 0)
                e.Handled = true;

            if (Text.Substring(Text.LastIndexOf('.')).Length > 2)
                e.Handled = true;

            base.OnKeyPress(e);
        }



        protected override void OnGotFocus(EventArgs e)
        {
            ResetValueOnFocus();
            base.OnGotFocus(e);
        }



        private void ResetValueOnFocus()
        {
            if (IsDecimal())
            {
                if (!IsDecimalZero())
                    return;
            }
            Text = "";
        }



        private bool IsDecimal()
        {
            decimal result;
            return decimal.TryParse(Text, out result);
        }



        private bool IsDecimalZero()
        {
            return (decimal.Parse(Text) == 0);
        }



        private void DecimalTextBox_Validating(object sender, CancelEventArgs e)
        {
            decimal value;
            decimal.TryParse(Text, out value);



            const string NUMBER_FORMAT_2_DIGITS = "N2";
            Text = value.ToString(NUMBER_FORMAT_2_DIGITS);
        }



        public decimal Value
        {
            get
            {
                return decimal.Parse(Text);
            }
        }
    }
}
