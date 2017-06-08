using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Diagnostics;

namespace Pixiv_Wallpaper_for_Win10.Util
{
    public class HttpUtil
    {
        public enum Contype
        {
            /// <summary>
            /// JSON 数据
            /// </summary>
            JSON,
            /// <summary>
            /// HTML 页面
            /// </summary>
            HTML,
            /// <summary>
            /// 图片类型
            /// </summary>
            IMG
        };
        private string[] contype = {
            "application/json, text/javascript, */*; q=0.01",
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
            "image/webp,image/apng,image/*,*/*;q=0.8"
        };

        private static readonly String USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";


        private string url;
        private Contype dataType;

        /// <summary>
        /// 设置或获取Cookie
        /// </summary>
        public string cookie { get; set; }

        /// <summary>
        /// 获取或设置原引用地址
        /// </summary>
        public string referer { get; set; }

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
            request.Accept = contype[(int)dataType];
            request.Headers["Cookie"] = cookie;
            request.Headers["User-Agent"] = USER_AGENT;
            if (referer != null)
            {
                request.Headers["Referer"] = referer;
            }
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            string res = "Error";
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream s = response.GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.GetEncoding("utf-8"));
                res = await sr.ReadToEndAsync();
                cookie = response.Headers["Set-Cookie"];

                sr.Dispose();
                s.Dispose();
            }
            response.Dispose();
            return res;
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
            request.Accept = contype[(int)dataType];
            request.Headers["Cookie"] = cookie;
            request.Headers["User-Agent"] = USER_AGENT;
            if (referer != null)
            {
                request.Headers["Referer"] = referer;
            }

            Stream write = await request.GetRequestStreamAsync();
            write.Write(databit, 0, databit.Length);
            write.Dispose();

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            string res = "ERROR";
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream s = response.GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.GetEncoding("utf-8"));
                res = await sr.ReadToEndAsync();
                cookie = response.Headers["Set-Cookie"];

                sr.Dispose();
                s.Dispose();

            }
            response.Dispose();

            return res;
        }

        public async Task<string> ImageDownloadAsync(string userid, string imgid)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = contype[(int)dataType];
            request.Headers["Cookie"] = cookie;
            request.Headers["Referer"] = referer;

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream s = response.GetResponseStream();
                StorageFile file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(userid + imgid, CreationCollisionOption.ReplaceExisting);

                Stream write = await file.OpenStreamForWriteAsync();
                int l;
                long a=0;
                do
                {
                    byte[] temp = new byte[1024];
                    l = s.Read(temp, 0, 1024);
                    if (l > 0)
                    {
                       await write.WriteAsync(temp, 0, l);
                    }
                    a += l;

                } while (l > 0);

                write.Dispose();
                s.Dispose();
            }
            else
            {
                return "ERROR";
            }

            response.Dispose();

            return userid + imgid;

        }

    }
}
