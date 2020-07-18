
//$(function () {


	//字体样式，大小设置
    var fontName0 = "微软雅黑";
	var fontName1 = "Oybab tuz, 微软雅黑";    
	var fontName2 = "微软雅黑";
	var fontSize0 = 12;   
	var fontSize1 = 16;  
	var fontSize2 = 12;
	



	
	

    /*店铺图片，名称更新*/
	var shop;
	
    //刷新列表和价格信息.
	var list;
    
    //切换语言: 切换列表标题和价格信息的名称Label的显示语言.还有顶部的软件名和餐厅名.
	var Language0, Language1, Language2;
   
    var ListModeImgHeightScale = 40; // 列表模式图片高度比例%
    var PageCount = 0;
    var CurrentPage = 0;




    /* 检查string空或null */
    function isEmpty(value) {
        return typeof value == 'string' && !value.trim() || typeof value == 'undefined' || value === null;
    }

    /* 让IE8支持trim() */
    if (typeof String.prototype.trim !== 'function') {
        String.prototype.trim = function () {
            return this.replace(/^\s+|\s+$/g, '');
        }
    }




    /* 初始化(测试) */
	function LoadImgList(shop) {


	    ChangeFontSize(fontSize1, fontName1);/*切换字体大小，字体样式*/

	    ChangeTheme(shop.ThemeIndex);/*加载主题*/

	    ChangeLanguage(shop.LangIndex);/*更改语言*/

	    ChangeMode(shop.ModeIndex); /*修改模式*/

	    CalculateHeighAndDisplayList(shop.ModeIndex, shop.LangIndex); /* 计算高度并显示列表 */

	    ChangeListAndPriceInfo(list);/*刷新列表和价格信息*/
	    

	    //NextPage(trNum, list); /*下一页*/

	    //自动轮播时间设置
	    $('#carousel').carousel({ interval: 5000 });


	}


    /* 从外部初始化 */
	function LoadAllInfo(shopX, Language0X, Language1X, Language2X) {
	    shop = jQuery.parseJSON(shopX);
	    Language0 = jQuery.parseJSON(Language0X);
	    Language1 = jQuery.parseJSON(Language1X);
	    Language2 = jQuery.parseJSON(Language2X);


	    ChangeFontSize(fontSize1, fontName1);/*切换字体大小，字体样式*/

	    ChangeTheme(shop.ThemeIndex);/*加载主题*/

	    ChangeLanguage(shop.LangIndex);/*更改语言*/

	    ChangeMode(shop.ModeIndex); /*修改模式*/

	    CalculateHeighAndDisplayList(shop.ModeIndex, shop.LangIndex); /* 计算高度并显示列表 */


	    //自动轮播时间设置
	    $('#carousel').carousel({ interval: 5000 });


	}


    /* 从外部修改账单 */
	function ChangeListAndPriceInfo(listXX) {

	    list = jQuery.parseJSON(listXX);

	    ChangeLanguageInfo(shop.LangIndex);
	}


    /* 从外部修改语言 */
	function ChangeLanguageInfo(index, modeIndex) {

	    shop.LangIndex = parseInt(index);

	    ChangeLanguage(shop.LangIndex);/*更改语言*/

	    CalculateHeighAndDisplayList(shop.ModeIndex, shop.LangIndex); /* 计算高度并显示列表 */
    }


	


    /* 更改模式 */ 		
    function ChangeMode(ModeNo) {   //列表 模式1  
        var imgList0 = shop.imgList0;  // 模式1 图片列表
        var imgList1 = shop.imgList1;  // 模式2 图片列表

        // 总共有多少个滚动动画和当前是第几个
        var totalItems = $('.item').length;
        var currentIndex = $('div.active').index() + 1;

        // 清空DIV
        $("#carousel").empty();
        var ol = $('<ol/>', { "class": "carousel-indicators" });
        var listDiv = $('<div/>', { "class": "carousel-inner", role: "listbox" });

        if (ModeNo == 0) {
          
          

            jQuery.each(imgList0, function (index, item) {
               
                ol.append($('<li/>', { "data-target": "#carousel", "data-slide-to": index }));
                
                var itemClass = "item";
                if (index == 0)
                    itemClass = "item active";
                
                listDiv.append($('<div/>', { "class": itemClass }).append($('<div/>', { "class": "full", "style": 'background-image:url(' + item + ')' })).append($('<div/>', { "class": "carousel-caption" })));
            });


          
            // 创建弹出框
            $("#carousel").append(ol).append(listDiv);


            $('.order').show();
            $('.item').addClass('item');
            $('.footer-shop').hide();

           


        } else if (ModeNo == 1) {    //全屏 模式2
            
           
            
            jQuery.each(imgList1, function (index, item) {
              
                ol.append($('<li/>', { "data-target": "#carousel", "data-slide-to": index }));

                var itemClass = "item";
                if (index == 0)
                    itemClass = "item active";

                listDiv.append($('<div/>', { "class": itemClass }).append($('<div/>', { "class": "full", "style": 'background-image:url(' + item + ')' })).append($('<div/>', { "class": "carousel-caption" })));
            });


            // 创建弹出框
            $("#carousel").append(ol).append(listDiv);

            $('.order').hide();
            $('.item').addClass('item1');
            $('.footer').hide();
            $('.SoftwareName').hide();
            $('.OwnerName').toggleClass('SoftwareName1');


        }

        // 定位到当前的位置
        $('.carousel').carousel(currentIndex);
    }

    //语言名称切换
    function ChangeLanguage(index) {
        // 根据索引选择当前语言
        var data = null;
        var ownerName = "";

        if (index == Language0.LangIndex) {
            data = Language0;
            ownerName = shop.OwnerName0;
        }
        else if (index == Language1.LangIndex) {
            data = Language1;
            ownerName = shop.OwnerName1;
        }
        else if (index == Language2.LangIndex) {
            data = Language2;
            ownerName = shop.OwnerName2;
        }
        $('#Money').text(shop.PriceSymbol);

        $("html").attr("dir", data.Dir);
        $("html").attr("lang", data.Culcure);

        $('body').css({ 'font-size': shop.fontSize1, 'font-family': data.FontName });
        $('#SoftwareName').text(data.SoftwareName);
        $('#OwnerName').text(ownerName);
        $('#SoftwareName1').text(data.SoftwareName);
        $('#num').text(data.Id);
        $('#ProductName').text(data.ProductName);

        $('#Count').text(data.Count);
        $('#Price').text(data.Price);
        $('#TotalPrice').text(data.TotalPrice);
        


        $('#Member').text(data.Member);
        $('#MemberDealsPrice').text(data.MemberDealsPrice);
        $('#MemberPaidPrice').text(data.MemberPaidPrice);
        $('#PaidPrice').text(data.PaidPrice);
        $('#CardPaidPrice').text(data.CardPaidPrice);
        $('#ReturnPrice').text(data.ReturnPrice);
        $('#BorrowPrice').text(data.BorrowPrice);
        $('#RoomNo').text(data.RoomNo);
        $('#RoomPrice').text(data.RoomPrice);
        $('#TotalTime').text(data.TotalTime);
        $('#selectedCount').text(data.SelectedCount);
        $('#SelectedCountV').text(0);
        $('#Amount').text(data.TotalPrice);
        $('#DealsPrice').text(data.DealsPrice);
        $('#CardPaidPrice').text(data.CardPaidPrice);
        $('#ActualPrice').text(data.ActualPrice);



        if (list == null) {
            $('.Member').hide();
            $('.MemberDealsPrice').hide();
            $('.MemberPaidPrice').hide();
            $('.PaidPrice').hide();
            $('.CardPaidPrice').hide();
            $('.ReturnPrice').hide();
            $('.BorrowPrice').hide();
            $('.RoomNo').hide();
            $('.RoomPrice').hide();
            $('.TotalTime').hide();
            $('#SelectedCount').text(0);
            $('#AmountV').text(PutItRight(data.Dir, shop.PriceSymbol , 0));
            $('.DealsPrice').hide();
            $('.CardPaidPrice').hide();
            $('.ActualPrice').hide();

            $('#trPaidReturnPrices').hide();
            $('#trTotalDeailsActualPrices').show();
            $('#trRoomPrices').hide();

            
        } else {

            var IsRoomMemberTr = false;
            var IsPaidReturnTr = false;

            //让会员名的值，可以语言切换
            if (list.tb_member == null) {
                $('.Member').hide();
                $('.MemberDealsPrice').hide();
                $('.MemberPaidPrice').hide();
            }
            else {
                $('.Member').show();
                IsRoomMemberTr = true;

                if (index == 0)
                    $('#MemberV').text(list.tb_member.MemberName0);
                else if (index == 1)
                    $('#MemberV').text(list.tb_member.MemberName1);
                else if (index == 2)
                    $('#MemberV').text(list.tb_member.MemberName2);

                // 会员优惠价
                if (list.MemberDealsPrice == 0) {
                    $('.MemberDealsPrice').hide();
                } else {
                    $('.MemberDealsPrice').show();
                    $('#MemberDealsPriceV').text(PutItRight(data.Dir, shop.PriceSymbol , list.MemberDealsPrice));
                    
                }

               
            };

            // 会员支付
            if (list.MemberPaidPrice == 0) {
                $('.MemberPaidPrice').hide();
            } else {
                $('.MemberPaidPrice').show();
                $('#MemberPaidPriceV').text(PutItRight(data.Dir, shop.PriceSymbol, list.MemberPaidPrice));

                IsPaidReturnTr = true;

            }



            // 现金支付
            if (list.PaidPrice == 0) {
                $('.PaidPrice').hide();
            } else {
                $('.PaidPrice').show();
                $('#PaidPriceV').text(PutItRight(data.Dir, shop.PriceSymbol , list.PaidPrice));

                IsPaidReturnTr = true;
                
            }

            // 刷卡支付
            if (list.CardPaidPrice == 0) {
                $('.CardPaidPrice').hide();
            } else {
                $('.CardPaidPrice').show();
                $('#CardPaidPriceV').text(PutItRight(data.Dir, shop.PriceSymbol , list.CardPaidPrice));

                IsPaidReturnTr = true;
                
            }

            // 退回金额
            if (list.ReturnPrice == 0) {
                $('.ReturnPrice').hide();
            } else {
                $('.ReturnPrice').show();
                $('#ReturnPriceV').text(PutItRight(data.Dir, shop.PriceSymbol , list.ReturnPrice));

                IsPaidReturnTr = true;
                
            }

            //// 借款金额(目前先不显示这个)
            $('.BorrowPrice').hide();
         


            // 检查雅座信息
            if (list.RoomInfo == null) {
                $('.RoomNo').hide();
                $('.RoomPrice').hide();
                $('.TotalTime').hide();
            } else {
                // 检查雅座编号
                if (isEmpty(list.RoomInfo.RoomNo)) {
                    $('.RoomNo').hide();
                } else {
                    $('.RoomNo').show();
                    $('#RoomNoV').text(list.RoomInfo.RoomNo);
                    
                    IsRoomMemberTr = true;
                }

                // 检查雅座价格
                if (list.RoomInfo.RoomPrice == 0) {
                    $('.RoomPrice').hide();
                } else {
                    $('.RoomPrice').show();
                    $('#RoomPriceV').text(PutItRight(data.Dir, shop.PriceSymbol , list.RoomInfo.RoomPrice));

                    IsRoomMemberTr = true;
                    
                }
                // 检查雅座时间
                if (isEmpty(list.RoomInfo.TotalTime)) {
                    $('.TotalTime').hide();
                } else {
                    $('.TotalTime').show();
                    $('#TotalTimeV').text(list.RoomInfo.TotalTime);
                    
                    IsRoomMemberTr = true;
                }
            }



            

            
            $('#AmountV').text(PutItRight(data.Dir, shop.PriceSymbol , list.TotalPrice));

            // 优惠价
            if (list.DealsPrice == 0) {
                $('.DealsPrice').hide();
            } else {
                $('.DealsPrice').show();
                $('#DealsPriceV').text(PutItRight(data.Dir, shop.PriceSymbol , list.DealsPrice));
                
            }

            // 实际价格
            if (list.ActualPrice == 0) {
                $('.ActualPrice').hide();
            } else {

                if (list.MemberDealsPrice == 0 && list.DealsPrice == 0)
                    $('.ActualPrice').hide();
                else {
                    $('.ActualPrice').show();
                    $('#ActualPriceV').text(PutItRight(data.Dir, shop.PriceSymbol, list.ActualPrice));
                }
            }

            // 显示雅座和会员名
            if (IsRoomMemberTr)
                $('#trRoomPrices').show();

            // 总是显示总价以及优惠实际
            $('#trTotalDeailsActualPrices').show();

            // 显示支付和退还
            if (IsPaidReturnTr)
                $('#trPaidReturnPrices').show();

        }


    }

    /* 解决语言问题导致的顺序不一致 */
