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
using Windows.Storage.Streams;
using System.Threading.Tasks;

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
        private BitmapImage useless;
        private StorageFile file;
        public ShowPage()
        {
            this.InitializeComponent();
            SetImage();

        }

        private async Task SetImage()
        {
            c = new Conf();
            if (c.lastImg == null)
            {
                file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Res/62258773_p0.png"));
            }
            else
            {
                img = c.lastImg;
                file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///local/" + c.lastImg.imgId));
                
                textBlock1.Text = img.imgName;
                textBlock2.Text = img.userName;
                textBlock3.Text = (img.viewCount + "次浏览");
            }
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapImage bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(fileStream);
                show_img.Source = bitmap;
            }
        }
        
    }
}
