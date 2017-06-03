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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace Pixiv_Wallpaper_for_Win10
{
    /// <summary>
    /// 图片预览界面
    /// @ democyann
    /// </summary>
    public sealed partial class ShowPage : Page
    {
        public ShowPage()
        {
            this.InitializeComponent();
            show_img.Source = new BitmapImage(new Uri("ms-appx:///Res/62258773_p0.png"));
        }
    }
}