function PutItRight(langDirection, first, second) {
    if (langDirection == "rtl")
            return second + first;
        else
            return first + second;
    }


    /* 计算高度以及显示列表 */
    function CalculateHeighAndDisplayList(ModeNo, langIndex) {

        if (ModeNo == 0) // 列表模式
        {

            // 计算图片高度
            $('.full').height($(window).height() / 100 * ListModeImgHeightScale);
            

            // 计算列表高度
            $('#datas').height($(window).height() - ($('.header').height() + $('.footer').height() + $('.page').height() + 20)); // 20是底部footer的padding
            var listValue = $('#datas').height() - 55; // ie 无法获取thead.手动设置45+10

            //初始化刷新列表信息
            var listCount = Math.floor(listValue / 40);

           

            $('.listValue').empty(); // IE不行用这种 $('#datas tbody').empty();
            if (null == list || list.BillDetailsModelList == null || list.BillDetailsModelList.length == 0) {
                $("#selectedCountValue").text(0);

            } else {

                PageCount = Math.ceil(list.BillDetailsModelList.length > listCount ? (list.BillDetailsModelList.length / listCount + (list.BillDetailsModelList.length % listCount)) : 1)
                CurrentPage = 1;

                var l = list.BillDetailsModelList.length;
                var j = list.BillDetailsModelList.length;
                for (var i = 0; i < listCount; i++) {
                    if (i >= l) {
                        //$('.listValue').append($("<tr></td></tr>"));
                        break;
                    }
                    else {
                        var ProductName = "";
                        if (langIndex == Language0.LangIndex) {
                            ProductName = list.BillDetailsModelList[i].ProductName0;
                        } else if (langIndex == Language1.LangIndex) {
                            ProductName = list.BillDetailsModelList[i].ProductName1;
                        } else if (langIndex == Language2.LangIndex) {
                            ProductName = list.BillDetailsModelList[i].ProductName2;
                        }
                        $('.listValue').append($("<tr><td>" + (j) + "</td><td>" + ProductName + "</td><td>" + list.BillDetailsModelList[i].Count + "</td><td>" + shop.PriceSymbol + list.BillDetailsModelList[i].Price + "</td><td>" + shop.PriceSymbol + list.BillDetailsModelList[i].TotalPrice + "</td></tr>"));
                        
                    }
                    --j;
                }

                if (list.BillDetailsModelList == null || list.BillDetailsModelList.length == 0)
                    $("#selectedCountValue").text(0);
                else
                    $("#selectedCountValue").text(l);
            }

        }
        else if (ModeNo == 1) { // 全屏模式

            // 计算图片高度
            $('.full').height($(window).height() - 120);
        }

        
    }




    /* 设置字体大小 */
    function ChangeFontSize(fs, ff) {
        $('body').css({ 'font-size': fs, 'font-family': ff });
    }

    /* 设置当前主题 */
    function ChangeTheme(themeIndex) {
        if (themeIndex == 1) {           //暗色
            $('.top').css('background', '#f9c700');
            //if ($.browser.msie && $.browser.version < 10 && $.browser.version > 8) {
            $(".top").css("filter", "none");
            $(".top").css("-ms - filter", "none");
            
            //}
            $('.SoftwareName').css({ 'background': '#333', 'color': '#f9c700' });
            $('.OwnerName').css('color', '#333');
            $('.footer').css('background', '#333');
            $('.SoftwareName11').css('background', '#f9c700').css('color', '#000');
        } else if (themeIndex == 0) {
        }    //亮色		
    }

