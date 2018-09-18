$(".input-number").keypress(function (evt) {
    evt.preventDefault();
});


$('.btn-number').click(function (e) {

    var incrementValue = parseFloat(0);
    var isNumericValue = true;
    e.preventDefault();

    fieldName = $(this).attr('data-field');
    type = $(this).attr('data-type');
    var input = $("input[name='" + fieldName + "']");
    var currentVal = parseFloat(input.val());

    var step = $(input).attr('step');

    if (!isNaN(step)) {
        incrementValue = parseFloat(step);
        isNumericValue = false;
    }
    else
        incrementValue = parseInt(1);

    if (!isNaN(currentVal)) {
        if (type == 'minus') {

            if (currentVal > input.attr('min')) {

                if (isNumericValue)
                    input.val(parseInt(currentVal - incrementValue).toFixed(0)).change();
                else
                    input.val(parseFloat(currentVal - incrementValue).toFixed(1)).change();
            }
            if (parseInt(input.val()) == input.attr('min')) {
                // $(this).attr('disabled', true);
            }

        } else if (type == 'plus') {

            if (currentVal < input.attr('max')) {
                if (isNumericValue)
                    input.val(parseInt(currentVal + incrementValue).toFixed(0)).change();
                else
                    input.val(parseFloat(currentVal + incrementValue).toFixed(1)).change();
            }
            if (parseInt(input.val()) == input.attr('max')) {
                // $(this).attr('disabled', true);
            }

        }
    } else {
        input.val(0);
    }
});
$('.input-number').focusin(function () {
    $(this).data('oldValue', $(this).val());
});
$('.input-number').change(function () {

    minValue = parseInt($(this).attr('min'));
    maxValue = parseInt($(this).attr('max'));
    valueCurrent = parseInt($(this).val());

    name = $(this).attr('name');
    if (valueCurrent >= minValue) {
        $(".btn-number[data-type='minus'][data-field='" + name + "']").removeAttr('disabled')
    } else {

        $(this).val($(this).data('oldValue'));
    }
    if (valueCurrent <= maxValue) {
        $(".btn-number[data-type='plus'][data-field='" + name + "']").removeAttr('disabled')
    } else {

        $(this).val($(this).data('oldValue'));
    }


});
$(".input-number").keydown(function (e) {
    // Allow: backspace, delete, tab, escape, enter and .
    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 190]) !== -1 ||
        // Allow: Ctrl+A
        (e.keyCode == 65 && e.ctrlKey === true) ||
        // Allow: home, end, left, right
        (e.keyCode >= 35 && e.keyCode <= 39)) {
        // let it happen, don't do anything
        return;
    }
    // Ensure that it is a number and stop the keypress
    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
        e.preventDefault();
    }
});