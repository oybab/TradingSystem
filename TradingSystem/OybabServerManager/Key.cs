using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Oybab.ServerManager.Exceptions;
using Oybab.ServerManager.Operate;

namespace Oybab.ServerManager.Res
{

    /// <summary>
    /// 已去除所有外置Key验证逻辑!
    /// </summary>
    public sealed class Key
    {
        private static Key key = null;

        private Key() { }
        public static Key GetKeys()
        {
            if (null == key)
                key = new Key();
            return key;
        }

        //定义查找检查用的常规参数


        private string LANG_TYPE_0 = "0";
        private string LANG_TYPE_1 = "1";
        private string LANG_TYPE_2 = "2";



        //定义判断存在和检查的参数
        private bool IsFind = false;
        private bool IsCheck = false;
        private bool IsOpen = false;
        private bool IsInitialTime = false;






     
        /// <summary>
        /// 查找KEY
        /// </summary>
        /// <returns></returns>
        private void FindKey()
        {
           
            IsFind = true;
            
        }

       /// <summary>
        /// 检测KEY
       /// </summary>
       /// <param name="IsFirt"></param>
        private void CheckKey(bool IsInstall = false, bool IsIgnoreData = false, bool IsFirt = true)
        {
            //如果都没发现,那就直接跳过检测
            if (IsFind == false)
            {
                FindKey();
                if (IsFind == false)
                {
                    IsCheck = false;
                    IsOpen = false;
                    IsInitialTime = false;

                    return;
                }
            }

            //检测
            bool Result = true; // return CheckKey()
            if (Result)
            {
                IsCheck = true;
                if (IsOpen == false)
                {
                    //如果是安装版忽略打开
                    if (!IsInstall)
                    {
                        if (!Resources.GetRes().IsCreateMainCheckThread)
                            return;

                        // 检查
                        if (!Check())
                        {
                            System.Environment.Exit(0);
                            return;
                        }
                        // 打开前先检查是否过期
                        IsExpired();
                        if (Resources.GetRes().IsExpired == 1)
                        {
                             Request();
                             GetUid();
                        }
                        else
                        {
                            Open();
                            if (IsOpen)
                            {
                                Resources.GetRes().KEY_NAME_0 = GetName(Key.GetKeys().LANG_TYPE_0);
                                Resources.GetRes().KEY_NAME_1 = GetName(Key.GetKeys().LANG_TYPE_1);
                                Resources.GetRes().KEY_NAME_2 = GetName(Key.GetKeys().LANG_TYPE_2);
                                GetPass();
                                GetCount();
                                GetUid();
                                //GetKey();
                                // 获取剩余时间
                                CalculateLeftDays();

                                // 不忽略数据时重新获取数据
                                if (!IsIgnoreData && !DBOperate.GetDBOperate().IsDataReady)
                                {
                                    //获取数据(之所以打算在新线程运行, 是因为怕第二次KeyCheck请求因为获取数据时会堵住.   但也之所以去掉是因为(貌似数据还没准备好这个错误不是很友好, 所以已在KeyCheck加入了锁)
                                    //Task.Factory.StartNew(() =>
                                    //{
                                        DBOperate.GetDBOperate().LoadData();
                                    //});
                                }
                            }
                            else
                            {
                                IsCheck = false;
                            }
                        }
                    }
                }
            }
            else
            {
                IsCheck = false;
                IsFind = false;
                IsOpen = false;
                IsInitialTime = false;


                if (IsFirt)
                {
                    CheckKey(IsInstall, IsIgnoreData, false);
                }
            }
        }

        /// <summary>
        /// 打开权限
        /// </summary>
        /// <returns></returns>
        private void Open(string type = "0")
        {
            
                IsOpen = true;

        }


        /// <summary>
        /// 关闭权限
        /// </summary>
        /// <returns></returns>
        internal void Close(string type = "0")
        {
            if (!IsFind)
                return;
           

            IsCheck = false;
            IsOpen = false;
            IsFind = false;
            IsInitialTime = false;

          

        }


        private readonly object _syncRootEnc = new Object();
        /// <summary>
        /// 加密转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="newValue"></param>
        internal string Encryption(string value)
        {
            return value;
        }


        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal string GetName(string type = "0")
        {
            return "Oybab Trading";
        }


        /// <summary>
        /// 获取密码
        /// </summary>
        /// <param name="Pass"></param>
        /// <returns></returns>
        internal void GetPass()
        {
            Resources.GetRes().DB_KEY = "";
        }

        /// <summary>
        /// 获取硬件ID
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        private void GetUid()
        {
            Resources.GetRes().UID = "";
        }




