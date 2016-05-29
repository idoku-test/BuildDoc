var report = new Object();

$(function () {

    report.BindMotherSet();

    $("#btnBuildDoc").click(function () {
        $.ajax({
            url: "/Report/IsSessionTimeOut?dt=" + new Date().getTime(),
            success: function (data) {
                ReportClass.DownLoadDoc();
            }, error: function () {
                //超时自动跳出
                window.location.reload();
            }
        });
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


ReportClass.DownLoadDoc = function () {

    var ids = $("#dropMotherSet").val();
    //var dialogTip = artDialog.tips("<img src='/Content/image/loading.gif'> 正在生成文档，请稍候…", 20);
    var masterId = ids.split('$')[0];
    // var json = ids.split('$')[1];
    var structureJson = ReportClass.GetChangeSortJson();
    var json = JSON.stringify(structureJson);

    //获得估价对象的对象实例ID
    var instanceIds = new Array();
    var objectIds = new Array();
    var tdId = ReportClass.BusinessType == 1 ? "tdEstimate_Instance_Document_ID" : "tdInstance_Document_ID";

    if (ReportClass.IsSingleObject)//单标的物
    {
        AlertClose($('#alert_wdsx'));
        var dialogTip = ArtDailogTips("<img src='/Content/image/loading.gif'> 正在生成" + ReportClass.OprationName + ",请稍候…", null, true);
        $.ajax({
            url: "/Report/DownloadDoc?dt=" + new Date().getTime(),
            dataType: "json",
            type: "post",
            data: {
                instanceDocumentId: ReportClass.InstanceID, masterId: masterId, objId: ReportClass.ObjectId,
                json: encodeURIComponent(json), prjId: ReportClass.ProjectId,
                businessType: $.getUrlParam("bizType")
            },
            success: function (data) {
                if (data.Succeeded == true) {
                    AlertClose($('#alert_wdsx'));
                    AlertTips('生成成功…', 'right', 3);
                } else {
                    AlertTips('生成失败，请重试…', 'error', 3);
                }
            }, error: function () {
                //超时自动跳出
                AlertTips('生成失败，请重试…', 'error', 3);
            }, complete: function () {
                dialogTip.close();
            }
        });
    }
    else {
        AlertConfirm('提示', '请确认排序？', "确定", '取消', function () {

            $.each($("#gdObject tr:visible:gt(0)"), function (i, trObj) {
                if ($(trObj).find("#" + tdId).html() != "") {
                    instanceIds.push($(trObj).find("#" + tdId).html());
                }
                if ($(trObj).find("#tdObject_id").html() != "") {
                    objectIds.push($(trObj).find("#tdObject_id").html());
                }
            });

            var framChange;
            framChange = window.frames["framChangeSort"].contentWindow;
            if (framChange == undefined)
                framChange = window.frames["framChangeSort"];

            var grid = $(framChange.document.getElementById("grid"));

            var config = [];
            var isFlag = false;
            $.each(grid.find("tr:visible:gt(0)"), function (i, obj) {
                isFlag = true;
                var info = {};
                info.Key = $(obj).find("#tdID").html();
                info.Type = $(obj).find("#tdType").html();
                info.InstanceId = $(obj).find("#tdInstance_document_id").html();
                info.MotherSetId = $(obj).find("#tdMotherSetId").html();
                config.push(info);
            });
            var info = {
                DOCUMENT_STRUCTURE: JSON.stringify(config),
                Project_ID: ReportClass.ProjectId,
                Object_ID: 0,
                INSTANCE_DOCUMENT_ID: ReportClass.InstanceID
            };
            //添加文档实例后保存报告
            $.ajax({
                url: '/Report/AddInstanceDocument?dt=' + new Date().getTime() + '&t=' + $.getUrlParam("bizType") + '&idd=1&iso=' + ReportClass.IsSingleObject,
                type: "POST",
                data: info,
                cache: false,
                async: false,
                dataType: 'json',
                success: function (data) {
                    if (data) {
                        if (data.Succeeded == true) {
                            ReportClass.InstanceID = data.ResultId;
                            AlertClose($('#alert_wdsx'));
                            var dialogTip = ArtDailogTips("<img src='/Content/image/loading.gif'> 正在生成" + ReportClass.OprationName + ",请稍候…", null, true);
                            $.ajax({
                                url: "/Report/DownloadDoc2?dt=" + new Date().getTime(),
                                dataType: "json",
                                type: "post",
                                data: {
                                    mergerInstanceDocumentID: ReportClass.InstanceID,
                                    objInstanceIds: instanceIds.join(','), objectIds: objectIds.join(','),
                                    prjId: ReportClass.ProjectId,
                                    businessType: $.getUrlParam("bizType")
                                },
                                success: function (data) {
                                    if (data.Succeeded == true) {
                                        AlertTips('生成成功…', 'right', 3);
                                        AlertClose($('#alert_wdsx'));
                                        // location.reload();
                                    } else {
                                        AlertTips('生成失败，请重试…', 'error', 3);
                                    }
                                }, error: function () {
                                    //超时自动跳出
                                    AlertTips('生成失败，请重试…', 'error', 3);
                                }, complete: function () {
                                    dialogTip.close();
                                }
                            });

                        }
                    }
                }
            });
            return true;
        }, null, 'ask');
    }
}
