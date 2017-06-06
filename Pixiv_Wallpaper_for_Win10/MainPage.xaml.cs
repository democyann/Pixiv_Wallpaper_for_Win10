using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Windows.Storage;
using Pixiv_Wallpaper_for_Win10.Util;

namespace Pixiv_Wallpaper_for_Win10
{
    /// <summary>
    /// 主界面
    /// @ democyann
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            main.Navigate(typeof(ShowPage));
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            lis.IsPaneOpen = !lis.IsPaneOpen;

            //============图片下载示例代码============

            string url = "https://i.pximg.net/img-original/img/2016/02/29/23/44/55/55558612_p0.jpg";
            string refurl = "https://www.pixiv.net/member_illust.php?mode=medium&illust_id=55558612";

            HttpUtil test = new HttpUtil(url, HttpUtil.Contype.IMG);
            test.referer = refurl;
            string a = await test.ImageDownloadAsync("234234", "12333");

            main.Navigate(typeof(ShowPage), a);

            //==========================================


        }

        private void show_btn_Click(object sender, RoutedEventArgs e)
        {
            main.Navigate(typeof(ShowPage));
        }

        private void setting_btn_Click(object sender, RoutedEventArgs e)
        {
            main.Navigate(typeof(SettingPage));
        }
    }
}
