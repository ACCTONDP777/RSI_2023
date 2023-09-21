/*此文件用于定义整个网站可能共用的JS*/
var AUO_GDS_LatestLangVer = "8"; //【语言版本号全局变量】若语言字典有更新，必须修改此值，只要有变化就会重新加载多语言
var AUO_GDS_LangDicKey = "AUO_GDS_LangDic";
var AUO_GDS_LangVerKey = "AUO_GDS_LangVer";
var AUO_GDS_LangIdKey = "AUO_GDS_LangId";

/*多语言处理 - 定义获取语言字典的方法
//获取多语言字典，此无参的方法不强制刷新多语言字典，适合每次页面加载前处理多语言时调用 */
function GetLangDic() {
    var dicJson;
    $.ajax({
        type: "POST",
        url: "../Repair/GetDictionary",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        async: false, //此处用同步的方式来调用
        cache: false, //此处不再使用缓存，否则可能导致多语言无法更新
        success: function (data) {
            dicJson = data;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus + ": " + errorThrown);
        }
    });
    return dicJson;
    //return GetLangDicPlus(false);
}
function getKeyData(val) {
    jQuery.support.cors = true;
    var arrData = new Array();
    // var langid = GetLangId();
    if (val != "") {

        $.ajax({
            url: "../Handler/Ajax.ashx?func=getKeyData&key=" + escape(val),
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {

                for (var i = 0; i < data.length; i++) {
                    if (val.toUpperCase() == data[i].keyvalue.toUpperCase().substring(0, val.toUpperCase().length)) {
                        arrData.push(data[i].keyvalue);
                    }
                }
            },
            error: function (xmlHttpRequest, textStatus, errorThrown) {
                alert("Ajax异常:" + XMLHttpRequest.readyState + textStatus);
            }

        });

    }
    return arrData;
}
function GetVisitFlag(indexoper, resultoper) {

    jQuery.support.cors = true;
    var result;
    $.ajax({
        url: "../Handler/Ajax.ashx?func=GetVisitFlag&indexoper=" + escape(indexoper) + "&resultoper=" + resultoper,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        crossDomain: true,
        async: false,
        cache: false,
        processData: false,
        success: function (data) {
            result = data;
        },
        error: function (xmlHttpRequest, textStatus, errorThrown) {

            alert("Ajax异常:" + XMLHttpRequest.readyState + textStatus);
        }

    });
    return result;
}

/*多语言处理 - 定义获取语言字典的方法
/*会使用localStorage缓存多语言字典，若localStorage不可用则每次重新获取，
参数forceReloadDic为true则不论是否已缓存都强制重刷新多语言字典，适合用于多语言切换时调用*/
function GetLangDicPlus(forceReloadDic) {
    var storage = window.localStorage;

    var latestLangId = GetLangId(); //获取需要的多语言种类
    var latestLangVer = AUO_GDS_LatestLangVer; //从全局变量中获取最新的语言字典版本号

    var langDic, langVer, langId;

    if (storage) {
        langVer = storage.getItem(AUO_GDS_LangVerKey); //获取缓存的语言版本号
        langDic = storage.getItem(AUO_GDS_LangDicKey); //获取缓存的多语言字典(字符串)
        langId = storage.getItem(AUO_GDS_LangIdKey); //获取缓存的多语言种类（简中zh-CHS，繁重zh-CHT，英文en）
    }
    if (forceReloadDic || langDic == null || langDic == "undefined" || langVer != latestLangVer || langId != latestLangId) {
        //重新至服务器端获取多语言字典并更新浏览器缓存后返回
        var langDicObj = GetLangDicJSON(latestLangId);
        return langDicObj;
    }
    return JSON.parse(langDic); //将取出的多语言字典字符串转换成JSON
}

