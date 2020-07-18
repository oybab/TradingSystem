using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Oybab.Res;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Oybab.ServicePC.Tools
{
    internal sealed class ComTextBox : KryptonTextBox
    {
        private KryptonListBox _listBox;
        private bool _isAdded;
        private String[] _values;
        private String _formerValue = "";
        private char sperator = '&';
        private bool IsMultiple = false;
        private bool IsDisplayAll = false;
        private bool InitialFinish = false;
        private bool ListBoxPositionChange = false;



        public ComTextBox()
        {
            InitializeComponent();

            ResetListBox();
            _listBox.Top = 99999;
            _listBox.Left = 99999;
            _listBox.Visible = true;

        }

        private void InitializeComponent()
        {
            _listBox = new KryptonListBox();
            

            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.this_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.this_KeyUp);
            

            _listBox.ListBox.KeyDown += (obj, e) =>
                {
                    if (e.KeyCode == Keys.Enter && _listBox.SelectedIndex != -1)
                    {
                        InsertWord((String)_listBox.SelectedItem);
                        ResetListBox();
                        _formerValue = this.Text;
                        UpdateListBox();
                        this.Focus();
                        this.SelectionStart = this.Text.Length;
                    }
                };


            _listBox.ListBox.MouseClick += (sender, e) =>
            {
                if (_listBox.SelectedIndex != -1)
                {
                    InsertWord((String)_listBox.SelectedItem);
                    ResetListBox();
                    _formerValue = this.Text;
                    UpdateListBox();
                    this.Focus();
                    this.SelectionStart = this.Text.Length;
                }
            };

            _listBox.ListBox.DrawItem += ListBox_DrawItem;

            this.LostFocus += (sender, e) =>
            {
                if (!this._listBox.ListBox.Focused)
                    ResetListBox();
            };

            this.GotFocus += (sender, e) =>
            {
                if (!InitialFinish)
                {
                    _listBox.Visible = false;
                    InitialFinish = true;
                }
            };


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


        private void ShowListBox()
        {
            if (!_isAdded || ListBoxPositionChange)
            {
                Parent.Controls.Add(_listBox);
                Parent.SizeChanged -= Parent_SizeChanged;
                Parent.SizeChanged += Parent_SizeChanged;


                int offset = this.FindForm().Height - 25 - this.Location.Y;
                if (offset > this.FindForm().Height / 2)
                {
                    _listBox.Left = this.Left;
                    _listBox.Top = this.Top + this.Height;
                }
                else
                {
                    _listBox.Left = this.Left;
                    _listBox.Top = this.Top - _listBox.Height;
                }

                _isAdded = true;
                ListBoxPositionChange = false;


                _listBox.BringToFront();
            }
            _listBox.Visible = true;

        }

        private void Parent_SizeChanged(object sender, EventArgs e)
        {
            ResetListBox();
            _isAdded = false;
        }

        private void ResetListBox()
        {
            _listBox.Visible = false;
            IsDisplayAll = false;
        }

        private void this_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {

                if (_listBox.Visible)
                {
                    ResetListBox();
                    e.Handled = true;
                    return;
                }
            }
            UpdateListBox();
        }
        
        private void this_KeyDown(object sender, KeyEventArgs e)
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
                            _formerValue = this.Text;
                            e.SuppressKeyPress = true;
                            e.Handled = true;
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
                        e.Handled = true;
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
                        e.Handled = true;
                        break;
                    }
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Tab:
                case Keys.Enter:
                case Keys.Escape:
                    return true;
                default:
                    return base.IsInputKey(keyData);
            }
        }

        private void UpdateListBox()
        {
            if (null == _values || _values.Length == 0)
                return;

            if (this.Text != _formerValue || this.Text == "" || this.Text.EndsWith(sperator.ToString()))
            {
                _formerValue = this.Text;
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

                        Display(_values.ToArray());

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


            using (Graphics graphics = _listBox.CreateGraphics())
            {
                int itemWidth = 0;

                for (int i = 0; i < _listBox.Items.Count; i++)
                {
                    SizeF size = graphics.MeasureString(((String)_listBox.Items[i]) + "_", this.Font);
                    //SizeF size = TextRenderer.MeasureText(graphics, ((String)_listBox.Items[i]) + "_", this.Font);
                    _listBox.Height += _listBox.GetItemHeight(i);
                    // it item width is larger than the current one
                    // set it to the new max item width
                    // GetItemRectangle does not work for me
                    // we add a little extra space by using '_'
                    int itemWidthTemp = (int)size.Width + 2;

                    if (itemWidthTemp > itemWidth)
                        itemWidth = itemWidthTemp;

                }

                _listBox.Height += 4;
                _listBox.Width = (this.Width > itemWidth) ? this.Width : itemWidth + 20;
                int offset = this.FindForm().Height - 25 - this.Location.Y;

                // 上下面显示时
                if (offset > this.FindForm().Height / 2)
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
                    if (_listBox.Height > this.FindForm().Height - offset - 25)
                    {
                        _listBox.ScrollAlwaysVisible = true;
                        _listBox.Height = this.FindForm().Height - offset - 25;
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
            if (this.Text == "" || this.Text.EndsWith(sperator.ToString()))
                return "";

            String text = this.Text;
            int pos = this.SelectionStart;

            int posStart = text.LastIndexOf(sperator, (pos < 1) ? 0 : pos - 1);
            posStart = (posStart == -1) ? 0 : posStart + 1;
            int posEnd = text.IndexOf(sperator, pos);
            posEnd = (posEnd == -1) ? text.Length : posEnd;

            int length = ((posEnd - posStart) < 0) ? 0 : posEnd - posStart;

            return text.Substring(posStart, length);
        }

        private void InsertWord(String newTag)
        {
            if (this.Text == "")
            {
                this.Text = newTag;
                this.SelectionStart = newTag.Length;
            }
            String text = this.Text;
            int pos = this.Text.Length - 1;

            int posStart = text.LastIndexOf(sperator, (pos < 1) ? 0 : this.Text.ToString() .EndsWith(sperator.ToString()) ?  pos : pos - 1);
            posStart = (posStart == -1) ? 0 : posStart + 1;
            int posEnd = text.IndexOf(sperator, pos);

            String firstPart = text.Substring(0, posStart) + newTag;
            String updatedText = firstPart + ((posEnd == -1) ? "" : "");


            this.Text = updatedText;
            this.SelectionStart = firstPart.Length;
        }

        internal void SetValues(string[] Values, bool IsMultiple)
        {
            this.Values = Values;
            this.IsMultiple = IsMultiple;

            if (!IsMultiple)
            {
                this.sperator = '▇';
            }
        }

        private string[] Values
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
                return Text.Split(new char[] { sperator }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        internal string[] SelectedValuesExcept(string[] Values, string word)
        {
            var result = Values.Where(x => x != word).ToList();
            if (Values.Where(x => x == word).Count() > 1)
                result.Add(word);
            return result.ToArray();
        }

    


    }
}



