using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixiv_Wallpaper_for_Win10.Util
{
    public class ImageInfo
    {
        public string imgName { set; get; }
        public string imgId { get; set; }
        public string imgUrl { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public bool isR18 { get; set; }
        public string tag { get; set; }
        public int viewCount { get; set; }

    }
}
