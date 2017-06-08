﻿using System;
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
        private DispatcherTimer timer;
        private Conf c;
        private PixivTop50 top50;

        public MainPage()
        {
            this.InitializeComponent();
            c = new Conf();
            top50 = new PixivTop50();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(c.time);
            timer.Tick += Timer_Tick;
            timer.Start();

            main.Navigate(typeof(ShowPage));
        }

        private void Timer_Tick(object sender, object e)
        {
            update();
        }

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


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lis.IsPaneOpen = !lis.IsPaneOpen;
        }

        private void show_btn_Click(object sender, RoutedEventArgs e)
        {
            main.Navigate(typeof(ShowPage));
        }

        private void setting_btn_Click(object sender, RoutedEventArgs e)
        {
            main.Navigate(typeof(SettingPage));
        }

        private void next_btn_Click(object sender, RoutedEventArgs e)
        {
            update();
        }
    }
}
