using Newtonsoft.Json;
using Oybab.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.ServerManager.Operate
{
    public static class Common
    {

        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this Order model)
        {
            model.tb_admin = null;
            model.tb_admin1 = null;
            model.tb_device = null;
            model.tb_device1 = null;
            model.tb_member = null;
            model.tb_orderdetail = null;
            model.tb_room = null;
            model.tb_orderpay = null;
        }


        public static void ClearReferences(this OrderPay model)
        {
            model.tb_admin = null;
            model.tb_balance = null;
            model.tb_device = null;
            model.tb_member = null;
            model.tb_order = null;
        }

        public static void ClearReferences(this TakeoutPay model)
        {
            model.tb_admin = null;
            model.tb_balance = null;
            model.tb_device = null;
            model.tb_member = null;
            model.tb_takeout = null;
        }

        public static void ClearReferences(this ImportPay model)
        {
            model.tb_admin = null;
            model.tb_balance = null;
            model.tb_device = null;
            model.tb_import = null;
            model.tb_supplier = null;
        }





        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this OrderDetail model)
        {
            model.tb_admin = null;
            model.tb_admin1 = null;
            model.tb_device = null;
            model.tb_device1 = null;
            model.tb_order = null;
            model.tb_product = null;
        }

        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this Takeout model)
        {
            model.tb_admin = null;
            model.tb_admin1 = null;
            model.tb_admin2 = null;
            model.tb_device = null;
            model.tb_device1 = null;
            model.tb_member = null;
            model.tb_takeoutdetail = null;
            model.tb_takeoutpay = null;
        }


        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this TakeoutDetail model)
        {
            model.tb_admin = null;
            model.tb_admin1 = null;
            model.tb_device = null;
            model.tb_device1 = null;
            model.tb_takeout = null;
            model.tb_product = null;
        }






        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this Import model)
        {
            model.tb_admin = null;
            model.tb_device = null;
            model.tb_importdetail = null;
            model.tb_supplier = null;
            model.tb_importpay = null;
        }


        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this ImportDetail model)
        {
            model.tb_admin = null;
            model.tb_device = null;
            model.tb_import = null;
            model.tb_product = null;
        }



        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this ProductType model)
        {
            model.tb_product = null;
            model.tb_producttype1 = null;
            model.tb_producttype2 = null;
        }


        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this Ppr model)
        {
            model.tb_printer = null;
            model.tb_product = null;
        }


        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this Product model)
        {
            model.tb_importdetail = null;
            model.tb_orderdetail = null;
            model.tb_producttype = null;
            model.tb_Ppr = null;
            model.tb_takeoutdetail = null;
            model.tb_product1 = null;
            model.tb_product2 = null;
        }


        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this Room model)
        {
            model.tb_device = null;
            model.tb_order = null;
        }


        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this Admin model)
        {
            model.tb_adminpay = null;
            model.tb_adminpay1 = null;
            model.tb_import = null;
            model.tb_importdetail = null;
            model.tb_member = null;
            model.tb_memberpay = null;
            model.tb_order = null;
            model.tb_order1 = null;
            model.tb_orderdetail = null;
            model.tb_orderdetail1 = null;
            model.tb_takeout = null;
            model.tb_takeout1 = null;
            model.tb_takeout2 = null;
            model.tb_takeoutdetail = null;
            model.tb_takeoutdetail1 = null;
            model.tb_supplier = null;
            model.tb_supplierpay = null;
            model.tb_adminlog = null;
            model.tb_balancepay = null;

            model.tb_orderpay = null;
            model.tb_takeoutpay = null;
            model.tb_importpay = null;

        }


        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this AdminPay model)
        {
            model.tb_admin = null;
            model.tb_admin1 = null;
            model.tb_device = null;
            model.tb_balance = null;
        }



        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this Device model)
        {
            model.tb_import = null;
            model.tb_importdetail = null;
            model.tb_memberpay = null;
            model.tb_order = null;
            model.tb_order1 = null;
            model.tb_orderdetail = null;
            model.tb_orderdetail1 = null;
            model.tb_room = null;
            model.tb_takeout = null;
            model.tb_takeout1 = null;
            model.tb_takeoutdetail = null;
            model.tb_takeoutdetail1 = null;
            model.tb_adminlog = null;
            model.tb_adminpay = null;
            model.tb_supplierpay = null;
            model.tb_balancepay = null;

            model.tb_orderpay = null;
            model.tb_takeoutpay = null;
            model.tb_importpay = null;

        }




        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this Printer model)
        {
            model.tb_Ppr = null;

        }



        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this Balance model)
        {
            model.tb_balancepay = null;
            model.tb_importpay = null;
            model.tb_memberpay = null;
            model.tb_orderpay = null;
            model.tb_supplierpay = null;
            model.tb_takeoutpay = null;
            model.tb_adminpay = null;
        }

        public static Balance ReChangeBalance(this Balance model)
        {
            // We need that to know Balance
            //x.BalanceId = 0;
            //x.BalancePrice = 0;

            model.AccountBank = null;
            model.AccountName = null;
            model.AccountNo = null;
            model.AddTime = 0;
            model.BalanceName0 = null;
            model.BalanceName1 = null;
            model.BalanceName2 = null;
            model.BalanceType = 0;
            model.HideType = 0;
            model.IsBind = 0;
            model.Order = 0;
            model.Remark = null;
            model.RemoveRate = 0;
            model.UpdateTime = null;
            model.ClearReferences();

            return model;
        }


        /// <summary>
        /// 快速复制
        /// </summary>
        /// <param name="model"></param>
        public static Balance FastCopy(this Balance model, bool IncludeRef = false, bool ClearBalance = false)
        {
            Balance result = new Balance();
            result.AccountBank = model.AccountBank;
            result.AccountName = model.AccountName;
            result.AccountNo = model.AccountNo;
            result.AddTime = model.AddTime;
            result.BalanceId = model.BalanceId;
            result.BalanceName0 = model.BalanceName0;
            result.BalanceName1 = model.BalanceName1;
            result.BalanceName2 = model.BalanceName2;
            result.BalancePrice = model.BalancePrice;
            result.BalanceType = model.BalanceType;
            result.HideType = model.HideType;
            result.IsBind = model.IsBind;
            result.Order = model.Order;
            result.Remark = model.Remark;
            result.RemoveRate = model.RemoveRate;
            result.UpdateTime = model.UpdateTime;

            if (IncludeRef)
            {
                result.tb_balancepay = model.tb_balancepay;
                result.tb_importpay = model.tb_importpay;
                result.tb_memberpay = model.tb_memberpay;
                result.tb_orderpay = model.tb_orderpay;
                result.tb_supplierpay = model.tb_supplierpay;
                result.tb_takeoutpay = model.tb_takeoutpay;
                result.tb_adminpay = model.tb_adminpay;
            }


            if (ClearBalance)
                result.BalancePrice = 0;

            return result;

        }


        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this BalancePay model)
        {
            model.tb_device = null;
            model.tb_admin = null;
            model.tb_balance = null;
        }




        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this AdminLog model)
        {
            model.tb_admin = null;
            model.tb_device = null;
        }


        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this Member model)
        {
            model.tb_admin = null;
            model.tb_memberpay = null;
            model.tb_order = null;
            model.tb_takeout = null;
            model.tb_orderpay = null;
            model.tb_takeoutpay = null;
        }


        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this MemberPay model)
        {
            model.tb_admin = null;
            model.tb_device = null;
            model.tb_member = null;
            model.tb_balance = null;
        }


        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this Supplier model)
        {
            model.tb_admin = null;
            model.tb_import = null;
            model.tb_supplierpay = null;
            model.tb_importpay = null;
        }


        /// <summary>
        /// 清除引用
        /// </summary>
        /// <param name="model"></param>
        public static void ClearReferences(this SupplierPay model)
        {
            model.tb_admin = null;
            model.tb_device = null;
            model.tb_supplier = null;
            model.tb_balance = null;
        }

        /// <summary>
        /// 去掉前部分
        /// </summary>
        /// <param name="target"></param>
        /// <param name="trimString"></param>
        /// <returns></returns>
        public static string TrimStart(this string target, string trimString)
        {
            string result = target;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }

            return result;
        }

        /// <summary>
        /// 去掉后部分
        /// </summary>
        /// <param name="target"></param>
        /// <param name="trimString"></param>
        /// <returns></returns>
        public static string TrimEnd(this string target, string trimString)
        {
            string result = target;
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, result.Length - trimString.Length);
            }

            return result;
        }


        /// <summary>
        /// 字典查找并返回默认
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        /// <summary>
        /// 字典查找并返回默认
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="defaultValueProvider"></param>
        /// <returns></returns>
        internal static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value
                 : defaultValueProvider();
        }


        /// <summary>
        /// 安全解析Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        internal static T DeserializeObject<T>(this string json) where T : new()
        {
            return json != null && json != "null"
                           ? JsonConvert.DeserializeObject<T>(json)
                           : new T();
        }



        /// <summary>
        /// 创建MD5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CreateMD5(this string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }





        private static readonly System.Threading.ThreadLocal<Random> appRandom = new System.Threading.ThreadLocal<Random>(() => new Random());
        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string GenereteRandomCode(this string input, int length, int mode)
        {
            StringBuilder str = new StringBuilder();

            if (mode == 0)
                str = new StringBuilder("1234567890");
            else if (mode == 1)
                str = new StringBuilder("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890");
            else if (mode == 2)
                str = new StringBuilder("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^()_+.,<>?&*-");
            else
            {
                throw new Exception("Invalid mode for generate random code!");
            }


            StringBuilder returnStr = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                int index = appRandom.Value.Next(str.Length);
                returnStr.Append(str[index]);
                str.Remove(index, 1);
            }
            return returnStr.ToString();

        }


        /// <summary>
        /// 根据行分割
        /// </summary>
        /// <param name="str"></param>
        /// <param name="removeEmptyLines"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetLines(this string str, bool removeEmptyLines = false)
        {
            return str.Split(new[] { "\r\n", "\r", "\n" },
                removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }
    }
}
