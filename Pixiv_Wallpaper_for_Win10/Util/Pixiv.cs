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

namespace Pixiv_Wallpaper_for_Win10.Util
{
    class Pixiv
    {
        private readonly String INDEX_URL = "https://www.pixiv.net";
        private readonly String POST_KEY_URL = "https://accounts.pixiv.net/login?lang=zh&source=pc&view_type=page&ref=wwwtop_accounts_index";
        private readonly String LOGIN_URL = "https://accounts.pixiv.net/api/login?lang=zh";
        private readonly String RECOMM_URL = "https://www.pixiv.net/rpc/recommender.php?type=illust&sample_illusts=auto&num_recommendations=500&tt=";
        private readonly String ILLUST_URL = "https://www.pixiv.net/rpc/illust_list.php?verbosity=&exclude_muted_illusts=1&illust_ids=";
        private readonly String DETA_URL = "https://app-api.pixiv.net/v1/illust/detail?illust_id=";
        private readonly String RALL_URL = "https://www.pixiv.net/ranking.php?mode=daily&content=illust&p=1&format=json";
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

            Debug.WriteLine(rall);

            if (!rall.Equals("ERROR"))
            {
                dynamic o = JObject.Parse(rall);
                JArray arr = o.contents;

                foreach (JToken j in arr)
                {
                    list.Add(j["illust_id"].ToString());
                    Debug.WriteLine(j["illust_id"].ToString());
                }
            }
            else
            {
                Debug.WriteLine("ERROR");
            }
            return list;
        }

        /// <summary>
        /// 获取POST KEY 私有方法
        /// </summary>
        /// <returns>POST KEY</returns>
        private async Task<string> postKey()
        {
            string key = "";
            //获取POST KEY

            HttpUtil posturl = new HttpUtil(POST_KEY_URL, HttpUtil.Contype.HTML);
            string poststr = await posturl.GetDataAsync();
            if (!posturl.Equals("ERROR"))
            {
                Regex r = new Regex("name=\"post_key\"\\svalue=\"([a-z0-9]{32})\"", RegexOptions.Singleline);
                if (r.IsMatch(poststr))
                {
                    key = r.Match(poststr).Groups[1].ToString();
                    cookie = posturl.cookie;
                }
            }
            Debug.WriteLine("POST KEY:" + key);
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
                dynamic o = JObject.Parse(data);
                if (o.body.success != null)
                {
                    cookie = loginurl.cookie;
                    f = true;
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
                if (!await login()) return f;
            }

            HttpUtil tokurl = new HttpUtil(INDEX_URL, HttpUtil.Contype.HTML);
            tokurl.cookie = cookie;
            string data = await tokurl.GetDataAsync();
            if (!data.Equals("ERROR"))
            {
                Regex r = new Regex("pixiv.context.token\\s=\\s\"([a-z0-9]{32})\"");
                if (r.IsMatch(data))
                {
                    token = r.Match(data).Groups[1].ToString();
                    Debug.WriteLine("TOKEN:" + token);
                    f = true;
                }
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
            HttpUtil recomm = new HttpUtil(RECOMM_URL+token, HttpUtil.Contype.JSON);
            recomm.cookie = cookie;
            recomm.referer = "http://www.pixiv.net/recommended.php";

            like = await recomm.GetDataAsync();

            Debug.WriteLine(like);

            if (like != "ERROR")
            {
                dynamic o = JObject.Parse(like);
                JArray arr = o.recommendations;
                foreach(JToken j in arr)
                {
                    list.Add(j.ToString());
                    Debug.WriteLine(j.ToString());
                }
            }
            else
                Debug.WriteLine("ERROR");
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
            Debug.WriteLine(data);

            return data;
        }
        /// <summary>
        /// 图片信息查询子方法2
        /// </summary>
        /// <param name="imgid">要查找的作品ID</param>
        /// <returns></returns>
        private async Task<string> getImageInfoSub2(string imgid)
        {
            HttpUtil info2 = new HttpUtil(ILLUST_URL + imgid + "&tt=" + token, HttpUtil.Contype.JSON);
            info2.cookie = c.cookie;
            info2.referer = "http://www.pixiv.net/recommended.php";
            string data = await info2.GetDataAsync();
            Debug.WriteLine(data);

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
                dynamic ill = o.illust;
                dynamic imgurl;
                imginfo.viewCount = (int)ill.total_view;

                if ((int)ill.page_count > 1)
                {
                    imgurl = ill.meta_pages[0].image_urls;
                    imginfo.imgUrl = imgurl.original;
                }
                else
                {
                    imgurl = ill.meta_single_page;
                    imginfo.imgUrl = imgurl.original_image_url;
                }

                imginfo.isR18 = imginfo.imgUrl.Contains("limit_r18");

                dynamic user = ill.user;

                imginfo.userId = user.id;
                imginfo.userName = user.name;
                imginfo.imgId = ill.id;
                imginfo.imgName = ill.title;
                imginfo.tag = ill.tags.ToString();
                imginfo.height = (int)ill.height;
                imginfo.width = (int)ill.width;

                //如为R18作品使用下面方法查询
                if (imginfo.isR18)
                {
                    string info2 = await getImageInfoSub2(id);
                    if (!info2.Equals("ERROR"))
                    {
                        dynamic d = JObject.Parse(info2);
                        imginfo.userId = d.illust_user_id;
                        imginfo.imgId = d.illust_id;
                        imginfo.imgUrl = d.url;
                        imginfo.userName = d.user_name;
                        imginfo.imgName = d.illust_title;
                        imginfo.tag = d.tags.ToString();

                    }
                    else
                    {
                        imginfo = null;
                    }
                }

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
