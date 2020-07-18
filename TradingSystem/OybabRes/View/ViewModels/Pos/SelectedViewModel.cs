using Oybab.DAL;
using Oybab.Res.Tools;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Converters;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using Oybab.Res.View.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Oybab.Res.View.ViewModels.Pos.Controls
{
    public sealed class SelectedViewModel : ViewModelBase
    {
        private UIElement _element;

        internal long RoomId = -1;
        internal long StartTimeTemp = 0;
        internal long EndTimeTemp = 0;
        internal Action Save;
        internal Action Checkout;


        internal SelectedViewModel(UIElement element)
        {
            this._element = element;
        }



        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init()
        {
           
        }


        private ObservableCollection<DetailsModel> _currentSelectedList = new ObservableCollection<DetailsModel>();
        /// <summary>
        /// 当前已选列表
        /// </summary>
        public ObservableCollection<DetailsModel> CurrentSelectedList
        {
            get { return _currentSelectedList; }
            set
            {
                _currentSelectedList = value;
                OnPropertyChanged("CurrentSelectedList");
            }
        }

        private ObservableCollection<DetailsModel> _currentSelectedListNew = new ObservableCollection<DetailsModel>();
        /// <summary>
        /// 当前已选列表
        /// </summary>
        public ObservableCollection<DetailsModel> CurrentSelectedListNew
        {
            get { return _currentSelectedListNew; }
            set
            {
                _currentSelectedListNew = value;
                OnPropertyChanged("CurrentSelectedListNew");
            }
        }




        private int _currentPage = 0;
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }


        private int _totalPage = 0;
        /// <summary>
        /// 总页
        /// </summary>
        public int TotalPage
        {
            get { return _totalPage; }
            set
            {
                _totalPage = value;
                OnPropertyChanged("TotalPage");
            }
        }


        private int _currentIndex = -1;
        /// <summary>
        /// 当前模型索引
        /// </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                _currentIndex = value;
                OnPropertyChanged("CurrentIndex");
            }
        }

        /// <summary>
        /// 重置编号
        /// </summary>
        private void ResetNo()
        {
            int no = 1;

            int page = 0;

            if (CurrentPage < TotalPage)
            { 
                page = (TotalPage - 1 - CurrentPage) * PosLine.ListCountSelected + (CurrentSelectedList.Count % PosLine.ListCountSelected);
            }

            for (int i = CurrentSelectedListNew.Count; i > 0; i--)
            {
                CurrentSelectedListNew[i - 1].No = no + page;
                ++no;
            }
        }

        /// <summary>
        /// 下一页
        /// </summary>
        private void GoNextPage()
        {
            if (CurrentPage < TotalPage)
            {
                ++CurrentPage;
                ReloadNewList();
            }
            
        }

        /// <summary>
        /// 上一页
        /// </summary>
        private void GoLastPage()
        {
            if (1 < CurrentPage)
            {
                --CurrentPage;
                ReloadNewList();
            }
        }

        /// <summary>
        /// 重置页
        /// </summary>
        internal void ResetPage()
        {
            CurrentPage = 1;
            CalcTotalPage();
            ReloadNewList();
            
        }

        /// <summary>
        /// 计算并重置页
        /// </summary>
        internal void CalcAndResetPage() {
            CalcTotalPage();
            if (CurrentPage > TotalPage)
            {
                CurrentPage = TotalPage;
                ReloadNewList();
            }
            ResetNo();
        }

        /// <summary>
        /// 计算页
        /// </summary>
        private void CalcTotalPage() {
            
            TotalPage = (int)((CurrentSelectedList.Count - 1 + PosLine.ListCountSelected) / PosLine.ListCountSelected);
        }

        /// <summary>
        /// 选择下一个
        /// </summary>
        private void GoDown()
        {
            if (CurrentSelectedListNew.Count - 1 > CurrentIndex)
            {
                CurrentSelectedListNew[CurrentIndex].IsNavigated = false;

                ++CurrentIndex;
                CurrentSelectedListNew[CurrentIndex].IsNavigated = true;
                CurrentSelectedListNew[CurrentIndex].NavigateMode = 0;
            }
        }

        /// <summary>
        /// 选择上一个
        /// </summary>
        private void GoUp()
        {
            if (CurrentIndex > 0 && CurrentSelectedListNew.Count > 0)
            {
                CurrentSelectedListNew[CurrentIndex].IsNavigated = false;

                --CurrentIndex;
                CurrentSelectedListNew[CurrentIndex].IsNavigated = true;
                CurrentSelectedListNew[CurrentIndex].NavigateMode = 0;
            }
        }

        /// <summary>
        /// 选择单价
        /// </summary>
        private void GoRight()
        {
            if (CurrentSelectedListNew.Count > 0 && CurrentSelectedListNew[CurrentIndex].NavigateMode != 1 && CurrentSelectedListNew[CurrentIndex].IsChangePrice)
            {
                CurrentSelectedListNew[CurrentIndex].NavigateMode = 1;
            }
        }

        /// <summary>
        /// 选择数量
        /// </summary>
        private void GoLeft()
        {
            if (CurrentSelectedListNew.Count > 0 && CurrentSelectedListNew[CurrentIndex].NavigateMode != 0)
            {
                CurrentSelectedListNew[CurrentIndex].NavigateMode = 0;
            }
        }

        /// <summary>
        /// 重新加载列表
        /// </summary>
        private void ReloadNewList()
        {
            CurrentSelectedListNew.Clear();
            foreach (var item in CurrentSelectedList.Skip(PosLine.ListCountSelected * (CurrentPage - 1)).Take(PosLine.ListCountSelected))
            {
                item.IsNavigated = false;
                item.NavigateMode = 0;
                CurrentSelectedListNew.Add(item);
            }

            if (CurrentSelectedListNew.Count > 0)
            {
                CurrentIndex = 0;
                CurrentSelectedListNew[CurrentIndex].IsNavigated = true;
                CurrentSelectedListNew[CurrentIndex].NavigateMode = 0;
            }
            CalcAndResetPage();
        }

        /// <summary>
        /// 处理KEY
        /// </summary>
        /// <param name="args"></param>
        internal void HandleKey(KeyEventArgs args)
        {
            // 如果是功能(如上下左右,换页)
            if (args.Key >= Key.PageUp && args.Key <= Key.Down)
            {
                switch (args.Key)
                {
                    case Key.Right:
                        GoRight();
                        break;
                    case Key.Left:
                        GoLeft();
                        break;
                    case Key.Up:
                        GoUp();
                        break;
                    case Key.Down:
                        GoDown();
                        break;
                    case Key.PageUp:
                        GoLastPage();
                        break;
                    case Key.PageDown:
                        GoNextPage();
                        break;
                }
            }
            // 如果要增加数量或减少数量
            else if (args.Key == Key.Add || args.Key == Key.Subtract || args.Key == Key.Delete)
            {
                if (CurrentSelectedListNew.Count > 0 && CurrentSelectedListNew[CurrentIndex].NavigateMode == 0)
                {
                    if (args.Key == Key.Delete) {
                        var lastIndex = CurrentIndex == 0 ? 1 : CurrentIndex;
                        var lastPage = CurrentPage;

                        CurrentSelectedListNew[CurrentIndex].Delete.Execute(CurrentSelectedListNew[CurrentIndex]);

                        if (this.CurrentSelectedListNew.Count > 1 && CurrentPage == lastPage)
                        {
                            bool findIt = false;
                            for (int i = CurrentSelectedListNew.Count - 1; i >= 0 ; i--)
                            {
                                if (i < lastIndex && !findIt)
                                {
                                    findIt = true;
                                    CurrentIndex = i;

                                    CurrentSelectedListNew[i].IsNavigated = true;
                                    CurrentSelectedListNew[i].NavigateMode = 0;
                                }
                                else
                                {
                                    CurrentSelectedListNew[i].IsNavigated = false;
                                    CurrentSelectedListNew[i].NavigateMode = 0;
                                }
                            }
                        }
                        //CalcAndResetPage();
                    }
                    else {

                        double count = double.Parse(CurrentSelectedListNew[CurrentIndex].CountPos);

                        if (args.Key == Key.Add)
                        {
                            if (count < 99999)
                                CurrentSelectedListNew[CurrentIndex].CountPos = (++count).ToString();

                            if (count == 0)
                                CurrentSelectedListNew[CurrentIndex].CountPos = (1).ToString();


                        }
                        else if (args.Key == Key.Subtract)
                        {
                            if (count <= 1 && Common.GetCommon().IsDecreaseProductCount())
                            {
                                if (count == 1)
                                    CurrentSelectedListNew[CurrentIndex].CountPos = (-1).ToString();
                                else
                                    CurrentSelectedListNew[CurrentIndex].CountPos = (--count).ToString();
                            }
                            if (count > 1)
                                CurrentSelectedListNew[CurrentIndex].CountPos = (--count).ToString();

                        }
                            
                    }
                }
            }
            // 如果是改动数字
            else if ((args.Key >= Key.D0 && args.Key <= Key.D9) || (args.Key >= Key.NumPad0 && args.Key <= Key.NumPad9) || args.Key == Key.OemPeriod || args.Key == Key.Decimal || args.Key == Key.Back)
            {
                if (CurrentSelectedListNew.Count > 0)
                {
                    if (CurrentSelectedListNew[CurrentIndex].NavigateMode == 0)
                    {
                        if (args.Key == Key.Back)
                        {

                            string countStr = CurrentSelectedListNew[CurrentIndex].CountPos;
                            if (countStr.Length > 1)
                                CurrentSelectedListNew[CurrentIndex].CountPos = countStr.Remove(countStr.Length - 1);
                            else
                                CurrentSelectedListNew[CurrentIndex].CountPos = "0";

                        }
                        else
                        {
                            string keyChar = Common.GetCommon().GetStrFromKey(args.Key);

                            string[] split = CurrentSelectedListNew[CurrentIndex].CountPos.Split('.');
                            if (split.Length > 1)
                            {
                                if (keyChar == "." || split[1].Length >= 3)
                                    return;
                            }
                            else if (CurrentSelectedListNew[CurrentIndex].CountPos.Length >= 8)
                            {
                                return;
                            }

                            if (CurrentSelectedListNew[CurrentIndex].CountPos == "0" && keyChar != ".")
                                CurrentSelectedListNew[CurrentIndex].CountPos = keyChar;
                            else
                                CurrentSelectedListNew[CurrentIndex].CountPos += keyChar;
                        }
                    }
                    else if (CurrentSelectedListNew[CurrentIndex].NavigateMode == 1)
                    {
                        if (args.Key == Key.Back)
                        {

                            string unitPriceStr = CurrentSelectedListNew[CurrentIndex].UnitPricePos;
                            if (unitPriceStr.Length > 1)
                                CurrentSelectedListNew[CurrentIndex].UnitPricePos = unitPriceStr.Remove(unitPriceStr.Length - 1);
                            else
                                CurrentSelectedListNew[CurrentIndex].UnitPricePos = "0";

                        }
                        else
                        {
                            string keyChar = Common.GetCommon().GetStrFromKey(args.Key);

                            string[] split  = CurrentSelectedListNew[CurrentIndex].UnitPricePos.Split('.');
                            if (split.Length > 1)
                            {
                                if (keyChar == "." || split[1].Length >= 2)
                                    return; 
                            }else if (CurrentSelectedListNew[CurrentIndex].UnitPricePos.Length >= 8)
                            {
                                return;
                            }

                            if (CurrentSelectedListNew[CurrentIndex].UnitPricePos == "0" && keyChar != ".")
                                CurrentSelectedListNew[CurrentIndex].UnitPricePos = keyChar;
                            else
                                CurrentSelectedListNew[CurrentIndex].UnitPricePos += keyChar;
                        }
                    }
                }
            }
            else if (args.Key == Key.Enter)
            {
                // 回车结账
                if (this.CurrentSelectedList.Count > 0)
                {
                    CheckoutCommand.Execute(null);
                }
            }


        }



        private string _roomNo = "";
        /// <summary>
        /// 雅座编号
        /// </summary>
        public string RoomNo
        {
            get { return _roomNo; }
            set
            {
                _roomNo = value;
                OnPropertyChanged("RoomNo");
            }
        }


        private double _totalPrice = 0;
        /// <summary>
        /// 总额
        /// </summary>
        public double TotalPrice
        {
            get { return _totalPrice; }
            set
            {
                _totalPrice = value;
                OnPropertyChanged("TotalPrice");
            }
        }


        private int _saveMode = 0;
        /// <summary>
        /// 保存类型(0不显示1保存2结账)
        /// </summary>
        public int SaveMode
        {
            get { return _saveMode; }
            set
            {
                _saveMode = value;
                OnPropertyChanged("SaveMode");
            }
        }


        private int _hasAddress = 0;
        /// <summary>
        /// 是否有地址(0不显示1显示2选中)
        /// </summary>
        public int HasAddress
        {
            get { return _hasAddress; }
            set
            {
                _hasAddress = value;
                OnPropertyChanged("HasAddress");
            }
        }



        private bool _roomDisplay = false;
        /// <summary>
        /// 是否显示雅座
        /// </summary>
        public bool RoomDisplay
        {
            get { return _roomDisplay; }
            set
            {
                _roomDisplay = value;
                OnPropertyChanged("RoomDisplay");
            }
        }

        private bool _roomTimeChange = false;
        /// <summary>
        /// 雅座时间是否变更了
        /// </summary>
        public bool RoomTimeChange
        {
            get { return _roomTimeChange; }
            set
            {
                _roomTimeChange = value;
                OnPropertyChanged("RoomTimeChange");
            }
        }

        


        private int _roomType = 0;
        /// <summary>
        /// 雅座类型(是否按时间收费)
        /// </summary>
        public int RoomType
        {
            get { return _roomType; }
            set
            {
                _roomType = value;
                OnPropertyChanged("RoomType");
            }
        }


        private double _roomPrice = 0;
        /// <summary>
        /// 雅座价格
        /// </summary>
        public double RoomPrice
        {
            get { return _roomPrice; }
            set
            {
                _roomPrice = value;
                OnPropertyChanged("RoomPrice");
            }
        }



        private string _roomTime = "";
        /// <summary>
        /// 雅座时间
        /// </summary>
        public string RoomTime
        {
            get { return _roomTime; }
            set
            {
                _roomTime = value;
                OnPropertyChanged("RoomTime");
            }
        }


        
        /// <summary>
        /// 更改时间
        /// </summary>
        private RelayCommand _changeTime;
        public ICommand ChangeTime
        {
            get
            {
                if (_changeTime == null)
                {
                    _changeTime = new RelayCommand(param =>
                    {
                        SelectedViewModel model = param as SelectedViewModel;

                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }
                        _element.RaiseEvent(new BoxRoutedEventArgs(PublicEvents.BoxEvent, null, null, null, BoxType.ChangeTime, model));
                    });
                }
                return _changeTime;
            }
        }


        /// <summary>
        /// 地址
        /// </summary>
        private RelayCommand _addressCommand;
        public ICommand AddressCommand
        {
            get
            {
                if (_addressCommand == null)
                {
                    _addressCommand = new RelayCommand(param =>
                    {
                        SelectedViewModel model = param as SelectedViewModel;

                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }

                        _element.RaiseEvent(new BoxRoutedEventArgs(PublicEvents.BoxEvent, null, null, null, BoxType.Address, model));
                    });
                }
                return _addressCommand;
            }
        }


        /// <summary>
        /// 保存
        /// </summary>
        private RelayCommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(param =>
                    {
                        Save();
                    });
                }
                return _saveCommand;
            }
        }




        /// <summary>
        /// 结账
        /// </summary>
        private RelayCommand _checkoutCommand;
        public ICommand CheckoutCommand
        {
            get
            {
                if (_checkoutCommand == null)
                {
                    _checkoutCommand = new RelayCommand(param =>
                    {
                        Checkout();
                    });
                }
                return _checkoutCommand;
            }
        }



    }
}
