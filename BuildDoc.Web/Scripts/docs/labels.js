var label = new Object();
label.DataSource = null;

$(function () {
  
    $('[name="GetDataMethod"]').hide();

    //数据源切换
    $('#DataMethod').change(function () {
        var val = $(this).val();
        $('[name="GetDataMethod"][method="' + val + '"]').show();
        $('[name="GetDataMethod"][method!="' + val + '"]').hide();
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
        $('[name="LabelFill"][control="'+ control +'"][fill="' + val + '"]').show();
        $('[name="LabelFill"][control="'+ control+'"][fill!="' + val + '"]').hide();
       
    });
    $('#FillType').change();

    //设置公式
    $('#txtLabelFormula').click(function () {
        AlertDiv('#alert_Formula');
        var fields = [];

        $('#grid').find("td[title=\"labelName\"]").each(function () {
            fields.push($(this).text());
        })
        var feditor = new formualEditor(fields);
    });

    //设置条件
    $('#AddConditionRow').click(function () {
        
    });

    label.BindDataSource($('#sltDataSource'));     
});

//绑定数据源
label.BindDataSource = function (sltSource) {
    $.getJSON("/DataSource/GetDataSource", function (datas) {
        label.DataSource = datas;
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
            label.BindFields($('#sltDisplayFields'), fields);
            label.BindFields($('#sltFilterFields'), fields);
            label.BindFields($('#sltFillDataSource'), fields);
            label.BindFields($('#sltFormatFields'), fields);
        });
        $(sltSource).change();
    });
};

//筛选数据源
label.BindFields = function (sltFields, fields) {
    if (fields != null) {
        for (var i = 0; i < fields.length; i++) {
            $('<option>', {
                val: fields[i],
                text: fields[i]
            }).appendTo($(sltFields));
        }
    }
}

//添加条件
label.AddConditionRow = function (table, info) {
    var mould = table.find("tr").last();
    //条件
    mould.find("td:eq(1)").click(function () {

    });

    //配置
    mould.find("td:eq(2)").click(function () {

    });
}

//弹出设置条件等式的界面
label.AlertCondition = function () {
    AlertDiv('#alert_Condition');
    var fields = new Array("估价目的", "估价对象类型");
   
}
