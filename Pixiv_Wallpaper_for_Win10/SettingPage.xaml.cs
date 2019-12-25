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
        Conf c = new Conf();

        public SettingPage()
        {
            this.InitializeComponent();
            
            //下拉框初始化
            combox1.Items.Add(new KeyValuePair<string, int>("10 分钟", 10));
            combox1.Items.Add(new KeyValuePair<string, int>("30 分钟", 30));
            combox1.Items.Add(new KeyValuePair<string, int>("60 分钟", 60));
            combox1.Items.Add(new KeyValuePair<string, int>("120 分钟", 120));
            combox1.Items.Add(new KeyValuePair<string, int>("180 分钟", 180));

            //值填充

            combox1.SelectedValue = c.time;

            switch (c.proxy)
            {
                case "enable":
                    radiobutton4.IsChecked = true;
                    textbox3.IsEnabled = true;
                    break;
                case "disable":
                    radiobutton5.IsChecked = true;
                    textbox3.IsEnabled = false;
                    break;
                default:
                    radiobutton5.IsChecked = true;
                    textbox3.IsEnabled = false;
                    break;
            }

            if(c.cookie!=null)
            {
                loginV1.Content = "已登录";
            }
            else
            {
                loginV1.Content = "请登录";
            }

            lock_che.IsChecked = c.lockscr;
            textbox1.Text = c.account;
            passwordbox1.Password = c.password;
            textbox3.Text = c.proxy_port;

            switch (c.mode)
            {
                case "Top_50":
                    radiobutton1.IsChecked = true;
                    textbox1.IsEnabled = false;
                    passwordbox1.IsEnabled = false;
                    loginV1.IsEnabled = false;
                    break;
                case "You_Like_V1":
                    radiobutton2.IsChecked = true;
                    textbox1.IsEnabled = false;
                    passwordbox1.IsEnabled = false;
                    loginV1.IsEnabled = true;
                    break;
                case "You_Like_V2":
                    radiobutton3.IsChecked = true;
                    loginV1.IsEnabled = false;
                    textbox1.IsEnabled = true;
                    passwordbox1.IsEnabled = true;
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
                localSettings.Values["Mode"] = "You_Like_V1";        //设置本地保存文件（模式）为猜你喜欢(Webview)
            }
            if (radiobutton1.IsChecked == true)
            {
                localSettings.Values["Mode"] = "Top_50";         //设置本地保存文件（模式）为TOP50
            }
            if(radiobutton3.IsChecked==true)
            {
                localSettings.Values["Mode"] = "You_Like_V2";   //设置本地保存文件 (模式) 为猜你喜欢(PixivCS)
            }
            if(radiobutton4.IsChecked==true)
            {
                localSettings.Values["proxy"] = "enable";
                if (textbox3.Text != null)
                {
                    localSettings.Values["proxy_port"] = textbox3.Text;
                }
                else
                {
                    radiobutton5.IsChecked = true;
                }                   
            }
            if (radiobutton5.IsChecked == true)
            {
                localSettings.Values["proxy"] = "disable";
            }
            localSettings.Values["Lock"] = lock_che.IsChecked;
            localSettings.Values["Account"] = textbox1.Text;     //保存账号
            localSettings.Values["Password"] = passwordbox1.Password;    //保存密码
            localSettings.Values["Time"] = combox1.SelectedValue;    //保存时间
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

        private void Radiobutton4_Checked(object sender, RoutedEventArgs e)
        {
            textbox3.IsEnabled = true;
        }

        private void Radiobutton5_Checked(object sender, RoutedEventArgs e)
        {
            textbox3.IsEnabled = false;
        }

        private void cleanToken_Click(object sender, RoutedEventArgs e)
        {
            c.token = null;
            c.cookie = null;
            loginV1.Content = "未登录";
        }

        private async void loginV1_Click(object sender, RoutedEventArgs e)
        {
            WebViewLogin wvl = new WebViewLogin(900, 600);
            wvl.targetUri = "https://www.pixiv.net/";
            wvl.loginUri = "https://accounts.pixiv.net/login";
            wvl.ClearCookies();
            wvl.Method += SetCookie;
            await wvl.ShowWebView();
        }

        private void radiobutton1_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(radiobutton1.IsChecked==true)
            {
                if (textbox1.Text == null)
                {
                    textbox1.Text = "";
                }
                if (passwordbox1.Password == null)
                {
                    passwordbox1.Password = "";
                }
                textbox1.IsEnabled = false;
                passwordbox1.IsEnabled = false;
                loginV1.IsEnabled = false;
            }
        }

        private void radiobutton2_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(radiobutton2.IsChecked==true)
            {
                if (textbox1.Text == null)
                {
                    textbox1.Text = "";
                }
                if (passwordbox1.Password == null)
                {
                    passwordbox1.Password = "";
                }
                textbox1.IsEnabled = false;
                passwordbox1.IsEnabled = false;
                loginV1.IsEnabled = true;
            }   
        }

        private void radiobutton3_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(radiobutton3.IsChecked==true)
            {
                textbox1.IsEnabled = true;
                passwordbox1.IsEnabled = true;
                loginV1.IsEnabled = false;
            }            
        }
    }
}
