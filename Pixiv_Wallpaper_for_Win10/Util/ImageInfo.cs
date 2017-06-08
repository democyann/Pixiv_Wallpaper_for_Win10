using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixiv_Wallpaper_for_Win10.Util
{
    /// <summary>
    /// 图片信息实体类
    /// </summary>
    public class ImageInfo
    {
        /// <summary>
        /// 作品名称
        /// </summary>
        public string imgName { set; get; }
        /// <summary>
        /// 作品ID
        /// </summary>
        public string imgId { get; set; }
        /// <summary>
        /// 作品URL
        /// </summary>
        public string imgUrl { get; set; }
        /// <summary>
        /// 作者ID
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 作者名称
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 是否为R18作品
        /// </summary>
        public bool isR18 { get; set; }
        /// <summary>
        /// 作品tags
        /// </summary>
        public string tag { get; set; }
        /// <summary>
        /// 浏览数
        /// </summary>
        public int viewCount { get; set; }

    }
}
