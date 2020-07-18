using ComponentFactory.Krypton.Toolkit;
using Newtonsoft.Json;
using Oybab.DAL;
using Oybab.Res;
using Oybab.Res.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Oybab.ServicePC.Tools
{
    internal sealed class BakInOperate
    {
        #region Instance
        private BakInOperate() { }
        private static readonly Lazy<BakInOperate> lazy = new Lazy<BakInOperate>(() => new BakInOperate());
        public static BakInOperate Instance { get { return lazy.Value; } }
        #endregion Instance


        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="form"></param>
        /// <param name="data"></param>
        internal void Output<T>(Form form, T data)
        {
            try
            {
                string encryptedStr = Encrypt(JsonConvert.SerializeObject(data), "ADGWTDSCSA214235", Encoding.UTF8.GetBytes("OybabCorp8888111"));


                //选择导出位置
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Title = Resources.GetRes().GetString("OutputData");
                saveFileDialog.Filter = "Data File(*.dt)|*.dt";
                saveFileDialog.FileName = "file.dt";

                DialogResult result = saveFileDialog.ShowDialog(form);
                if (result != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }



                using (StreamWriter stream = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8))
                {
                    stream.Write(encryptedStr);
                }

                KryptonMessageBox.Show(form, Resources.GetRes().GetString("OutputDataSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
                KryptonMessageBox.Show(form, Resources.GetRes().GetString("Exception_OutputData"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }




        /// <summary>
        /// 导出条码秤数据
        /// </summary>
        /// <param name="form"></param>
        /// <param name="data"></param>
        internal void OutputScalesData(Form form, List<Product> products)
        {
            try
            {
                if (null == products || products.Count == 0)
                {
                    KryptonMessageBox.Show(form, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //选择导出位置
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Title = Resources.GetRes().GetString("OutputData");
                saveFileDialog.Filter = "Data File(*.txt)|*.txt";
                saveFileDialog.FileName = "file.txt";

                DialogResult result = saveFileDialog.ShowDialog(form);
                if (result != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }



                using (StreamWriter stream = new StreamWriter(saveFileDialog.FileName, false, Encoding.Default))
                {
                    foreach (var item in products.OrderBy(x => x.Barcode))
                    {
                        // 输出DHBZ5格式
                        // plu号 4位 + 商品编码 4位, 跟PLU号一样    + 5位价格信息                                                                      +模式0+有效期 + 店号 + 皮重 +   + "特殊信息+A 分隔符" + 产品名和分隔符B +
                        //string code = item.Barcode.Substring(1,4) + item.Barcode.PadLeft(6, '0') + string.Format("{0:0.00}", item.Price).Replace(".", "").PadLeft(5, '0') + "0" + "000" + "22" + "00000" + "0A" + new string(item.ProductNameZH.Replace("A", "").Replace("B", "").Take(9).ToArray()) + "B";


                        // 输出DHTMA07
                        //*分隔符1P(0位)+ PLU编号,分隔符A(4位)   + 商品编号(7位) + 单价(6位) + 模式(1位) +  特殊信息1(2位) +  特殊信息2(2位) +  特殊信息3(2位) + 有效期(3位) + 店号(2位) + *(2位) + 13位码 +　皮重(5位) + 标签号(2位) + * (26位分隔符##) + 品名(0位分隔符##) + 备注A(0位分隔符##)
                        
                        string code = "1P" + item.Barcode.Substring(1,4) + "A" + item.Barcode.PadLeft(7, '0') + string.Format("{0:0.00}", item.Price).Replace(".", "").PadLeft(6, '0') + "0" + "00" + "00" + "00" + "000" + "22" + "00" + "0000000000000" + "00000" + "00" + "00000000000000000000000000##" + new string(item.ProductName0.Replace("#", "").Take(9).ToArray()) + "##" + "##";
                        stream.WriteLine(code);
                    }
                }

                KryptonMessageBox.Show(form, Resources.GetRes().GetString("OutputDataSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
                KryptonMessageBox.Show(form, Resources.GetRes().GetString("Exception_OutputData"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        internal T Import<T>(Form form)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.Multiselect = false;
                openFileDialog.Title = Resources.GetRes().GetString("InputData");
                openFileDialog.Filter = "Data File(*.dt)|*.dt";
                openFileDialog.FileName = "file.dt";

                DialogResult result = openFileDialog.ShowDialog(form);
                if (result != System.Windows.Forms.DialogResult.OK)
                {
                    return default(T);
                }

                if (!openFileDialog.CheckFileExists)
                {
                    return default(T);
                }


                string encryptedStr = "";

                using (System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog.FileName, Encoding.UTF8))
                {
                    encryptedStr = sr.ReadToEnd();
                }

                string decryptedstring = Decrypt(encryptedStr, "ADGWTDSCSA214235", Encoding.UTF8.GetBytes("OybabCorp8888111"));

                T model = JsonConvert.DeserializeObject<T>(decryptedstring);

                if (null == model)
                {
                    KryptonMessageBox.Show(form, Resources.GetRes().GetString("Exception_InputReadData"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    KryptonMessageBox.Show(form, Resources.GetRes().GetString("InputDataSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                return model;
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
                KryptonMessageBox.Show(form, Resources.GetRes().GetString("Exception_InputData"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return default(T);
            }
        }
        

        private int keysize = 256;

        private string Encrypt(string plainText, string passPhrase, byte[] initVectorBytes)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null))
            {
                byte[] keyBytes = password.GetBytes(keysize / 8);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                byte[] cipherTextBytes = memoryStream.ToArray();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        private string Decrypt(string cipherText, string passPhrase, byte[] initVectorBytes)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            using (PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null))
            {
                byte[] keyBytes = password.GetBytes(keysize / 8);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

    }


    internal sealed class BakInModel
    {
        [JsonProperty(PropertyName="S#F")]
        public List<Room> Rooms { get; set; }
        [JsonProperty(PropertyName = "C@1")]
        public List<Product> Products { get; set; }
        [JsonProperty(PropertyName = "@#F")]
        public List<ProductType> ProductTypes { get; set; }
    }
}
