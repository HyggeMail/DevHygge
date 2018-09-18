
var sID, cID = 0;
$(document).ready(function () {
    $(document).on('click', '#formProfileUpdate', function () {
        Profile.UpdateProfileDetails();
    });

    $(document).on('click', '#savePassword', function () {
        Profile.UpdatePassword();
    });

    //$(document).on('change', '#CountryID', function () {
    //    Profile.GetStateDDLByCountryID($(this));
    //})
    //$(document).on('change', '#StateID', function () {
    //    Profile.GetCitiesDDLByStateID($(this));
    //})
    $(document).on('change', '.change-check', function () {
        Profile.NotificationSetting($(this));
    })
    sID = $('#StateID').val();
    cID = $('#CityID').val();
    //$('#CountryID').trigger('change');
    $(document).on('change', '#image', function () {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#image_Preview').attr('src', e.target.result);
            };
            reader.readAsDataURL(input.files[0]);
        }
    })
});
var Profile = {
    UpdateProfileDetails: function () {
        var form = $("#formProfile");
        $.ajaxExt({
            url: baseUrl + siteURL.UpdateProfileDetails,
            type: 'POST',
            validate: true,
            formToValidate: form,
            formToPost: form,
            isAjaxForm: true,
            showThrobber: false,
            showErrorMessage: false,
            messageControl: $('div.messageAlert'),
            success: function (results, message, status) {
                //$.ShowMessage($('div.messageAlert'), message, status);
                $('.alert').show();
                $(".statusMessage").html(results || message);
                if (status == ActionStatus.Successfull) {
                    setTimeout(function () {
                         $(".alert").fadeOut();
                        $(".statusMessage").html('');
                        window.location.reload();
                    }, 2000);
                }
            }, error: function (results, message) {
                $('.alert').show();
                $('.alert').addClass('alert-danger');
                $(".statusMessage").html(results || message);
                setTimeout(function () {
                     $(".alert").fadeOut();
                    $('.alert').removeClass('alert-danger');
                    $(".statusMessage").html('');
                }, 2000);
            }
        });
    },
    UpdatePassword: function () {
        var form = $("#formChangePassword");
        $.ajaxExt({
            url: baseUrl + siteURL.UpdatePassword,
            type: 'POST',
            validate: true,
            formToValidate: form,
            formToPost: form,
            isAjaxForm: true,
            showThrobber: false,
            showErrorMessage: false,
            messageControl: $('div.messageAlert'),
            success: function (results, message, status) {
                //$.ShowMessage($('div.messageAlert'), message, status);
                $('.alert').show();
                $(".statusMessage").html(results || message);
                if (status == ActionStatus.Successfull) {
                    setTimeout(function () {
                         $(".alert").fadeOut();
                        $(".statusMessage").html('');
                        window.location.href = siteURL.Signout;
                    }, 2000);
                }
            }, error: function (results, message) {
                $('.alert').show();
                $('.alert').addClass('alert-danger');
                $(".statusMessage").html(results || message);
                setTimeout(function () {
                     $(".alert").fadeOut();
                    $('.alert').removeClass('alert-danger');
                    $(".statusMessage").html('');
                }, 2000);
            }
        });
    },
    NotificationSetting: function () {
        var form = $("#formMySettings");
        $.ajaxExt({
            url: baseUrl + siteURL.UpdateNotificationSettings,
            type: 'POST',
            validate: true,
            formToValidate: form,
            formToPost: form,
            isAjaxForm: true,
            showThrobber: false,
            showErrorMessage: false,
            messageControl: $('div.messageAlert'),
            success: function (results, message, status) {
                //$.ShowMessage($('div.messageAlert'), message, status);
                $('.alert').show();
                $(".statusMessage").html(results || message);
                setTimeout(function () {
                     $(".alert").fadeOut();
                    $(".statusMessage").html('');                 
                }, 2000);
            }, error: function (results, message) {
                $('.alert').show();
                $('.alert').addClass('alert-danger');
                $(".statusMessage").html(results || message);
                setTimeout(function () {
                     $(".alert").fadeOut();
                    $('.alert').removeClass('alert-danger');
                    $(".statusMessage").html('');
                }, 2000);
            }
        });
    },
    GetStateDDLByCountryID: function (sender) {

        var id = $(sender).val();
        var procemessage = "<option value='0'> Please wait...</option>";
        $("#StateID").html(procemessage).show();
        $.ajaxExt({
            type: "POST",
            validate: false,
            data: { country: id },
            messageControl: null,
            showThrobber: false,
            url: baseUrl + siteURL.StateByCountry,
            success: function (results, message, status, id, list) {

                var markup = "<option value='0'>Select State </option>";
                for (var x = 0; x < list.length; x++) {
                    markup += "<option value=" + list[x].Value + ">" + list[x].Text + "</option>";
                }
                $("#StateID").find('option').remove();
                $("#StateID").append(markup);

                if (parseInt(sID) > 0) {
                    $('#StateID').val(sID);
                    $('#StateID').trigger('change');
                    $('#StateID').val(sID);
                    sID = 0;
                }
            }, error: function () {
                var error = "<option value='0'>Data not found</option>";
                $("#StateID").append(error);

            }
        });
    },

    GetCitiesDDLByStateID: function (sender) {
        var id = $(sender).val();
        var procemessage = "<option value='0'> Please wait...</option>";
        $("#City").html(procemessage).show();
        $.ajaxExt({
            type: "POST",
            validate: false,
            parentControl: $(sender).parents("form:first"),
            data: { state: id },
            messageControl: null,
            showThrobber: false,
            throbberPosition: { my: "left center", at: "right center", of: sender, offset: "5 0" },
            url: baseUrl + siteURL.CityByState,
            success: function (results, message, status, id, list) {

                var markup = "<option value='0'>Select City </option>";
                for (var x = 0; x < list.length; x++) {
                    markup += "<option value=" + list[x].Value + ">" + list[x].Text + "</option>";
                }
                $("#CityID").find('option').remove();
                $("#CityID").append(markup);
                $('#CityID').val(cID);
            }, error: function () {
                var error = "<option value='0'>Data not found</option>";
                $("#CityID").append(error);

            }
        });
    },


}