var cTop = 145;
var urecipientID = 1;
var RecipientList = { List: [] };
var freeSpaceForRecipient = [];
$(document).ready(function () {
    $("input[type=button]#addRecipient").live("click", function () {

        Recipient.AddEditRecipient($(this).val());
    });

    $(document).on('click', '.rec-edit-a', function () {
        Recipient.BindRecipientModel($(this).data('id'));
    })
    $(document).on('click', '.rec-cancel', function () {
        Recipient.ResetRecipientForm();
    })
    $(document).on('click', '.add-recipient', function () {
        $('#AddToBookCheck').show();
        Recipient.ResetRecipientForm();
    })

    $("#adminreceipient").click(function () {
        $(".add-recipient").hide();
    })

    $("#userreceipient").click(function () {
        $(".add-recipient").show();
    })

    Recipient.GetRecipients('');

    $("#stepthird").click(function () {
        $("#addressdiv").show();
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
    //$(document).on('change', '#State', function () {
    //    Recipient.GetCitiesDDLByStateID($(this));
    //})

    $(document).on('keyup', '#searchRecipient', function (e) {

        var key = e.which;
        if (key == 13)  // the enter key code
        {
            return Recipient.GetRecipients($(this).val());
        }

    });

    $(document).on('keyup', '#searchAddressBook', function (e) {

        var key = e.which;
        if (key == 13)  // the enter key code
        {
            return Recipient.GetAddressBook($(this).val());
        }

    });

    $(document).on('change', '.recipient-a-select', function () {
        var Isrecepientexist = false;
        for (var i = 0; i < RecipientList.List.length; i++) {
            if ($(this).attr('data-id') == RecipientList.List[i].ID && $(this).is(':checked')) {
                Isrecepientexist = true;
            }
        }
        if (!Isrecepientexist)
            fillRecipientOnCanvas(this);
        else
            return;

    });

    OpenRecipientPopBox();
});

$(document).on('click', '.addressline', function () {
    var input = $(this).parent().parent().find('input');
    var name = input.attr('data-nm');
    var city = input.attr('data-ct');
    var country = input.attr('data-co');
    var address = input.attr('data-ad');
    var state = input.attr('data-st');
    var zip = input.attr('data-zp');
    var keyword = input.attr('data-keyword');
    var agency = input.attr('data-agen');
    var image = input.attr('data-img');
    var category = "<span>" + input.attr('data-cat') + "</span>";
    var subcategory = input.attr('data-subcat');
    var htmlstring = "<span class='verified'><sub><i class='fa fa-check' aria-hidden='true'></i></sub> Address verified</span>"
    $("#name").html(name + category);
    $("#famousfor").text(keyword);
    $("#agency").text(agency);
    $("#address").html(address + ',' + city + ',' + state + ',' + country + htmlstring)
    $('#img').css('backgroundImage', 'url(' + image + ')');
})

function OpenRecipientPopBox() {

    var recTop = cTop;
    for (var i = 0; i < 6; i++) {
        var recSpace = cTop;
        var rect = new fabric.Rect({
            left: 450,
            top: recSpace,
            fill: '#fff',
            width: 375,
            height: 17,
            angle: 0,
            id: 0,
            selectable: false,
            lockRotation: true,
            opacity: 0.0
        });


        canvasBack.add(rect);
        canvasBack.renderAll();
        cTop += 45;
        recipientcanvasgroup.push(rect);

        if (i == 5) {
            cTop = recTop;
        }
    }
}


function fillRecipientOnCanvas(sender) {
    var record = {};
    record.Name = $(sender).data('nm');
    record.ID = $(sender).data('id');
    record.Zip = $(sender).data('zp');
    record.Address = $(sender).data('ad');
    record.Country = $(sender).data('co');
    record.City = $(sender).data('ct');
    record.State = $(sender).data('st');
    if ($(sender).is(":checked")) {
        Recipient.addRecipientOnCanvas(record);
    }
    else {
        Recipient.removeRecipientOnCanvas(record);
    }
}

var Recipient = {
    ResetRecipientForm: function () {
        $('#RecipientForm').find("input[type=text], textarea, select").val("");
    },
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
                    $('#AddUpdateRecipients').html(results[0]);
                    $('#myModal-AddRecipient').modal('show');
                    $('#AddToBookCheck').hide();
                }
            }
        });
    },

    DeleteRecipientByID: function (id) {
        $.ajaxExt({
            type: "POST",
            validate: false,
            global: false,
            data: { id: id },
            messageControl: null,
            showThrobber: false,
            url: baseUrl + siteURL.DeleteRecipientByID,
            success: function (results, message, status, id, List, Object) {
                //$.ShowMessage($('div.messageAlert'), message, status);
                $('.alert').show();
                $(".statusMessage").html(results || message);
                if (status == ActionStatus.Successfull) {
                    //$('.rec-cancel').click()
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
    },

    AddEditRecipient: function (sender) {


        if ($('#RecipientForm').find('input[name="ID"]').val() != undefined) {
            if ($('#RecipientForm').find('input[name="ID"]').val() > 0) {
                Recipient.UncheckRecipientFromListWhileEditing($('#RecipientForm').find('input[name="ID"]').val());
            }
        }


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
            success: function (results, message, status, id, List, Object) {
                //$.ShowMessage($('div.messageAlert'), message, status);
                $('.rec-cancel').click();
                $('.alert').show();
                $(".statusMessage").html(results || message);
                if (status == ActionStatus.Successfull) {
                    //var record = {};
                    //record.Name = Object.Name;
                    //record.Zip = Object.Zip;
                    //record.ID = Object.ID;
                    //record.Country = Object.Country;
                    //record.Address = Object.Address;
                    //record.State = Object.State;
                    //record.City = Object.City;
                    //Recipient.addRecipientOnCanvas(record);
                    setTimeout(function () {
                        $(".alert").fadeOut();
                        $(".statusMessage").html('');
                        Recipient.GetRecipients('', Object.ID);
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

    GetRecipients: function (sender, newObjectId) {
        $('.load-rec').show();
        $.ajaxExt({
            type: "POST",
            global: false,
            cache: false,
            validate: false,
            data: { keyword: sender },
            messageControl: null,
            showThrobber: false,
            url: baseUrl + siteURL.GetPostCardUserRecipients,
            success: function (results, message) {
                $('#Recipients').html(results[0]);
                $('.recipient-a-select').each(function () {
                    if (newObjectId == $(this).attr("data-id")) {
                        $(this).attr('checked', true);
                    }

                    if ($(this).is(':checked')) {
                        fillRecipientOnCanvas(this);
                    }
                });
                $('.load-rec').hide();
                //$('#recentlyUsed').html(results[0]);
                //Recipient.AccordionInitilize();
                $(".useradd").smk_Accordion({
                    closeAble: true, //boolean
                });
                Binding();
            }
        });
    },

    GetAddressBook: function (sender, newObjectId) {
        $('.load-rec').show();
        $.ajaxExt({
            type: "POST",
            global: false,
            cache: false,
            validate: false,
            data: { keyword: sender },
            messageControl: null,
            showThrobber: false,
            url: baseUrl + siteURL.GetAdminAddressBook,
            success: function (results, message) {
                $('#FavouriteRecipients').html(results[0]);
                //$('.recipient-a-select').each(function () {
                //    if (newObjectId == $(this).attr("data-id")) {
                //        $(this).attr('checked', true);
                //    }

                //    if ($(this).is(':checked')) {
                //        fillRecipientOnCanvas(this);
                //    }
                //});
                $('.load-rec').hide();
                //$('#recentlyUsed').html(results[0]);
                //Recipient.AccordionInitilize();
                //$(".adminadd").smk_Accordion({
                //    closeAble: true, //boolean
                //});
                Binding();
            }
        });
    },

    AccordionInitilize: function () {
        $(".acc-Recipients").smk_Accordion({
            closeAble: true, //boolean
        });
    },

    addRecipientOnCanvas: function (record) {
        var curSpace = cTop;
        freeSpaceForRecipient = freeSpaceForRecipient.sort();
        if (freeSpaceForRecipient.length > 0) {
            curSpace = freeSpaceForRecipient[0];
            freeSpaceForRecipient.splice(0, 1)
        }

        var txt = new fabric.IText(record.Name, {
            left: 480,
            top: curSpace,
            id: record.ID,
            fontFamily: 'helvetica neue',
            selectable: false,
            lockRotation: true,
            fill: '#000',
            stroke: '#fff',
            strokeWidth: .1,
            fontSize: 24,
            hoverCursor: 'pointer'
        });

        txt.hasControls = false;
        // txt.setControlsVisibility(HideControls);
        canvasBack.add(txt);
        canvasBack.renderAll();
        cTop += 40;
        recipientcanvasgroup.push(txt);
        RecipientList.List.push(record);
    },
    removeRecipientOnCanvas: function (record) {
        for (var i = 0; i < recipientcanvasgroup.length; i++) {
            if (record.ID == recipientcanvasgroup[i].id) {
                canvas.remove(recipientcanvasgroup[i]);
                freeSpaceForRecipient.push(recipientcanvasgroup[i].top);
                recipientcanvasgroup.remove(recipientcanvasgroup[i]);
                cTop -= 40;

            }
            updateModifications(true);
            canvas.counter++;
        }
        for (var i = 0; i < RecipientList.List.length; i++) {
            if (record.ID == RecipientList.List[i].ID) {
                RecipientList.List.splice(0, 1);
            }
        }
    },
    removeRecipientsFromCanvas: function () {
        debugger
        var length = recipientcanvasgroup.length;
        //for (let i in recipientcanvasgroup) {
        //    canvas.remove(recipientcanvasgroup[i]);
        //}
        if (recipientcanvasgroup && recipientcanvasgroup.length > 0) {
            for (var i = 0; i < recipientcanvasgroup.length; i++) {
                canvas.remove(recipientcanvasgroup[i]);
            }
        }
    },
    ShowRecipientDetailsOnHover: function (id) {
        debugger
        var record = {};
        for (var i = 0; i < RecipientList.List.length; i++) {
            if (id == RecipientList.List[i].ID) {
                record.Name = RecipientList.List[i].Name;
                record.Zip = RecipientList.List[i].Zip;
                record.ID = RecipientList.List[i].ID;
                record.Country = RecipientList.List[i].Country;
                record.Address = RecipientList.List[i].Address;
                record.State = RecipientList.List[i].State;
                record.City = RecipientList.List[i].City;
            }
        }
        //return record;
        var details = '<p>' + record.Name + '</p><br/><p>' + record.Address + '</p><p>' + record.City + '</p><p>' + record.State + '</p><p>' + record.Country + '</p><p>' + record.Zip + '</p><br/>';
        var editButton = '<a href="javascript:" class="rec-edit-a" data-id="' + record.ID + '" title="Edit"> <img src="~/Content/images/edit-icon1.png" alt="" /></a>';
        var deleteButton = '<a href="javascript:" class="rec-delete-a" data-id="' + record.ID + '" title="Edit"> <img src="~/Content/images/delete-icon.png" alt="" /></a>';
        //$('.rec-hoverd-details').html(details + editButton + deleteButton);
        $('.rec-hoverd-details').html(details);
        $('.rec-hoverd-details').addClass('rec-hoverd-details-p');
        $('.rec-hoverd-details').fadeIn(100);

    },
    HideRecipientDetailsOnHover: function () {
        $('.rec-hoverd-details').addClass('rec-hoverd-details-p');
        $('.rec-hoverd-details').fadeOut(100);
    },
    UncheckRecipientFromList: function (id) {
        $('.recipient-a-select').each(function () {
            if (id == $(this).data('id'))
                $(this).attr('checked', false)

        })
        for (var i in RecipientList.List) {
            if (id == RecipientList.List[i].ID) {
                RecipientList.List.splice(i, 1);
            }
        }

        for (var i in recipientcanvasgroup) {
            if (id == recipientcanvasgroup[i].id) {
                recipientcanvasgroup.remove(recipientcanvasgroup[i])
            }
        }
    },
    UncheckRecipientFromListWhileEditing: function (id) {

        for (var i = 0; i < recipientcanvasgroup.length; i++) {
            if (id == recipientcanvasgroup[i].id) {
                canvas.remove(recipientcanvasgroup[i]);
                freeSpaceForRecipient.push(recipientcanvasgroup[i].top);
                recipientcanvasgroup.remove(recipientcanvasgroup[i]);
                cTop -= 40;

            }
            updateModifications(true);
            canvas.counter++;
        }
        for (var i = 0; i < RecipientList.List.length; i++) {
            if (id == RecipientList.List[i].ID) {
                RecipientList.List.splice(0, 1);
            }
        }

        //for (var i in RecipientList.List) {
        //    if (id == RecipientList.List[i].ID) {
        //        RecipientList.List.splice(i, 1);
        //    }
        //}

        //for (var i in recipientcanvasgroup) {
        //    if (id == recipientcanvasgroup[i].id) {
        //        recipientcanvasgroup.remove(recipientcanvasgroup[i])
        //    }
        //}
    }
};