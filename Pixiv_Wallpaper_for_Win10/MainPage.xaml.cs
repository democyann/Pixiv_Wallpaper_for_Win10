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
using System.Collections;
using System.Threading;
using Windows.System.UserProfile;
using Windows.UI.Popups;

namespace Pixiv_Wallpaper_for_Win10
{
    /// <summary>
    /// 主界面
    /// @ democyann
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer timer;  //图片推送定时器
        private DispatcherTimer li_uptimer; //列表更新定时器

        private Conf c;
        private PixivTop50 top50;
        private PixivLike like;
        private ImageInfo img;

        public MainPage()
        {
            this.InitializeComponent();
            c = new Conf();
            top50 = new PixivTop50();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(c.time);
            timer.Tick += Timer_Tick;
            timer.Start();

            li_uptimer = new DispatcherTimer();
            li_uptimer.Interval = TimeSpan.FromHours(1);
            li_uptimer.Tick += Li_uptimer_Tick;
            li_uptimer.Start();

            main.Navigate(typeof(ShowPage));
        }

        private async void Li_uptimer_Tick(object sender, object e)
        {
            //定时更新列表(1h/次)
            switch (c.mode)
            {
                case "Top_50":
                    await top50.listUpdate(true);
                    break;
                case "You_Like":
                    await like.ListUpdate(true);
                    break;
                default:
                    await top50.listUpdate(true);
                    break;
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            update();
        }
        /// <summary>
        /// 作品更新并显示
        /// </summary>
        private async void update()
        {
            timer.Stop();
            var dialog = new MessageDialog("");

            ImageInfo img;
            switch (c.mode)
            {
                case "Top_50":
                    img = await top50.SelectArtWork();
                    break;
                case "You_Like":
                    img = await like.SelectArtWork();
                    break;
                default:
                    img = await top50.SelectArtWork();
                    break;
            }

            if (img != null)
            {
                c.lastImg = img;
                main.Navigate(typeof(ShowPage));
            }


            if (!UserProfilePersonalizationSettings.IsSupported())
            {
                dialog.Content = "您的设备不支持自动更换壁纸";
                await dialog.ShowAsync();
                return;
            }
            UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///local/" + c.lastImg.userId + c.lastImg.imgId));

            if (c.lockscr)
            {
                //更换锁屏
                bool lockscr = await settings.TrySetLockScreenImageAsync(file);
                if (!lockscr)
                {
                    dialog.Content = "更换锁屏操作失败。";
                    await dialog.ShowAsync();
                }
            }
            //更换壁纸
            bool deskscr = await settings.TrySetWallpaperImageAsync(file);

            if (!deskscr)
            {
                dialog.Content = "更换壁纸操作失败。";
                await dialog.ShowAsync();
            }


            timer.Interval = TimeSpan.FromMinutes(c.time);
            timer.Start();
        }


        private void Button_Click(object sender, RoutedEventArgs e)     //汉堡界面开关
        {
            lis.IsPaneOpen = !lis.IsPaneOpen;
        }

        private void show_btn_Click(object sender, RoutedEventArgs e)   //展示页面按钮
        {
            main.Navigate(typeof(ShowPage));
        }

        private void setting_btn_Click(object sender, RoutedEventArgs e) //设置界面按钮
        {
            main.Navigate(typeof(SettingPage));
        }

        private void next_btn_Click(object sender, RoutedEventArgs e)    //下一张图
        {
            update();
        }

        private async void visiturl_btn_Click(object sender, RoutedEventArgs e)       //访问p站
        {
            var uriPixiv = new Uri(@"https://www.pixiv.net/member_illust.php?mode=medium&illust_id=" + img.imgId);
            var visit = await Windows.System.Launcher.LaunchUriAsync(uriPixiv);
        }
    }
}
