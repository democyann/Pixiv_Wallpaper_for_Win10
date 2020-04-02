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
using Windows.Storage;
using Pixiv_Wallpaper_for_Win10.Util;
using System.Collections.ObjectModel;
using Windows.System;
using System.Threading.Tasks;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Pixiv_Wallpaper_for_Win10
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        StorageFolder folder = ApplicationData.Current.LocalFolder;
        Conf c = new Conf();

        public SettingPage()
        {
            this.InitializeComponent();
            
            //下拉框初始化
            combox1.Items.Add(new KeyValuePair<string, int>("15 分钟", 15));
            combox1.Items.Add(new KeyValuePair<string, int>("30 分钟", 30));
            combox1.Items.Add(new KeyValuePair<string, int>("60 分钟", 60));
            combox1.Items.Add(new KeyValuePair<string, int>("120 分钟", 120));
            combox1.Items.Add(new KeyValuePair<string, int>("180 分钟", 180));

            //值填充

            combox1.SelectedValue = c.time;

            if(c.proxy)
            {
                proxy_check.IsChecked = true;
                //textbox3.IsEnabled = true;
            }
            else
            {
                proxy_check.IsChecked = false; 
                //textbox3.IsEnabled = false;
            }

            if(c.cookie!=null&&!"".Equals(c.cookie))
            {
                loginV1.Content = "已登录";
            }
            else
            {
                loginV1.Content = "请登录";
            }

            lock_switch.IsOn = c.lockscr;
            textbox1.Text = c.account;
            passwordbox1.Password = c.password;
            textbox3.Text = c.proxy_port;

            switch (c.mode)
            {
                case "Top_50":
                    radiobutton1.IsChecked = true;
                    break;
                case "You_Like_V1":
                    radiobutton2.IsChecked = true;
                    break;
                case "You_Like_V2":
                    radiobutton3.IsChecked = true;
                    break;
                default:
                    radiobutton1.IsChecked = true;
                    break;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (radiobutton2.IsChecked == true)
            {
                c.mode = "You_Like_V1";        //设置本地保存文件（模式）为猜你喜欢(Webview)
            }
            if (radiobutton1.IsChecked == true)
            {
                c.mode = "Top_50";         //设置本地保存文件（模式）为TOP50
            }
            if(radiobutton3.IsChecked == true)
            {
                c.mode = "You_Like_V2";   //设置本地保存文件 (模式) 为猜你喜欢(PixivCS)
            }
            if(proxy_check.IsChecked == true)
            {
                if (textbox3.Text != null)
                {
                    c.proxy_port = textbox3.Text;
                    c.proxy = true;
                }
                else
                {
                    proxy_check.IsChecked = false;
                    c.proxy = false;
                }                   
            }
            else
            {
                c.proxy = false;
                if (textbox3.Text != null)
                {
                    c.proxy_port = textbox3.Text;
                }
            }
            c.lockscr = lock_switch.IsOn;
            c.account = textbox1.Text;     //保存账号
            c.password = passwordbox1.Password;    //保存密码
            c.time = (int)combox1.SelectedValue;    //保存时间
        }

        private void SetCookie(string str)
        {
            c.cookie = str;
            loginV1.Content = "已登录";
        }

        private async void openFilePath_Click(object sender, RoutedEventArgs e)
        {
            var t = new FolderLauncherOptions();
            await Launcher.LaunchFolderAsync(folder, t);
        }

        private async void clearPicture_Click(object sender, RoutedEventArgs e)
        {
            foreach (StorageFile f in await folder.GetItemsAsync())
            {
                if(!f.Name.Equals(c.lastImg.imgId))
                {
                    await f.DeleteAsync();
                }     
            }
        }

        private void cleanToken_Click(object sender, RoutedEventArgs e)
        {
            c.token = null;
            c.cookie = null;
            loginV1.Content = "未登录";
        }

        private async void loginV1_Click(object sender, RoutedEventArgs e)
        {
            WebViewLogin wvl = new WebViewLogin("https://accounts.pixiv.net/login", "https://www.pixiv.net/");
            wvl.ClearCookies();
            wvl.Method += SetCookie;
            await wvl.ShowWebView(1000, 800);
        }

        private void radiobutton2_Checked(object sender, RoutedEventArgs e)
        {
            loginV1.IsEnabled = true;
        }

        private void radiobutton3_Checked(object sender, RoutedEventArgs e)
        {
            textbox1.IsEnabled = true;
            passwordbox1.IsEnabled = true;
        }

        private void radiobutton2_Unchecked(object sender, RoutedEventArgs e)
        {
            loginV1.IsEnabled = false;
        }

        private void radiobutton3_Unchecked(object sender, RoutedEventArgs e)
        {
            textbox1.IsEnabled = false;
            passwordbox1.IsEnabled = false;
        }

        private void proxy_check_Checked(object sender, RoutedEventArgs e)
        {
            textbox3.IsEnabled = true;
        }

        private void proxy_check_Unchecked(object sender, RoutedEventArgs e)
        {
            textbox3.IsEnabled = false;
        }
    }
}
