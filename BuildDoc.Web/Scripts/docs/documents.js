var document = new Object();
document.remarks = new Array();
document.datasource = new Array();


$(function () {

    document.GetRemarks();

    $('[name="GetDataMethod"]').hide();

    //标签行点击
    $(document).on('click', '#remarks tr[name="remarkMould"]', function () {
        document.GetRemarkInfo(null, $(this).find("[title='labelName']").html());
    });

    //标签类型切换
    $('#LabelType').change(function () {        
        var val = $(this).val();
        //标签属性控制
        $.each($('[id^="div_"]'), function () {
            $(this).show();
            if ($(this).is('[labeltype]') && $(this).attr("labeltype") != val) {
                $(this).hide();
            }
        });        
        
        //数据源切换控制
        if (val == 'TextLabel') {
            //文本标签可选其他数据源            
            $('#DataMethod').show();
        } else {
            //非文本标签只能设置数据源
            $('#DataMethod').val("Source");
            $('#DataMethod').hide();           
        }

        $('#DataMethod').change();
    });
    $('#LabelType').change();

    //数据源切换
    $('#DataMethod').change(function () {
        var val = $(this).val();
        var labelType = $('#LabelType').val();
        //数据源切换
        $.each($('[name="GetDataMethod"]'), function () {
            $(this).show();
            //隐藏其他类型数据源
            if ($(this).attr("method") != val) {
                $(this).hide();
            }
            //隐藏标签类型控制数据源
            if ($(this).is('[labeltype]') && $(this).attr("labeltype") != labelType) {
                $(this).hide();
            }
        });       

        //格式化切换        
        $('[id="div_FormatInfo"][method!="' + val + '"]').hide();
        $('[id="div_FormatInfo"][method*="' + val + '"]').show();

       
     
    });
    $('#DataMethod').change();

    //数据类型
    $('#FormatType').change(function () {
        var val = $(this).val();
        $('[name="FormatInfo"][format!="' + val + '"]').hide();
        $('[name="FormatInfo"][format*="' + val + '"]').show();
    });
    $('#FormatType').change();

    //控件类型
    $('#ControlType').change(function () {
        var val = $(this).val();
        $('[name="LabelControl"][control="' + val + '"]').show();
        $('[name="LabelControl"][control!="' + val + '"]').hide();
        $('#FillType').change();
    });
    $('#ControlType').change();

    //填充方式
    $('#FillType').change(function () {
        var val = $(this).val();
        var control = $('#ControlType').val();
        //屏蔽填充方式不一致的选项
        $('[name="LabelFill"][control!="' + control + '"]').hide();
        $('[name="LabelFill"][control="' + control + '"][fill="' + val + '"]').show();
        $('[name="LabelFill"][control="' + control + '"][fill!="' + val + '"]').hide();

    });
    $('#FillType').change();

    //设置公式
    $('#txtLabelFormula').click(function () {
        AlertDiv('#alert_Formula');
        var fields = [];

        $.each(document.remarks, function (i, remark) {
            fields.push(remark.LabelName);
        })
        var feditor = new formualEditor(fields);
    });

    //条件条件
    $('#btnAddCondition').click(function () {
        document.AddConditionRow(0, null);
    });

    //设置条件公式
    $('#btnAddConditionSet').click(function () {
        document.AddConditionSetExpRow("", "");
    });
    
    //保存条件公式
    $('#btnConditionSetSave').click(function () {
        document.SaveConditionSet();
    });

    //关联标签
    $('#btnAddRelate').click(function () {
        document.AddRelateRow(0, null);
    });

    //表格标签
    $('#btnAddTableField').click(function () {
        document.AddTableFieldRow(0, null);
    });

    //暂存
    $('#btnSave').click(function () {
        document.Save();
    })


    //版定数据源
    document.BindDataSource($('#sltDataSource'));

});

//获取书签
document.GetRemarks = function () {
    $.ajax({
        url: "GetRemarks",
        type: "post",
        dataType:"json",
        success: function (datas) {
            document.remarks = new Array();
            if (datas) {             
                $.each(datas, function (i, remark) {
                    var bookInfo = [];
                    bookInfo.LabelName = remark;
                    bookInfo.Name = remark;
                    bookInfo.LabelType = "TextLabel";
                    document.AddRemarkRow(i + 1, bookInfo);
                    document.remarks.push(bookInfo);
                });

            }
        }
    });
}

//获取书签信息
document.GetRemarkInfo = function (config, labelName) {
    $('#LabelName').val(labelName);
}

