var documents = new Object();


documents.GetRemarks = function () {
    $.ajax({
        url: "/Labels/Index",
        type: "post",        
        success: function (data) {
            $('#dataLabelList').html(data);
        }

    });
}