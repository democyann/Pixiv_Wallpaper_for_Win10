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
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.UI.Core;

namespace Pixiv_Wallpaper_for_Win10
{
    /// <summary>
    /// 主界面
    /// @ democyann
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer timer;  //图片推送定时器

        private Conf c;
        private ImageInfo img;
        public static MainPage mp;
        private ExtendedExecutionSession session;
        private PixivTop50 top50;
        private PixivLike like;

        public MainPage()
        {
            this.InitializeComponent();
            mp = this;
            top50 = new PixivTop50();
            like = new PixivLike();
            c = new Conf();
            img = c.lastImg;
            session = null;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(c.time);
            timer.Tick += Timer_Tick;
            timer.Start();

            BeginExtendedExecution(); //申请后台常驻

            if(c.proxy=="enable")
            {
                HttpUtil.proxyPort = c.proxy_port;
            }
            else
            {
                HttpUtil.proxyPort = null;
            }

            if (c.lastImg != null) { 
                ImageBrush br = new ImageBrush();
                br.Stretch = Stretch.UniformToFill;
                br.AlignmentX = AlignmentX.Left;
                br.AlignmentY = AlignmentY.Top;
                br.ImageSource = new BitmapImage(new Uri("ms-appdata:///local/" + c.lastImg.imgId));
            }
            else
            {
                ImageBrush br = new ImageBrush();
                br.Stretch = Stretch.UniformToFill;
                br.AlignmentX = AlignmentX.Left;
                br.AlignmentY = AlignmentY.Top;
                br.ImageSource = new BitmapImage(new Uri("ms-appx:///Res/62258773_p0.png"));
            }

            main.Navigate(typeof(ShowPage));
        }


        private async void Timer_Tick(object sender, object e)
        {
            SetWallpaper(await update());
        }
        /// <summary>
        /// 作品更新并显示
        /// </summary>
        private async Task<bool> update()
        {
            timer.Stop();

            //ImageInfo img;
            switch (c.mode)
            {
                case "Top_50":
                    await Task.Run(async () => { img = await top50.SelectArtWork(); });         
                    break;
                case "You_Like_V1":
                    await Task.Run(async () => { img = await like.SelectArtWorkV1(); });
                    break;
                case "You_Like_V2":
                    img = await like.SelectArtWorkV2();//该API无法在子线程中调用
                    break;
                default:
                    await Task.Run(async () => { img = await top50.SelectArtWork(); });
                    break;
            }

            if (img != null)
            {
                c.lastImg = img;
                main.Navigate(typeof(ShowPage));//图片展示页面更新

                timer.Interval = TimeSpan.FromMinutes(c.time);
                timer.Start();
            }
            return true;
        }

        private async void SetWallpaper(bool done)
        {
            if(done)
            {
                var dialog = new MessageDialog("");
                if (!UserProfilePersonalizationSettings.IsSupported())
                {
                    dialog.Content = "您的设备不支持自动更换壁纸";
                    await dialog.ShowAsync();
                    return;
                }
                UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
                StorageFile file = null;
                try
                {
                    file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///local/" + img.imgId));
                }
                catch (Exception)
                {
                    timer.Interval = TimeSpan.FromSeconds(2);
                    timer.Start();
                }

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
                ImageBrush br = new ImageBrush();
                br.Stretch = Stretch.UniformToFill;
                br.AlignmentX = AlignmentX.Left;
                br.AlignmentY = AlignmentY.Top;
                if(img.imgId!=null)
                {
                    br.ImageSource = new BitmapImage(new Uri("ms-appdata:///local/" + img.imgId));
                }
            }
        }
        private void ClearExtendedExecution()
        {
            if (session != null)
            {
                session.Revoked -= SessionRevoked;
                session.Dispose();
                session = null;
            }
        }

        private async void BeginExtendedExecution()
        {
            // The previous Extended Execution must be closed before a new one can be requested.
            ClearExtendedExecution();

            var newSession = new ExtendedExecutionSession();
            newSession.Reason = ExtendedExecutionReason.Unspecified;
            newSession.Description = "Raising periodic toasts";
            newSession.Revoked += SessionRevoked;
            ExtendedExecutionResult result = await newSession.RequestExtensionAsync();

            switch(result)
            {
                case ExtendedExecutionResult.Allowed:
                    Debug.WriteLine("Extended execution allowed.");
                    session = newSession;
                    break;
                default:
                case ExtendedExecutionResult.Denied:
                    Debug.WriteLine("Extended execution denied.");
                    newSession.Dispose();
                    break;
            }
        }

        private async void SessionRevoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            //session被系统回收时记录原因，session被回收则无法保持后台运行
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch (args.Reason)
                {
                    case ExtendedExecutionRevokedReason.Resumed:
                        Debug.WriteLine("Extended execution revoked due to returning to foreground.");
                        break;
                    case ExtendedExecutionRevokedReason.SystemPolicy:
                        Debug.WriteLine("Extended execution revoked due to system policy.");
                        break;
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)     //汉堡视图开关
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

        private void visiturl_btn_Click(object sender, RoutedEventArgs e)       //访问p站
        {
            var uriPixiv = new Uri(@"https://www.pixiv.net/artworks/" + img.imgId);
            var visit = Windows.System.Launcher.LaunchUriAsync(uriPixiv);
        }

        private void setWallpaper_btn_Click(object sender, RoutedEventArgs e)
        {
            SetWallpaper(true);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            switch(c.mode)
            {
                case "Top_50":
                    await Task.Run(async () => { await top50.listUpdate(true); });
                    break;
                case "You_Like_V1":
                    await Task.Run(async () => { await like.ListUpdateV1(true); });
                    break;
                case "You_Like_V2":
                    await Task.Run(async () => { await like.ListUpdateV2(true); });
                    break;
                default:
                    await Task.Run(async () => { await top50.listUpdate(true); });
                    break;
            }
        }
    }
}