//解析json配置--为表格增加标签行
document.AddRemarkRow = function (num,remark) {
    var mould = $('#docMould').find("[name='remarkMould']").clone();
    mould.css("display", "");
    //行赋值
    mould.find("[title='number']").text(num);
    mould.find("[title='labelName']").text(remark.LabelName);
    mould.find("[title='labelType']").text(remark.LabelType);    

    $('#remarks tr:last').after(mould);

}

//添加关联标签行
document.AddRelateRow = function (num, relate) {
    var mould = $('#docMould').find("[name='relateMould']").clone();
    mould.css("display", "");
    //行赋值
    if (num <= 0) {
        num = $('#relates tr').length;
        relate = [];
        relate.LabelName = "";
        relate.FieldName = "";
    }
    mould.find("[title='number']").text(num);
    mould.find("[title='labelName']").val(relate.LabelName);       
    mould.find("[title='conditionConfig']").val(relate.FieldName);

    $('#relates tr:last').after(mould);
}

//添加表格标签行
document.AddTableFieldRow = function (num, field) {
    var mould = $('#docMould').find("[name='tableMould']").clone();
    mould.css("display", "");
    //行赋值
    if (num <= 0) {
        num = $('#tableFields tr').length;
        field = [];
        field.FormatInfo = "";
        field.FieldName = "";
    }
    mould.find("[title='number']").text(num);
    mould.find("[title='configStr']").val(field.FormatInfo);
    mould.find("[title='config']").val(field.FieldName).click(function () {
        document.AlertTableFieldConfig(num, "");
    });


    $('#tableFields tr:last').after(mould);
}

//添加条件行
document.AddConditionRow = function (num, condition) {
    var mould = $('#docMould').find("[name='conditionMould']").clone();
    mould.css("display", "");
    //行赋值
    if (num <= 0) {
        num = $('#conditions tr').length;
        condition = [];
        condition.ConditionStr = "";
        condition.LabelType = "";
    }
    mould.find("[title='number']").text(num);
    mould.find("[title='conditionStr']").html(condition.ConditionStr).click(function () {
        document.AlertCondition(condition.ConditionStr, num);
    });

    mould.find("[title='conditionConfig']").click(function () {
        //document.AlertCondition(condition.ConditionStr, num);
    });

    $('#conditions tr:last').after(mould);
}

//添加条件表达式行
document.AddConditionSetExpRow = function (conditionSetExp, expLogic) {
    var mould = $('#docMould').find("[name='conditionSetExpMould']").clone();
    mould.css("display", "");

    //条件大于1，显示逻辑
    if ($('#condtionSetExps tr:visible').length > 0) { 
        mould.find("[name='sltConditionSetJoin']").show();
    }

    //行赋值
    if (conditionSetExp != "") {
        //表达式解析 
        //方法1：逐条解析
        //方法2：空格解析
        //方法3：先解析运算符，再解析分量
        var reg = /[>,<,==,>=,<=]/;
        var operate = conditionSetExp.match(reg);
        //运算符
        mould.find("[name='sltConditionSetOperate']").val(operate);
        //运算分量
        var expArray = conditionSetExp.split(reg);
        mould.find("[name='txtConditionSetRemark']").val(expArray[0]);
        mould.find("[name='txtConditionSetValue']").val(expArray[1]);
        //逻辑分量
        mould.find("[name='sltConditionSetJoin']").val(expLogic);
    }

    $('#condtionSetExps').append(mould);

}

//保存条件公式
document.SaveConditionSet = function () {
    var conditionStr = "";
    var reg = /^(-?\d+)(\.\d+)?$/;   //判断字符串是否为数字
    $.each($('#condtionSetExps tr:visible'), function (i, exp) {
        var quote = "";
        if (!reg.test($.trim($(exp).find("[name='txtConditionSetValue']").val())) ||
            $.trim($(exp).find("[name='txtConditionSetValue']").val()) == "0") {
            quote = "'";
        }
        //是否存在多个条件
        if ($(exp).find("[name='sltConditionSetJoin']").is(":visible") > 0) {
            conditionStr += " " + $(exp).find("[name='sltConditionSetJoin']").val() + " ";
        }
        //左分量（书签）
        conditionStr += quote + $(exp).find("[name='txtConditionSetRemark']").val() + quote;
        //运算符
        conditionStr += " " + $(exp).find("[name='sltConditionSetOperate']").val() + " ";
        //右分量（值）
        conditionStr += quote + $(exp).find("[name='txtConditionSetValue']").val() + quote;
    });
    //将条件字符串保存到条件下
    var num = $('#hidConditionNum').val();
    $('#conditions tr:eq(' + num + ')').find("[title='conditionStr']").text(conditionStr);

    AlertClose($('#alert_ConditionSet'));
}

