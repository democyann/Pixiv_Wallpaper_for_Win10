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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Pixiv_Wallpaper_for_Win10
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;  //获取本地应用设置容器(单例)
       /* public SettingPage()
        {
            this.InitializeComponent();
        }

        private void RadioButton_Checked(System.Object sender, RoutedEventArgs e)     
        {
            
        }

        private void radiobutton2_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void textbox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        } */

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (radiobutton1.IsChecked == true)
            {
                localSettings.Values["Mode"] = "You_Like";        //设置本地保存文件（模式）为猜你喜欢
            }
            if (radiobutton2.IsChecked == true)
            {
                localSettings.Values["Mode"] = "Top_50";         //设置本地保存文件（模式）为TOP50
            }
            localSettings.Values["Account"] = textbox1.Text;     //保存账号
            localSettings.Values["Password"] = passwordbox1.Password;    //保存密码
            localSettings.Values["Time"] = combox1.SelectedValue;    //保存时间
        }

       /* private void combox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        } */
    }
}
