//var wimg = "分享图片网址123";
//var wurl = "分享网址123";
//var wdesc = '分享内容123';
//var wtit = '分享标题123';
//var wappid = '';

//wx.config({
//    debug: true, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
//    appId: 'wx1b5c7dbfe9a3555d', // 必填，公众号的唯一标识
//    timestamp: Date.parse(new Date()), // 必填，生成签名的时间戳
//    nonceStr: Math.random(), // 必填，生成签名的随机串
//    signature: ComFunJS.getCookie("jsapi_ticket"),// 必填，签名
//    jsApiList: ['onMenuShareTimeline'] // 必填，需要使用的JS接口列表
//});
 
//function shareMsg() {//<span style="font-family: Arial, Helvetica, sans-serif;">发送给好友</span><span style="font-family: Arial, Helvetica, sans-serif;">标题和内容默认都显示</span>
//    WeixinJSBridge.invoke('sendAppMessage',{
//        "appid": wappid,
//        "img_url": wimg,
//        "img_width": "200",
//        "img_height": "200",
//        "link": wurl,
//        "desc": wdesc,
//        "title": wtit,
//    })
//}
//function shareQuan() {  //<span style="font-family: Arial, Helvetica, sans-serif;">分享到朋友圈只有标题显示</span>
//    WeixinJSBridge.invoke('shareTimeline',{
//        "img_url": wimg,
//        "img_width": "200",
//        "img_height": "200",
//        "link": wurl,
//        "desc": wdesc,
//        "title": wtit
//    });
//}
//function shareWeibo() { 
//    WeixinJSBridge.invoke('shareWeibo',{
//        "content": wdesc,
//        "url": wurl,
//    });
//}
//// 当微信内置浏览器完成内部初始化后会触发WeixinJSBridgeReady事件。
//document.addEventListener('WeixinJSBridgeReady', function onBridgeReady() {
//    // 发送给好友
//    WeixinJSBridge.on('menu:share:appmessage', function(argv){
//        shareMsg();
//    });
//    // 分享到朋友圈
//    WeixinJSBridge.on('menu:share:timeline', function(argv){
//        shareQuan();
//    });
//    // 分享到微博
//    WeixinJSBridge.on('menu:share:weibo', function(argv){
//        shareWeibo();
//    });
//}, false);


//var WxControlJS = new Object({
//    reloadauthrise: function () {
      
//    }
//})
