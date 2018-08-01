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
        private Pixiv pixiv=new Pixiv();
        private Conf c=new Conf();
        private ImageInfo img=new ImageInfo();
        public PixivLike()
        {
            pixiv = new Pixiv();
            c = new Conf();
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
            if(c.account.Equals("")|| c.password.Equals(""))
            {
                return null;
            }

            if (pixiv.token == null || pixiv.token.Equals(""))
            {
                if (c.token.Equals(""))
                {
                    if (!await pixiv.getToken(true))
                    {
                        return null;
                    }
                    else
                    {
                        c.cookie = pixiv.cookie;
                        c.token = pixiv.token;
                    }
                }
                else
                {
                    pixiv.cookie = c.cookie;
                    pixiv.token = c.token;
                }
            }
            

            

            await ListUpdate();
            ImageInfo img = null;
            if(like!=null&&like.Count!=0)
            {
                Random r = new Random();
                while (true)
                {
                    int number=r.Next(0, like.Count);
                    string id = like[number].ToString();
                    img = await pixiv.getImageInfo(id);
                    if (like[number]!=null&& img.WHratio>=1.33&&!img.isR18)
                    {
                        await pixiv.downloadImg(img);
                        break;
                    }
                    else
                    {
                        like.RemoveAt(number);
                    }
                   
                }
            }
            return img;
        }
    }
}
