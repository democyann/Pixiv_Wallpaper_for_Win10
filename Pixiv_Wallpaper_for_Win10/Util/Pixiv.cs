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
using Windows.Data.Json;
using PixivCS;
using System.Collections.Concurrent;
using System.Web;
using System.IO;

namespace Pixiv_Wallpaper_for_Win10.Util
{
    public class Pixiv
    {
        private readonly string INDEX_URL = "https://www.pixiv.net";
        private readonly string RECOMM_URL = "https://www.pixiv.net/rpc/recommender.php?type=illust&sample_illusts=auto&num_recommendations=1000&page=discovery&mode=all&tt=";
        private readonly string DETA_URL = "https://api.imjad.cn/pixiv/v1/?type=illust&id=";
        private readonly string RALL_URL = "https://www.pixiv.net/ranking.php?mode=daily&content=illust&p=1&format=json";

        public string cookie { get; set; }
        public string token { get; set; }
        private string nexturl = "begin";
        private PixivBaseAPI baseAPI;
        public Pixiv()
        {
            baseAPI = new PixivBaseAPI();
        }


        /// <summary>
        /// 获取TOP 50推荐列表
        /// </summary>
        /// <returns></returns>
        public async Task<ConcurrentQueue<string>> getRallist()
        {
            string rall;
            ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
            HttpUtil top50 = new HttpUtil(RALL_URL, HttpUtil.Contype.JSON);

            rall = await top50.GetDataAsync();

            if (!rall.Equals("ERROR"))
            {
                dynamic o = JObject.Parse(rall);
                JArray arr = o.contents;

                foreach (JToken j in arr)
                {
                    queue.Enqueue(j["illust_id"].ToString());
                }
            }
            else
            {
                MessageDialog dialog = new MessageDialog("update rall list Error");
                await dialog.ShowAsync();
            }
            return queue;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        public async Task<bool> getToken(string cookie)
        {
            bool f = false;
            HttpUtil tokurl = new HttpUtil(INDEX_URL, HttpUtil.Contype.HTML);
            this.cookie = cookie;
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
        /// 获取"猜你喜欢"推荐列表(Web模拟)
        /// </summary>
        /// <returns></returns>

        public async Task<ConcurrentQueue<string>> getRecommlistV1()
        {
            string like;
            ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
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
                    queue.Enqueue(j.ToString());
                }
            }
    
            return queue;
        }

        /// <summary>
        /// 获取"猜你喜欢"(PixivCS Api)
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<ConcurrentQueue<ImageInfo>> getRecommenlistV2(string account = null, string password = null)
        {
            ConcurrentQueue<ImageInfo> queue = new ConcurrentQueue<ImageInfo>();
            JsonObject recommends = new JsonObject();
            if (baseAPI.AccessToken == null)
            {
                try
                {
                    JsonObject baseRes = null;
                    baseRes = await baseAPI.Auth(account, password);
                    string db = baseRes.ToString();
                    Debug.Write(db);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return null;
                }
            }
            //是否使用nexturl更新list
            if ("begin".Equals(nexturl))
            {
                recommends = await new PixivAppAPI(baseAPI).IllustRecommended();
            }
            else
            {
                Uri next = new Uri(nexturl);
                string getparam(string param) => HttpUtility.ParseQueryString(next.Query).Get(param);
                recommends = await new PixivAppAPI(baseAPI).IllustRecommended
                    (ContentType: getparam("content_type"),
                     IncludeRankingLabel: bool.Parse(getparam("include_ranking_label")),
                     Filter: getparam("filter"),
                     MinBookmarkIDForRecentIllust: getparam("min_bookmark_id_for_recent_illust"),
                     MaxBookmarkIDForRecommended: getparam("max_bookmark_id_for_recommend"),
                     Offset: getparam("offset"),
                     IncludeRankingIllusts: bool.Parse(getparam("include_ranking_illusts")),
                     IncludePrivacyPolicy: getparam("include_privacy_policy"));
            }
            try
            {
                string str = recommends["illusts"].ToString();
                nexturl = recommends["next_url"].GetString();
                JArray arr = JArray.Parse(recommends["illusts"].ToString());
                foreach (JToken ill in arr)
                {
                    if (ill["meta_single_page"]["original_image_url"] != null)
                    {
                        ImageInfo imginfo = new ImageInfo();
                        imginfo.imgUrl = ill["meta_single_page"]["original_image_url"].ToString();
                        imginfo.viewCount = (int)ill["total_view"];
                        imginfo.isR18 = false;
                        imginfo.userId = ill["user"]["id"].ToString();
                        imginfo.userName = ill["user"]["account"].ToString();
                        imginfo.imgId = ill["id"].ToString();
                        imginfo.imgName = ill["title"].ToString();
                        imginfo.height = (int)ill["height"];
                        imginfo.width = (int)ill["width"];

                        queue.Enqueue(imginfo);                        
                    }  
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return queue;
        }

        /// <summary>
        /// 图片信息查询子方法1
        /// </summary>
        /// <param name="imgid">要查找的作品ID</param>
        /// <returns></returns>
        private async Task<string> getImageInfoSub(string imgid)
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
            string info1 = await getImageInfoSub(id);
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
                imginfo.userId = ill[0]["user"]["id"].ToString();
                imginfo.userName = ill[0]["user"]["name"].ToString();
                imginfo.imgId = ill[0]["id"].ToString() ;
                imginfo.imgName = ill[0]["title"].ToString();
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
        public async Task downloadImg(ImageInfo img)
        {
            Regex reg = new Regex("/c/[0-9]+x[0-9]+/img-master");
            img.imgUrl = reg.Replace(img.imgUrl, "/img-master", 1);

            HttpUtil download = new HttpUtil(img.imgUrl, HttpUtil.Contype.IMG);
            download.referer = "https://www.pixiv.net/artworks/" + img.imgId;
            download.cookie = cookie;
            await download.ImageDownloadAsync(img.imgId);
        }

        public async Task downloadImgV2(ImageInfo img)
        {
            try
            {
                using (Stream resStream = await (await new PixivAppAPI(baseAPI).RequestCall("GET",
                      img.imgUrl, new Dictionary<string, string>() { { "Referer", "https://app-api.pixiv.net/" } })).
                      Content.ReadAsStreamAsync())
                {
                    StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(img.imgId, CreationCollisionOption.OpenIfExists);
                    using (Stream writer = await file.OpenStreamForWriteAsync())
                    {
                        await resStream.CopyToAsync(writer);
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
