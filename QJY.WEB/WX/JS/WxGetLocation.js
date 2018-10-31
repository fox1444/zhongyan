function WXLocation(data) {
    this.initWX = function () {
        wx.config(data);
        wx.ready(function () {
            wx.getLocation({
                type: 'wgs84',
                success: function (argv) {
                    if (argv.errMsg == "getLocation:fail") {
                        var gc = new BMap.Geocoder();
                        var pointAdd = new BMap.Point(res.longitude, res.latitude);
                        gc.getLocation(pointAdd, function (rs) {
                            var city = rs.addressComponents.city;  //城市
                        });
                    } else {
                        //todo
                        $('#html').html(JSON.stringify(argv));
                    }
                },
                cancel: function (res) {
                    alert('用户拒绝授权获取地理位置');
                }
            });
        });

        wx.error(function (res) {
            alert("调用微信jsapi返回的状态:" + res.errMsg);
        });
    }

    this.Refresh = function () {
        wx.getLocation({
            type: 'wgs84',
            success: function (argv) {
                if (argv.errMsg == "getLocation:fail") {
                    var gc = new BMap.Geocoder();
                    var pointAdd = new BMap.Point(res.longitude, res.latitude);
                    gc.getLocation(pointAdd, function (rs) {
                        var city = rs.addressComponents.city;  //城市
                    });
                } else {
                    //todo
                    $('#html').html(JSON.stringify(argv));
                }
            },
            cancel: function (res) {
                alert('用户拒绝授权获取地理位置');
            }
        });
    }
}