$(document).ready(function () {
    $("#formContactButton").live("click", function () {
        return ContactUs.AddContactUs($(this));
    });
    $("a.deleteContactUs").live("click", function () {
        return ContactUs.DeleteContactUs($(this));
    });

    $("a.ResolveContactUs").live("click", function () {
        return ContactUs.ResolveContactUs($(this));
    });
    $("input[type=button]#btnFilterVersion").live("click", function () {
        return ContactUs.ManageContactUs($(this));
    });
    $("select#showRecords").on("change", function () {
        return ContactUs.ShowRecords($(this));
    });
    $('.sorting').live("click", function () {
        return ContactUs.SortContactUs($(this));
    });
    $('#Search').keypress(function (e) {
        var key = e.which;
        if (key == 13)  // the enter key code
        {
            return ContactUs.SearchContactUs($(this));
        }
    });
});

var ContactUs = {
    SortContactUs: function (sender) {
        if ($(sender).hasClass("sorting_asc")) {
            $('.sorting').removeClass("sorting_asc");
            $('.sorting').removeClass("sorting_desc")
            $(sender).addClass("sorting_desc");
            $('#SortBy').val($(sender).attr('data-sortby'));
            $('#SortOrder').val('Desc');
            paging.startIndex = 1;
            paging.currentPage = 0;
            Paging();
        } else {
            $('.sorting').removeClass("sorting_asc");
            $('.sorting').removeClass("sorting_desc")
            $(sender).addClass("sorting_asc");
            $('#SortBy').val($(sender).attr('data-sortby'));
            $('#SortOrder').val('Asc');
            paging.startIndex = 1;
            paging.currentPage = 0;
            Paging();
        }
    },
    AddContactUs: function (sender) {
        $.ajaxExt({
            url: siteURL.SubmitContactUs,
            type: 'POST',
            validate: true,
            showErrorMessage: false,
            messageControl: $('div#status-division'),
            formToValidate: $("#formContactUs"),
            formToPost: $("#formContactUs"),
            isAjaxForm: true,
            showThrobber: false,
            button: $(sender),
            throbberPosition: { my: "left center", at: "right center", of: $(sender) },
            success: function (results, message) {
                //$.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                $(".loginerroemsg").css('color', '#fff').css('background','#c3ebc3');
                $(".loginerroemsg").text(message);                
                setTimeout(function () {
                    window.location.reload();
                }, 1500);
            }, error: function (results, message) {
                $(".loginerroemsg").css('color', '#fff').css('background', '#dd9696');
                $(".loginerroemsg").text(message);
                setTimeout(function () {
                    $(".loginerroemsg").text(' ');
                }, 1500);
            }
        });

    },
    DeleteContactUs: function (sender) {
        $.ConfirmBox("", "Are you sure to delete this record?", null, true, "Yes", true, null, function () {
            $.ajaxExt({
                url: baseUrl + siteURL.DeleteContactUs,
                type: 'POST',
                validate: false,
                showErrorMessage: true,
                messageControl: $('div.messageAlert'),
                showThrobber: true,
                button: $(sender),
                throbberPosition: { my: "left center", at: "right center", of: $(sender) },
                data: { ContactUsId: $(sender).attr("data-contactusid") },
                success: function (results, message) {
                    $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                    Paging();
                }
            });
        });
    },
    ManageContactUs: function (totalCount) {
        var totalRecords = 0;
        totalRecords = parseInt(totalCount);
        //alert(totalRecords);
        PageNumbering(totalRecords);
    },
    SearchContactUs: function (sender) {
        paging.startIndex = 1;
        Paging(sender);
    },
    ShowRecords: function (sender) {

        paging.startIndex = 1;
        paging.pageSize = parseInt($(sender).find('option:selected').val());
        Paging(sender);
    },
    ResolveContactUs: function (sender) {
        $.ConfirmBox("", "Are you sure want to resolve this query?", null, true, "Yes", true, null, function () {
            $.ajaxExt({
                url: baseUrl + siteURL.ResolvedContactUs,
                type: 'POST',
                validate: false,
                showErrorMessage: true,
                messageControl: $('div.messageAlert'),
                showThrobber: true,
                button: $(sender),
                throbberPosition: { my: "left center", at: "right center", of: $(sender) },
                data: { ContactUsId: $(sender).attr("data-contactusid") },
                success: function (results, message) {
                    $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                    Paging();
                }
            });
        });
    },
};

function Paging(sender) {
    var obj = new Object();
    obj.Search = $('#Search').val();
    obj.PageNo = paging.startIndex;
    obj.RecordsPerPage = paging.pageSize;
    obj.SortBy = $('#SortBy').val();
    obj.SortOrder = $('#SortOrder').val();
    $.ajaxExt({
        type: "POST",
        validate: false,
        parentControl: $(sender).parents("form:first"),
        data: $.postifyData(obj),
        messageControl: null,
        showThrobber: false,
        throbberPosition: { my: "left center", at: "right center", of: sender, offset: "5 0" },
        url: baseUrl + '/Admin/ContactUs/GetContactUsPagingList',
        success: function (results, message) {
            $('#divResult table:first tbody').html(results[0]);
            PageNumbering(results[1]);

        }
    });
}