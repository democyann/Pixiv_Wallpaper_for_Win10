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

        public async Task<ArrayList> listUpdate(bool flag = false)
        {
            if (results == null || results.Count <=0 || flag)
            {
                results = await p.getRallist();
            }
            return results;
        }

        public async Task<ImageInfo> SelectArtWork()                     //从数组中选取一个作品推送
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

                string path = await p.downloadImg(img);

            }
            return img;
        }

    }
}
