using Oybab.DAL;
using Oybab.TradingSystemX.VM.Commands;
using Oybab.TradingSystemX.VM.ModelsForViews;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using Oybab.TradingSystemX.Tools;
using System.Threading.Tasks;
using Oybab.TradingSystemX.VM.ViewModels.Pages.Controls;

namespace Oybab.TradingSystemX.VM.ViewModels.Controls
{
    internal sealed class RequestViewModel : ViewModelBase
    {
        private Xamarin.Forms.View _element;
        private DetailsModel model;
        private SelectedViewModel SelectedViewModel;

        private Xamarin.Forms.StackLayout _spList;
        private Xamarin.Forms.ControlTemplate _ctControlTemplate;

        internal RequestViewModel(Xamarin.Forms.View element, SelectedViewModel SelectedViewModel, Xamarin.Forms.StackLayout spList, Xamarin.Forms.ControlTemplate ctControlTemplate)
        {
            this._element = element;
            this.SelectedViewModel = SelectedViewModel;

            this._spList = spList;
            this._ctControlTemplate = ctControlTemplate;

        }



        private ObservableCollection<RequestModel> _requestList = new ObservableCollection<RequestModel>();
        /// <summary>
        /// 请求列表
        /// </summary>
        public ObservableCollection<RequestModel> RequestList
        {
            get { return _requestList; }
            set
            {
                _requestList = value;
                OnPropertyChanged("RequestList");
            }
        }
        /// <summary>
        /// 初始化需求
        /// </summary>
        internal void InitialRequest(DetailsModel model)
        {
            this.model = model;
            ClearList();

            if (model.IsPackage)
                IsPackage = true;
            else
                IsPackage = false;

            List<Request> lists = Resources.Instance.Requests.Where(x => x.IsEnable == 1).OrderByDescending(x=>x.Order).ThenByDescending(x=>x.RequestId).ToList();

            RequestModel requestIsPackage = new RequestModel();

            foreach (var item in lists)
            {
                RequestModel request = new RequestModel() { IsSelected = false, Request = item };

                if (!string.IsNullOrWhiteSpace(model.OrderDetail.Request))
                {
                    if (model.OrderDetail.Request.Split(',').Contains(item.RequestId.ToString()))
                    {
                        request.IsSelected = true;
                    }
                }

                AddList(request);
            }

            IsShow = true;
        }




