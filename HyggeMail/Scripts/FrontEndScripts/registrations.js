$(document).ready(function () {
    $("button#signUpBtn").live("click", function () {
        return User.Registration($(this));
    });
    $("button#signInBtn").live("click", function () {
        return User.Login($(this));
    });
    $('#loginForm input').keyup(function (event) {
        if (event.keyCode == 13) {
            return User.Login($(this));
        }
    });
    $('#registerForm input').keyup(function (event) {
        if (event.keyCode == 13) {
            return User.Registration($(this));
        }
    });
    $('#form-login input').keyup(function (event) {
        if (event.keyCode == 13) {
            return User.Login($(this));
        }
    });
    $("#forget").click(function () {
        User.SendResetLink();
    });
    $("#changePassword").click(function () {
        User.ChangePassword($(this));
    });
    $(".setpassword").click(function () {
        User.SetFacebookPassword($(this));
    });
    $(".next-div").click(function () {
        if ($('#firstStep').valid()) {
            $('#spanUsername').html("Hi " + $('#Step1_FullName').val() + "!");
            $(".modal-custom").addClass("step2");
        }
    });
    $("#myModal-terms .close").click(function () {
        $("body").addClass("modal-open");
    });
    $(document).on('click', '.agree-btn', function () {
        $('#Step2_Terms').prop('checked', true);
    });
    $(document).on('change', '#Step2_CountryID', function () {
        User.GetStateDDLByCountryID($(this));
    })
    $(document).on('change', '#Step2_StateID', function () {
        User.GetCitiesDDLByStateID($(this));
    })
    $(document).on('click', 'a#formSubmit', function () {
        if ($("#Step2_Terms").is(':checked')) {
            if ($('#stepTwo').valid()) {
                User.Registration();
            }
            $(".Termsloginerroemsg").html('');
        } else {
            $('#stepTwo').valid();
            $(".Termsloginerroemsg").html("Please accept Terms & Conditions");
        }
    })
    $(document).on('click', 'a#formLogin', function () {
        User.Login($(this));
    })
    $(document).on('click', 'a#formForgotPassword', function () {
        User.ForgotPassword($(this));
    })
    $(document).on('click', 'a#formRestorePassword', function () {
        User.SubmitResetPassword($(this));
    })
    $(document).on('click', 'a#mc-embedded-subscribe', function () {
        User.GetGuide($(this));
    })
    $(document).on('click', 'a#btnSubscripeSubmit', function () {
        User.SubmitSubscriber($(this));
    })
    $(document).on('click', '.forgotpass', function () {
        $('.loginmodal').modal('hide');
    });
    $(document).on('click', '.signuphere', function () {
        $('#myModal').modal('hide');
        $('.loginmodal').modal('show');
    });
    $(document).on('click', '.login-btn', function () {
        $('.loginmodal').modal('show');
    });
});
var User = {
    GetCountryList: function () {
        $.ajaxExt({
            type: "POST",
            validate: false,
            messageControl: null,
            showThrobber: false,
            url: baseUrl + SiteURL.CountryList,
            success: function (results, message, status, id, list) {
                var markup = "<option value='0'>Select Country </option>";
                for (var x = 0; x < list.length; x++) {
                    markup += "<option value=" + list[x].Value + ">" + list[x].Text + "</option>";
                }
                $("#Step2_CountryID").find('option').remove();
                $("#Step2_CountryID").append(markup);
            },
            error: function () {
                var error = "<option value='0'>Data not found</option>";
                $("#Step2_CountryID").append(error);
            }
        });
    },
    GetStateDDLByCountryID: function (sender) {
        var id = $(sender).val();
        var procemessage = "<option value='0'> Please wait...</option>";
        $("#Step2_StateID").html(procemessage).show();
        //$.ajaxExt({
        //    type: "POST",
        //    validate: false,
        //    data: {
        //        country: id
        //    },
        //    messageControl: null,
        //    showThrobber: false,
        //    url: baseUrl + SiteURL.StateByCountry,
        //    success: function (results, message, status, id, list) {
        //        var markup = "<option value='0'>Select State </option>";
        //        for (var x = 0; x < list.length; x++) {
        //            markup += "<option value=" + list[x].Value + ">" + list[x].Text + "</option>";
        //        }
        //        $("#Step2_StateID").find('option').remove();
        //        $("#Step2_StateID").append(markup);
        //    },
        //    error: function () {
        //        $('#gritter-notice-wrapper').hide();
        //        var error = "<option value='0'>Data not found</option>";
        //        $("#Step2_StateID").append(error);
        //    }
        //});
    },
    GetGuide: function (sender) {
        $.ajaxExt({
            url: baseUrl + SiteURL.GetGuide,
            type: 'POST',
            validate: true,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            formToValidate: $(sender).parents("form:first"),
            showThrobber: true,
            button: $(sender),
            throbberPosition: {
                my: "left center",
                at: "right center",
                of: $(sender)
            },
            data: $(sender).parents("form:first").serializeArray(),
            success: function (results, message, status, data) {
                //$.ShowMessage($('div.messageAlert'), message, status);
                var message = "<p>Yay, all done! Check your inbox. We have sent you an email with a link to download Hygge - The Danish Guide to Happiness.Have you not received your email? Please email us at <a href='#' >friends@hyggemail.com</a> and we will fix that for you as quickly as we can.</p>";
                swal({
                    title: "Guide Sent Successfully.",
                    text: message,
                    type: "success",
                    showCancelButton: false,
                    confirmButtonClass: "btn-danger",
                    confirmButtonText: "Ok",
                    closeOnConfirm: false,
                    html: true
                });
                $("#mce-EMAIL").val('');
                //if (status == ActionStatus.Successfull) {
                //    setTimeout(function () {
                //        window.location.href = SiteURL.HomePage;
                //    }, 2000);
                //}
            }
        });
    },
    GetCitiesDDLByStateID: function (sender) {
        var id = $(sender).val();
        var procemessage = "<option value='0'> Please wait...</option>";
        $("#Step2_CityID").html(procemessage).show();
        //$.ajaxExt({
        //    type: "POST",
        //    validate: false,
        //    parentControl: $(sender).parents("form:first"),
        //    data: {
        //        state: id
        //    },
        //    messageControl: null,
        //    showThrobber: false,
        //    throbberPosition: {
        //        my: "left center",
        //        at: "right center",
        //        of: sender,
        //        offset: "5 0"
        //    },
        //    url: baseUrl + SiteURL.CityByState,
        //    success: function (results, message, status, id, list) {
        //        var markup = "<option value='0'>Select City </option>";
        //        for (var x = 0; x < list.length; x++) {
        //            markup += "<option value=" + list[x].Value + ">" + list[x].Text + "</option>";
        //        }
        //        $("#Step2_CityID").find('option').remove();
        //        $("#Step2_CityID").append(markup);
        //    },
        //    error: function () {
        //        $('#gritter-notice-wrapper').hide();
        //        var error = "<option value='0'>Data not found</option>";
        //        $("#Step2_CityID").append(error);
        //    }
        //});
    },
    ForgotPassword: function (sender) {
        $.ajaxExt({
            url: baseUrl + SiteURL.ForgetPassword,
            type: 'POST',
            validate: true,
            showErrorMessage: false,
            messageControl: $('div.messageAlert'),
            formToValidate: $(sender).parents("form:first"),
            showThrobber: false,
            button: $(sender),
            throbberPosition: {
                my: "left center",
                at: "right center",
                of: $(sender)
            },
            data: $(sender).parents("form:first").serializeArray(),
            success: function (results, message, status) {
                $('.alert').show();
                $(".statusMessage").html(results || message);
                if (status == ActionStatus.Successfull) {
                    setTimeout(function () {
                        window.location.reload();
                    }, 2000);
                }
            },
            error: function (results, message) {
                $('.alert').show();
                $('.alert').addClass('alert-danger');
                $(".statusMessage").html(results || message);
                setTimeout(function () {
                    $(".alert").fadeOut();
                    $('.alert').removeClass('alert-danger');
                    $(".statusMessage").html('');
                }, 6000);
            }
        });
        return false;
    },
    SubmitResetPassword: function (sender) {
        $.ajaxExt({
            url: baseUrl + SiteURL.SubmitResetPassword,
            type: 'POST',
            validate: true,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            formToValidate: $(sender).parents("form:first"),
            showThrobber: true,
            button: $(sender),
            throbberPosition: {
                my: "left center",
                at: "right center",
                of: $(sender)
            },
            data: $(sender).parents("form:first").serializeArray(),
            success: function (results, message, status, data) {
                $.ShowMessage($('div.messageAlert'), message);
                if (status == ActionStatus.Successfull) {
                    setTimeout(function () {
                        window.location.href = SiteURL.HomePage;
                    }, 2000);
                }
            }
        });
        return false;
    },
    SubmitSubscriber: function (sender) {
        $.ajaxExt({
            url: baseUrl + SiteURL.SubmitSubscriber,
            type: 'POST',
            validate: true,
            showErrorMessage: false,
            messageControl: $('div.messageAlert'),
            formToValidate: $(sender).parents("form:first"),
            showThrobber: true,
            button: $(sender),
            throbberPosition: {
                my: "left center",
                at: "right center",
                of: $(sender)
            },
            data: $(sender).parents("form:first").serializeArray(),
            success: function (results, message, status, data) {
                // $.ShowMessage($('div.messageAlert'), message, status);
                var message = "<p>Thanks for choosing to sign up to the HyggeMail newsletter. You will get an email every now and then with some great ways to make the most of HyggeMail, awesome exclusive offers, and tips on how to enjoy the little things in life!</p>";
                swal({
                    title: "Subscription is done Successfully.",
                    text: message,
                    type: "success",
                    showCancelButton: false,
                    confirmButtonClass: "btn-danger",
                    confirmButtonText: "Ok",
                    closeOnConfirm: false,
                    html: true
                });
                $("#EmailID").val('');
                //if (status == ActionStatus.Successfull) {
                //    setTimeout(function () {
                //        window.location.href = SiteURL.HomePage;
                //    }, 2000);
                //}
            }, error: function () {
                var message = "<p>This Subscriber email already exists and is also not marked as deleted.</p>";
                swal({
                    title: "",
                    text: message,
                    type: "error",
                    showCancelButton: false,
                    confirmButtonClass: "btn-danger",
                    confirmButtonText: "Ok",
                    closeOnConfirm: false,
                    html: true
                });
            }
        });
        return false;
    },
    Registration: function (sender) {
        $.ajaxExt({
            url: baseUrl + '/Home/SignUp',
            type: 'POST',
            messageControl: $('div.messageAlert'),
            showThrobber: false,
            showErrorMessage: false,
            button: $(sender),
            data: $('form').serializeArray(),
            success: function (results, message) {
                $('#myModal').modal('hide');
                var message = "<p>Hygge Mail has sent you a verification email with a link. Please click that link to complete your registration.</p>";
                swal({
                    title: "Registration is Almost Complete.",
                    text: message,
                    type: "success",
                    showCancelButton: false,
                    confirmButtonClass: "btn-danger",
                    confirmButtonText: "Ok",
                    closeOnConfirm: false,
                    html: true
                }, function () {
                    window.location.reload();
                });
            },
            error: function (results, message) {
                $('.alert').show();
                $('.alert').addClass('alert-danger');
                $(".statusMessage").html(results || message);
                setTimeout(function () {
                    $(".alert").fadeOut();
                    $('.alert').removeClass('alert-danger');
                    $(".statusMessage").html('');
                }, 6000);
            }
        });
        return false;
    },
    Login: function (sender) {
        $.ajaxExt({
            url: baseUrl + SiteURL.UserLogin,
            type: 'POST',
            validate: true,
            showErrorMessage: false,
            messageControl: $('div.messageAlert'),
            formToValidate: $(sender).parents("form:first"),
            showThrobber: false,
            button: $(sender),
            throbberPosition: {
                my: "left center",
                at: "right center",
                of: $(sender)
            },
            data: $(sender).parents("form:first").serializeArray(),
            success: function (results, message) {

                var returnUrl = localStorage.getItem("EditCardReturnUrl");

                if (returnUrl != undefined) {
                    localStorage.removeItem('EditCardReturnUrl');
                    window.location.href = returnUrl;
                }
                else
                    window.location.href = baseUrl + SiteURL.UserDashboard;
            },
            error: function (results, message) {
                $('.alert').show();
                $('.alert').addClass('alert-danger');
                $(".statusMessage").html(results || message);
                setTimeout(function () {
                    $(".alert").fadeOut("slow");
                    $('.alert').removeClass('alert-danger');
                    $(".statusMessage").html('');
                }, 6000);
            }
        });
        return false;
    },
    SendResetLink: function () {
        if ($("#Email").val() == "" || $("#Email").val() == undefined) {
            $.ShowMessage($('div.messageAlert'), "Please enter a username.", MessageType.Error);
        } else {
            $.ajaxExt({
                url: baseUrl + '/Home/ForgetPassword',
                type: 'POST',
                validate: false,
                showErrorMessage: true,
                messageControl: $('div.messageAlert'),
                showThrobber: true,
                throbberPosition: {
                    my: "left center",
                    at: "right center",
                    of: $(window)
                },
                data: {
                    email: $("#Email").val()
                },
                success: function (results, message) {
                    $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                }
            });
        }
    },
    ChangePassword: function (sender) {
        $.ajaxExt({
            url: baseUrl + '/Home/ChangeUserPassword',
            type: 'POST',
            validate: true,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            formToValidate: $(sender).parents("form:first"),
            showThrobber: true,
            button: $(sender),
            throbberPosition: {
                my: "left center",
                at: "right center",
                of: $(sender)
            },
            data: $(sender).parents("form:first").serializeArray(),
            success: function (results, message) {
                $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                setTimeout(function () {
                    if ($(sender).attr("isAdmin") == "True") window.location.assign(baseUrl + '/Admin');
                    else
                        window.location.assign(baseUrl + '/Home/Index');
                }, 2000);
            }
        });
        return false;
    }
};