//呼叫Handler获取多语言字典JSON对象，若需要透过URL参数传入多语言语种而非后台取CAP，请修改此方法
function GetLangDicJSON(langid) {
    var dicJson;
    $.ajax({
        type: "GET",
        url: "../../GradePromotion/Handler/Public.ashx?func=GetLangDic&langId=" + langid, 
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        async: false, //此处用同步的方式来调用
        cache: false, //此处不再使用缓存，否则可能导致多语言无法更新
        success: function (data) {
            dicJson = data;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus + ": " + errorThrown);
        }
    });

    var storage = window.localStorage;
    if (storage) {
        try {
            //重新缓存最新的多语言版本号
            storage.removeItem(AUO_GDS_LangIdKey);
            storage.setItem(AUO_GDS_LangIdKey, dicJson.DIC_INFO.LANG_ID);
            //重新缓存多语言字典
            storage.removeItem(AUO_GDS_LangDicKey);
            storage.setItem(AUO_GDS_LangDicKey, JSON.stringify(dicJson)); //将多语言字典JSON对象转换成字符串
            //重新缓存最新的多语言版本号
            storage.removeItem(AUO_GDS_LangVerKey);
            storage.setItem(AUO_GDS_LangVerKey, AUO_GDS_LatestLangVer);
        }
        catch (e) {
            //设置浏览器缓存异常，缓存不适用，则什么都不做
        }
    }

    return dicJson;
}

//呼叫Handler获取多语言种类（此处为从后台取CAP，若是url传参，将此方法改成从Url参数提取即可）
function GetLangId() {
//    var storage = window.localStorage;
//    //localStorage缓存的Key，请包含系统名，以免与其他系统的缓存冲突
//    var langIdKey = AUO_GDS_LangIdKey;
//    var langId = "";
//    if (storage) {
//        langId = storage.getItem(langIdKey); //获取缓存的多语言种类（简中zh-CHS，繁重zh-CHT，英文en）
//    }
//    if (langId != null && langId != "undefined") {
//        langId = langId.toString();
//        if (langId == 'en' || langId == 'zh-CHS' || langId == 'zh-CHT') {
//            $("#storagelangid").val(langId)
//            //alert(langId);
//            return langId; //1.优先从浏览器缓存中获取，能获到有效的langId则直接返回
//        }
//    }

    //2.客户端浏览器中没有有效的langId，则去后台获取Session缓存/DB中的Language
    $.ajax({
        type: "GET",
        url: "../../GradePromotion/Handler/Public.ashx?func=GetLangId", //【此处url请根据实际修改】
        dataType: "text",
        contentType: "text/plain; charset=utf-8",
        async: false, //此处用同步的方式来调用
        cache: false, //此处不再使用缓存，否则可能导致多语言无法更新
        success: function (data) {
            langId = data;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            langId = "zh-CHT"; //获取多语言Id发生异常，设置成默认的繁中
        }
    });

    return langId;
}

//设置/更新浏览器localStorage缓存的多语言Id
function SetLangId(langId) {
    var storage = window.localStorage;
    //localStorage缓存的Key，请包含系统名，以免与其他系统的缓存冲突
    var langIdKey = AUO_GDS_LangIdKey;
    if (storage) {
        try {
            //重新缓存最新的多语言版本号
            storage.removeItem(langIdKey);
            storage.setItem(langIdKey, langId);
        }
        catch (e) { } //设置浏览器缓存异常，缓存不适用，则什么都不做
    }
}

