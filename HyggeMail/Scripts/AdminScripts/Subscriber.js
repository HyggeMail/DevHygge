$(document).ready(function () {
    $("input[type=button]#addSubscriberBtn").live("click", function () {
        return Subscriber.AddUpdateSubscriber($(this));
    });

    $("a.deleteSubscriber").live("click", function () {
        return Subscriber.DeleteSubscriber($(this));
    });

    $("input[type=button]#btnFilterVersion").live("click", function () {
        return Subscriber.ManageSubscriber($(this));
    });
    $("select#showRecords").on("change", function () {
        return Subscriber.ShowRecords($(this));
    });
    $('.sorting').live("click", function () {
        return Subscriber.SortSubscriber($(this));
    });
    $('#Search').keypress(function (e) {
        var key = e.which;
        if (key == 13)  // the enter key code
        {
            return Subscriber.SearchSubscriber($(this));
        }
    });
});

var Subscriber = {
    SortSubscriber: function (sender) {
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
    AddUpdateSubscriber: function (sender) {
        $("#Description").val(CKEDITOR.instances["Description"].getData());

        $.ajaxExt({
            url: baseUrl + '/Admin/Subscriber/AddUpdateSubscriberDetails',
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
            success: function (results, message) {
                $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                setTimeout(function () {
                    window.location.href = baseUrl + '/Admin/Subscriber/ManageSubscriber';
                }, 1500);

            }
        });

    },


    DeleteSubscriber: function (sender) {
        debugger
        $.ConfirmBox("", "Are you sure to delete this record?", null, true, "Yes", true, null, function () {
            $.ajaxExt({
                url: baseUrl + '/Admin/Subscriber/DeleteSubscriber',
                type: 'POST',
                validate: false,
                showErrorMessage: true,
                messageControl: $('div.messageAlert'),
                showThrobber: true,
                button: $(sender),
                throbberPosition: { my: "left center", at: "right center", of: $(sender) },
                data: { SubscriberId: $(sender).attr("data-id") },
                success: function (results, message) {
                    debugger
                    $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                    Paging();
                }
            });
        });
    },

    ManageSubscriber: function (totalCount) {
        var totalRecords = 0;
        totalRecords = parseInt(totalCount);
        //alert(totalRecords);
        PageNumbering(totalRecords);
    },

    SearchSubscriber: function (sender) {
        paging.startIndex = 1;
        Paging(sender);

    },

    ShowRecords: function (sender) {

        paging.startIndex = 1;
        paging.pageSize = parseInt($(sender).find('option:selected').val());
        Paging(sender);

    }
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
        url: baseUrl + '/Admin/Subscriber/GetSubscriberPagingList',
        success: function (results, message) {
            debugger
            $('#divResult table:first tbody').html(results[0]);
            PageNumbering(results[1]);

        }
    });
}