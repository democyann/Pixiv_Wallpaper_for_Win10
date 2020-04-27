# Pixiv Wallpaper for Windows 10
抓取pixiv.net的图片并设置为Windows 10桌面壁纸的UWP APP，需要在Windows 10 1903以上的版本系统中运行。

依赖部分NuGet包，没有多语种支持，目前只有简体中文一种语言。

有top50与"猜你喜欢"两种模式，"猜你喜欢"对应网页pixiv.net/discovery 内容，需要登录pixiv账号。

有两种登录模式供选择，一种采用WebView登录，支持外部账号关联登录(如weibo、Twitter账号登录)，该模式下均使用pixiv web接口进行http通信;
PixivCS登录模式采用[PixivCS](https://github.com/tobiichiamane/pixivcs/blob/master/PixivAppAPI.cs/ "PixivCS") API，内部大部分使用ios客户端进行通信。部分代码参考[pixivfs](https://github.com/tobiichiamane/pixivfs-uwp/ "pixivfs")项目

## UWP使用代理
您需要自己准备代理服务器与代理软件，通过全局代理或者PAC代理+[Loopback](https://sspai.com/post/41137 "UWP loopback")的方式解决访问pixiv.net的问题。
