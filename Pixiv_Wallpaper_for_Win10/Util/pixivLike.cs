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
        Conf c;
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
