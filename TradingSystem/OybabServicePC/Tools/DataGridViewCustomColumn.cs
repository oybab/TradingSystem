using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Oybab.ServicePC.Tools
{
    internal sealed class DataGridViewCustomColumn : KryptonDataGridViewTextBoxColumn
    {
        public AutoCompleteStringCollection AutoCompleteStringCollection;
        public CharacterCasing CharacterCasing = CharacterCasing.Normal;
        public bool RestrictAutoComplete = false;
        public int DrowDownWidth = 0;

        private int OldWidth;
        public DataGridViewCustomColumn()
            : base()
        {
            CellTemplate = new Cell();
        }

        private DataGridViewCustomColumn(DataGridViewTextBoxCell Cell)
            : base()
        {
            CellTemplate = Cell;
        }

        public override object Clone()
        {
            DataGridViewCustomColumn c = (DataGridViewCustomColumn)base.Clone();
            c.CharacterCasing = this.CharacterCasing;
            c.AutoCompleteStringCollection = this.AutoCompleteStringCollection;
            c.ValueType = base.ValueType;

            return c;
        }

        private class Cell : KryptonDataGridViewTextBoxCell
        {

            public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
            {
                // Set the value of the editing control to the current cell value.
                base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
                DataGridViewCustomColumn col = (DataGridViewCustomColumn)OwningColumn;
                KryptonDataGridViewTextBoxEditingControl Tb = (KryptonDataGridViewTextBoxEditingControl)DataGridView.EditingControl;
                Tb.CharacterCasing = col.CharacterCasing;
                if (col.AutoCompleteStringCollection != null)
                {
                    Autocomplete(Tb, col.AutoCompleteStringCollection);
                    if (col.DrowDownWidth > 0) { col.OldWidth = col.Width; col.Width = col.DrowDownWidth; }
                }
            }

            public override void DetachEditingControl()
            {
                base.DetachEditingControl();

                KryptonDataGridViewTextBoxEditingControl Tb = (KryptonDataGridViewTextBoxEditingControl)DataGridView.EditingControl;
                if (Tb.AutoCompleteCustomSource != null)
                {
                    DataGridViewCustomColumn col = (DataGridViewCustomColumn)OwningColumn;
                    if (col.OldWidth > 0)
                        col.Width = col.OldWidth;
                    Autocomplete(Tb, null);
                }
                Tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            }

            public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
            {
                DataGridViewCustomColumn c = (DataGridViewCustomColumn)OwningColumn;
                object v = base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
                if (v != null && c.RestrictAutoComplete && formattedValue is string)
                {
                    dynamic s = ((string)v).Trim();
                    if (s.Length == 0)
                        return null;
                    if (c.AutoCompleteStringCollection.Contains(s) == false)
                        throw new FormatException("AutoCompleteFormatException!");
                    return s;
                }
                return v;
            }

            public void Autocomplete(KryptonDataGridViewTextBoxEditingControl Tb, AutoCompleteStringCollection Ac)
            {
                if (Ac == null)
                {
                    Tb.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
                    Tb.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
                }
                else
                {
                    Tb.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
                    Tb.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
                }
                Tb.AutoCompleteCustomSource = Ac;
            }


        }
    }
}
