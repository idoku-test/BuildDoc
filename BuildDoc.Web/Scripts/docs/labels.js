$(function () {

    $('[name*="GetDataMethod_"]').hide();

    $('#GetDataMethod').change(function () {
        var val = $(this).find("option:selected").text();;
        $('[name="GetDataMethod_' + val + '"]').show();
    });
})