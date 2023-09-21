var myApp = new Framework7();

//var $$ = Dom7;

//var mainView = myApp.addView('.view-main', {
//    dynamicNavbar: true
//});

var C = {
    Confirm: function (param, alertMsg, alertTitle) {
        if (param == "" || param == null || param == undefined) {
            myApp.alert(alertMsg, alertTitle);
            return;
        }
    },
    CheckIsEmpty: function (param) {
        if (param == "" || param == null || param == undefined) {
            return true;
        }
    },
    GetDateDiff: function (startDate, endDate) {
        //var startTime = new Date(Date.parse(startDate.replace(/-/g, "/"))).getTime();
        //var endTime = new Date(Date.parse(endDate.replace(/-/g, "/"))).getTime();
        var dates = Math.abs((startDate - endDate)) / (1000 * 60 * 60 * 24);
        return dates;
    }
}