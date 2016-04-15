$(document).ready(function () { 
        var _move = false;//移动标记  
        var _x, _y;//鼠标离控件左上角的相对位置  
        $(".drag").click(function () {
            //alert("click");//点击（松开后触发）  
        }).mousedown(function (e) {
            _move = true;
            _x = e.pageX - parseInt($(".drag").css("left"));
            _y = e.pageY - parseInt($(".drag").css("top"));
            $(".drag").fadeTo(20, 0.5);//点击后开始拖动并透明显示  
        });
        $(document).mousemove(function (e) {
            if (_move) {
                var x = e.pageX - _x;//移动时根据鼠标位置计算控件左上角的绝对位置  
                var y = e.pageY - _y;
                $(".drag").css({ top: y, left: x });//控件新位置  
            }
        }).mouseup(function () {
            _move = false;
            $(".drag").fadeTo("fast", 1);//松开鼠标后停止移动并恢复成不透明  
        });
 


    //var Dragging = function (validateHandler) { //参数为验证点击区域是否为可移动区域，如果是返回欲移动元素，负责返回null
    //    var draggingObj = null; //dragging Dialog
    //    var diffX = 0;
    //    var diffY = 0;

    //    function mouseHandler(e) {
    //        switch (e.type) {
    //            case 'mousedown':
    //                draggingObj = validateHandler(e); //验证是否为可点击移动区域
    //                if (draggingObj != null) {
    //                    diffX = e.clientX - draggingObj.offsetLeft;
    //                    diffY = e.clientY - draggingObj.offsetTop;
    //                }
    //                break;

    //            case 'mousemove':
    //                if (draggingObj) {
    //                    draggingObj.style.left = (e.clientX - diffX) + 'px';
    //                    draggingObj.style.top = (e.clientY - diffY) + 'px';
    //                }
    //                break;

    //            case 'mouseup':
    //                draggingObj = null;
    //                diffX = 0;
    //                diffY = 0;
    //                break;
    //        }
    //    };

    //    return {
    //        enable: function () {
    //            $(document).bind("mousedown", mouseHandler);
    //            $(document).bind("mousemove", mouseHandler);
    //            $(document).bind("mouseup", mouseHandler);
    //            //document.addEventListener('mousedown',mouseHandler);
    //            //document.addEventListener('mousemove',mouseHandler);
    //            //document.addEventListener('mouseup',mouseHandler);
    //        },
    //        disable: function () {
    //            $(document).unbind("mousedown", mouseHandler);
    //            $(document).unbind("mousemove", mouseHandler);
    //            $(document).unbind("mouseup", mouseHandler);
    //            //document.removeEventListener('mousedown',mouseHandler);
    //            //document.removeEventListener('mousemove',mouseHandler);
    //            //document.removeEventListener('mouseup',mouseHandler);
    //        }
    //    };
    //};

    //function getDraggingDialog(e) {
    //    var target = e.target;
    //    while (target && target.className.indexOf('alertCT') == -1) {
    //        target = target.offsetParent;
    //    }
    //    if (target != null) {
    //        return target.offsetParent;
    //    } else {
    //        return null;
    //    }
    //}

    //Dragging(getDraggingDialog).enable();
});

//对话框
function AlertDiv(seletor, zindex) {
    $(seletor).css("display", "block");
    var y1 = $(window).height();
    var x1 = $(window).width();
    var y2 = $(seletor).find(".alertC").height();
    var x2 = $(seletor).find(".alertC").width();
    var y = (y1 - y2) / 2;
    var x = (x1 - x2) / 2;
    $(seletor).find(".alertC").css("top", y);
    $(seletor).find(".alertC").css("left", x);
    zindex = zindex ? zindex : 99999;
    $(seletor).css("z-index", zindex);
}

//对话框
function AlertClose(seletor) {
    $(seletor).closest(".alert").css("display", "none");
}
