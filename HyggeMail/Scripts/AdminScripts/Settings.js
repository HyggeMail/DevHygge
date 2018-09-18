$(document).ready(function () {

    $(document).on("click", "#btnUpdatePassword", function () {
        MyAccount.ChangePassword($(this));
    });
});
var MyAccount = {

    ChangePassword: function (sender) {
        $.ajaxExt({
            url: baseUrl + siteURL.ChangePassword,
            type: 'POST',
            validate: true,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            formToValidate: $(sender).parents("form:first"),
            formToPost: $(sender).parents("form:first"),
            isAjaxForm: true,
            showThrobber: true,
            button: $(sender),
            throbberPosition: { my: "left center", at: "right center", of: $(sender) },
            data: $(sender).parents("form:first").serializeArray(),
            success: function (results, message, status) {
                $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                if (status == ActionStatus.Successfull) {
                    setInterval(function () {
                        window.location.href = baseUrl + siteURL.Login;
                    }, 3000);
                }
            }
        });
        return false;
    }
}