//绑定数据源
document.BindDataSource = function (sltSource) {
    $.getJSON("/DataSource/GetDataSource", function (datas) {
        document.datasource = datas;
        //绑定数据源
        for (var i = 0; i < datas.length; i++) {
            $('<option>', {
                val: datas[i].DATA_SOURCE_NAME,
                text: datas[i].DATA_SOURCE_NAME
            }).appendTo(sltSource);
        };
        //取数据源字段
        //
        $(sltSource).change(function () {
            var index = $(this).prop('selectedIndex');
            var fields = datas[index].Fields;
            document.BindFields($('#sltDisplayFields'), fields);
            document.BindFields($('#sltFilterFields'), fields);
            document.BindFields($('#sltFillDataSource'), fields);
            document.BindFields($('#sltFormatFields'), fields);
        });
        $(sltSource).change();
    });
};

//筛选数据源
document.BindFields = function (sltFields, fields) {
    if (fields != null) {
        for (var i = 0; i < fields.length; i++) {
            $('<option>', {
                val: fields[i],
                text: fields[i]
            }).appendTo($(sltFields));
        }
    }
}

//弹出表格标签配置界面
document.AlertTableFieldConfig = function (num, config) {
    AlertDiv('#alert_ConfigTableField');

    //为配置添加数据处理div_FormatInfo
    $('#tableFieldContent').append($('#div_FormatInfo').show());
    
    if (config != "") {
        $('#FormatType').val(config.FormatType).change();

    }    
}

//弹出设置条件等式的界面
document.AlertCondition = function (conditionStr,num) {
    AlertDiv('#alert_ConditionSet');
    //条件序号
    $('#hidConditionNum').val(num);
    var fields = new Array("估价目的", "估价对象类型");
    $.each(document.remarks, function (i, remark) {
        fields.push(remark.LabelName);
    });
    //解析条件表达式
    var reg = /[&&,||]/;    
    var conditions = conditionStr.split(reg);
    $(conditions, function (i, conditionExp) {
        //新增行
        document.AddConditionExpRow(conditionExp,reg.source);
    })
    
}


//清空控件
document.ClearControl = function (container) {
    $.each($(container).find("input,select"), function (n, obj) {
        if ($(this).is("select")) {
            $(obj).val('');
        }
        else if ($(this).is("input:checkbox")) {
            $(obj).attr("checked", false);
        }
        else if ($(this).is("input:radio")) {
            $("input[name='" + $(obj).attr("id") + "'][alt='1']").click();
        }
        else {
            $(obj).val("");
        }
    });
}

//获取文本标签配置
document.GetTextConfig = function () {    
    var config = {};
    var getDataMethod = $('#DataMethod').val();
    config.GetDataMethod = getDataMethod;
    if (GetDataMethod == "Source" || GetDataMethod == "MultiSource") {
       // config.GetDataMethod = 
    }

}

//保存配置
document.Save = function () {

    var remark = {};
    remark.LabelName = $.trim($("#LabelName").val());
    remark.LabelType = $("#LabelType").val();
    //标签保存
    document.GetConfig(remark);
}

