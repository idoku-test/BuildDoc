var documents = new Object();
documents.remarks = new Array();

documents.GetRemarks = function () {
    $.ajax({
        url: "/Labels/Index",
        type: "post",        
        success: function (datas) {                      
            $('#dataLabelList').html(datas);
        }

    });
}