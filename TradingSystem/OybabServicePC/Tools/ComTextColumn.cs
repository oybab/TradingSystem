using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Oybab.ServicePC.Tools
{
    internal sealed class ComTextColumn : KryptonDataGridViewTextBoxColumn
    {
        private KryptonDataGridView parent;
        private Func<string, string> action;
        private string[] values;
        private bool IsMultiple;

        public ComTextColumn()
            : base()
        {
            CellTemplate = new ComCell();
        }

        internal void SetParent(KryptonDataGridView parent, Func<string, string> action = null)
        {

            this.parent = parent;
            this.action = action;
        }

        internal void SetValues(string[] values, bool IsMultiple)
        {
            this.values = values;
            this.IsMultiple = IsMultiple;

        }


        public override object Clone()
        {
            ComTextColumn c = (ComTextColumn)base.Clone();
            c.parent = this.parent;
            c.action = this.action;
            c.values = this.values;
            c.IsMultiple = this.IsMultiple;
            c.ValueType = base.ValueType;


            return c;
        }


        internal sealed class ComCell : KryptonDataGridViewTextBoxCell
        {

            private KryptonListBox _listBox;
            private KryptonDataGridView control;
            private Func<string, string> action;
            private KryptonDataGridViewTextBoxEditingControl editControl;
            private bool _isAdded;
            private String[] _values;
            private bool IsMultiple;
            private String _formerValue = "";
            private char sperator = '&';
            private bool IsDisplayAll = false;
            private bool InitialFinish = false;
            private bool ListBoxPositionChange = false;


            public ComCell()
            {
       
            }

            private void InitializeComponent()
            {
                _listBox = new KryptonListBox();

                _listBox.ListBox.PreviewKeyDown += (obj, e) =>
                {
                    if (e.KeyCode == Keys.Enter && _listBox.SelectedIndex != -1)
                    {
                        InsertWord((String)_listBox.SelectedItem);
                        ResetListBox();
                        this.control.CurrentCell.Value = this.editControl.Text = _formerValue = this.EditedFormattedValue.ToString();

                        this.editControl.Focus();
                        this.editControl.SelectionStart = this.editControl.Text.Length;
                    }
                };
                _listBox.ListBox.MouseClick += (sender, e) =>
                {
                    if (_listBox.SelectedIndex != -1)
                    {
                        InsertWord((String)_listBox.SelectedItem);
                        ResetListBox();
                        this.control.CurrentCell.Value = this.editControl.Text = _formerValue = this.EditedFormattedValue.ToString();

                        this.editControl.Focus();
                        this.editControl.SelectionStart = this.editControl.Text.Length;
                    }
                };

                _listBox.ListBox.DrawItem += ListBox_DrawItem;

            }


            private void ListBox_DrawItem(object sender, DrawItemEventArgs e)
            {
               
                e.DrawBackground();
                
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    LinearGradientBrush itemSelected = new LinearGradientBrush(e.Bounds, Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBackColor2(PaletteBackStyle.ButtonListItem, PaletteState.CheckedNormal), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBackColor1(PaletteBackStyle.ButtonListItem, PaletteState.CheckedNormal), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBackColorAngle(PaletteBackStyle.ButtonListItem, PaletteState.CheckedNormal));
                    LinearGradientBrush itemBorderSelected = new LinearGradientBrush(e.Bounds, Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBorderColor1(PaletteBorderStyle.ButtonListItem, PaletteState.CheckedNormal), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBorderColor2(PaletteBorderStyle.ButtonListItem, PaletteState.CheckedNormal), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBorderColorAngle(PaletteBorderStyle.ButtonListItem, PaletteState.CheckedNormal));
                    e.Graphics.FillRectangle(itemSelected, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                    e.Graphics.DrawRectangle(new Pen(itemBorderSelected), e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                }
                else
                {
                    int index = -1;
                    if (_listBox.ListBox.GetItemRectangle(e.Index).Contains(_listBox.ListBox.PointToClient(Control.MousePosition)))
                    {
                        index = e.Index;
                        if (Control.MouseButtons == MouseButtons.Left)
                        {

                            LinearGradientBrush itemTracking = new LinearGradientBrush(e.Bounds, Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBackColor1(PaletteBackStyle.ButtonListItem, PaletteState.Pressed), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBackColor2(PaletteBackStyle.ButtonListItem, PaletteState.Pressed), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBackColorAngle(PaletteBackStyle.ButtonListItem, PaletteState.Pressed));
                            LinearGradientBrush itemBorderSelected = new LinearGradientBrush(e.Bounds, Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBorderColor1(PaletteBorderStyle.ButtonListItem, PaletteState.Pressed), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBorderColor2(PaletteBorderStyle.ButtonListItem, PaletteState.Pressed), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBorderColorAngle(PaletteBorderStyle.ButtonListItem, PaletteState.Pressed));
                            e.Graphics.FillRectangle(itemTracking, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                            e.Graphics.DrawRectangle(new Pen(itemBorderSelected), e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                        }
                        else
                        {
                            LinearGradientBrush itemTracking = new LinearGradientBrush(e.Bounds, Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBackColor1(PaletteBackStyle.ButtonListItem, PaletteState.Tracking), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBackColor2(PaletteBackStyle.ButtonListItem, PaletteState.Tracking), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBackColorAngle(PaletteBackStyle.ButtonListItem, PaletteState.Tracking));
                            LinearGradientBrush itemBorderSelected = new LinearGradientBrush(e.Bounds, Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBorderColor1(PaletteBorderStyle.ButtonListItem, PaletteState.Tracking), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBorderColor2(PaletteBorderStyle.ButtonListItem, PaletteState.Tracking), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBorderColorAngle(PaletteBorderStyle.ButtonListItem, PaletteState.Tracking));
                            e.Graphics.FillRectangle(itemTracking, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                            e.Graphics.DrawRectangle(new Pen(itemBorderSelected), e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                        }

                    }
                    else
                    {
                        e.Graphics.FillRectangle(Brushes.White, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                    }




                }
               
                
                TextRenderer.DrawText(e.Graphics, ((ListBox)sender).Items[e.Index].ToString(), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().currentResourceFont, e.Bounds, Color.Black, TextFormatFlags.Default);


                e.DrawFocusRectangle();
                
                


            }


            private void this_KeyUp(object sender, KeyEventArgs e)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        {

                            if (_listBox.Visible)
                            {
                                ResetListBox();

                            }
                            e.Handled = true;
                            return;
                        }
                    case Keys.Tab:
                    case Keys.Enter:
                    case Keys.Down:
                    case Keys.Up:
                    case Keys.F1:
                        e.Handled = true;
                        return;
                }

                UpdateListBox();

            }



            private void this_KeyDown(object sender, PreviewKeyDownEventArgs e)
            {
                switch (e.KeyCode)
                {
                    case Keys.Tab:
                    case Keys.Enter:
                        {
                            if (_listBox.Visible)
                            {
                                InsertWord((String)_listBox.SelectedItem);
                                ResetListBox();
                                this.editControl.Text = _formerValue = this.EditedFormattedValue.ToString();

                            }
                            else
                            {
                                if (IsMultiple)
                                {
                                    this.control.EndEdit();
                                }
                            }
                           

                            break;
                        }
                    case Keys.Down:
                        {
                            if (_listBox.Visible)
                            {
                                if ((_listBox.SelectedIndex < _listBox.Items.Count - 1))
                                    _listBox.SelectedIndex++;
                                else
                                    _listBox.SelectedIndex = 0;
                            }
                            else
                            {
                                UpdateListBox();
                            }

                            break;
                        }
                    case Keys.Up:
                        {
                            if (_listBox.Visible)
                            {
                                if (_listBox.SelectedIndex > 0)
                                    _listBox.SelectedIndex--;
                                else
                                    _listBox.SelectedIndex = _listBox.Items.Count - 1;
                            }
                            else
                            {
                                UpdateListBox();
                            }

                            break;
                        }
                    case Keys.F1:
                        {
                            if (null != action)
                            {
                                if (_listBox.Visible)
                                {
                                    ResetListBox();
                                }

                                string result = action(this.editControl.Text);
                                ResetListBox();
                                if (null != result)
                                    this.editControl.Text = _formerValue = result;
                            }

                            

                            break;
                        }
                }

            }



            private void ShowListBox()
            {
                if (!_isAdded || ListBoxPositionChange)
                {
                    control.Controls.Add(_listBox);


                    var cellRectangle = control.GetCellDisplayRectangle(control.SelectedCells[0].ColumnIndex, control.SelectedCells[0].RowIndex, true);

                    int offset = this.control.FindForm().Height - 25 - cellRectangle.Y;
                    if (offset > this.control.FindForm().Height / 2)
                    {
                        _listBox.Left = cellRectangle.Left;
                        _listBox.Top = cellRectangle.Top + cellRectangle.Height;
                    }
                    else
                    {
                        _listBox.Left = cellRectangle.Left;
                        _listBox.Top = cellRectangle.Top - _listBox.Height;
                    }

                    _isAdded = true;
                    ListBoxPositionChange = false;

                    _listBox.BringToFront();
                }
                _listBox.Visible = true;

            }

            private void ResetListBox()
            {
                _listBox.Visible = false;
                IsDisplayAll = false;
            }





            private void UpdateListBox()
            {
                if (null == _values || _values.Length == 0)
                    return;

                if (this.EditedFormattedValue.ToString() != _formerValue || this.EditedFormattedValue.ToString() == "" ||  this.EditedFormattedValue.ToString().EndsWith(sperator.ToString()))
                {
                    _formerValue = this.EditedFormattedValue.ToString();
                    String word = GetWord();

                    if (word.Length > 0 || (_formerValue.EndsWith(sperator.ToString()) && _formerValue.Trim(sperator) != ""))
                    {
                        IsDisplayAll = false;

                        string[] Values = SelectedValues;

                        string[] matches = _values.Where(x => x.StartsWith(word) || Values.Contains(x)).Distinct().ToArray();

                        string[] expectValues = SelectedValuesExcept(Values, word);


                        matches = matches.Except(expectValues).ToArray();

                        if (matches.Length > 0)
                        {
                            Display(matches);
                        }
                        else
                        {
                            ResetListBox();

                        }
                    }
                    else
                    {
                        if (!IsDisplayAll)
                        {

                            Display(_values);

                            IsDisplayAll = true;
                        }
                    }
                }
            }


            private void Display(string[] matches)
            {
                _listBox.Items.Clear();
                Array.ForEach(matches.Count() > 100 ? matches.Take(100).ToArray() : matches, x => _listBox.Items.Add(x));
                _listBox.SelectedIndex = 0;
                int oldHeight = _listBox.Height;
                _listBox.Height = 0;
                _listBox.Width = 0;
                _listBox.ScrollAlwaysVisible = false;

                //this.Focus();
                using (Graphics graphics = _listBox.CreateGraphics())
                {
                    int itemWidth = 0;
                    for (int i = 0; i < _listBox.Items.Count; i++)
                    {
                        _listBox.Height += _listBox.GetItemHeight(i);

                        int itemWidthTemp = (int)graphics.MeasureString(((String)_listBox.Items[i]) + "_", this.control.Font).Width;


                        if (itemWidthTemp > itemWidth)
                            itemWidth = itemWidthTemp;
                    }

                    _listBox.Height += 4;
                    // 解决超出索引异常问题
                    if (null == control.SelectedCells || control.SelectedCells.Count == 0)
                        return;
                    var cellRectangle = control.GetCellDisplayRectangle(control.SelectedCells[0].ColumnIndex, control.SelectedCells[0].RowIndex, true);

                    _listBox.Width = (cellRectangle.Width > itemWidth) ? cellRectangle.Width : itemWidth + 20 < this.control.FindForm().Width - cellRectangle.Location.X ? itemWidth + 20 : this.control.FindForm().Width - cellRectangle.Location.X - 20;

                    int offset = control.Height - 25 - cellRectangle.Y;

                    // 上下面显示时
                    if (offset > this.control.Height / 2)
                    {
                        // 超出底部部分处理
                        if (_listBox.Height > offset - 35)
                        {
                            _listBox.ScrollAlwaysVisible = true;
                            _listBox.Height = offset - 35;
                        }
                    }
                    //上面显示时
                    else
                    {
                        // 超出顶部部分处理
                        if (_listBox.Height > this.control.Height - offset - 25)
                        {
                            _listBox.ScrollAlwaysVisible = true;
                            _listBox.Height = this.control.Height - offset - 25;
                        }

                        // 如果高度变化, 因为从下到上, 所以得重新绘制它
                        if (_listBox.Height > 0 && _listBox.Height != oldHeight)
                            ListBoxPositionChange = true;
                    }
                }
                ShowListBox();
            }

            private String GetWord()
            {
                if (this.EditedFormattedValue.ToString() == "" || this.EditedFormattedValue.ToString().EndsWith(sperator.ToString()))
                    return "";
                String text = this.EditedFormattedValue.ToString();
                int pos = this.EditedFormattedValue.ToString().Length - 1;

                int posStart = text.LastIndexOf(sperator, (pos < 1) ? 0 : pos - 1);
                posStart = (posStart == -1) ? 0 : posStart + 1;
                int posEnd = text.IndexOf(sperator, pos);
                posEnd = (posEnd == -1) ? text.Length : posEnd;

                int length = ((posEnd - posStart) < 0) ? 0 : posEnd - posStart;

                return text.Substring(posStart, length);
            }

            private void InsertWord(String newTag)
            {
                if (this.EditedFormattedValue.ToString() == "")
                {
                    this.editControl.Text = newTag;
                    this.editControl.SelectionStart = newTag.Length;
                }
                String text = this.EditedFormattedValue.ToString();
                int pos = this.EditedFormattedValue.ToString().Length - 1;

                int posStart = text.LastIndexOf(sperator, (pos < 1) ? 0 : this.EditedFormattedValue.ToString() .EndsWith(sperator.ToString()) ?  pos : pos - 1);
                posStart = (posStart == -1) ? 0 : posStart + 1;
                int posEnd = text.IndexOf(sperator, pos);

                String firstPart = text.Substring(0, posStart) + newTag;

                String updatedText = firstPart + ((posEnd == -1) ? "" : "");


                this.editControl.Text = updatedText;

                this.editControl.SelectionStart = firstPart.Length;
            }

            internal String[] Values
            {
                get
                {
                    return _values;
                }
                set
                {
                    _values = value;
                }
            }

            internal string[] SelectedValues
            {
                get
                {
                    return this.EditedFormattedValue.ToString().Split(new char[] { sperator }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            internal string[] SelectedValuesExcept(string[] Values, string word)
            {
                var result = Values.Where(x => x != word).ToList();
                if (Values.Where(x => x == word).Count() > 1)
                    result.Add(word);
                return result.ToArray();
            }

            public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
            {
                // Set the value of the editing control to the current cell value.
                base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
                ComTextColumn col = (ComTextColumn)OwningColumn;
                KryptonDataGridViewTextBoxEditingControl Tb = (KryptonDataGridViewTextBoxEditingControl)DataGridView.EditingControl;
                Tb.PreviewKeyDown += this_KeyDown;
                Tb.KeyUp += this_KeyUp;

                this.Values = col.values;
                this.IsMultiple = col.IsMultiple;
                this.control = col.parent;
                this.action = col.action;
                this.editControl = Tb;

                InitializeComponent();
                ResetListBox();
                _isAdded = false;
                _formerValue = null;

                _listBox.Top = 99999;
                _listBox.Left = 99999;
                _listBox.Visible = true;


                if (!IsMultiple)
                {
                    this.sperator = '▇';
                }

                this.editControl.GotFocus += editControl_GotFocus;
                
            }

            

            private void editControl_GotFocus(object sender, EventArgs e)
            {
                if (!InitialFinish)
                {
                    _listBox.Visible = false;
                    InitialFinish = true;
                }
            }


            public override void DetachEditingControl()
            {
                ResetListBox();
                base.DetachEditingControl();
                KryptonDataGridViewTextBoxEditingControl Tb = (KryptonDataGridViewTextBoxEditingControl)DataGridView.EditingControl;
                ComTextColumn col = (ComTextColumn)OwningColumn;


                Tb.PreviewKeyDown -= this_KeyDown;
                Tb.KeyUp -= this_KeyUp;

                this.editControl.GotFocus -= editControl_GotFocus;
                this.Values = null;
                this.IsMultiple = false;
                this.control = null;
                this.editControl = null;
                this._listBox = null;
                this.action = null;
                InitialFinish = false;
                
            }


        }

    }



}
