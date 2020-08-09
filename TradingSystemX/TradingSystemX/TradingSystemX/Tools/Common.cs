using Oybab.Res;
using Oybab.Res.Server.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Oybab.TradingSystemX.Server;
using Oybab.Res.Exceptions;
using Oybab.DAL;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Oybab.TradingSystemX.VM.DService;
using Xamarin.Essentials;
using System.Linq;
using System.Security.Cryptography;
using System.IO;

namespace Oybab.TradingSystemX.Tools
{
    /// <summary>
    /// 常用
    /// </summary>
    public sealed class Common
    {

        #region Instance
        private Common()
        {
        }

        private static readonly Lazy<Common> _instance = new Lazy<Common>(() => new Common());
        public static Common Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion Instance


        /// <summary>
        /// 读取还原
        /// </summary>
        internal async Task ReadBak()
        {
            await Restore.Instance.ReadBak();
        }

        /// <summary>
        /// 写入还原
        /// </summary>
        internal async Task SetBak()
        {
            await Restore.Instance.WriteBak();
        }


        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="adminNo"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        internal async Task<ResultModel> Load(string adminNo, string password)
        {
            ResultModel result = new ResultModel();


            //新请求
            if (!Resources.Instance.IsSessionExists())
            {
                result = await OperatesService.Instance.NewRequest(adminNo, password);
            }



            return result;
        }




        /// <summary>
        /// 格式化
        /// </summary>
        /// <returns></returns>
        internal string GetFormat()
        {
            return "\n";

        }


        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> Close()
        {
            return await OperatesService.Instance.ServiceClose();
        }



        private int Count = 0; //错误次数



        /// <summary>
        /// 检测并提示用户(非循环)!
        /// </summary>
        /// <param name="alert"></param>
        /// <param name="close"></param>
        internal async Task CheckAndAlertOnce(Action<string> alert, Action close = null, bool IsAuto = true)
        {
            
                //连接到服务器检测会话
                bool IsSuccess = false;
                string message = null;

               
                IsSuccess = false;

               
                    try
                    {
                        IsSuccess = await OperatesService.Instance.ServiceSession(true);
                    }
                    catch (Exception ex)
                    {
                        if (ex is OybabException)
                            message = ex.Message;
                        ExceptionPro.ExpLog(ex);
                    }
      



                if (OperatesService.Instance.IsExpired || OperatesService.Instance.IsAdminUsing)
                {
                    Count = 3;
                    Session.Instance.ChangeInterval(false);
                }
                else if (!IsSuccess)
                {
                    ++Count;
                    Session.Instance.ChangeInterval(false);
                }
                else
                {
                    Count = 0;
                    Session.Instance.ChangeInterval(true);
                }


                if (Count >= 3)
                {
                    if (null != alert)
                        alert(message);
                }
                else
                {
                    if (null != close)
                        close();
                }
            
        }




