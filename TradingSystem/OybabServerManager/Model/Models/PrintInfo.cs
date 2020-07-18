using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Oybab.ServerManager.Model.Models
{
    /// <summary>
    /// 打印底部附加信息
    /// </summary>
    public sealed class PrintInfo
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "Name0")]
        public string Name0 { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "Name1")]
        public string Name1 { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "Name2")]
        public string Name2 { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "Phone")]
        public string Phone { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "Msg1")]
        public string Msg1 { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "Msg0")]
        public string Msg0 { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "Msg2")]
        public string Msg2 { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "PriceSymbol")]
        public string PriceSymbol { get; set; } = "￥"; // 钱符号 
        [Newtonsoft.Json.JsonProperty(PropertyName = "MainLangList")]
        public string MainLangList { get; set; } = "0,1,2"; // 主要语言


        [Newtonsoft.Json.JsonProperty(PropertyName = "PageHeight")]
        public int PageHeight
        {
            set { _pageHeight = value; }
            get { if (_pageHeight == 0) return 1400; else return _pageHeight; }
        }
        private int _pageHeight;

        [Newtonsoft.Json.JsonProperty(PropertyName = "IsPrintBillAfterBuy")]
        public bool IsPrintBillAfterBuy { get; set; } // 购买账单时打印
        [Newtonsoft.Json.JsonProperty(PropertyName = "IsPrintBillAfterCheckout")]
        public bool IsPrintBillAfterCheckout { get; set; } // 结账账单时打印
        [Newtonsoft.Json.JsonProperty(PropertyName = "IsPrintImportAfterCheckout")]
        public bool IsPrintImportAfterCheckout { get; set; } // 结账支出时打印

        internal string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            // Maybe should get key from config file

            string key = "oybabTrading";

            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }



        internal string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);


            // Maybe should get key from config file
            string key = "oybabTrading";

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
