using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.DAL;
using System.Text.RegularExpressions;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class BarcodeWindow : KryptonForm
    {
        private Product CurrentProduct = null;
        private bool IsOrder = true; // 是不是收入

        public BarcodeWindow(bool IsOrder)
        {
            InitializeComponent();
            this.krptCount.LostFocus += krptCount_LostFocus;
            this.IsOrder = IsOrder;


            this.Text = Resources.GetRes().GetString("Search");
            krplBarcodeNo.Text = Resources.GetRes().GetString("BarcodeNo");
            krpbAdd.Text = Resources.GetRes().GetString("Add");
            krpbCancel.Text = Resources.GetRes().GetString("Cancel");

            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Barcode.ico"));

            ReInitialLayout();
        }

       

        // delegate
        public delegate void EventProduct(object sender, Product value, int count);
        /// <summary>
        /// 添加了新产品
        /// </summary>
        public event EventProduct ProductAdded;

        /// <summary>
        /// 重置布局
        /// </summary>
        private void ReInitialLayout()
        {
            // 添加后直接返回布局.
            krplProductName.Text = "";
            krptCount.Text = Count = "1";
            krptBarcodeNo.Text = "";
            krpgProductInfo.Visible = false;
            krpgBarcodeInput.Visible = true;
            CurrentProduct = null;
            krptBarcodeNo.Focus();
        }




        

        /// <summary>
        /// 输入条形码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krptBarcodeNo_KeyUp(object sender, KeyEventArgs e)
        {
            // 如果是确认, 则搜索产品并增加到队列
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(krptBarcodeNo.Text))
                    return;


                int hideIndex = 2;
                if (!IsOrder)
                    hideIndex = 3;


                CurrentProduct = Resources.GetRes().Products.Where(x => (x.Barcode == krptBarcodeNo.Text) && (x.HideType == 0 || x.HideType == hideIndex)).FirstOrDefault();

                if (null != CurrentProduct)
                {
                    if (Resources.GetRes().MainLangIndex == 0)
                        krplProductName.Text = CurrentProduct.ProductName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        krplProductName.Text = CurrentProduct.ProductName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        krplProductName.Text = CurrentProduct.ProductName2;

                    krpgBarcodeInput.Visible = false;
                    krpgProductInfo.Visible = true;

                    krptCount.Text = Count = "1";
                    krptCount.SelectAll();
                    krptCount.Focus();
                }

                krptBarcodeNo.Text = "";


                e.SuppressKeyPress = true;
                e.Handled = true;
                

            }

        }



        private void Enter_KeyDown(object sender, KeyEventArgs e)
        {
            // 如果是确认, 则直接加入Product到队列
            if (e.KeyCode == Keys.Enter && null != CurrentProduct)
            {
                krpbAdd_Click(null, null);

                e.SuppressKeyPress = true;
                e.Handled = true;

            }
        }


        Regex match = new Regex(@"^[1-9][0-9]*$");
        private string Count;
        /// <summary>
        /// 确认数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krptCount_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                krpbCancel_Click(null, null);
            }
            else
            {
                if (krptCount.Text == "")
                {
                    krptCount.Text = "1";
                    krptCount.SelectAll();
                }
                if (!match.IsMatch(krptCount.Text))
                {
                    krptCount.Text = Count;
                    krptCount.SelectionStart = krptCount.TextLength;
                    return;
                }
                Count = krptCount.Text;
            }
       
        }

        /// <summary>
        /// 数量失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void krptCount_LostFocus(object sender, EventArgs e)
        {
            if (krptCount.Text != Count)
            {
                krptCount.Text = Count;
                krptCount.SelectionStart = krptCount.TextLength;
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbAdd_Click(object sender, EventArgs e)
        {
            int count = int.Parse(Count);

            ProductAdded(null, CurrentProduct, count);
            ReInitialLayout();
        }

        /// <summary>
        /// 恢复(取消)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbCancel_Click(object sender, EventArgs e)
        {
            ReInitialLayout();
        }


        

    }
}
