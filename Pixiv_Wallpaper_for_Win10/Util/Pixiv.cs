using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;


namespace Pixiv_Wallpaper_for_Win10.Util
{
    class Pixiv
    {
        private ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private static String token;
        private static String account;
        private static String password;
        public string  rall;
        public string ulike;
        public string IllInfo;
        private PixivTop50 p;

        private readonly String INDEX_URL = "https://www.pixiv.net";
        private readonly String POST_KEY_URL = "https://accounts.pixiv.net/login?lang=zh&source=pc&view_type=page&ref=wwwtop_accounts_index";
        private readonly String LOGIN_URL = "https://accounts.pixiv.net/api/login?lang=zh";
        private readonly String RECOMM_URL = "https://www.pixiv.net/rpc/recommender.php?type=illust&sample_illusts=auto&num_recommendations=500&tt=";
        private readonly String ILLUST_URL="https://www.pixiv.net/rpc/illust_list.php?verbosity=&exclude_muted_illusts=1&illust_ids=";
        private readonly String DETA_URL="https://app-api.pixiv.net/v1/illust/detail?illust_id=";
        private readonly String RALL_URL="https://www.pixiv.net/ranking.php?mode=daily&content=illust&p=1&format=json";
        private static readonly String USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";

        public String getaccount()
        {
            account = localSettings.Values["Account"].ToString();
            return account;
        }
        public String getpassword()
        {
            password = localSettings.Values["Password"].ToString();
            return password;
        }
        public Pixiv()
        {

        }
        public async void getRallist()                                      //获取top50
        {
            HttpUtil top50 = new HttpUtil(RALL_URL, HttpUtil.Contype.JSON);
            rall= await top50.GetDataAsync();
        }
        public async void getrecomm()                                      //获取推荐（登陆后操作）
        {
            HttpUtil recomm = new HttpUtil(RECOMM_URL, HttpUtil.Contype.JSON);
            ulike = await recomm.GetDataAsync(); 
        }
        public async void getIllInfo(String id)
        {
            HttpUtil info = new HttpUtil(DETA_URL, HttpUtil.Contype.JSON);
            IllInfo = await info.GetDataAsync();
        }
        public void ModeSelect()             //模式选取(未完成）
        {
            String mode = localSettings.Values["Mode"].ToString();      
            if (mode == "Top_50")
            {
                p.SelectArtWork();
            }
                
        }
    }
}
