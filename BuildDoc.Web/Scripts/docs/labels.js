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
        $('[name="FormatInfo"][format="' + val + '"]').show();
        $('[name="FormatInfo"][format!="' + val + '"]').hide();
    });
    $('#FormatType').change();
  
    //控件类型
    $('#ControlType').change(function () {
        var val = $(this).val();
        $('[name="LabelControl"][control="' + val + '"]').show();
        $('[name="LabelControl"][control!="' + val + '"]').hide();
    });
    $('#ControlType').change();

    //填充方式
    
   
    //设置公式
    $('#txtLabelFormula').click(function () {

        AlertDiv('#alert_Formula');
        var fields = [];

        $('#grid').find("td[title=\"labelName\"]").each(function () {
            fields.push($(this).text());
        })
        var feditor = new formualEditor(fields);

    });

    label.BindDataSource($('#sltDataSource'));
});

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
            $('#sltDisplayFields').empty();
            $('#sltFilterFields').empty();
            if (fields != null) {
                for (var i = 0; i < fields.length; i++) {
                    $('<option>', {
                        val: fields[i],
                        text: fields[i]
                    }).appendTo($('#sltDisplayFields'));

                    $('<option>', {
                        val: fields[i],
                        text: fields[i]
                    }).appendTo($('#sltFilterFields'));
                }
            }
        });
        $(sltSource).change();
    });
};



