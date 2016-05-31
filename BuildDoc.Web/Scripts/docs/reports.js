var report = new Object();

$(function () {

    report.BindMotherSet();

    $("#btnBuildDoc").click(function () {
        report.DownLoadDoc();
    });
 
})

report.BindMotherSet = function () {

    $.ajax({
        url: "/Report/GetMontherSet?dt=" + new Date().getTime(),
        cache: false,
        type: "POST",
        dataType: 'json',
        async: false,
        success: function (data) {
            if (data == null)
                return;
            var selObj = $("#dropMotherSet");
            $.each(data, function (i, info) {
                selObj.append("<option value=" + info.MOTHER_SET_ID + ">" + info.MOTHER_SET_NAME + "</option>");
            });            
        }
    });

}


report.DownLoadDoc = function () {
    var masterId =  $("#dropMotherSet").val();
    $.ajax({
        url: "/Report/DownloadDoc?dt=" + new Date().getTime(),
        cache: false,
        type: "POST",
        dataType: 'json',
        data:{ masterId: masterId},
        async: false,
        success: function (data) {

        }
    });
    
}
