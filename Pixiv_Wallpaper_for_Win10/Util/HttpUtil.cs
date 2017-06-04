using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Pixiv_Wallpaper_for_Win10.Util
{
    public class HttpUtil
    {
        public enum Contype { JSON, HTML };
        private string[] contype = {
            "application/json, text/javascript, */*; q=0.01",
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"
        };


        private string url;
        private Contype dataType;

        /// <summary>
        /// 设置或获取Cookie
        /// </summary>
        public string cookie { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">要请求的 URL</param>
        /// <param name="dataType">要请求的 MIME 类型</param>
        public HttpUtil(string url, Contype dataType)
        {
            this.url = url;
            this.dataType = dataType;
        }


        /// <summary>
        /// HTTP GET请求
        /// </summary>
        /// <returns>返回的数据</returns>
        public async Task<string> GetDataAsync()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = contype[(int)dataType];
            request.Headers["Cookie"] = cookie;
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            Task<string> res = new Task<string>(() => { return "ERROR"; });
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream s = response.GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.GetEncoding("utf-8"));
                res = sr.ReadToEndAsync();
                cookie = response.Headers["Set-Cookie"];
            }
            return await res;
        }
        /// <summary>
        /// HTTP POST 请求
        /// </summary>
        /// <param name="data">请求数据</param>
        /// <returns>返回的数据</returns>
        public async Task<string> PostDataAsync(string data)
        {
            byte[] databit = Encoding.UTF8.GetBytes(data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = contype[(int)dataType];

            Stream write = await request.GetRequestStreamAsync();
            write.Write(databit, 0, databit.Length);

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            Task<string> res = new Task<string>(() => { return "ERROR"; });
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream s = response.GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.GetEncoding("utf-8"));
                res = sr.ReadToEndAsync();
                cookie = response.Headers["Set-Cookie"];
            }
            return await res;
        }
    }
}