        /// <summary>
        /// 是否允许该平台 1为m100: 电脑, 2为m200: 平板, 3为m300: 手机
        /// </summary>
        /// <returns></returns>
        internal bool IsAllowPlatform(int code)
        {
            Admin model = Resources.Instance.AdminModel;

            string platformCode = "?";
            if (code == 1)
                platformCode = "m100";
            else if (code == 2)
                platformCode = "m200";
            else if (code == 3)
                platformCode = "m300";


            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains(platformCode)))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }

        /// <summary>
        /// 是否允许打开财务日志
        /// </summary>
        /// <returns></returns>
        internal bool IsAllowFinancceLog()
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("3500")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }



        /// <summary>
        /// 是否允许打开支出管理
        /// </summary>
        /// <returns></returns>
        internal bool IsAllowImportManager()
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("1200")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }


        /// <summary>
        /// 是否允许打开余额管理
        /// </summary>
        /// <returns></returns>
        internal bool IsAllowBalanceManager()
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("3800")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }

        /// <summary>
        /// 是否允许打开统计
        /// </summary>
        /// <returns></returns>
        internal bool IsAllowStatistic()
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("3600")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }

        /// <summary>
        /// 是否允许收入交易管理
        /// </summary>
        /// <returns></returns>
        internal bool IsIncomeTradingManage()
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p100")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }



        /// <summary>
        /// 是否允许修改金额
        /// </summary>
        /// <returns></returns>
        internal bool IsTemporaryChangePrice()
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p200")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }



        /// <summary>
        /// 是否允许修改支出金额
        /// </summary>
        /// <returns></returns>
        internal bool IsChangeCostPrice()
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p400")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }




        /// <summary>
        /// 是否允许替换包厢
        /// </summary>
        /// <returns></returns>
        internal bool IsReplaceRoom()
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p350")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }



        /// <summary>
        /// 是否允许取消订单
        /// </summary>
        /// <returns></returns>
        internal bool IsCancelOrder(bool IsNew = false)
        {
            if (IsNew)
            {
                return true;
            }
            else
            {
                Admin model = Resources.Instance.AdminModel;

                if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p300")))
                    return false;
                else if (model.Mode == 0)
                    return false;

                return true;
            }

        }


        /// <summary>
        /// 是否允许添加内部账单(主页)
        /// </summary>
        /// <returns></returns>
        internal bool IsAddInnerBill()
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("1000")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }

        /// <summary>
        /// 是否允许添加外部账单
        /// </summary>
        /// <returns></returns>
        internal bool IsAddOuterBill()
        {

            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p900")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }


        /// <summary>
        /// 是否允许删除商品
        /// </summary>
        /// <returns></returns>
        internal bool IsDeleteProduct(bool IsNew = false)
        {
            if (IsNew)
            {
                return true;
            }
            else
            {
                Admin model = Resources.Instance.AdminModel;

                if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p800")))
                    return false;
                else if (model.Mode == 0)
                    return false;

                return true;
            }

        }



        /// <summary>
        /// 是否允许减少产品数量
        /// </summary>
        /// <returns></returns>
        internal bool IsDecreaseProductCount()
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p850")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }



        /// <summary>
        /// 是否允许退回金钱
        /// </summary>
        /// <returns></returns>
        internal bool IsReturnMoney()
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p880")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }


        /// <summary>
        /// 允许通过会员号码绑定会员
        /// </summary>
        /// <returns></returns>
        internal bool IsBindMemberByNo()
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p600")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }

        /// <summary>
        /// 允许通过供应商号码绑定供应商
        /// </summary>
        /// <returns></returns>
        internal bool IsBindSupplierByNo()
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p700")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }


        /// <summary>
        /// 是否允许更改语言
        /// </summary>
        /// <returns></returns>
        internal bool IsAllowChangeLanguage()
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("o100")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }



        /// <summary>
        /// 是否允许修改不限时时间
        /// </summary>
        /// <param name="IsNullOrder"></param>
        /// <returns></returns>
        internal bool IsChangeUnlimitedTime(bool IsNullOrder)
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 2)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 查询会员余额
        /// </summary>
        internal async Task CheckMemberBalance(string memberNo, Action Result, Action<Member> EditMember)
        {
            // A QR code picture in members card backgound, like: https://www.YoureWebSide/Members/2uTvLbZNo3oLnHH8QkaqVg    2uTvLbZNo3oLnHH8QkaqVg is 1000 which encrypted for consecutive numbers(you could change the encryption code by change source code)

            try
            {
                memberNo = memberNo.Substring(memberNo.LastIndexOf('/') + 1);
                memberNo = Decrypt(memberNo.Replace("@", "/").Replace(",", "+") + "==");

                List<Member> Members;
                var taskResult = await OperatesService.Instance.ServiceGetMembers(0, memberNo, null, null, null, true);
                bool result = taskResult.result;
                Members = taskResult.members;


                if (result && Members.Count > 0)
                {

                    if (Members.FirstOrDefault().Sex == -1)
                    {
                        EditMember(Members.FirstOrDefault());
                    }
                    else
                    {
                        string message = memberNo + " : ";

                        if (Res.Instance.MainLangIndex == 0)
                            message += Members.FirstOrDefault().MemberName0;
                        else if (Res.Instance.MainLangIndex == 1)
                            message += Members.FirstOrDefault().MemberName1;
                        else if (Res.Instance.MainLangIndex == 2)
                            message += Members.FirstOrDefault().MemberName2;

                        message += Environment.NewLine;

                        message += Members.FirstOrDefault().Phone;

                        message += Environment.NewLine;

                        message += Resources.Instance.PrintInfo.PriceSymbol + Members.FirstOrDefault().BalancePrice;

                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, message, VM.ViewModels.Controls.MessageBoxMode.Dialog, VM.ViewModels.Controls.MessageBoxImageMode.Information, VM.ViewModels.Controls.MessageBoxButtonMode.OK, (msg) =>
                        {
                            Result();
                        }, null);
                    }

                   
                }
                else
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("PropertyNotFound"), Resources.Instance.GetString("MemberNo")), VM.ViewModels.Controls.MessageBoxMode.Dialog, VM.ViewModels.Controls.MessageBoxImageMode.Warn, VM.ViewModels.Controls.MessageBoxButtonMode.OK, (msg) =>
                    {
                        Result();
                    }, null);
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);

                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, string.Format(Resources.Instance.GetString("OperateFaild"), Resources.Instance.GetString("Search")), VM.ViewModels.Controls.MessageBoxMode.Dialog, VM.ViewModels.Controls.MessageBoxImageMode.Error, VM.ViewModels.Controls.MessageBoxButtonMode.OK, (msg) =>
                {
                    Result();
                }, null);


            }
        }


        private string Decrypt(string cipherText)
        {
            string EncryptionKey = "MemberN0.";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, Encoding.UTF8.GetBytes("Oybab..."));
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }


        /// <summary>
        /// 打开钱箱
        /// </summary>
        internal bool IsAllowOpenCashDrawer(string location = null)
        {
            Admin model = Resources.Instance.AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("t000")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }


        /// <summary>
        /// 隐藏功能时复制名称
        /// </summary>
        internal void CopyForHide(ref string Cell0, ref string Cell1, ref string Cell2)
        {
            if (Res.Instance.MainLangIndex == 0)
            {
                Cell1 = Cell0;
                Cell2 = Cell0;
            }
            else if (Res.Instance.MainLangIndex == 1)
            {
                Cell0 = Cell1;
                Cell2 = Cell1;
            }
            else if (Res.Instance.MainLangIndex == 2)
            {
                Cell0 = Cell2;
                Cell1 = Cell2;
            }
        }


        /// <summary>
        /// 查询并获取权限
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> CheckCamera()
        {
           
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();


                if (status != PermissionStatus.Granted)
                {
                    return false;
                }
            }
          
            return true;
        }


        /// <summary>
        /// 关闭程序
        /// </summary>
        public async void Exit()
        {
            await Common.Instance.Close();
            var closer = DependencyService.Get<ICloseApplication>();
            closer?.closeApplication();
        }



    }
}
