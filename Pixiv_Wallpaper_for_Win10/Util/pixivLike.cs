using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pixiv_Wallpaper_for_Win10.Util
{
    class PixivLike
    {
        private ArrayList like = new ArrayList();
        Pixiv pixiv;
        public PixivLike()
        {
            pixiv = new Pixiv();
        }
        public async Task<ArrayList>  ListUpdate(bool flag=false)
        {
            if(like==null||like.Count==0||flag)
            {
                like =await pixiv.getRecommlist();
            }
            return like;
        }
        public async Task<ImageInfo> SelectArtWork()
        {
            await ListUpdate();
            ImageInfo img = null;
            if(like!=null&&like.Count!=0)
            {
                while(true)
                {
                    Random r = new Random();
                    int number=r.Next(0, 50);
                    if(like[number]!=null)
                    {
                        string id=like[number].ToString();
                        img =await pixiv.getImageInfo(id);
                    }
                    break;
                }
                await pixiv.downloadImg(img);
            }
            return img;
        }
    }
}