//更新页面的语言
function RefreshPageLanguage(langId) {

    var currentLangId = GetLangId(); //获取当前（变更前）的多语言Id
    //如果传入了不一样的多语言Id，则重新加载多语言字典，并刷新页面多语言
    if (langId != currentLangId) {
        GetLangDicJSON(langId);

        //将选择的语言存入Session
        $.ajax({
            type: "GET",
            url: "../../GradePromotion/Handler/Public.ashx?func=SetLangId&langId=" + langId, //【此处url请根据实际修改】
            dataType: "text",
            contentType: "text/plain; charset=utf-8",
            async: false, //此处用同步的方式来调用
            cache: false, //此处不再使用缓存，否则可能导致多语言无法更新
            success: function (data) {

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {

            }
        });

        ProcessLanguage();
    }
}

function GetAlertMess(key) {
    var langDic = GetLangDic();
    return langDic[key];
}

//处理替换页面元素中的多语言标记内容
function ProcessLanguage() {
    var langDic = GetLangDic();
    //多语言处理 replace HTML
    $("*").each(function (index, domNode) {
        var currLanguage = langDic;
        var langID = $(domNode).attr("langid");
        if (langID) {
            var langIDSlipt = langID.split(".");
            for (var i = 0; i < langIDSlipt.length; i++) {
                currLanguage = currLanguage[langIDSlipt[i]];
            }
            $(domNode).html(currLanguage);
        }
        //再检查是否有placeholder要处理多语言
        currLanguage = langDic;
        var phlangID = $(domNode).attr("phlangid");
        if (phlangID) {
            var phlangIDSlipt = phlangID.split(".");
            for (var i = 0; i < phlangIDSlipt.length; i++) {
                currLanguage = currLanguage[phlangIDSlipt[i]];
            }
            var inputvalue = $(domNode).val();
            if (inputvalue == $(domNode).attr("placeholder")) {
                inputvalue = "";
            }
            $(domNode).attr("placeholder", currLanguage);
            $(domNode).val(inputvalue);

        }
        //再检查是否有title要处理多语言titlelangid
        currLanguage = langDic;
        var phlangID = $(domNode).attr("titlelangid");
        if (phlangID) {
            var phlangIDSlipt = phlangID.split(".");
            for (var i = 0; i < phlangIDSlipt.length; i++) {
                currLanguage = currLanguage[phlangIDSlipt[i]];
            }
            $(domNode).attr("title", currLanguage);
        }
        //再检查是否有value要处理多语言valuelangid
        currLanguage = langDic;
        var phlangID = $(domNode).attr("valuelangid");
        if (phlangID) {
            var phlangIDSlipt = phlangID.split(".");
            for (var i = 0; i < phlangIDSlipt.length; i++) {
                currLanguage = currLanguage[phlangIDSlipt[i]];
            }
            $(domNode).attr("value", currLanguage);
        }

        currLanguage = langDic;
        var phlangID = $(domNode).attr("introkey");
        if (phlangID) {
            var phlangIDSlipt = phlangID.split(".");
            for (var i = 0; i < phlangIDSlipt.length; i++) {
                currLanguage = currLanguage[phlangIDSlipt[i]];
            }
            $(domNode).attr("data-intro", currLanguage);
        }
    });

}

//多语言处理 - 页面初始化之前处理多语言
$(document).on("ready", function (event) {
    //$.loading("show");
    ProcessLanguage(); //处理多语言
    //$.loading("hide");
});

//獲取QueryString參數
function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}
//獲取QueryString參數-根据传入的URL来获取
function GetUrlQueryString(url, name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r, startPos = 0, subLength = 0;
    if (url.indexOf("?") >= 0) {
        startPos = url.indexOf("?") + 1;
        if (url.indexOf("#") >= 1) {
            subLength = url.indexOf("#") - 1;
        }
    }
    r = url.substr(startPos, subLength).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

/** * 数字格式化 * 
@param s 数字、包含数字的字符串 如'aa1234.11' * 
@param n 小数点位数 * 
@returns 带有千分符的字符串,如'1,234.11' */
function formatNumber(s, n) {
    n = n > 0 && n <= 20 ? n : 2;
    s = parseFloat((s + '').replace(/[^\d\.-]/g, '')).toFixed(n) + '';
    //负数处理
    negPrefix = '';
    if (s[0] == '-') {
        negPrefix = '-';
        s = s.replace(/-/, '');
    }
    var l = s.split('.')[0].split('').reverse(),
      r = s.split('.')[1];
    var t = '';
    for (var i = 0; i < l.length; i++) {
        t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != l.length ? ',' : '');
    }
    if (n == 0)//不要小数部分
        return negPrefix + t.split('').reverse().join('');
    else //根据要保留的小数位数来显示小数部分
        return negPrefix + t.split('').reverse().join('') + '.' + r;
}

//使用Bootstrap Modal来显示提示讯息（取代alert()方法）
function GDS_showModalAlert(msg) {
    $("#lblDialogMsg").html(msg);
    $("#divModalDialog").modal('show');
}

function showModalConfirm(msg) {
    $("#lblDialogMsg2").html(msg);
    $("#divModalDialog2").modal('show');
}

function GetWebSiteName() {
    var pathName = window.document.location.pathname;
    return pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
}