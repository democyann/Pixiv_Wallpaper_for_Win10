using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Concurrent;

namespace Pixiv_Wallpaper_for_Win10.Util
{
    class PixivTop50
    {
        public ConcurrentQueue<string> results = new ConcurrentQueue<string>();
        private Pixiv p = new Pixiv();


        /// <summary>
        /// 作品类别更新
        /// </summary>
        /// <param name="flag">是否无视列表情况强制更新 默认为否</param>
        /// <returns></returns>
        public async Task<ConcurrentQueue<string>> listUpdate(bool flag = false)
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
                string id = "";
                while (true)
                {
                    if (results.TryDequeue(out id))
                    {
                        img = await p.getImageInfo(id);                      
                    }
                    break;
                }

                if(!await p.downloadImg(img))
                {
                    img = null;
                }

            }
            return img;
        }

    }
}
