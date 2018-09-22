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
using Windows.Storage;
using Pixiv_Wallpaper_for_Win10.Util;
using System.Collections.ObjectModel;
using Windows.System;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Pixiv_Wallpaper_for_Win10
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;  //获取本地应用设置容器(单例)
        StorageFolder folder = ApplicationData.Current.LocalFolder;

        public SettingPage()
        {
            this.InitializeComponent();
            
            //下拉框初始化
            combox1.Items.Add(new KeyValuePair<string, int>("1 分钟", 1));
            combox1.Items.Add(new KeyValuePair<string, int>("5 分钟", 5));
            combox1.Items.Add(new KeyValuePair<string, int>("10 分钟", 10));
            combox1.Items.Add(new KeyValuePair<string, int>("15 分钟", 15));
            combox1.Items.Add(new KeyValuePair<string, int>("30 分钟", 30));
            combox1.Items.Add(new KeyValuePair<string, int>("60 分钟", 60));

            //值填充
            Conf c = new Conf();

            combox1.SelectedValue = c.time;

            switch (c.mode)
            {
                case "Top_50":
                    radiobutton1.IsChecked = true;
                    break;
                case "You_Like":
                    radiobutton2.IsChecked = true;
                    break;
                default:
                    radiobutton1.IsChecked = true;
                    break;
            }
            lock_che.IsChecked = c.lockscr;
            textbox1.Text = c.account;
            passwordbox1.Password = c.password;


        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (radiobutton2.IsChecked == true)
            {
                localSettings.Values["Mode"] = "You_Like";        //设置本地保存文件（模式）为猜你喜欢
            }
            if (radiobutton1.IsChecked == true)
            {
                localSettings.Values["Mode"] = "Top_50";         //设置本地保存文件（模式）为TOP50
            }
            localSettings.Values["Lock"] = lock_che.IsChecked;
            localSettings.Values["Account"] = textbox1.Text;     //保存账号
            localSettings.Values["Password"] = passwordbox1.Password;    //保存密码
            localSettings.Values["Time"] = combox1.SelectedValue;    //保存时间
        }

        private async void openfilepath_Click(object sender, RoutedEventArgs e)
        {
            var t = new FolderLauncherOptions();
            await Launcher.LaunchFolderAsync(folder, t);
        }

        private async void clear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var f in await folder.GetItemsAsync())
            {
                await f.DeleteAsync();
            }
        }
    }
}