        /// <summary>
        /// 清空
        /// </summary>
        private void ClearList()
        {

            foreach (Xamarin.Forms.TemplatedView item in this._spList.Children.Reverse())
            {
                item.BindingContext = null;
                item.IsVisible = false;
                if (!tempTemplateViewList.Contains(item))
                    tempTemplateViewList.Push(item);
            }

            RequestList.Clear();

        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Index"></param>
        private void AddList(RequestModel model, int Index = -1)
        {

            model.GoCommand = SelectRequest;

          

            AddSelectedItem(model, Index);

            if (Index != -1)
                RequestList.Insert(Index, model);
            else
                RequestList.Add(model);

        }


        private Stack<Xamarin.Forms.TemplatedView> tempTemplateViewList = new Stack<Xamarin.Forms.TemplatedView>();
        /// <summary>
        /// 添加已选对象
        /// </summary>
        /// <param name="item"></param>
        private void AddSelectedItem(RequestModel item, int Index = -1)
        {


            Xamarin.Forms.TemplatedView view = null;
            if (tempTemplateViewList.Count > 0)
            {
                view = tempTemplateViewList.Pop();
                view.IsVisible = true;
                view.BindingContext = item;
            }
            else
            {
                view = new Xamarin.Forms.TemplatedView();
                view.ControlTemplate = _ctControlTemplate;

                view.BindingContext = item;
                _spList.Children.Add(view);
            }


        }

        /// <summary>
        /// 删除已选
        /// </summary>
        /// <param name="item"></param>
        private void RemoveSelected(RequestModel item)
        {
         

            Xamarin.Forms.TemplatedView _view = null;
            foreach (Xamarin.Forms.TemplatedView items in this._spList.Children)
            {
                if (items.BindingContext == item)
                {
                    _view = items;
                    break;
                }
            }

            if (null != _view)
            {
                _view.BindingContext = null;
                if (!tempTemplateViewList.Contains(_view))
                    tempTemplateViewList.Push(_view);
                _view.IsVisible = false;
            }

               RequestList.Remove(item);
        }



        private bool _isShow = false;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow
        {
            get { return _isShow; }
            set
            {
                _isShow = value;
                if (_isShow == true)
                {
                    NavigationPath.Instance.OpenPanel();
                }
                else
                {
                    if (RequestList.Count > 0)
                        ClearList();
                }
                OnPropertyChanged("IsShow");
            }
        }


        /// <summary>
        /// 选择需求
        /// </summary>
        private RelayCommand _selectRequest;
        public Xamarin.Forms.Command SelectRequest
        {
            get
            {
                return _selectRequest ?? (_selectRequest = new RelayCommand(param =>
                {

                    RequestModel model = param as RequestModel;
                    if (null == model)
                        return;

                    bool IsSelected = false;
                    if (model.IsSelected)
                        IsSelected = false;
                    else
                        IsSelected = true;

                    List<string> seleced = RequestList.Where(x => x.IsSelected).Select(x => x.Request.RequestId.ToString()).ToList();

                    if (seleced.Count >= 3 && IsSelected)
                    {
                        return;
                    }

                    foreach (var item in RequestList)
                    {
                        if (model.Request.RequestId == item.Request.RequestId)
                            item.IsSelected = IsSelected;

                    }



                }));
            }
        }







        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {


        }






        private bool _isPackage = false;
        /// <summary>
        /// 打包
        /// </summary>
        public bool IsPackage
        {
            get { return _isPackage; }
            set
            {
                _isPackage = value;
                OnPropertyChanged("IsPackage");
            }
        }

        private string _oldRoomNo = "";
        /// <summary>
        /// 新雅座编号
        /// </summary>
        public string OldRoomNo
        {
            get { return _oldRoomNo; }
            set
            {
                _oldRoomNo = value;
                OnPropertyChanged("OldRoomNo");
            }
        }



        /// <summary>
        /// 确定按钮
        /// </summary>
        private RelayCommand _okCommand;
        public Xamarin.Forms.Command OKCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new RelayCommand(param =>
                {
                    SelectedViewModel.IsLoading = true;

                    Task.Run(async () =>
                    {

                        await ExtX.WaitForLoading();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            List<string> seleced = RequestList.Where(x => x.IsSelected).Select(x => x.Request.RequestId.ToString()).ToList();

                            if (seleced.Count > 0)
                            {
                                this.model.OrderDetail.Request = string.Join(",", seleced);
                                this.model.IsRequest = false;
                            }
                            else
                            {
                                this.model.OrderDetail.Request = null;
                                this.model.IsRequest = false;
                            }

                            this.model.IsPackage = IsPackage;

                            // 关闭面板
                            NavigationPath.Instance.ClosePanels(false);

                            ClearList();

                            this.IsShow = false;

                            SelectedViewModel.IsLoading = false;

                            

                        });
                    });

                }));
            }
        }


        /// <summary>
        /// 取消按钮
        /// </summary>
        private RelayCommand _cancelCommand;
        public Xamarin.Forms.Command CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(param =>
                {
                    SelectedViewModel.IsLoading = true;

                    Task.Run(async () =>
                    {

                        await ExtX.WaitForLoading();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            // 关闭面板
                            NavigationPath.Instance.ClosePanels(false);

                            ClearList();

                            this.IsShow = false;

                            SelectedViewModel.IsLoading = false;

                        });
                    });
                }));
            }
        }







    }
}
