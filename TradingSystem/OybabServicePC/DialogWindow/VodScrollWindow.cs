using ComponentFactory.Krypton.Toolkit;
using Newtonsoft.Json;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oybab.Res.Tools;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class VodScrollWindow : KryptonForm
    {
        public VodScrollWindow()
        {
            InitializeComponent();

            try
            {
                if (File.Exists(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "VodKey.txt")))
                {
                    using (StreamReader sr = new StreamReader(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "VodKey.txt"), Encoding.UTF8))//Environment.GetEnvironmentVariable("windir")
                    {
                        txtId.Text = sr.ReadLine().Substring(0, 16);
                    }
                }
            }
            catch
            {
            }
        }


        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text)){
                KryptonMessageBox.Show("请先输入ID!", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Multiselect = false;
            openFileDialog.Title = "导入";
            openFileDialog.Filter = "Vod Data File(*.dll)|*.dll";
            openFileDialog.FileName = "vod.data";

            DialogResult result = openFileDialog.ShowDialog(this);
            if (result != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            if (!openFileDialog.CheckFileExists)
            {
                return;
            }


            string encryptedStr = "";

            using (System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog.FileName, Encoding.UTF8))
            {
                encryptedStr = sr.ReadToEnd();
            }

            string decryptedstring = Decrypt(encryptedStr, txtId.Text.Trim(), Encoding.UTF8.GetBytes("OybabCorp8888000"));

            Scroll model = JsonConvert.DeserializeObject<Scroll>(decryptedstring);

            txtInterval.Text = model.Interval.ToString();
            txtZh.Text = model.MsgZH;
            txtUg.Text = model.MsgUG;
            txtEn.Text = model.MsgEN;



            MessageBox.Show(this, "导入成功!", Res.Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtInterval.Text) || string.IsNullOrWhiteSpace(txtZh.Text) || string.IsNullOrWhiteSpace(txtUg.Text) || string.IsNullOrWhiteSpace(txtEn.Text))
            {
                KryptonMessageBox.Show(this, "请完整输入信息!", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            Scroll scroll = new Scroll();
            scroll.Interval = int.Parse(txtInterval.Text);
            scroll.MsgZH = txtZh.Text;
            scroll.MsgUG = txtUg.Text;
            scroll.MsgEN = txtEn.Text;

            

           

            string encryptedStr = Encrypt(JsonConvert.SerializeObject(scroll), txtId.Text.Trim(), Encoding.UTF8.GetBytes("OybabCorp8888000"));

            //选择导出位置
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Title = "导出";
            saveFileDialog.Filter = "Vod Data File(*.dll)|*.dll";
            saveFileDialog.FileName = "VScroll";

            DialogResult result = saveFileDialog.ShowDialog(this);
            if (result != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }



            using (StreamWriter stream = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8))
            {
                stream.Write(encryptedStr);
            }

            KryptonMessageBox.Show(this, "导出成功!", Res.Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }





        // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        //private static readonly byte[] initVectorBytes = Encoding.UTF8.GetBytes("OybabCorp8888000");//ASCII

        // This constant is used to determine the keysize of the encryption algorithm.
        private const int keysize = 256;

        public static string Encrypt(string plainText, string passPhrase, byte[] initVectorBytes)
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

        public static string Decrypt(string cipherText, string passPhrase, byte[] initVectorBytes)
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

        internal new sealed class Scroll
        {
            [Newtonsoft.Json.JsonProperty(PropertyName = "Interval")]
            internal int Interval { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "MsgZH")]
            internal string MsgZH { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "MsgUG")]
            internal string MsgUG { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "MsgEN")]
            internal string MsgEN { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "Type")]
            internal int Type { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "Mode")]
            internal int Mode { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "Model")]
            internal string Model { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "Remark")]
            internal int Remark { get; set; }
        }


        /// <summary>
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string value = "0";

            if (!cbRemove.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtInterval.Text) || string.IsNullOrWhiteSpace(txtZh.Text) || string.IsNullOrWhiteSpace(txtUg.Text) || string.IsNullOrWhiteSpace(txtEn.Text))
                {
                    KryptonMessageBox.Show(this, "请完整输入信息!", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Scroll scroll = new Scroll();
                scroll.Interval = int.Parse(txtInterval.Text);
                scroll.MsgZH = txtZh.Text;
                scroll.MsgUG = txtUg.Text;
                scroll.MsgEN = txtEn.Text;





                value = Encrypt(JsonConvert.SerializeObject(scroll), txtId.Text.Trim(), Encoding.UTF8.GetBytes("OybabCorp8888000"));
            }

            StartLoad(this, null);
            Task.Factory.StartNew(() =>
            {
                try
                {

                    string countent = null;

                    bool result = OperatesService.GetOperates().ServiceSetContent(value, out countent, cbRemove.Checked, cbIsRestart.Checked);


                    this.BeginInvoke(new Action(() =>
                    {
                        if (result)
                            KryptonMessageBox.Show(this, Oybab.Res.Resources.GetRes().GetString("SaveSuccess"), Oybab.Res.Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            KryptonMessageBox.Show(this, Oybab.Res.Resources.GetRes().GetString("SaveFailt"), Oybab.Res.Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }));



                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Oybab.Res.Resources.GetRes().GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }));
                }

                StopLoad(this, null);
            });
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
           

            StartLoad(this, null);
            Task.Factory.StartNew(() =>
            {
                try
                {

                    string content = null;

                    bool result = OperatesService.GetOperates().ServiceSetContent("-1", out content, false, false);


                    this.BeginInvoke(new Action(() =>
                    {
                        if (result)
                        {

                            content = Decrypt(content, txtId.Text.Trim(), Encoding.UTF8.GetBytes("OybabCorp8888000"));

                            Scroll scroll = content.DeserializeObject<Scroll>();

                            txtInterval.Text = scroll.Interval.ToString();
                            txtZh.Text = scroll.MsgZH;
                            txtUg.Text = scroll.MsgUG;
                            txtEn.Text = scroll.MsgEN;
                        }
                    }));



                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Oybab.Res.Resources.GetRes().GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }));
                }

                StopLoad(this, null);
            });
        }
    
    }
}
