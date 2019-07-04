using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace Pixiv_Wallpaper_for_Win10.Util
{
    public class Pixiv
    {
        private readonly string INDEX_URL = "https://www.pixiv.net";
        private readonly string POST_KEY_URL = "https://accounts.pixiv.net/login?lang=zh&source=pc&view_type=page&ref=wwwtop_accounts_index";
        private readonly string LOGIN_URL = "https://accounts.pixiv.net/api/login?lang=zh";
        private readonly string RECOMM_URL = "https://www.pixiv.net/rpc/recommender.php?type=illust&sample_illusts=auto&num_recommendations=1000&page=discovery&mode=all&tt=";
        private readonly string ILLUST_URL = "https://www.pixiv.net/rpc/illust_list.php?verbosity=&exclude_muted_illusts=1&illust_ids=";
        private readonly string DETA_URL = "https://api.imjad.cn/pixiv/v1/?type=illust&id=";
        private readonly string RALL_URL = "https://www.pixiv.net/ranking.php?mode=daily&content=illust&p=1&format=json";
        private Conf c;

        

        public string cookie { get; set; }
        public string token { get; set; }

        public Pixiv()
        {
            c = new Conf();
        }

        /// <summary>
        /// 获取TOP 50推荐列表
        /// </summary>
        /// <returns></returns>
        public async Task<ArrayList> getRallist()
        {
            string rall;
            ArrayList list = new ArrayList();
            HttpUtil top50 = new HttpUtil(RALL_URL, HttpUtil.Contype.JSON);

            rall = await top50.GetDataAsync();

            if (!rall.Equals("ERROR"))
            {
                dynamic o = JObject.Parse(rall);
                JArray arr = o.contents;

                foreach (JToken j in arr)
                {
                    list.Add(j["illust_id"].ToString());
                }
            }
            else
            {
                MessageDialog dialog = new MessageDialog("update rall list Error");
                await dialog.ShowAsync();
            }
            return list;
        }

        /// <summary>
        /// 获取POST KEY 私有方法
        /// </summary>
        /// <returns>POST KEY</returns>
        public async Task<string> postKey()
        {
            string key = "";
            //获取POST KEY

            HttpUtil posturl = new HttpUtil(POST_KEY_URL, HttpUtil.Contype.HTML);
            posturl.authority = "accounts.pixiv.net";
            string poststr = await posturl.GetDataAsync();
            if (!poststr.Equals("ERROR"))
            {
                Console.WriteLine("postkey" + poststr);
                Regex r = new Regex("name=\"post_key\"\\svalue=\"([a-z0-9]{32})\"", RegexOptions.Singleline);
                if (r.IsMatch(poststr))
                {
                    key = r.Match(poststr).Groups[1].ToString();
                    cookie = posturl.cookie;
                }
            }
            return key;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns>true 登录成功 false 登录失败</returns>
        private async Task<bool> login()
        {
            bool f = false;
            string postkey = await postKey();
            HttpUtil loginurl = new HttpUtil(LOGIN_URL, HttpUtil.Contype.JSON);
            loginurl.cookie = cookie;
            string pram = "pixiv_id=" + c.account
                        + "&password=" + c.password
                        + "&captcha=&g_recaptcha_response=&post_key=" + postkey
                        + "&source=pc&ref=wwwtop_accounts_index&return_to=http://www.pixiv.net/";

            string data = await loginurl.PostDataAsync(pram);

            if (!data.Equals("ERROR"))
            {
                Debug.Write("login:" + data);
                dynamic o = JObject.Parse(data);
                if (o.body.success != null)
                {
                    cookie = loginurl.cookie;
                    f = true;
                }
                else
                {
                    //使UI线程调用lambda表达式内的方法
                    await MainPage.mp.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async() => 
                    {
                        //UI code here
                        MessageDialog dialog = new MessageDialog("登录失败,请输入正确的账号密码");
                        await dialog.ShowAsync();
                    });
                }
            }
            return f;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="flag">true 登录获取，false 不登录获取</param>
        /// <returns></returns>
        public async Task<bool> getToken(bool flag = false)
        {
            bool f = false;
            if (flag)
            {
                if (!await login()) return f;                //如果没登录成功，返回结果false
            }

            HttpUtil tokurl = new HttpUtil(INDEX_URL, HttpUtil.Contype.HTML);
            tokurl.cookie = cookie;
            string data = await tokurl.GetDataAsync();
            if (!data.Equals("ERROR"))
            {
                Console.WriteLine("token:" + data);
                Regex r = new Regex("pixiv.context.token\\s=\\s\"([a-z0-9]{32})\"");
                if (r.IsMatch(data))
                {
                    token = r.Match(data).Groups[1].ToString();
                    f = true;
                }
            }
            else
            {
                //使UI线程调用lambda表达式内的方法
                await MainPage.mp.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    //UI code here
                    MessageDialog dialog = new MessageDialog("get token failure");
                    await dialog.ShowAsync();
                });
            }
            return f;
        }

        /// <summary>
        /// 获取"猜你喜欢"推荐列表
        /// </summary>
        /// <returns></returns>

        public async Task<ArrayList> getRecommlist()
        {
            string like;
            ArrayList list = new ArrayList();
            HttpUtil recomm = new HttpUtil(RECOMM_URL + token, HttpUtil.Contype.JSON);
            recomm.cookie = cookie;
            recomm.referer = "https://www.pixiv.net/discovery";

            like = await recomm.GetDataAsync();

            if (like != "ERROR")
            {
                dynamic o = JObject.Parse(like);
                JArray arr = o.recommendations;
                foreach (JToken j in arr)
                {
                    list.Add(j.ToString());
                }
            }
            else
            {
                //使UI线程调用lambda表达式内的方法
                await MainPage.mp.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    //UI code here
                    MessageDialog dialog = new MessageDialog("update recomm list failure");
                    await dialog.ShowAsync();
                });

            }
    
            return list;
        }
        /// <summary>
        /// 图片信息查询子方法1(R18作品无法查询地址)
        /// </summary>
        /// <param name="imgid">要查找的作品ID</param>
        /// <returns></returns>
        private async Task<string> getImageInfoSub1(string imgid)
        {
            HttpUtil info1 = new HttpUtil(DETA_URL + imgid, HttpUtil.Contype.JSON);
            string data = await info1.GetDataAsync();
            return data;
        }
       

        /// <summary>
        /// 查询图片信息
        /// </summary>
        /// <param name="id">要查找的作品ID</param>
        /// <returns></returns>
        public async Task<ImageInfo> getImageInfo(string id)
        {
            ImageInfo imginfo = null;
            string info1 = await getImageInfoSub1(id);
            if (!info1.Equals("ERROR"))
            {
                imginfo = new ImageInfo();

                dynamic o = JObject.Parse(info1);
                dynamic ill = o.response;
                imginfo.viewCount = (int)ill[0]["stats"]["views_count"];
                imginfo.imgUrl = ill[0]["image_urls"]["large"].ToString();
                switch(ill[0]["age_limit"].ToString())
                {
                    case "all_age":
                        imginfo.isR18 = false;
                        break;
                    case "limit_r18":
                        imginfo.isR18 = true;
                        break;
                }

                dynamic user = ill[0]["user"].ToString();
                imginfo.userId = ill[0]["user"]["id"].ToString();
                imginfo.userName = ill[0]["user"]["name"].ToString();
                imginfo.imgId = ill[0]["id"].ToString() ;
                imginfo.imgName = ill[0]["title"].ToString();
                imginfo.tag = ill[0]["tags"].ToString();
                imginfo.height = (int)ill[0]["height"];
                imginfo.width = (int)ill[0]["width"];

            }

            return imginfo;
        }

        /// <summary>
        /// 图片下载
        /// </summary>
        /// <param name="img">要下载的图片信息</param>
        /// <returns></returns>
        public async Task<string> downloadImg(ImageInfo img)
        {
            Regex reg = new Regex("/c/[0-9]+x[0-9]+/img-master");
            img.imgUrl = reg.Replace(img.imgUrl, "/img-master", 1);

            HttpUtil download = new HttpUtil(img.imgUrl, HttpUtil.Contype.IMG);
            download.referer = "https://www.pixiv.net/member_illust.php?mode=medium&illust_id=" + img.imgId;
            download.cookie = c.cookie;
            string path = await download.ImageDownloadAsync(img.imgId);
            return path;
        }

    }
}
