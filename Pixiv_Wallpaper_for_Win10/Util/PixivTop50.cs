using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace Pixiv_Wallpaper_for_Win10.Util
{
    class PixivTop50
    {
        public static ArrayList results = new ArrayList();
        private Pixiv p;

        public PixivTop50()
        {
            p = new Pixiv();
        }

        /// <summary>
        /// 作品类别更新
        /// </summary>
        /// <param name="flag">是否无视列表情况强制更新 默认为否</param>
        /// <returns></returns>
        public async Task<ArrayList> listUpdate(bool flag = false)
        {
            if (results == null || results.Count <=0 || flag)
            {
                results = await p.getRallist();
            }
            return results;
        }

        /// <summary>
        /// 作品推送
        /// </summary>
        /// <returns></returns>
        public async Task<ImageInfo> SelectArtWork()
        {
            await listUpdate();

            ImageInfo img = null;
            if (results != null && results.Count > 0)
            {
                while (true)
                {
                    Random r = new Random();
                    int number = r.Next(0, 50);
                    if (results[number] != null)
                    {
                        string imgid = results[number].ToString();
                        img = await p.getImageInfo(imgid);                      
                    }
                    break;
                }

                await p.downloadImg(img);

            }
            return img;
        }

    }
}
