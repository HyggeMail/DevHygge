
$(document).ready(function () {
    $(document).on('click', '.rec-edit-a', function () {
        Recipient.BindRecipientModel($(this).data('id'));
    })
    $(document).on('click', '.rec-delete-a', function () {
        var sender = $(this).data('id');
        $.ConfirmBox("", "Are you sure to delete this record?", null, true, "Yes", true, sender, function () {
            Recipient.DeleteRecipientByID(sender);
        });
    });
    //$(document).on('change', '#Country', function () {

    //    Recipient.GetStateDDLByCountryID($(this));
    //})
    $(document).on('click', '#add-rec', function () {
        $('#RecipientForm').find("input[type=text], textarea, select").val("");
        var markup = "<option value='0'>Select City </option>";
        $("#State").find('option').remove();
        $("#State").append(markup);
        $("#City").find('option').remove();
        $("#City").append(markup);
        $("#myModalLabel").text("Add New Recipient");
    })

    //$(document).on('change', '#State', function () {
    //    Recipient.GetCitiesDDLByStateID($(this));
    //});
    $(document).on('keyup', '#searchkeyword', function (e) {

        var key = e.which;
        if (key == 13)  // the enter key code
        {
            return Recipient.GetRecipients($(this).val());
        }

    });
    $(document).on('click', '#btnSearch', function (e) {
        return Recipient.GetRecipients($('#searchkeyword').val());
    });
    $("input[type=button]#addRecipient").live("click", function () {
        Recipient.AddEditRecipient($(this).val());
    });
    $('#AddToBookCheck').hide();

}); var Recipient = {

    BindRecipientModel: function (id) {
        $.ajaxExt({
            type: "POST",
            validate: false,
            data: { id: id },
            messageControl: null,
            showThrobber: false,
            url: baseUrl + siteURL.GetRecipientByID,
            success: function (results, message, status, id, list) {
                if (status == ActionStatus.Successfull) {
                    debugger
                    $('#AddUpdateRecipients').html(results[0]);
                    $('#myModal-AddRecipient').modal('show');
                    $("#myModalLabel").text("Edit Recipient");
                    $('#Address').locify()
                }
            }
        });
    },

    DeleteRecipientByID: function (id) {
        $.ajaxExt({
            type: "POST",
            validate: false,
            data: { id: id },
            messageControl: null,
            showThrobber: false,
            url: baseUrl + siteURL.DeleteRecipientByID,
            success: function (results, message, status, id, list) {
                //$.ShowMessage($('div.messageAlert'), message, status);
                $('.alert').show();
                $(".statusMessage").html(results || message);
                if (status == ActionStatus.Successfull) {
                    setTimeout(function () {
                        $('.alert').hide()
                        $(".statusMessage").html('');
                        Recipient.GetRecipients('');
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

    AddEditRecipient: function (sender) {
        var form = $("#RecipientForm");
        $.ajaxExt({
            url: baseUrl + siteURL.AddEditRecipient,
            type: 'POST',
            validate: true,
            formToValidate: form,
            formToPost: form,
            isAjaxForm: true,
            showThrobber: false,
            showErrorMessage: false,
            messageControl: $('div.messageAlert'),
            success: function (results, message, status) {
               // $.ShowMessage($('div.messageAlert'), message, status);
                if (status == ActionStatus.Successfull) {
                    $('.rec-cancel').click()
                    window.scrollTo(0, 0);
                    $('.alert').show();
                    $(".statusMessage").html(results || message);
                    setTimeout(function () {
                         $(".alert").fadeOut();
                        $(".statusMessage").html('');
                        Recipient.GetRecipients('');
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
        return false;
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
                $("#State").find('option').remove();
                $("#State").append(markup);
            }, error: function () {
                var error = "<option value='0'>Data not found</option>";
                $("#State").append(error);

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
                $("#City").find('option').remove();
                $("#City").append(markup);
            }, error: function () {
                var error = "<option value='0'>Data not found</option>";
                $("#City").append(error);

            }
        });
    },

    GetRecipients: function (sender) {
        $('.load-rec').show();
        $.ajaxExt({
            type: "POST",
            validate: false,
            data: { keyword: sender },
            messageControl: null,
            showThrobber: false,
            url: baseUrl + siteURL.GetRecipientList,
            success: function (results, message) {
                $('#Recipients').html(results[0]);
            }
        });
    },


};