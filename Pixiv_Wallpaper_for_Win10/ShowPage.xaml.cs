using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Pixiv_Wallpaper_for_Win10.Util;

namespace Pixiv_Wallpaper_for_Win10
{
    /// <summary>
    /// 图片预览界面
    /// @ democyann
    /// </summary>
    public sealed partial class ShowPage : Page
    {

        private Conf c;
        private ImageInfo img;
        public ShowPage()
        {
            this.InitializeComponent();
            c = new Conf();
            if (c.lastImg == null)
            {
                show_img.Source = new BitmapImage(new Uri("ms-appx:///Res/62258773_p0.png"));
            }
            else
            {
                img = c.lastImg;

                show_img.Source = new BitmapImage(new Uri("ms-appdata:///local/" + c.lastImg.imgId));

                textBlock1.Text = img.imgName;
                textBlock2.Text = img.userName;
                textBlock3.Text = (img.viewCount + "次浏览");
            }

        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    if (e.Parameter != null)
        //    {
        //        Debug.WriteLine(e.Parameter.ToString());
        //        show_img.Source = new BitmapImage(new Uri("ms-appdata:///temp/" + e.Parameter.ToString()));
        //    }
        //}
    }
}
