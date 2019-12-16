using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.UI.Popups;

namespace Pixiv_Wallpaper_for_Win10.Util
{
    class PixivLike
    {
        private ArrayList like = new ArrayList();
        private Pixiv pixiv = new Pixiv();
        private Conf c = new Conf();
        private ImageInfo img = new ImageInfo();
        public PixivLike()
        {
            pixiv = new Pixiv();
            c = new Conf();
        }
        public async Task<ArrayList> ListUpdate(bool flag = false)
        {
            if(like==null||like.Count==0||flag)
            {
                like = await pixiv.getRecommlistV1(); 
            }
            return like;
        }
        public async Task<ImageInfo> SelectArtWork()
        {
            if (pixiv.token == null || pixiv.token.Equals(""))
            {
                if (c.token.Equals(""))
                {
                    if (!await pixiv.getToken(c.cookie))            //getToken不成功，返回null
                    {
                        return null;
                    }
                    else                                       //getToken成功，将token写入配置文件
                    {
                        c.cookie = pixiv.cookie;
                        c.token = pixiv.token;
                    }
                }
                else                                          //配置文件中已有token，直接调用(可能出现token过期情况)
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
                    int number = r.Next(0, like.Count - 1);
                    string id = like[number].ToString();
                    img = await pixiv.getImageInfo(id);
                    if (like[number]!=null&& img.WHratio>=1.33&&!img.isR18)
                    {
                        await pixiv.downloadImg(img);
                        like.RemoveAt(number);
                        break;
                    }
                    like.RemoveAt(number);
                }
            }
            else
            {
                //使UI线程调用lambda表达式内的方法
                await MainPage.mp.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    //UI code here
                    MessageDialog dialog = new MessageDialog("更新推荐列表失败");
                    await dialog.ShowAsync();
                });
            }
            return img;
        }
    }
}
