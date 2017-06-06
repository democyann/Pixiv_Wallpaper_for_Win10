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
        public static string selected;
        private ArrayList listUpdate()           
        {
            Pixiv pixiv = new Pixiv();
            if (results == null || results.Count > 0)
            {
                pixiv.getRallist();
                JObject top50 = new JObject();
                top50 = JObject.Parse(pixiv.rall);
                IList<JToken> illist_id = top50["illist_id"].ToList();
                foreach (JToken result in illist_id)
                {
                    PixivTop50 pixivTop50 = JsonConvert.DeserializeObject<PixivTop50>(result.ToString());
                    results.Add(pixivTop50);
                }
                return results;
            }
            else
                return results;
        }
        public string SelectArtWork()                     //从数组中选取一个作品推送
        {
            if (results != null || results.Count > 0)
            {
                bool p = true;
                while (p)
                {
                    Random r = new Random();
                    int number = r.Next(0, 50);
                    if (results[number] != null)
                    {
                        p = false;
                        selected = results[number].ToString();
                        results[number] = null;
                        return selected;
                    }
                }
            }
            else
            {
                listUpdate();
                return SelectArtWork();
            }
        }
        
    }
}
