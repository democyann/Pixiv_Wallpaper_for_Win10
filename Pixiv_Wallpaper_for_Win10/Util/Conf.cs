using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Pixiv_Wallpaper_for_Win10.Util
{
    /// <summary>
    /// 设置管理类
    /// </summary>
    public class Conf
    {
        private ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        /// <summary>
        /// 登录信息
        /// </summary>
        public string account
        {
            get
            {
                if (localSettings.Values["Account"]!=null)
                {
                    return localSettings.Values["Account"].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string password
        {
            get
            {
                if (localSettings.Values["Password"] != null)
                {
                    return localSettings.Values["Password"].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 推荐模式
        /// </summary>
        public string mode
        {
            get
            {
                if (localSettings.Values["Mode"] == null)
                {
                    return "Top_50";
                }
                else
                {
                    return localSettings.Values["Mode"].ToString();
                }
            }
        }

        /// <summary>
        /// 推荐时间
        /// </summary>
        public int time
        {
            get
            {
                if (localSettings.Values["Time"] == null)
                {
                    return 10;
                }
                else
                {
                    return Convert.ToInt32(localSettings.Values["Time"]);
                }
            }
        }
        /// <summary>
        /// 是否更改锁屏
        /// </summary>
        public bool lockscr
        {
            get
            {
                if (localSettings.Values["Lock"] == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(localSettings.Values["Lock"]);
                }
            }
        }

        /// <summary>
        /// 获取或设置 token
        /// </summary>
        public string token
        {
            get
            {
                if (localSettings.Values["token"] == null)
                {
                    return "";
                }
                else
                {
                    return localSettings.Values["token"].ToString();
                }
            }
            set
            {
                localSettings.Values["token"] = value;
            }
        }

        /// <summary>
        /// 获取或设置 cookie
        /// </summary>
        public string cookie
        {
            get
            {
                if (localSettings.Values["cookie"] == null)
                {
                    return "";
                }
                else
                {
                    return localSettings.Values["cookie"].ToString();
                }
            }
            set
            {
                localSettings.Values["cookie"] = value;
            }
        }

        /// <summary>
        /// 最后一次显示成功的图片信息
        /// </summary>
        public ImageInfo lastImg
        {
            get
            {
                if (localSettings.Values["lastImg"] == null)
                {
                    return null;
                }
                else
                {
                    ImageInfo i = JsonConvert.DeserializeObject<ImageInfo>(localSettings.Values["lastImg"].ToString());
                    return i;
                }
            }
            set
            {
                localSettings.Values["lastImg"] = JsonConvert.SerializeObject(value);
            }
        }

        /// <summary>
        /// HTTP代理开关
        /// </summary>
        public string proxy
        {
            get
            {
                if(localSettings.Values["proxy"] == null)
                {
                    return "disable";
                }
                else
                {
                    return localSettings.Values["proxy"].ToString();
                }
            }
            set
            {
                localSettings.Values["proxy"] = JsonConvert.SerializeObject(value);
            }
        }

        /// <summary>
        /// 代理端口
        /// </summary>
        public string proxy_port
        {
            get
            {
                if (localSettings.Values["proxy_port"] == null)
                {
                    return "";
                }
                else
                    return localSettings.Values["proxy_port"].ToString();
            }
            set
            {
                localSettings.Values["proxy_port"] = JsonConvert.SerializeObject(value);
            }
        }
    }
}
