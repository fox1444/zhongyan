$(function () {
    setTimeout(function () {
        ReloadRecord();
        SetRecordHeight();
        ScrollRecordHeight();

    }, 100)

    $(window).resize(function () {
        SetRecordHeight();
    })
})

function Send() {
    if ($.trim($('#txtInput').val()).length > 0) {
        $.post("send.ashx", { toid: toid, comments: escape($('#txtInput').val()) },
    function (data) {
        $('#AllTalk').append(data);
        ScrollRecordHeight()
        editor.html('');
        $('#txtInput').val('');
    });
    }
}

function ReloadRecord() {
    $.post("load.ashx", { toid: toid, startid: startid }, function (data) {
        if (data.length != 0) {
            try {
                $('#AllTalk').html(data.split("%---------%")[0]);
                $('#UserInfo').html(data.split("%---------%")[1]);
                var nScrollHight = $('#AllTalk')[0].scrollHeight; //滚动距离总长(注意不是滚动条的长度)
                var nScrollTop = $('#AllTalk')[0].scrollTop;   //滚动到的当前位置
                var nDivHight = $('#AllTalk').height();
                //alert(nScrollTop + '_' + nDivHight + '_' + nScrollHight);
                if (nScrollTop + nDivHight >= (nScrollHight - 150)) {
                    ScrollRecordHeight();
                }
            }
            catch (e) {
            }
        }
        setTimeout(function () {
            ReloadRecord();
        }, 3000)

    });
}

function SetRecordHeight() {
    $("#AllTalk").height($(window).height() - $('#editerBox').height());
    $("#ALLHistory").height($(window).height() - $('#UserInfo').height() - 2);
}

function ScrollRecordHeight() {
    $('#AllTalk').scrollTop($('#AllTalk')[0].scrollHeight);
}

function PopChartBox() {
    window.open('../message/box.aspx?g=', 'newwindow', 'height=600px, width=800px, scrollbars=yes, resizable=yes')
}

function ShowHistory(guid) {
    if ($("#ALLHistory").css('display') == 'none') {
        $("#ALLHistory").show();
        $("#talkHistory").attr('src', 'history.aspx?g=' + guid);
        setTimeout(function () {
            var x = document.getElementById("talkHistory").contentWindow.document;
            x.documentElement.scrollTop = x.body.offsetHeight;
        }, 300);
    }
    else {
        $("#ALLHistory").hide();
    }
}
function SearchHistory(guid, date, pagenum) {
    window.location.href = 'history.aspx?g=' + guid + '&d=' + date + '&pagenum=' + pagenum;
}

function SearchHistoryPage(guid, date, pagenum) {
    window.location.href = 'history.aspx?g=' + guid + '&d=' + date + '&pagenum=' + pagenum;
}