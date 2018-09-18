$(document).ready(function () {
   

    $("input[type=button]#btnFilterVersion").live("click", function () {
        return AddressBook.ManageAddressBook($(this));
    });
    $("select#showRecords").on("change", function () {
        return AddressBook.ShowRecords($(this));
    });
    $('.sorting').live("click", function () {
        return AddressBook.SortAddressBook($(this));
    });
    $('#btnSearchFilterPeople').live("click", function () {
        return AddressBook.SearchAddressBook($(this));
    });
    $('#btnResetSearch').live("click", function () {
        $('#Search').val('');
        return AddressBook.SearchAddressBook($(this));
    });
    $("a.DeleteBookAddress").live("click", function () {
        return AddressBook.DeleteBookAddress($(this));
    });
    $("input[type=button]#UpdateBookAddress").live("click", function () {
        return AddressBook.UpdateBookAddress($(this));
    });
});

var AddressBook = {

    UpdateBookAddress: function (sender) {
        $.ajaxExt({
            url: baseUrl + '/Admin/AddressBook/UpdateBookAddress',
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
                    window.location.href = baseUrl + '/Admin/AddressBook/ManageAddressBook';
                }, 1500);
            }
        });

    },

    DeleteBookAddress: function (sender) {
        $.ajaxExt({
            url: baseUrl + '/Admin/AddressBook/DeleteBookAddress',
            type: 'POST',
            validate: false,
            showErrorMessage: true,
            messageControl: $('div.messageAlert'),
            showThrobber: true,
            button: $(sender),
            throbberPosition: { my: "left center", at: "right center", of: $(sender) },
            data: { id: $(sender).attr("data-addressid") },
            success: function (results, message) {
                $.ShowMessage($('div.messageAlert'), message, MessageType.Success);
                Paging();
            }
        });
    },

    SortAddressBook: function (sender) {
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

    ManageAddressBook: function (totalCount) {
        var totalRecords = 0;
        totalRecords = parseInt(totalCount);
        //alert(totalRecords);
        PageNumbering(totalRecords);
    },

    SearchAddressBook: function (sender) {
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
        url: baseUrl + '/Admin/AddressBook/GetAddressBookPagingList',
        success: function (results, message) {
            $('#divResult table:first tbody').html(results[0]);
            PageNumbering(results[1]);
          
        }
    });
}