/// <reference path="F:\My\ShoesTaster\St.AdWeb\layer/layer.js" />
$ = window.$ || {},
$.keyD = 0;
$.clD = 0;
$.TEA = function () {
    kd = function (event) {
        if (event.keyCode != 13) {
            $.keyD++;
        } else {
            goLogin();
        }
    },
    cd = function () {
        $.clD++;
    };
    return {
        keyDown: kd,
        onClick: cd,
    }
}();

document.onkeydown = $.TEA.keyDown;
document.onclick = $.TEA.onClick;

function getInfo() {
    var u = $("#username").val();
    var p = $("#password").val();
    var c = $("#code").val();
    var cT = $("#vcode").attr("data-data");
    return [u, p, c, cT, $.keyD, $.clD];
}


function goLogin() {
    var info = getInfo();
    $.ajax({
        url: "/Login/Into",
        type: "post",
        data: { username: info[0], password: info[1], vcode: info[2], vctime: info[3], kd: info[4], cd: info[5] }
    }).done(function (data) {
        if (data.islogin) {
            window.location.href = "/";
        } else {
            layer.msg(data.message, { time: 5000, btn: ["关闭"] });
            return false;
        }
    })
}