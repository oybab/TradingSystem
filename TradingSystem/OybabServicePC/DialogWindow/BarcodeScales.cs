using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oybab.DAL;
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using System.IO;
using Oybab.Res.Tools;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class BarcodeScales : KryptonForm
    {
        public BarcodeScales()
        {
            InitializeComponent();


            krpbSend.Text = Resources.GetRes().GetString("SendData");
            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krptIPAddress.Location = new Point(krptIPAddress.Location.X, krptIPAddress.Location.Y - 4.RecalcMagnification2());
            }

            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            
                this.Text = Resources.GetRes().GetString("BarcodeScales");
                krplIPAddress.Text = Resources.GetRes().GetString("IpAddress");
                this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.PrintBarcode.ico"));

            krplLanguage.Text = Resources.GetRes().GetString("Language");
            krpcLanguage.Items.AddRange(Resources.GetRes().MainLangList.OrderBy(x=>x.Value.MainLangIndex).Select(x => x.Value.LangName).ToArray());

            krptIPAddress.Text = "192.168.1.150";

            if (Resources.GetRes().DefaultPrintLang == -1)
            {
                krpcLanguage.SelectedItem = Resources.GetRes().MainLang.LangName;
            }
            else
            {
                krpcLanguage.SelectedIndex = Resources.GetRes().DefaultPrintLang;
            }


           
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbAdd_Click(object sender, EventArgs e)
        {

            //判断是否空
            if (krptIPAddress.Text.Trim().Equals("") || !System.Text.RegularExpressions.Regex.Match(krptIPAddress.Text.Trim(), @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$").Success)
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("Address")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
                }
            else
            {
                //先不让用户单击按钮
                krpbSend.Enabled = false;

                StartLoad(this, null);
                bool result = true;
                Task.Factory.StartNew(() =>
                {
                    try
                    {

                        List<string> records = new List<string>();
                       

                        try
                        {
                            foreach (var item in Resources.GetRes().Products.Where(x => x.IsScales == 1 && (x.HideType == 0 || x.HideType == 2)).OrderBy(x => x.Barcode))
                            {
                                // 输出DHBZ5格式
                                // plu号 4位 + 商品编码 4位, 跟PLU号一样    + 5位价格信息                                                                      +模式0+有效期 + 店号 + 皮重 +   + "特殊信息+A 分隔符" + 产品名和分隔符B +
                                //string code = item.Barcode.Substring(1,4) + item.Barcode.PadLeft(6, '0') + string.Format("{0:0.00}", item.Price).Replace(".", "").PadLeft(5, '0') + "0" + "000" + "22" + "00000" + "0A" + new string(item.ProductNameZH.Replace("A", "").Replace("B", "").Take(9).ToArray()) + "B";


                                // 输出DHTMA07
                                //*分隔符1P(0位)+ PLU编号,分隔符A(4位)   + 商品编号(7位) + 单价(6位) + 模式(1位) +  特殊信息1(2位) +  特殊信息2(2位) +  特殊信息3(2位) + 有效期(3位) + 店号(2位) + *(2位) + 13位码 +　皮重(5位) + 标签号(2位) + * (26位分隔符##) + 品名(0位分隔符##) + 备注A(0位分隔符##)
                                //string code = "1P" + item.Barcode.Substring(1, 4) + "A" + item.Barcode.PadLeft(7, '0') + string.Format("{0:0.00}", item.Price).Replace(".", "").PadLeft(6, '0') + "0" + "00" + "00" + "00" + "000" + "22" + "00" + "0000000000000" + "00000" + "00" + "00000000000000000000000000##" + new string(item.ProductNameZH.Replace("#", "").Take(9).ToArray()) + "##" + "##";


                                string productName = "";

                                try
                                {
                                    if (krpcLanguage.SelectedIndex == 0)
                                        productName = item.ProductName0;
                                    else if (krpcLanguage.SelectedIndex == 1)
                                        productName = item.ProductName1;
                                    else if (krpcLanguage.SelectedIndex == 2)
                                        productName = item.ProductName2;

                                    productName = GetCode(new string(productName.Take(12).ToArray()));
                                }
                                catch (Exception)
                                {
                                    this.BeginInvoke(new Action(() =>
                                    {
                                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("IncorrectProductName"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }));
                                    result = false;
                                    break;
                                }

                                records.Add("!0V" + item.Barcode.Substring(1, 4) + "A" + item.Barcode.PadLeft(7, '0') + string.Format("{0:0.00}", item.Price).Replace(".", "").PadLeft(6, '0') + "0" + "00" + "00" + "00" + "000" + "22" + "00" + "0000000000000" + "00000" + "00" + "00000000000000000000000000" + "B" + productName + "C" + "" + "D" + "" + "E");

                            }

                             
                        }
                        catch (Exception)
                        {
                            result = false;
                            this.BeginInvoke(new Action(() =>
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("IncorrectProductData"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));
                            
                        }

                        

                        // 先PING这个IP
                        if (result && !PingHost(krptIPAddress.Text.Trim()))
                        {
                            result = false;
                            this.BeginInvoke(new Action(() =>
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("UnableToConnectIPAddress"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));
                            
                        }


                        // 发送数据
                        if (result)
                        {

                            
                                using (TcpClient client = new TcpClient(krptIPAddress.Text.Trim(), 4001))
                                {
                                    client.SendTimeout = 60000;
                                    client.ReceiveTimeout = 60000;
                                    NetworkStream nwStream = client.GetStream();

                                // 先发送清空命令
                                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("!0IA");

                                nwStream.Write(bytesToSend, 0, bytesToSend.Length);

                                byte[] endHex = new byte[2];
                                endHex[0] = 0x0d;
                                endHex[1] = 0x0a;

                                // 发送结束命令
                                nwStream.Write(endHex, 0, endHex.Length);

                                // 读取返回信息
                                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                                int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);

                                string resultMsg = BitConverter.ToString(bytesToRead, 0, bytesRead).Replace(" ", "-");

                                // 命令返回成功
                                if (resultMsg.EndsWith("0D-0A-03"))
                                {
                                    foreach (var item in records)
                                    {
                                        // 发送PLU信息
                                        bytesToSend = ASCIIEncoding.ASCII.GetBytes(item);

                                        nwStream.Write(bytesToSend, 0, bytesToSend.Length);

                                        // 发送结束命令
                                        nwStream.Write(endHex, 0, endHex.Length);

                                        // 读取返回信息
                                        bytesToRead = new byte[client.ReceiveBufferSize];
                                        bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);

                                        resultMsg = BitConverter.ToString(bytesToRead, 0, bytesRead).Replace(" ", "-");

                                        // 命令返回成功
                                        if (!resultMsg.EndsWith("0D-0A-03"))
                                        {
                                            throw new OybabException("Return item code not excepted!");
                                        }
                                    }



                                    Resources.GetRes().DefaultPrintLang = krpcLanguage.SelectedIndex;

                                }
                                else
                                {
                                    throw new OybabException("Return first code not excepted!");
                                }

                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        this.BeginInvoke(new Action(() =>
                        {
                            ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                            {
                                KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }));
                        }));
                    }

                    StopLoad(this, null);

                    if (result)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateSuccess"), Resources.GetRes().GetString("SendData")), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            DialogResult = System.Windows.Forms.DialogResult.OK;
                            this.Close();
                        }));
                    }
                });
            }
            krpbSend.Enabled = true;

        }


        private bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            return pingable;
        }


        /// <summary>
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;

        private void krptMemberNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                krpbAdd_Click(null, null);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
               
            
        }








        /// <summary>
        /// 返回码
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private string GetCode(string word)
        {
            string resultCode = "";
            foreach (var item in word)
            {
                if ((int)item <= 127)
                {
                    resultCode += ChineseToCoding(ToSBC(item.ToString()));
                }
                else
                {
                    resultCode += ChineseToCoding(item.ToString());
                }
            }

            return resultCode;
        }




        /// 转全角的函数(SBC case) /// 
        ///任意字符串 /// 全角字符串 ///
        ///全角空格为12288,半角空格为32 
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248 ///
        private string ToSBC(string input)
        { //半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288; continue;
                }
                if (c[i] < 127) c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }

        /// 转半角的函数(DBC case) ///
        ///任意字符串
        /// 半角字符串 ///
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248 ///
        private string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32; continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }




        //如为汉字，高低8位各减0xA0即得。

        /// <summary>  
        /// 汉字转区位码方法  
        /// </summary>  
        /// <param name="chinese">汉字</param>  
        /// <returns>区位码</returns>  
        private string ChineseToCoding(string chinese)
        {
            string pCode = "";
            byte[] pArray = new byte[2];
            pArray = Encoding.Default.GetBytes(chinese);//得到汉字的字节数组  
            int front = (short)(pArray[0] - '\0') - 160;//将字节数组的第一位160  
            int back = (short)(pArray[1] - '\0') - 160;//将字节数组的第二位160  
            pCode = front.ToString().PadLeft(2, '0') + back.ToString().PadLeft(2, '0');//再连接成字符串就组成汉字区位码  
            return pCode;
        }


        /// <summary>  
        /// 区位码转汉字方法  
        /// </summary>  
        /// <param name="coding">区位码</param>  
        /// <returns>汉字</returns>  
        private string CodingToChinese(string coding)
        {
            string chinese = "";

            byte[] pArray = new byte[2];
            string front = coding.Substring(0, 2);//区位码分为两部分  
            string back = coding.Substring(2, 2);
            pArray[0] = (byte)(Convert.ToInt16(front) + 160);//前两位加160,存入字节数组  
            pArray[1] = (byte)(Convert.ToInt16(back) + 160);//后两位加160,存入字节数组  
            chinese = Encoding.Default.GetString(pArray);//由字节数组获得汉字  
            return chinese;
        }












        // 底部这两个不是纯数字类型

        //如果用编译器，会将汉字直接编译为机内码，如“啊”的机内码为  B0 A1
        //则区码为 B0H-A0H=10H=  16
        //位码为 A1H-A0H=01H=  1
        //汉字是以内码形式存储的，16以前的区，存的是符号、数字、其他字符。

        /// <summary>       
        /// 汉字转区位码       
        /// </summary>       
        /// <paramname="character"></param>
        ///<returns></returns>       
        private string CharacterToCoding(string character)
        {
            string coding = string.Empty;
            for (int i = 0; i < character.Length; i++)
            {
                byte[] bytes =
                System.Text.Encoding.GetEncoding("GB2312").GetBytes(character.Substring(i,
                1));
                string lowCode = System.Convert.ToString(bytes[0], 16);
                //取出低字节编码内容（两位16进制）               
                if (lowCode.Length == 1)
                    lowCode = "0" + lowCode;
                string hightCode = System.Convert.ToString(bytes[1],
                16);//取出高字节编码内容（两位16进制）               
                if (hightCode.Length == 1)
                    hightCode = "0" + hightCode;
                coding += (lowCode + hightCode);//加入到字符串中,           
            }
            return coding;
        }
        /// <summary>       
        /// 区位码取汉字       
        /// </summary>       
        /// <paramname="coding"></param>       
        ///<returns></returns>       
        private string CodingToCharacter(string coding)
        {
            string characters = string.Empty;
            if (coding.Length % 4 != 0)//编码为16进制,必须为4的倍数。           
            {
                throw new System.Exception("编码格式不正确");
            }
            for (int i = 0; i < coding.Length / 4; i++)
            {
                byte[] bytes = new byte[2];
                int j = i * 4;
                string lowCode = coding.Substring(j, 2); //取出低字节,并以16进制进制转换               
                bytes[0] = System.Convert.ToByte(lowCode, 16);
                string highCode = coding.Substring(j + 2, 2);
                //取出高字节,并以16进制进行转换               
                bytes[1] = System.Convert.ToByte(highCode, 16);
                string character =
                System.Text.Encoding.GetEncoding("GB2312").GetString(bytes);
                characters += character;
            }
            return characters;
        }

    }
}
