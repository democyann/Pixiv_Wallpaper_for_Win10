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
        private readonly String ILLUST_URL="https://www.pixiv.net/rpc/illust_list.php?verbosity=&exclude_muted_illusts=1&illust_ids=";
        private readonly String DETA_URL="https://app-api.pixiv.net/v1/illust/detail?illust_id=";
        private readonly String RALL_URL="https://www.pixiv.net/ranking.php?mode=daily&content=illust&p=1&format=json";
        private Conf c;
        public Pixiv()
        {
            c = new Conf();
        }

        public async Task<ArrayList> getRallist()                                      //获取top50
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

                if (arr.Count > 0)
                {
                    foreach (JToken j in arr) {
                        list.Add(j["illust_id"].ToString());
                        Debug.WriteLine(j["illust_id"].ToString());                       
                    }
                }
            }
            else
            {
                Debug.WriteLine("ERROR");
            }
            return list;   
        }

        private async Task<string> getImageInfoSub1(string imgid)
        {
            HttpUtil info1 = new HttpUtil(DETA_URL+imgid, HttpUtil.Contype.JSON);
            string data = await info1.GetDataAsync();
            Debug.WriteLine(data);

            return data;
        }

        private async Task<string> getImageInfoSub2(string imgid)
        {
            HttpUtil info2 = new HttpUtil(ILLUST_URL + imgid + "&tt=" + c.token, HttpUtil.Contype.JSON);
            info2.cookie = c.cookie;
            string data = await info2.GetDataAsync();
            Debug.WriteLine(data);

            return data;
        }


        public async Task<ImageInfo> getImageInfo(string id)
        {
            ImageInfo imginfo = null;
            string info1 = await getImageInfoSub1(id);
            if (!info1.Equals("ERROR"))
            {
                imginfo=new ImageInfo();

                dynamic o = JObject.Parse(info1);
                dynamic ill = o.illust;
                dynamic imgurl;
                imginfo.viewCount =(int)ill.total_view;

                if ((int)ill.page_count > 1)
                {
                    imgurl = ill.meta_pages[0].image_urls;
                    imginfo.imgUrl=imgurl.original;
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

                if (imginfo.isR18)
                {
                    string info2 = await getImageInfoSub2(id);
                    if (!info2.Equals("ERROR"))
                    {
                        dynamic d = JObject.Parse(info2);
                        imginfo.userId= d.illust_user_id;
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


        public async Task<string> downloadImg(ImageInfo img)
        {
            Regex reg = new Regex("/c/[0-9]+x[0-9]+/img-master");
            img.imgUrl=reg.Replace(img.imgUrl, "/img-master", 1);

            HttpUtil download = new HttpUtil(img.imgUrl,HttpUtil.Contype.IMG);
            download.referer = "https://www.pixiv.net/member_illust.php?mode=medium&illust_id=" + img.imgId;
            download.cookie = c.cookie;
            string path = await download.ImageDownloadAsync(img.userId, img.imgId);
            return path;
        }



        //public async void getrecomm()                                      //获取推荐（登陆后操作）
        //{
        //    HttpUtil recomm = new HttpUtil(RECOMM_URL, HttpUtil.Contype.JSON);
        //    ulike = await recomm.GetDataAsync(); 
        //}
        //public async void getIllInfo(String id)
        //{
        //    HttpUtil info = new HttpUtil(DETA_URL, HttpUtil.Contype.JSON);
        //    IllInfo = await info.GetDataAsync();
        //}


        // 这个不需要
        //public void ModeSelect()             //模式选取(未完成）
        //{
        //    String mode = localSettings.Values["Mode"].ToString();      
        //    if (mode == "Top_50")
        //    {
        //        p.SelectArtWork();
        //    }
                
        //}
    }
}
