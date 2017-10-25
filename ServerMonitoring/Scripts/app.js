//for checking if a new user is using the system
var tempMobile = 0;
var tempWeb = 0;
//boolean to avoid notification when loading the page
var firstTime = true;

//FOR THE INFO DROPDOWN
function requestData() {
    $.ajax({
        url: "/api/requestData",
        type: 'GET',
        datatype: 'json',
        success: function (requestData) {
            $('#webLoader').css("display", "none");
            $('#mobileLoader').css("display", "none");
            $('#mobileUsersOnline').html(requestData.ActiveUserModel.MobilUsers).css("display", "block");
            $('#webUsersOnline').html(requestData.ActiveUserModel.WebUsers).css("display", "block");

            //first time
            if (firstTime) {
                firstTime = false;
                tempMobile = requestData.ActiveUserModel.MobilUsers;
                tempWeb = requestData.ActiveUserModel.WebUsers;
            }
            if (requestData.ActiveUserModel.MobilUsers > tempMobile) {
                $.notify("New mobile user", { position: "right bottom" });
            }
            if (requestData.ActiveUserModel.WebUsers > tempWeb) {
                $.notify("New web user", { position: "right bottom" });
            }
            tempMobile = requestData.ActiveUserModel.MobilUsers;
            tempWeb = requestData.ActiveUserModel.WebUsers;


            $('#totalRequests').html(requestData.TotalRequests);
            $('#averageMobileRequest').html(requestData.AverageMobilRequests);
            $('#averageWebRequest').html(requestData.AverageWebRequests);
            $('#webRequests').html(requestData.WebRequests);
            $('#mobilRequests').html(requestData.MobilRequests);
            $('#averageTotalRequest').html(requestData.TotalAverageRequests);
        },
        error: function (error) {
        }
    });
}
setInterval(requestData, 100 * 60 * 1);

function GetNotificationCount() {
    $.ajax({
        url: "/Event/GetNotificationCount",
        type: 'GET',
        success: function (count) {

            $(".num").html(count);
            if (count == 0) {
                $('#notification-alert').css('display', 'none');
            } else {
                    $('#notification-alert').css('display', 'block');
                    if (count > 100) {
                        $('.num').css('right', '-7px');
                    } else if (count > 10) {
                        $('.num').css('right', '-4px');
                    } else {
                        $('.num').css('right', '-2px');
                    }
            }
        },
        error: function (error) {
            console.log('GetNotificationCount: ' + error);
        }
    });
};
setInterval(GetNotificationCount, 100 * 60 * 1);



////Chart js get color and set font family
//Chart.defaults.global.defaultFontFamily = "Tahoma";
//var color = Chart.helpers.color;
////Pie chart
//var configRam = {

//    type: 'pie',
//    data: {
//        datasets: [{
//            data: [
//                PercentAvailable(0,0),
//                PercentUnavailable(0,0)
//            ],
//            backgroundColor: [
//                window.chartColors.red,
//                window.chartColors.green
//            ],
//            label: 'Dataset 1'
//        }],
//        labels: [
//            "Used",
//            "Free"
//        ]
//    },
//    options: {
//        responsive: true
//    }
//};
//function PercentAvailable(total, available) {
//    return percentage = Math.round(available/total*100);
//};
//function PercentUnavailable(total, available) {
//    return percentage = Math.round(100-available/total*100);
//};
//function GetUsedRAM(total, available)
//{
//    return total-available;
//};
//function ConvertToGbFormat(mb)
//{
//    return formatBytes(mb*1000*1000);
//}

//function formatBytes(bytes) {
//    if(bytes < 1024) return bytes + " Bytes";
//    else if(bytes < 1048576) return(bytes / 1024).toFixed(2) + " KB";
//    else if(bytes < 1073741824) return(bytes / 1048576).toFixed(2) + " MB";
//    else return(bytes / 1073741824).toFixed(2) + " GB";
//};


//var ctxRam = document.getElementById("canvas-ram").getContext("2d");
//window.myPie = new Chart(ctxRam, configRam);