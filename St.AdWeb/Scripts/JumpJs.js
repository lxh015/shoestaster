/// <reference path="F:\My\ShoesTaster\St.AdWeb\layer/layer.js" />

var Jump = new Object();
Jump.nowPage = 0;
Jump.Url = "";
Jump.hasData = false;
Jump.GetType = "post";
Jump.SetHtml = new Object();
Jump.ShowPageId = "pageNum";
Jump.DeleteUrl = "";
Jump.ReplaceArray = [];
Jump.ShowHtml = function (data) {
    try {
        $("#tbody").html("");
        if (data.Result) {

            var addHtml = "";
            for (var i = 0; i < data.Data.length; i++) {
                var item = data.Data[i];
                var itemHtml = htmlCopy;
                for (var j = 0; j < this.ReplaceArray.length; j++) {
                    var ritem = this.ReplaceArray[j].toLowerCase();
                    for (var key in item) {//获取json对象属性值，同C#的反射。
                        var keytolower = key.toLowerCase();
                        if (keytolower == ritem) {
                            var replaceStr = "[" + ritem + "]";
                            if (keytolower == "stata") {
                                item[key] = changeAudit(item[key]);
                            }

                            if (typeof (item[key]) == "boolean") {
                                item[key] = item[key] ? "是" : "否";
                            }

                            if (keytolower == "level") {
                                item[key]= changeLevel(item[key]);
                            }

                            itemHtml = itemHtml.replace(replaceStr, item[key]);
                        }
                    }
                }

                addHtml += itemHtml;
            }
            if (data.Data.length < 15)
                Jump.hasData = false;

            layer.closeAll();
            $("#tbody").html(addHtml);
        }
        else {
            ShowJSError("暂无数据！");
        }
    } catch (e) {
        ShowJSError();
    }
}
Jump.Previous = function () {
    Jump.nowPage = Jump.nowPage <= 1 ? 1 : Jump.nowPage - 1;
    this.GoLoad();
};
Jump.Next = function () {
    Jump.nowPage = Jump.nowPage == 0 ? 2 : Jump.nowPage + 1;
    this.GoLoad();
};
Jump.Jump = function (page) {
    Jump.nowPage = page;
    this.GoLoad();
};
Jump.GoLoad = function () {
    if (this.Url == "" || this.Url == undefined) {
        ShowMessage("跳转URL错误！");
        return;
    } else {
        $.ajax({
            url: this.Url,
            type: this.GetType,
            data: { page: this.nowPage == 0 ? 0 : (this.nowPage - 1) }
        }).done(function (data) {
            Jump.ChangePage();
            Jump.ShowHtml(data);
        })
    }
};
Jump.ChangePage = function () {
    $("#" + this.ShowPageId).text(this.nowPage);
};
Jump.GoDelete = function (id) {
    if (Jump.DeleteUrl == "" || Jump.DeleteUrl == undefined) {
        ShowMessage("删除数据URL错误！");
        return;
    } else {
        $.ajax({
            url: this.DeleteUrl,
            type: this.GetType,
            data: { id: id }
        }).done(function (data) {
            ShowMessage(data.returnMessage);
            window.setTimeout(Jump.GoLoad(), 1500);
        })
    }
}


function ShowMessage(message) {
    //配置一个透明的询问框
    layer.msg(message, {
        time: 5000, //20s后自动关闭
        btn: ['关闭'],
        //btn1: function () {
        //dosome
        //}
    });
}

function ShowJSError(message) {
    layer.closeAll();
    layer.msg(message == "" ? "信息加载错误！" : message, { time: 3000 });
}

function changeAudit(audit) {
    switch (audit) {
        case "0":
            return "待审核";
        case "1":
            return "审核成功";
        case "2":
            return "审核失败";
    }
    switch (audit) {
        case 0:
            return "待审核";
        case 1:
            return "审核成功";
        case 2:
            return "审核失败";
    }
}

function changeLevel(level) {
    switch (level) {
        case "0":
            return "超级管理员";
        case "1":
            return "管理员";
        case "2":
            return "区域管理";
        case "4":
            return "普通";
        case "16":
            return "游客";
    }
    switch (level) {
        case 0:
            return "超级管理员";
        case 1:
            return "管理员";
        case 2:
            return "区域管理";
        case 4:
            return "普通";
        case 16:
            return "游客";
    }
}
