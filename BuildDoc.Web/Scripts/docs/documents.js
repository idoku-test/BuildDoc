var document = new Object();
document.remarks = new Array();
document.datasource = new Array();
//激活的标签类型
//切换取值方式时需要判断标签类型
document.ActiveLabelType = "";

var feditor = null;

$(function () {
    //获取构件列表
    document.GetStructures(0,0);
    //document.GetRemarks();         
    //$('[name="GetDataMethod"]').hide();
    document.InitDataSource();
    //标签行点击
    $(document).on('click', '#remarks tr[name="remarkMould"]', function () {
        document.GetRemarkInfo($(this).find("[title='labelName']").html(),$(this).find("[title='configInfo']").html());
        $("#remarks tr").removeClass("hover");
        $(this).attr("class", "hover");
    });

    //选择构件
    $("#dropStructure").change(function () {
        document.GetStructure($(this).val());
    });

    //标签类型切换
    $('#LabelType').change(function () {        
        var val = $(this).val();
        document.ActiveLabelType = val;
        //标签属性控制      
        document.Selector($('#mark_main [id^="div_"]'), "labeltype", val);

        //数据源切换控制
        if (val == 'TextLabel') {
            $('#DataMethod').val("Const");
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
        var labelType = document.ActiveLabelType;
        //数据源切换            
        document.Selector($('[name="GetDataMethod"]'), "method", val);
        document.Selector($('[name="GetDataMethod"][labeltype]'), "labeltype", labelType);
        
        if (labelType == 'TextLabel') {            
            document.Selector($('#div_FormatInfo'), "method", val);          
        }
    });

    $('#DataMethod').change();

    //数据类型
    $('#FormatType').change(function () {
        var val = $(this).val();       
        document.Selector($('[name="FormatInfo"]'), "format", val);
    });
    $('#FormatType').change();

    //控件类型
    $('#ControlType').change(function () {
        var val = $(this).val();       
        document.Selector($('[name="LabelControl"]'), "control", val);      
        $('#FillType').change();
    });
    $('#ControlType').change();

    //填充方式
    $('#FillType').change(function () {
        var val = $(this).val();
        var control = $('#ControlType').val();
        //屏蔽填充方式不一致的选项
        document.Selector($('[name="LabelFill"]'), "fill", val);
        document.Selector($('[name="LabelFill"][control]'), "control", control);
       
    });
    $('#FillType').change();

    //设置公式
    $('[name="txtLabelFormula"]').click(function () {              
        AlertDiv('#alert_Formula');
        
        var fields = [];
        $.each(document.remarks, function (i, remark) {
            fields.push("@" + remark.LabelName);
        });
        feditor = new formualEditor(fields);                               
    });

    //获取公式值
    $('#btnSetFormulaSave').click(function () {
        if (feditor != null) {
            var formula = feditor.save();
            $('#txtLabelFormula').val(formula);
            AlertClose('#alert_Formula');
        }
    })

    //保存条件配置
    $('#btnConditionConfigSave').click(function () {
        var remark = {};
        remark.LabelType = $('#sltConditionLabelType').val();
        //标签保存
        remark = document.GetRemarkConfig(remark, "condtion_content");
        var condNum = $('#hidConditionConfigNum').val();
        $("#conditions [number='" + condNum + "']").find("[title=ConditionConfigStr]").html(JSON.stringify(remark));
        document.CloseConfigConditionDailog();
    });

    //条件条件
    $('#btnAddCondition').click(function () {
        document.AddConditionRow(null);
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


   

});

/*
stype:header,body,footer.
dtype:report,estimate
*/
document.GetStructures = function (stype,dtype) {
    var dialogTip = ArtDailogTips('<img src="/Content/image/loading.gif"> 正在查询构件…', null, true);
    $.ajax({
        url: "GetStructures",
        cache: false,
        dataType: 'json',
        type: "get",
        data: { stype: stype, dtype: dtype },
        success: function (data) {
            var option = "<option value=''>-选择构件-</option>";
            $(data).each(function (i, obj) {
                option += "<option value='" + obj.STRUCTURE_ID + "'>" + obj.STRUCTURE_NAME + "</option>";
            });
            $("#dropStructure").html("").append(option);
        },
        complete: function () {
            dialogTip.close();
        }
    });
}

//获取构件信息
document.GetStructure = function (Id) {
    //清除之前构件
    $("#remarks tr:gt(0)").remove();
    var dialogTip = ArtDailogTips("<img src='/Content/image/loading.gif'> 正在读取构件配置,请稍候…", null, true);
    $.ajax({
        url: "GetStructureInfo", //获取构件配置内容
        data: { id: Id },
        success: function (info) {
            //解析json配置信息            
            document.AnalysisConfig(info.SET_CONTENT);
            //绘制表格
            document.PaintTable("remarks");
            //选择首项
            $("#remarks tr:eq(1)").click();
        }, complete: function () {
            dialogTip.close();
        }
    });
}

//解析配置信息
document.AnalysisConfig = function (config) {
    if (config == null || $.trim(config) == "") {
        return;
    }  
    var marks = $.parseJSON(config);
    $.each(marks, function (i, mark) {
        document.AddRemarkRow(i + 1, mark);
    });
};

/*
    初始化界面元素
    绑定界面数据源
*/
document.GetConditionDataLabel = function () {   
    //条件数据类型
    $("#sltConditionLabelType").change(function () {
        var val = $(this).val();        
        document.ActiveLabelType = val;
        document.Selector($('#condtion_content > [id^="div_"]'), "labeltype", val);
        //数据源切换控制
        if (val == 'TextLabel') {
            $('#DataMethod').val("Const");
            //文本标签可选其他数据源            
            $('#DataMethod').show();
        } else {
            //非文本标签只能设置数据源
            $('#DataMethod').val("Source");
            $('#DataMethod').hide();
        }
        $('#DataMethod').change();
    });
    $("#sltConditionLabelType").change();


    //为条件标签 插入配置
    var dataContiner = $('#div_DataLabel');
    dataContiner.addClass("Vsearch");
    $('#condtion_content').append(dataContiner);
    
    //为条件标签 插入格式化
    var formatContiner = $('#div_FormatInfo');
    $('#condtion_content').append(formatContiner);
    formatContiner.addClass("Vsearch");
}


//还原控件
document.ResetDataLabel = function ()
{
    //为条件标签 插入配置
    var dataContiner = $('#div_DataLabel');
    $('#div_label').append(dataContiner);

    //为条件标签 插入格式化
    var formatContiner = $('#div_FormatInfo');
    $('#div_label').append(formatContiner);

    //回到主界面
    $('#LabelType').change();
        
}

 
 
document.CloseConfigConditionDailog = function () {
    AlertClose($("#alert_ConfigCondition"));
    document.ResetDataLabel();
}

//初始化数据来源
document.InitDataSource = function () {
    
    document.BindDataSource($('#div_DataLabel').find('[name="sltDataSource"]'));
    document.BindDataSource($('#alert_ConfigCondition').find('[name="sltDataSource"]'));
    //绑定数据源
    document.BindConstSource($('[name="sltFillDataSource"]'));
    document.BindConstSource($('[name="sltFormatFields"]'));        
}

//获取书签
document.GetRemarks = function () {
    $.ajax({
        url: "/Documents/GetRemarks",
        type: "GET",
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
document.GetRemarkInfo = function (labelName, config) {
    $('#LabelName').val(labelName);
    if (config === "")
        return;
    var info = $.parseJSON(config);

}

//解析json配置--为表格增加标签行
document.AddRemarkRow = function (num,remark) {
    var mould = $('#docMould').find("[name='remarkMould']").clone();
    mould.css("display", "");
    //行赋值
    mould.find("[title='number']").text(num);
    mould.find("[title='labelName']").text(remark.LabelName);
    mould.find("[title='labelType']").text(remark.LabelType);    
    mould.find("[title='config']").text(JSON.stringify(remark));
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
    var labelCtrl = mould.find("[title='labelName']").find('input');
    labelCtrl.val(relate.LabelName);
    var filedCtrl = mould.find("[title='fieldName']").find('input');
    filedCtrl.val(relate.FieldName);
    document.AutoReMarks(labelCtrl);
    document.AutoReMarks(filedCtrl);
    $('#relates tr:last').after(mould);
}

//自动完成书签
document.AutoReMarks = function (control) {   
    $(control).autocomplete({
        minLength: 0,        
        source: function (request, response) {
            response($.map(document.remarks, function (value, key) {
                return {
                    label: value.LabelName,
                    value:value.labelName
                }
            }));
        }
    }).click(function () {
        $(this).autocomplete("search", $(this).val());
    });
};

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

    var filedCtrl = mould.find("[title='field']").find('input');
    document.AutoReMarks(filedCtrl);
    mould.find("[title='configStr']").val(field.FormatInfo);    
    mould.find("[title='config']").val(field.FieldName).click(function () {
        document.AlertTableFieldConfig(num, "");

    });


    $('#tableFields tr:last').after(mould);
}

//添加条件行
document.AddConditionRow = function (condition) {
    var mould = $('#docMould').find("[name='conditionMould']").clone();
    mould.css("display", "");
    var num = Number($('#conditions tr:last').find('[title="number"]').text()) + 1;
    mould.attr("number", num);
    //行赋值
    if (!condition) {
        condition = [];
        condition.ConditionStr = "";
        condition.LabelType = "";
    }
    mould.find("[title='number']").text(num);
    mould.find("[title='conditionStr']").html(condition.ConditionStr).click(function () {
        document.AlertCondition(condition.ConditionStr, num);
    });
   
    mould.find("[title='conditionConfig']").click(function () {
        var configStr = mould.find("[title='ConditionConfigStr']").html();
        
        document.AlertConditionConfig(num, configStr);

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

    //条件字段自动完成
    document.AutoReMarks(mould.find("[name='txtConditionSetRemark']"));
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
    $("#conditions [number='"+ num +"']").find("[title='conditionStr']").text(conditionStr);

    AlertClose($('#alert_ConditionSet'));
}

//绑定常量数据源
document.BindConstSource = function (sltSource) {
    $.getJSON("/DataSource/GetLabelDealSource", function (datas) {
        document.datasource = datas;
        //绑定数据源
        for (var i = 0; i < datas.length; i++) {
            $('<option>', {
                val: datas[i].DATA_SOURCE_NAME,
                text: datas[i].DATA_SOURCE_NAME
            }).appendTo(sltSource);
        };
    });
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
        var closet = $(sltSource).closest('div');
        
        $(sltSource).change(function () {
            var index = $(this).prop('selectedIndex');
            var fields = datas[index].Fields;
            document.BindFields($(closet).find('[name="sltDisplayFields"]'), fields);
            document.BindFields($(closet).find('[name="sltFilterFields"]'), fields);
        });
        $(sltSource).change();
    });
};

//筛选数据源
document.BindFields = function (sltFields, fields) {
    $(sltFields).empty();
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
    
    if (config != "") {
        $('#FormatType').val(config.FormatType).change();
        $('#Condition_DataMethod').change();
    }    
}

//弹出设置条件等式的界面
document.AlertCondition = function (conditionStr,num) {
    AlertDiv('#alert_ConditionSet');  
    //条件序号
    $('#hidConditionNum').val(num);    
    //解析条件表达式
    var reg = /[&&,||]/;    
    var conditions = conditionStr.split(reg);
    
    $(conditions, function (i, conditionExp) {
        //新增行
        document.AddConditionSetExpRow(conditionExp, reg.source);
    })
  
    
}

//弹出设置条件等式配置界面
document.AlertConditionConfig = function (num, configStr) {  
    AlertDiv("#alert_ConfigCondition");
    document.GetConditionDataLabel();   
    $('#hidConditionConfigNum').val(num);
    //条件配置赋值
    if (configStr == "") {
        $('#sltConditionLabelType').change();
        $('#Condition_DataMethod').change();
    } else {
        var bookmark = $.parseJSON(configStr);
        $('#sltConditionLabelType').val(bookmark.LabelType).change();
        if (bookmark.Config) {
            var config = bookmark.Config;
            document.SetConfigJson("condtion_content", config);
            if (config.FormatInfo) {
                document.SetConfigJson("condtion_content", config.FormatInfo)
            }
        }
    }
}


//保存配置
document.Save = function () {

    var remark = {};
    remark.LabelName = $.trim($("#LabelName").val());
    remark.LabelType = $("#LabelType").val();
    //标签保存
    document.GetRemarkConfig(remark, "div_DataLabel");

    var ety = {};
    ety.DATA_LABEL_ID = 0;   
    ety.CONFIG_CONTENT = JSON.stringify(remark);
    ety.LABEL_NAME = remark.LabelName;
    ety.DATA_SOURCE_NAME = "";

    $.ajax({
        url: "/Labels/Save",
        cache: false,
        type: "POST",
        dataType: 'json',
        data: ety,
        success: function (data) {
        }
    });
}

//获取标签配置
document.GetRemarkConfig = function (remark,continer) {
    remark.Config = {};
    switch (remark.LabelType) {
        case "TextLabel":            
            remark.Config = document.GetValueConfig(continer);
            remark.Relate = document.GetRelateConfig();
            remark.Control = document.GetControlConfig();
            break;
        case "DocLabel":         
            remark.Config = document.GetValueConfig(continer);
            break;
        case "TableLabel":          
            remark.Config = document.GetValueConfig(continer);
            remark.Config.ColumnInfo = document.GetColumnInfo();
            break;
        case "ImageLabel":           
            remark.Config = document.GetValueConfig(continer);
            break;
        case 'ConditionLabel':
            //            
            break;
        default:
            //出错
            break;
           
    }
    return remark;
}

//获取文本配置
document.GetValueConfig = function (container) {
    config = {};
    var getDataMethod = $('#' + container + ' [name="DataMethod"]').val();
    config.GetDataMethod = getDataMethod;
    switch (getDataMethod) {
        case "Const":
            document.GetConfigJson(container,"GetDataMethod", "method", "Const", config);
            break;
        case "Formula":
            document.GetConfigJson(container,"GetDataMethod", "method", "Formula", config);
            config.FormatInfo = document.GetFormatConfig(container);
            break;
        case "Source":
            document.GetConfigJson(container,"GetDataMethod", "method", "Source", config);
            config.FormatInfo = document.GetFormatConfig(container);
            break;
        default:
            //报错
    }
    return config;
}

//获取条件配置
document.GetConditionConfig = function (conifg) {
    var conditions = [];
    $.each($('#conditions'), function (i, obj) {
        var conditionConfigStr = $(obj).find("").find()
    })
}

//获取常量配置
document.GetFormatConfig = function (container) {
    var format = {};
    format.FormatType = $('#FormatType').val();
    //获取format配置
    format = document.GetConfigJson(container, "FormatInfo", "format", format.FormatType, format);
    return format;
}

//获取关联配置
document.GetRelateConfig = function () {
    var relates = [];
    $.each($('#relates tr:visible:gt(0)'), function (i, obj) {
        var relate = {};
        relate.LabelName = $(obj).find("[title='labelName']").find('input').val();
        relate.FieldName = $(obj).find("[title='fieldName']").find('input').val();
        if (relate.LabelName != "" && relate.FieldName != "") {
            relates.push(relate);
        }
    });
    return relates;
}

//获取填充字段配置
document.GetColumnInfo = function () {
    var cols = [];
    $.each($('#tableFields tr:visable'), function (i, obj) {
        var formatStr = $(obj).find("[title='configStr']").text();
        var field = {};       
        filed.FIeldName = $(obj).find("[title='field']").find('input').val();
        filed.ColumnIndex = i;
        if (formatStr != "") {
            field.FormatInfo = $.parseJSON(formatStr);
        }
        cols.push(field);
    });
    return cols;
}

//获取控件配置
document.GetControlConfig = function () {
    var control = {};
    control.ControlType = $('#ControlType').val();
    control = document.GetConfigJson("div_Control", "LabelControl", "control", control.ControlType, control);
    if (control.ControlType == "DropDown") {
        control = document.GetConfigJson("div_Control", "LabelFill", "fill", control.FillType, control);
    }
    return control;
}

//获取配置项
//获取筛选属性的输入配置项
document.GetConfigJson = function (continer,ctrl, filter, value, config) {
    //遍历显示控件
    $('#'+ continer +' [name="' + ctrl + '"]:visible').each(function (i, element) {
        //筛选属性值
        if ($(this).is(filter) && $(this).attr(filter) != value) {
            return;
        }
        $(this).find(':input').each(function (i, obj) {
            var key = $(this).attr("key");
            var configStr = "config." + key + "=";
            if ($(this).is("select")) {
                var value = $(obj).val();
                value = value == "-请选择-" ? "" : value;
                configStr += "\"" + value + "\"";
            }
            else if ($(this).is("input:checkbox")) {
                configStr += "\"" + $(obj).is(":checked") + "\"";
            }
            else if ($(this).is("input:radio")) {
                configStr += "\"" + $(":radio[name='" + key + "']:checked").val() + "\"";
            }
            else if ($(this).hasClass("autocomplate")) {
                configStr += "\"" + $(obj).attr("val") + "\"";
            }
            else {
                configStr += "\"" + $(obj).val() + "\"";
            }
            //执行设置config属性
            eval(configStr);
        });
    });
    return config;
}

//设置配置项
document.SetRemarkConfig = function (remark, continer) {
    switch (remark.LabelType) {
        case "TextLabel":
            remark.Config = document.SetConfigJson(remark, continer);

            break;
        case "DocLabel":

            break;
        case "TableLabel":

            break;
        case "ImageLabel":

            break;
        case 'ConditionLabel':
            //            
            break;
        default:
            //出错
            break;

    }

}

//设置配置项 
document.SetConfigJson = function (continer, config){
    for (var c in config)
    {
        var obj = $("#" + continer + " [key='" + c + "']:visible");
        if (obj.length == 0)
            continue;
        if (obj.is("select"))
            obj.val(config[c]).change();
        else if (obj.is("input:checkbox")) {           
                 obj.prop("checked",config[c])
        } else if (obj.is("input:radio")) {
            obj.filter("[value='" + config[c] + "']").prop("checked", true);
        } else {
            obj.val(config[c]);
        }
    }
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

//选择属性-值显示
document.Selector = function (ctrls, attribute, value) {  
    $.each(ctrls, function () {
        var ctrl = $(this);
        var hasAttr = $(ctrl).is('[' + attribute + ']');
        if (!hasAttr) {
            $(this).show();
            return;
        }
        var val = $(ctrl).attr(attribute);
        if (val == "*") {
            $(ctrl).show();
        }
        else if (val.startsWith("!")) {
            $(ctrl).show();
            var val = val.slice(1, val.length);
            if (val == value) {
                $(ctrl).hide();
            }
        } else {
            $(ctrl).show();
          
            if (value == "" || (val != "" && val.indexOf(value) == -1)) {
                $(ctrl).hide();
            }
        }
    });
}

//绘制表
document.PaintTable = function (Id) {
    $("#" + Id).find("tr:visible").mouseover(function () {
        //如果鼠标移到class为stripe的表格的tr上时，执行函数    
        $(this).addClass("over");
    }).mouseout(function () {
        //给这行添加class值为over，并且当鼠标一出该行时执行函数    
        $(this).removeClass("over");
    }) //移除该行的class    
    $("#" + Id).find("tr:visible:even").addClass("alt");
    //给class为stripe的表格的偶数行添加class值为alt 
    //www.divcss5.com 整理特效 



}
 
String.prototype.startsWith = function (prefix) {
    return this.slice(0, prefix.length) === prefix;
};