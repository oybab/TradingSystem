using ComponentFactory.Krypton.Toolkit;
using Oybab.Res;
using Oybab.Res.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Oybab.ServicePC.Tools
{
    internal sealed class ComTextBoxNumericUpDownColumn : KryptonDataGridViewTextBoxColumn
    {


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal Maximum { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal Minimum { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal Increment { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int DecimalPlaces { get; set; }

        public ComTextBoxNumericUpDownColumn()
            : base()
        {
            CellTemplate = new ComNumCell();
        }

        

        public override object Clone()
        {
            ComTextBoxNumericUpDownColumn c = (ComTextBoxNumericUpDownColumn)base.Clone();
            c.ValueType = base.ValueType;
            c.Maximum = this.Maximum;
            c.Minimum = this.Minimum;
            c.Increment = this.Increment;
            c.DecimalPlaces = this.DecimalPlaces;

            return c;
        }


        internal sealed class ComNumCell : KryptonDataGridViewTextBoxCell
        {
            private KryptonDataGridViewTextBoxEditingControl editControl;
            private decimal Maximum = 0;
            private decimal Minimum = 0;
            private decimal Increment;
            private int DecimalPlaces = 0;

            public ComNumCell()
            {

            }

            private void InitializeComponent()
            {
                LastNum = editControl.Text;
                
            }

            Regex match = new Regex(@"^[0-9]\d*(\.\d{0,3})?$");
            Regex match2 = new Regex(@"^\-?([0-9]\d*(\.\d{0,3})?)?$");
            private string LastNum = "";

            private void this_KeyUp(object sender, KeyEventArgs e)
            {


                ToNumber(false);

            }



            private void ToNumber(bool IsLost)
            {
                editControl.Text = editControl.Text.Trim();

                if (editControl.Text == "")
                {
                    editControl.Text = "0";
                    LastNum = "0";
                    editControl.SelectAll();
                }
                if (this.Minimum > 0)
                {
                    if (!match.IsMatch(editControl.Text))
                    {
                        editControl.Text = LastNum;
                        editControl.SelectionStart = editControl.TextLength;
                        return;
                    }
                }
                else
                {
                    if (!match2.IsMatch(editControl.Text))
                    {
                        editControl.Text = LastNum;
                        editControl.SelectionStart = editControl.TextLength;
                        return;
                    }
                }



                string valueToString = editControl.Text;

                if (valueToString != "-")
                {
                    decimal parse = decimal.Parse(valueToString);



                    if (parse > this.Maximum)
                    {
                        parse = this.Maximum;

                    }

                    if (parse < this.Minimum)
                    {
                        parse = this.Minimum;
                    }



                    if (!IsLost && valueToString.ToString().EndsWith(".") && !valueToString.ToString().EndsWith(".."))
                    {

                    }
                    else
                    {
                        if (DecimalPlaces <= 0)
                        {
                            editControl.Text = parse.ToString("0");
                        }
                        else
                        {
                            editControl.Text = parse.ToString("0." + new String('#', 3));
                        }
                    }

                }
                else if (IsLost)
                {
                    editControl.Text = LastNum;
                    editControl.SelectionStart = editControl.TextLength;
                    return;
                }


               

                editControl.SelectionLength = editControl.TextLength;

                if ("-" != valueToString)
                {
                    LastNum = valueToString;
                }

            }




            private void editControl_GotFocus(object sender, EventArgs e)
            {
                if (string.IsNullOrWhiteSpace(this.editControl.Text) || this.editControl.Text.EndsWith(".") || this.editControl.Text == "-")
                    ToNumber(true);
            }




            public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
            {
                
                // Set the value of the editing control to the current cell value.
                base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
                ComTextBoxNumericUpDownColumn col = (ComTextBoxNumericUpDownColumn)OwningColumn;
                KryptonDataGridViewTextBoxEditingControl Tb = (KryptonDataGridViewTextBoxEditingControl)DataGridView.EditingControl;
                
                this.editControl = Tb;

                this.Maximum = col.Maximum;
                this.Minimum = col.Minimum;
                this.Increment = col.Increment;
                this.DecimalPlaces = col.DecimalPlaces;

                Tb.KeyUp += this_KeyUp;
                this.editControl.LostFocus += editControl_GotFocus;

                InitializeComponent();
                

              
                
            }

            public override void DetachEditingControl()
            {
          
                base.DetachEditingControl();
                KryptonDataGridViewTextBoxEditingControl Tb = (KryptonDataGridViewTextBoxEditingControl)DataGridView.EditingControl;
                ComTextBoxNumericUpDownColumn col = (ComTextBoxNumericUpDownColumn)OwningColumn;


                
                Tb.KeyUp -= this_KeyUp;

                this.editControl.LostFocus -= editControl_GotFocus;


                this.editControl = null;


                
                
            }


        }
    }
}