//获取标签配置
document.GetConfig = function (remark) {
    var config = {};
    switch (remark.LabelType) {
        case "TextLabel":
            config.GetDataMethod = $('#DataMethod').val();
            config = document.GetConfigJson("DataMethod", config);
            break;
        case "DocLabel":
            config.Config = {};
            config.Config = StructureClass.DynamicAddJson("DocLabel_", config.Config);
            var getDataMethod = $("#DocLabel_GetDataMethod").val();
            config.Config.GetDataMethod = getDataMethod;
            break;
        case "TableLabel":
            config.Config = {};
            config.Config = StructureClass.DynamicAddJson("TableLabel_", config.Config);
            //config.Config.ColumnInfo =eval($("#ColumnInfo").val());
            var lstFiled = [];
            if ($("#tbTableField tr:visible:gt(0)").length <= 0) {
                flagStr = "该标签未绑定填充字段";
                flag = false;
            } else {
                $.each($("#tbTableField tr:visible:gt(0)"), function (i, obj) {
                    var configStr = $.trim($(obj).find("#tbConfigStr").html());
                    var field = {};
                    field.FieldName = $.trim($(obj).find("#tbField").val());
                    field.ColumnIndex = i;
                    field.FormatInfo = {};
                    // $.parseJSON($(obj).find("#tbConditionConfig").html())
                    if (field.FieldName == "") {
                        flagStr = "表格标签" + (i + 1) + "未设置字段名";
                        flag = false;
                        return false;
                    }
                    if (configStr != "") {
                        field.FormatInfo = $.parseJSON(configStr);
                    }
                    lstFiled.push(field);
                });
            }
            config.Config.ColumnInfo = lstFiled;
            break;
        case "ImageLabel":
            config.Config = {};
            config.Config = StructureClass.DynamicAddJson("ImageLabel_", config.Config);
            var getDataMethod = $("#ImageLabel_GetDataMethod").val();
            config.Config.GetDataMethod = getDataMethod;
            break;
        default:
            var lstCondition = [];
            if ($("#tbCondition tr:visible").length <= 1) {
                flagStr = "未设置条件";
                flag = false;
            }
            else {
                $.each($("#tbCondition tr:visible:gt(0)"), function (i, obj) {
                    var conditionConfigStr = $.trim($(obj).find("#tbConditionConfig").html());
                    if (conditionConfigStr == "") {
                        flagStr = "条件" + (i + 1) + "未设置条件配置";
                        flag = false;
                        return false;
                    }
                    var condition = $.parseJSON($(obj).find("#tbConditionConfig").html());
                    condition.Condition = $(obj).find("#tbCondition").html();
                    if (condition.Condition == "") {
                        flagStr = "条件" + (i + 1) + "未设置条件";
                        flag = false;
                        return false;
                    }
                    lstCondition.push(condition);
                });
            }
            //config.LabelName = $("#LabelName").val();
            //config.LabelType = $("#LabelType").val();
            config.Config = lstCondition;

            break;
    }
}


//获取常量配置
document.GetFormatConfig = function () {    
    var format = {};
    format.FormatType = $('#FormatType').val();
    //获取format配置
    format = document.GetConfigJson("FormatInfo", format);
    return format;
}

//获取关联配置
document.GetRelateConfig = function () {
    var relates = [];
    $.each($('#relates tr:visable'), function (i, obj) {
        var relate = {};
        relate.LabelName = $(obj).find("[title='labelName']").find('input').val();
        relate.FieldName = $(obj).find("[title='fieldName']").find('input').val();
        if (relate.LabelName != "" && relate.FieldName != "") {
            relates.push(relate);
        }
    });
    return relates;
}

//获取控件配置
document.GetControlConfig = function () {
    var control = {};
    control.ControlType = $('#ControlType').val();
    control = document.GetConfigJson("LabelControl", control);
    if (control.FillType == "DropDown") {
        control = document.GetConfigJson("LabelFill", control);
    }
    return control;
}

//获取配置项
//获取筛选属性的输入配置项
document.GetConfigJson = function (filterCtrl,config) {
    //遍历
    $('[name="' + filterCtrl + '"]').find('input,select').each(function (i,obj) {
        var key = $(this).attr("key");
        var config = "config." + key + "=";
        if ($(this).is("select")) {
            var value = $(obj).val();
            value = value == "-请选择-" ? "" : value;
            config += "\"" + value + "\"";
        }
        else if ($(this).is("input:checkbox")) {
            config += "\"" + $(obj).is(":checked") + "\"";
        }
        else if ($(this).is("input:radio")) {
            config += "\"" + $(":radio[name='" + $(this).attr("id") + "']:checked").val() + "\"";
        }
        else if ($(this).hasClass("autocomplate")) {
            config += "\"" + $(obj).attr("val") + "\"";
        }
        else {
            config += "\"" + $(obj).val() + "\"";
        }
    });
    return config;
}
 

//提交配置 
document.Submit = function () {
    $.each(document.remarks, function (i, remark) {
        //没有配置采用默认
        if (remark.Conifg == undefined) {
            remark.LabelType = "TextLabel";
            remark.Config = {
                GetDataMethod: "Const", Value: ""
            };
            remark.Control = {
                ControlType: "Text",
                Required: "false",
                ValidateString:""
            };
        }       
    });

    var remarkContents = JSON.stringify(document.remarks);
    //提交配置
    $.ajax({
        url: "SaveRemarks",
        cache: false,
        type: "POST",
        dataType: 'json',
        data: { remarkContents: remarkContents },
        success: function (data) {
            if (data.IsSuccess) {
                AlertTips('保存成功', 'right', 3);
                $("#dropStructure").change();
            }
            else {
                AlertTips('保存失败', 'error', 3);
            }
        }
    });
   
}

//获取
document.GetData = function ($container) {

}