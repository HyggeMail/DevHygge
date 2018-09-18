﻿$(document).ready(function () {
    $("input[type=button]#btnFilterVersion").live("click", function () {
        return Transactions.ManageTransactions($(this));
    });
    $("select#showRecords").on("change", function () {
        return Transactions.ShowRecords($(this));
    });
 
});

var Transactions = {
    ManageTransactions: function (totalCount) {
        var totalRecords = 0;
        totalRecords = parseInt(totalCount);
        //alert(totalRecords);
        PageNumbering(totalRecords);
    },

    SearchTransactions: function (sender) {
        paging.startIndex = 1;
        Paging(sender);

    },

    ShowRecords: function (sender) {

        paging.startIndex = 1;
        paging.pageSize = parseInt($(sender).find('option:selected').val());
        Paging(sender);

    }
}
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
        url: baseUrl + '/User/Payment/GetTransactionsPagingList',
        success: function (results, message) {
            $('#divResult table:first tbody').html(results[0]);
            PageNumbering(results[1]);
        }
    });
}