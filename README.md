# Pixiv_Wallpaper_for_Win10
抓取Pixiv.net的图片并设置为Win10壁纸的UWP APP，需要在Windows10 1903以上的版本环境中运行。
有top50与"为你推荐"两种模式，"为你推荐"需要登录账号。
目前需要登录的操作(为你推荐模块)在中国大陆区域需要走代理才能连接，Top50功能不受影响可以直接连接。
由于Pixiv.net更新了Google reCAPTCHA v3,以往的通过web端模拟的登录方式失效，现改为通过WebView登录获取Cookie来进行后续操作。除此之外还包含了基于PixivCS(https://github.com/tobiichiamane/pixivcs/blob/master/PixivAppAPI.cs)的部分功能，部分代码参考https://github.com/tobiichiamane/pixivfs-uwp
预览图:
![Image text](https://github.com/democyann/Pixiv_Wallpaper_for_Win10/blob/relife/Pixiv_Wallpaper_for_Win10/preview_img/N8QG~I%7BTZK%5B115CMF60BCX0.png)
![Image text](https://github.com/democyann/Pixiv_Wallpaper_for_Win10/blob/relife/Pixiv_Wallpaper_for_Win10/preview_img/RX5E%40ZD%5B7_%60(%25~PYAAY%25%60P5.png)
