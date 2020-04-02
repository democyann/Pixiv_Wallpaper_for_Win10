using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.QueryStringDotNET;
using Windows.UI.Notifications;

namespace Pixiv_Wallpaper_for_Win10.Util
{
    class ToastManagement
    {
        private string title { get; set; }
        private string content { get; set; }
        private string image { get; set; }
        private string mode { get; set; }
        private readonly string logo = "ms-appdata:///Square44x44Logo.scale-200.png";
        private ToastVisual visual;
        private ToastActionsCustom actions;
        private ToastContent toastContent;

        public ToastManagement(string title,string content,string mode,string image = null)
        {
            this.title = title;
            this.content = content;
            this.image = image;
            this.mode = mode;
        }
        /// <summary>
        /// 推送本地Toast通知
        /// </summary>
        /// <param name="hours">toast消息过期时间(小时)</param>
        public void ToastPush(int hours)
        {
            // Construct the visuals of the toast
            if(image != null)
            {
                visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = title
                            },
                            new AdaptiveText()
                            {
                                Text = content
                            },
                            new AdaptiveImage()
                            {
                                Source = image
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = logo,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                };
            }
            else
            {
                visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = title
                            },
                            new AdaptiveText()
                            {
                                Text = content
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = logo,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                };
            }
            actions = new ToastActionsCustom();
            switch (mode)
            {
                case "BackgroundDenied":
                    actions.Buttons.Add(new ToastButton("电源设置", new QueryString() { "action", "BatterySetting" }.ToString())
                    {
                        ActivationType = ToastActivationType.Protocol
                    });
                    break;
                case "WallpaperUpdate":
                    actions.Buttons.Add(new ToastButton("下一张", new QueryString() { "action", "NextIllust" }.ToString())
                    {
                        ActivationType = ToastActivationType.Background
                    });
                    break;
            }
            toastContent = new ToastContent();
            toastContent.Visual = visual;
            if(actions!=null)
            {
                toastContent.Actions = actions;
            }
            var toast = new ToastNotification(toastContent.GetXml());
            toast.ExpirationTime = DateTime.Now.AddHours(hours);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

    }
}
