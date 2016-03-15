$(function () {

    $('[name="GetDataMethod"]').hide();

    $('#DataMethod').change(function () {
        var val = $(this).val();
        $('[name="GetDataMethod"][method="' + val + '"]').show();
        $('[name="GetDataMethod"][v!="' + val + '"]').hide();
    });

    $('#DataMethod').change();
})