        /// <summary>
        /// 不线程安全的话, 导致多个现成同时访问导致有些数据重复加载,比如(RoomModel.Add)
        /// </summary>

        private readonly object _syncRoot = new Object();
        /// <summary>
        /// 检查
        /// </summary>
        public bool Check(bool IsInstall = false, bool IsIgnoreData = false)
        {
            lock (_syncRoot)
            {
                CheckKey(IsInstall, IsIgnoreData);
                return Key.GetKeys().IsCheck;
            }
        }

        /// <summary>
        /// 初始化TC
        /// </summary>
        internal void Clear(bool IsAll = false)
        {
            //keyHandlesTC = new int[8];

            if (IsAll)
            {
                IsCheck = false;
                IsFind = false;
                IsOpen = false;
                IsInitialTime = false;
                Resources.GetRes().RegTimeRequestCode = null;
                Resources.GetRes().RegCountRequestCode = null;
            }
        }

        /// <summary>
        /// 最后一次KEY状态
        /// </summary>
        /// <returns></returns>
        internal bool LastCheck()
        {
            return Key.GetKeys().IsCheck;
        }


        /// <summary>
        /// 锁住
        /// </summary>
        /// <returns></returns>
        internal bool LockKey()
        {
            
            return true;
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="Count"></param>
        /// <returns></returns>
        private void GetCount()
        {
            Resources.GetRes().SERVICE_COUNT = 99;
            Resources.GetRes().ROOM_COUNT = 999;
        }


        /// <summary>
        /// 获取KEY时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private bool GetTime(ref DateTime time)
        {
            time = DateTime.Now;

            return true;
        }



        /// <summary>
        /// 获取数量请求码
        /// </summary>
        /// <returns></returns>
        internal bool GetCountRequest()
        {
            if (!string.IsNullOrWhiteSpace(Resources.GetRes().RegCountRequestCode))
            {
                return true;
            }


            Resources.GetRes().RegCountRequestCode = "custom";

            return true;
        }




        /// <summary>
        /// 注册新数量
        /// </summary>
        /// <param name="CountRegNo"></param>
        /// <returns></returns>
        internal bool RegCount(string CountRegNo)
        {

            return true;
        }



        /// <summary>
        /// 时间解锁(TC)
        /// </summary>
        /// <param name="regValue"></param>
        /// <returns></returns>
        internal bool SetRegCode(string regValue)
        {
            if (!InitialTC())
                return false;

            return true; // 解锁成功
        }



        /// <summary>
        /// 是否过期(TC)
        /// </summary>
        /// <returns></returns>
        private bool IsExpired()
        {
            if (!InitialTC())
                return false;

           
               // Resources.GetRes().IsExpired = 1; // 过期
         
                Resources.GetRes().IsExpired = 0; // 未过期

            return true;
        }



        /// <summary>
        /// 获取请求码(TC)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal bool Request()
        {
            if (!string.IsNullOrWhiteSpace(Resources.GetRes().RegTimeRequestCode))
            {
                return true;
            }

            if (!InitialTC())
                return false;

            StringBuilder requestValue = new StringBuilder();
          

            Resources.GetRes().RegTimeRequestCode = "custom";
            return true;
        }



        /// <summary>
        /// 获取KEY过期时间(TC)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private bool GetExpiredTime(ref DateTime time)
        {
            time = new DateTime(int.Parse("2999"), int.Parse("12"), int.Parse("31"), int.Parse("23"), int.Parse("59"), int.Parse("59"));

            return true;

        }

        /// <summary>
        /// 计算剩余时间: 只有小于30才给, 不然返回-1
        /// </summary>
        private void CalculateLeftDays()
        {
            Resources.GetRes().ExpiredRemainingDays = -1;
            // 当前时间
            DateTime now = DateTime.MaxValue;
            if (GetTime(ref now))
            {
                // 设置当前系统时间
                Time.Instance.SetTime(now);
            }


            // 过期时间
            DateTime expiredTime = DateTime.MinValue;
            GetExpiredTime(ref expiredTime);

            int day = Convert.ToInt32((expiredTime - now).TotalDays);

            Resources.GetRes().ExpiredRemainingDays = day;

            if (day <= 7)
            {
                // 同时获取一下申请码
                Request();
            }
        }


        /// <summary>
        /// 初始化TC
        /// </summary>
        /// <returns></returns>
        private bool InitialTC()
        {
            IsInitialTime = true;
            return true;
        }


        /// <summary>
        /// 检查
        /// </summary>
        /// <returns></returns>
        private bool Check()
        {
            return true;
        }


  
    }
}
