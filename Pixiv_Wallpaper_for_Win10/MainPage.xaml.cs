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

            //测试方法，输出top50列表
        
            //HttpUtil http = new HttpUtil("https://www.pixiv.net/ranking.php?mode=daily&content=illust&p=1&format=json", HttpUtil.Contype.JSON);
            HttpUtil http = new HttpUtil("https://www.pixiv.net", HttpUtil.Contype.HTML);

            string s = await http.GetDataAsync();
            Debug.WriteLine(s);
            Debug.WriteLine(http.cookie);

            //================
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
