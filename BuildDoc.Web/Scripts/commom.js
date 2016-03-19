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
